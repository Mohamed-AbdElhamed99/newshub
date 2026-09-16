using Microsoft.Extensions.Caching.Memory;
using NewsHub.Application.Site.DTOs;
using NewsHub.Application.Site.DTOs.Weather;
using NewsHub.Application.Site.Interfaces.Services;

namespace NewsHub.Infrastructure.ExternalServices.Weather;

public class CachedWeatherService : IWeatherService
{
    private readonly IWeatherService _inner;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public CachedWeatherService(IWeatherService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }
    
    public async Task<WeatherDto?> GetCurrentWeatherAsync(double lat, double lng, CancellationToken ct = default)
    {
        var roundedLat = Math.Round(lat, 1);
        var roundedLng = Math.Round(lng, 1);
        var cacheKey = $"weather:{roundedLat}:{roundedLng}";

        if (_cache.TryGetValue(cacheKey, out WeatherDto? cached))
            return cached;

        var result = await _inner.GetCurrentWeatherAsync(roundedLat, roundedLng, ct);

        if (result is not null)
            _cache.Set(cacheKey, result, CacheDuration);

        return result;
    }
}