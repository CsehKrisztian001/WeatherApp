namespace WeatherApp.Models
{
    public class WeatherViewModel
    {
        public string? City { get; set; }

        // Eredmények
        public double? TemperatureC { get; set; }
        public string? LocalTime { get; set; }
        public string? ResolvedName { get; set; }

        // Hiba/üzenet
        public string? Error { get; set; }
    }
}
