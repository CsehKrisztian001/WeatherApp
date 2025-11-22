namespace WeatherApp.Models
{
    public class WeatherViewModel
    {
        public string? City { get; set; }

        // Eredmények
        public double? TemperatureC { get; set; }
        public string? LocalTime { get; set; }
        public string? ResolvedName { get; set; }

        // Új mezők ikonhoz / állapothoz
        public int? WeatherCode { get; set; }
        public bool? IsDay { get; set; }
        public double? RainMm { get; set; }
        public double? SnowMm { get; set; }
        public string? IconFile { get; set; }   // pl. "sun.svg"
        public string? ConditionText { get; set; } // pl. "Tiszta nappal"

        // Hiba/üzenet
        public string? Error { get; set; }
    }
}
