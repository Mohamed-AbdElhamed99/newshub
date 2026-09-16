using System.Text.Json.Serialization;

namespace NewsHub.Infrastructure.ExternalServices.Weather;

internal class OpenMeteoResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather? Current { get; set; }
}