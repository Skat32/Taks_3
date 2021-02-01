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

        public async Task SaveCursesAsync(IEnumerable<CurseRequest> requests)
        {
            var query = $"{_url}{string.Join(',', requests.Select(x => x.ToStringCurse()))}&compact=ultra&apiKey={_apiKey}";

            var result = await _httpClientProxy.GetAsync<IEnumerable<CurseFromConverterResponse>>(query);

            await _context.Curses.AddRangeAsync(result.Select(x => new Curse
                {
                    Value = x.Value,
                    CurrenciesFrom = x.Key.ParseStr().from,
                    CurrenciesTo = x.Key.ParseStr().to
                }));
            
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CurseResponse>> GetCursesAsync(CurseRequest request)
        {
            var result = new List<CurseResponse>();

            var cursesDb = await _context.Curses.Where(x => x.Created >= DateTime.Now.AddHours(1)).ToListAsync();
            
            foreach (var enumValue in typeof(PeriodEnum).GetEnumValues())
            {
                var type = Enum.Parse<PeriodEnum>(enumValue.ToString() ?? string.Empty);
                
                result.AddRange(GetCursesByTime(cursesDb, type, request));
            }

            return result;
        }

        /// <summary>
        /// Получаем отсортированный список курсов для выбранного периода и курса из переданного списка курсов в БД
        /// </summary>
        /// <param name="cursesDb"> список курсов из БД </param>
        /// <param name="periodEnum"> период за который необходимо взять информацию </param>
        /// <param name="curseRequest"> валюта </param>
        /// <returns></returns>
        private static IEnumerable<CurseResponse> GetCursesByTime(IEnumerable<Curse> cursesDb, PeriodEnum periodEnum, CurseRequest curseRequest)
        {
            var result = cursesDb.Where(x => x.Created >= DateTime.Now.AddMinutes((int) periodEnum)
                                                   && x.CurrenciesFrom == curseRequest.From
                                                   && x.CurrenciesTo == curseRequest.To)
                .OrderBy(x => x.Created)
                .Select(x => x.Value).ToList();

            return result.Select(x => new CurseResponse
            {
                Key = curseRequest.ToStringCurse(),
                FirstValue = result.First(),
                LastValue = result.Last(),
                MaxValue = result.Max(), 
                MinValue = result.Min(), 
                PeriodEnum = periodEnum
            });
        }
    }
}