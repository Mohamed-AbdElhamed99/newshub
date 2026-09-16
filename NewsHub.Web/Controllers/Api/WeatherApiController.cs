using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.Interfaces.Services;

namespace NewsHub.Web.Controllers.Api;

[ApiController]
[Route("api/weather")]
public class WeatherApiController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherApiController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] double lat, [FromQuery] double lng, CancellationToken ct)
    {
        if (lat is < -90 or > 90 || lng is < -180 or > 180)
            return BadRequest(new { message = "Invalid coordinates." });

        var weather = await _weatherService.GetCurrentWeatherAsync(lat, lng, ct);

        if (weather is null)
            return NotFound();

        // Only what the navbar needs — don't leak the full internal DTO shape.
        return Ok(new
        {
            temperature = weather.Temperature,
            condition = weather.Condition,
            iconUrl = weather.IconUrl,
            observedAt = weather.ObservedAtUtc
        });
    }
}