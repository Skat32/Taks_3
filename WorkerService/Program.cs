using DataLayer;
using Logic;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                    services.AddDbContext<HomeContext>(opt =>
                        opt.UseNpgsql(hostContext.Configuration.GetConnectionString("HomeContext")), ServiceLifetime.Singleton);
                    
                    services.AddSingleton(hostContext.Configuration.GetSection(nameof(CurrencyConverterApi)).Get<CurrencyConverterApi>());
                    
                    services.AddScoped<IHttpClientProxy, HttpClientProxy>();
                    services.AddScoped<ICurrencyConverterService, CurrencyConverterService>();
                    
                    services.AddSingleton<ILoggerService, LoggerService>();
                    services.AddHostedService<Worker>();
                });
    }
}