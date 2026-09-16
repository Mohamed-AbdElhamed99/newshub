namespace NewsHub.Application.Site.DTOs.Weather;

public class WeatherDto
{
    public double Temperature { get; set; }
    public int WeatherCode { get; set; }
    public string Condition { get; set; } = string.Empty; // human-readable, mapped from code
    public string IconUrl { get; set; } = string.Empty;   // resolved icon path/url
    public string LocationName { get; set; } = string.Empty; // e.g. "NEW YORK"
    public DateTime ObservedAtUtc { get; set; }
}