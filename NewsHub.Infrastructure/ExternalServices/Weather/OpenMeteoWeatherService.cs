using System.Globalization;
using System.Net.Http.Json;
using NewsHub.Application.Site.DTOs.Weather;
using NewsHub.Application.Site.Interfaces.Services;

namespace NewsHub.Infrastructure.ExternalServices.Weather;

public class OpenMeteoWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    
    public OpenMeteoWeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient; 
    }

    public async Task<WeatherDto?> GetCurrentWeatherAsync(double lat, double lng, CancellationToken ct = default)
    {
        var url = string.Create(CultureInfo.InvariantCulture,
            $"forecast?latitude={lat}&longitude={lng}&current=temperature_2m,weather_code&timezone=auto");

        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url, ct);

        if (response?.Current is null)
            return null;

        var current = response.Current;

        return new WeatherDto
        {
            Temperature = Math.Round(current.Temperature2m, 0),
            WeatherCode = current.WeatherCode,
            Condition = WeatherCodeMapper.ToCondition(current.WeatherCode),
            IconUrl = WeatherCodeMapper.ToIconUrl(current.WeatherCode),
            ObservedAtUtc = current.Time,
            LocationName = string.Empty // filled by caller/reverse-geocode step if needed
        };
    }
}