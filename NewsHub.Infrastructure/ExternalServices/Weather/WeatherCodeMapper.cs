namespace NewsHub.Infrastructure.ExternalServices.Weather;

internal static class WeatherCodeMapper
{
    public static string ToCondition(int code) => code switch
    {
        0 => "Clear sky",
        1 or 2 or 3 => "Partly cloudy",
        45 or 48 => "Fog",
        51 or 53 or 55 => "Drizzle",
        56 or 57 => "Freezing drizzle",
        61 or 63 or 65 => "Rain",
        66 or 67 => "Freezing rain",
        71 or 73 or 75 => "Snow",
        77 => "Snow grains",
        80 or 81 or 82 => "Rain showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown"
    };

    public static string ToIconUrl(int code) => code switch
    {
        0 => "/site/img/weather/clear.png",
        1 or 2 or 3 => "/site/img/weather/partly-cloudy.png",
        45 or 48 => "/site/img/weather/fog.png",
        51 or 53 or 55 or 56 or 57 => "/site/img/weather/drizzle.png",
        61 or 63 or 65 or 66 or 67 or 80 or 81 or 82 => "/site/img/weather/rain.png",
        71 or 73 or 75 or 77 or 85 or 86 => "/site/img/weather/snow.png",
        95 or 96 or 99 => "/site/img/weather/thunderstorm.png",
        _ => "/site/img/weather/default.png"
    };
}