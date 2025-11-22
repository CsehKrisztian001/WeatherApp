using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWeatherService _weather;

        public HomeController(ILogger<HomeController> logger, IWeatherService weather)
        {
            _logger = logger;
            _weather = weather;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new WeatherViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(WeatherViewModel input, CancellationToken ct)
        {
            var result = await _weather.GetCurrentTemperatureAsync(input.City ?? string.Empty, ct);
            return View(result);
        }
    }
}
