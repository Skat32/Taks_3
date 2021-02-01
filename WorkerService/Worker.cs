using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Enums;
using Models.Requests;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ICurrencyConverterService _currencyConverterService;
        private readonly ILoggerService _loggerService;
        private readonly int _countMinutes;
        
        public Worker(IServiceScopeFactory serviceScopeFactory)
        {
            using var scope = serviceScopeFactory.CreateScope();

            _currencyConverterService = scope.ServiceProvider.GetRequiredService<ICurrencyConverterService>();
            _loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();
            
            _countMinutes = int.Parse(scope.ServiceProvider.GetRequiredService<IConfiguration>().GetSection("TimeInMinutes").Value) * 60;
            
            if (_countMinutes == 0) _countMinutes = 1;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _loggerService.Info($"Worker running at: {DateTimeOffset.Now}");
                
                try
                {
                    await _currencyConverterService.SaveCursesAsync(new List<CurseRequest>
                    {
                        new() {From = CurrenciesEnum.USD, To = CurrenciesEnum.RUB},
                        new() {From = CurrenciesEnum.EUR, To = CurrenciesEnum.RUB}
                    });
                }
                catch (Exception e)
                {
                    _loggerService.Error(e, "Ошибка в работе воркера для нераспределённых средств");
                }
                
                await Task.Delay(1000 * _countMinutes, stoppingToken);
            }
        }
    }
}