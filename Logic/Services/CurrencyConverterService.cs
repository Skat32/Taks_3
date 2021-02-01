using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enums;
using Models.Requests;
using Models.Responses;

namespace Logic.Services
{
    public class CurrencyConverterService : ICurrencyConverterService, ICurrencyService
    {
        private readonly IHttpClientProxy _httpClientProxy;
        private readonly HomeContext _context;
        private readonly string _apiKey;
        private readonly string _url;

        public CurrencyConverterService(IHttpClientProxy httpClientProxy, CurrencyConverterApi currencyConverter, HomeContext context)
        {
            _httpClientProxy = httpClientProxy;
            _context = context;
            _apiKey = currencyConverter.ApiKey;
            _url = currencyConverter.Url;
        }

        public async Task SaveCurseAsync(CurseRequest request)
        {
            if (request.From == request.To)
            {
                await _context.Curses.AddAsync(new Curse
                    {Value = 1, CurrenciesFrom = request.From, CurrenciesTo = request.To});
            
                await _context.SaveChangesAsync();

                return;
            }

            var query = $"{_url}{request.ToStringCurse()}&compact=ultra&apiKey={_apiKey}";

            var result = await _httpClientProxy.GetAsync<CurseFromConverterResponse>(query);

            await _context.Curses.AddAsync(new Curse
                {Value = result.Value, CurrenciesFrom = request.From, CurrenciesTo = request.To});
            
            await _context.SaveChangesAsync();
        }

        public async Task SaveCursesAsync(IList<CurseRequest> requests)
        {
            var query = $"{_url}{string.Join(',', requests.Select(x => x.ToStringCurse()))}&compact=ultra&apiKey={_apiKey}";

            var result = await _httpClientProxy.GetAsync<dynamic>(query);
            
            var res = requests.Select(x => new CurseFromConverterResponse
                {Key = x.ToStringCurse(), Value = (decimal) result[x.ToStringCurse()]});

            await _context.Curses.AddRangeAsync(res.Select(x => new Curse
            {
                Value = x.Value,
                CurrenciesFrom = x.Key.ParseCurrencies().from,
                CurrenciesTo = x.Key.ParseCurrencies().to
            }));

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CurseResponse>> GetCursesAsync(CurseRequest request)
        {
            var cursesDb = await _context.Curses.Where(x => x.Created >= DateTime.Now.AddHours(-1)).ToListAsync();

            return (from object enumValue in typeof(PeriodEnum).GetEnumValues()
                select Enum.Parse<PeriodEnum>(enumValue.ToString() ?? string.Empty)
                into type
                select GetCursesByTime(cursesDb, type, request)).ToList();
        }

        /// <summary>
        /// Получаем отсортированный список курсов для выбранного периода и курса из переданного списка курсов в БД
        /// </summary>
        /// <param name="cursesDb"> список курсов из БД </param>
        /// <param name="periodEnum"> период за который необходимо взять информацию </param>
        /// <param name="curseRequest"> валюта </param>
        /// <returns></returns>
        private static CurseResponse GetCursesByTime(IEnumerable<Curse> cursesDb, PeriodEnum periodEnum, CurseRequest curseRequest)
        {
            var result = cursesDb.Where(x => x.Created >= DateTime.Now.AddMinutes(-(int) periodEnum)
                                                   && x.CurrenciesFrom == curseRequest.From
                                                   && x.CurrenciesTo == curseRequest.To)
                .OrderBy(x => x.Created)
                .Select(x => x.Value).ToList();

            return new CurseResponse
            {
                Key = curseRequest.ToStringCurse(),
                FirstValue = result.FirstOrDefault(),
                LastValue = result.LastOrDefault(),
                MaxValue = !result.Any() ? 0 : result.Max(),
                MinValue = !result.Any() ? 0 : result.Min(),
                PeriodEnum = periodEnum
            };
        }
    }
}