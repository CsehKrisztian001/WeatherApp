using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class OpenMeteoWeatherService : IWeatherService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public OpenMeteoWeatherService(HttpClient http)
        {
            _http = http;
            // Open-Meteo API base nem szükséges, több host van (geocoding + forecast)
            _http.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<WeatherViewModel> GetCurrentTemperatureAsync(string city, CancellationToken ct = default)
        {
            var vm = new WeatherViewModel { City = city?.Trim() };

            if (string.IsNullOrWhiteSpace(vm.City))
            {
                vm.Error = "Adj meg egy városnevet.";
                return vm;
            }

            try
            {
                // 1) Geokódolás – koordináták a városnévhez
                var geoUrl =
                    $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(vm.City)}&count=1&language=hu&format=json";

                var geo = await _http.GetFromJsonAsync<GeocodingResponse>(geoUrl, _jsonOptions, ct);
                var place = geo?.Results?.FirstOrDefault();

                if (place is null)
                {
                    vm.Error = "Nem találtam ilyen várost. Próbáld pontosabban (pl. ország megadásával).";
                    return vm;
                }

                // 2) Időjárás – aktuális hőmérséklet, automatikus helyi időzóna
                var weatherUrl =
                    $"https://api.open-meteo.com/v1/forecast?latitude={place.Latitude}&longitude={place.Longitude}&current=temperature_2m&timezone=auto";

                var forecast = await _http.GetFromJsonAsync<ForecastResponse>(weatherUrl, _jsonOptions, ct);

                if (forecast?.Current == null)
                {
                    vm.Error = "Nem sikerült lekérni az aktuális hőmérsékletet.";
                    return vm;
                }

                vm.TemperatureC = forecast.Current.Temperature_2m;
                vm.LocalTime = forecast.Current.Time; // ISO string, helyi időzóna szerint
                vm.ResolvedName = BuildResolvedName(place);

                return vm;
            }
            catch (TaskCanceledException)
            {
                vm.Error = "Időtúllépés történt a lekérdezés során. Próbáld újra.";
                return vm;
            }
            catch (Exception ex)
            {
                vm.Error = $"Váratlan hiba történt: {ex.Message}";
                return vm;
            }
        }

        private static string BuildResolvedName(GeoResult r)
        {
            // pl. "Budapest, Hungary"
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(r.Name)) parts.Add(r.Name);
            if (!string.IsNullOrWhiteSpace(r.Admin1)) parts.Add(r.Admin1);
            if (!string.IsNullOrWhiteSpace(r.Country)) parts.Add(r.Country);
            return string.Join(", ", parts.Distinct());
        }

        // --- DTO-k ---

        private sealed class GeocodingResponse
        {
            public List<GeoResult>? Results { get; set; }
        }

        private sealed class GeoResult
        {
            public string? Name { get; set; }
            public string? Country { get; set; }
            public string? Admin1 { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        private sealed class ForecastResponse
        {
            public CurrentBlock? Current { get; set; }
        }

        private sealed class CurrentBlock
        {
            public string? Time { get; set; }
            public double Temperature_2m { get; set; }
        }
    }
}
