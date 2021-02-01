using System.Diagnostics;
using System.Threading.Tasks;
using Logic;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILoggerService _loggerService;

        public HomeController(ICurrencyService currencyService, ILoggerService loggerService)
        {
            _currencyService = currencyService;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> Index(string keyCurse)
        {
            if (string.IsNullOrEmpty(keyCurse))
                return View();
            
            var (from, to) = keyCurse.ParseCurrencies();

            return View(await _currencyService.GetCursesAsync(new CurseRequest {From = from, To = to}));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}