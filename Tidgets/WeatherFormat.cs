using System.Globalization;
using System.Text;

namespace Tidgets;

/// <summary>
/// Turns PourCast's conditions and temperatures into text for the widget.
/// </summary>
public static class WeatherFormat
{
    /// <summary>
    /// Gets the icon for a condition, using the moon for clear and part cloudy nights.
    /// </summary>
    /// <param name="condition">The condition name, like PartlyCloudy.</param>
    /// <param name="isDay">Whether the sun is up.</param>
    public static string Icon(string condition, bool isDay)
    {
        return condition switch
        {
            "Clear" => isDay ? "☀️" : "🌙",
            "PartlyCloudy" => isDay ? "⛅" : "☁️",
            "Cloudy" => "☁️",
            "Fog" => "🌫️",
            "Drizzle" or "Showers" => "🌦️",
            "Rain" or "HeavyRain" => "🌧️",
            "FreezingRain" or "Sleet" or "Hail" or "SnowShowers" => "🌨️",
            "Snow" => "❄️",
            "Thunderstorm" => "⛈️",
            _ => "–"
        };
    }

    /// <summary>
    /// Splits a condition name into words, so PartlyCloudy becomes Partly Cloudy.
    /// </summary>
    /// <param name="condition">The condition name.</param>
    public static string Words(string condition)
    {
        StringBuilder builder = new();
        foreach (char letter in condition)
        {
            if (char.IsUpper(letter) && builder.Length > 0)
            {
                builder.Append(' ');
            }
            builder.Append(letter);
        }
        return condition == "Unknown" ? string.Empty : builder.ToString();
    }

    /// <summary>
    /// Describes when the rain starts or stops, in the device's time zone.
    /// </summary>
    /// <param name="isRaining">Whether it is raining now.</param>
    /// <param name="start">When the next rain starts.</param>
    /// <param name="end">When the current rain stops.</param>
    public static string Rain(bool isRaining, DateTimeOffset? start, DateTimeOffset? end)
    {
        if (isRaining)
        {
            return end is null ? "Rain Continuing" : $"Rain Until {Clock(end.Value)}";
        }
        return start is null ? "No Rain Expected" : $"Rain from {Clock(start.Value)}";
    }

    /// <summary>
    /// Gets the UV label, such as UV 3, or nothing when the index rounds to zero.
    /// </summary>
    /// <param name="uvIndex">The UV index.</param>
    public static string Uv(double? uvIndex)
    {
        return uvIndex is null || Math.Round(uvIndex.Value, MidpointRounding.AwayFromZero) < 1 ? string.Empty : $"UV {Math.Round(uvIndex.Value, MidpointRounding.AwayFromZero)}";
    }

    /// <summary>
    /// Gets the colour hex for a UV index, from green for low through yellow and orange to red for very high.
    /// </summary>
    /// <param name="uvIndex">The UV index.</param>
    public static string UvColor(double? uvIndex)
    {
        return uvIndex switch { >= 8 => "#FF5252", >= 6 => "#FF9100", >= 3 => "#FFEA00", _ => "#00E676" };
    }

    /// <summary>
    /// Formats a time as the hour, with the day name if it isn't today.
    /// </summary>
    /// <param name="time">The time.</param>
    private static string Clock(DateTimeOffset time)
    {
        DateTimeOffset local = time.ToLocalTime();
        return local.ToString(local.Date == DateTime.Today ? "h tt" : "ddd h tt", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Formats a temperature as a whole number with a degree sign, or dashes if it's missing.
    /// </summary>
    /// <param name="temperature">The temperature in °C.</param>
    public static string Degrees(double? temperature)
    {
        // Adding zero turns a rounded -0 into 0.
        return temperature is null ? "--" : $"{Math.Round(temperature.Value, MidpointRounding.AwayFromZero) + 0d}°";
    }
}