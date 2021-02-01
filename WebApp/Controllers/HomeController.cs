using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Logic;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models.Requests;
using Models.Responses;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _options;
        
        public HomeController(ICurrencyService currencyService, IMemoryCache memoryCache)
        {
            _currencyService = currencyService;
            _memoryCache = memoryCache;

            _options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)); // при условии, что будет большое количествл запросов, стоит использовать кэш
        }

        public async Task<IActionResult> Index(string keyCurse)
        {
            if (string.IsNullOrEmpty(keyCurse))
                return View();

            if (_memoryCache.TryGetValue($"valueCurses{keyCurse}", out IEnumerable<CurseResponse> result)) 
                return View(result);
            
            var (from, to) = keyCurse.ParseCurrencies();
            result = await _currencyService.GetCursesAsync(new CurseRequest {From = from, To = to});
                
            _memoryCache.Set($"valueCurses{keyCurse}", result, _options);

            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}