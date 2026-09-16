using System.Text.Json.Serialization;

namespace NewsHub.Infrastructure.ExternalServices.Weather;

internal class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature2m { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
}