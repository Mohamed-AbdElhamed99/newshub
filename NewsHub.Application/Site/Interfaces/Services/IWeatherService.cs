using NewsHub.Application.Site.DTOs.Weather;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IWeatherService
{
    Task<WeatherDto?> GetCurrentWeatherAsync(double lat, double lng, CancellationToken ct = default);

}