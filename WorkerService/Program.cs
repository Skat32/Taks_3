using Logic.Interfaces;
using Logic.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<IHttpClientProxy, HttpClientProxy>();
                    services.AddScoped<ICurrencyConverterService, CurrencyConverterService>();
                    services.AddScoped<ICurrencyService, CurrencyConverterService>();
                    
                    services.AddSingleton<ILoggerService, LoggerService>();
                    services.AddHostedService<Worker>();
                });
    }
}