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
    /// Formats a temperature as a whole number with a degree sign, or dashes if it's missing.
    /// </summary>
    /// <param name="temperature">The temperature in °C.</param>
    public static string Degrees(double? temperature)
    {
        // Adding zero turns a rounded -0 into 0.
        return temperature is null ? "--" : $"{Math.Round(temperature.Value, MidpointRounding.AwayFromZero) + 0d}°";
    }
}