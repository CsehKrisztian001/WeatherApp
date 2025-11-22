using WeatherApp.Models;

namespace WeatherApp.Services
{
    public interface IWeatherService
    {
        Task<WeatherViewModel> GetCurrentTemperatureAsync(string city, CancellationToken ct = default);
    }
}
