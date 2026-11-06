namespace Tidgets;

/// <summary>
/// One hour in PourCast's hourly strip.
/// </summary>
public class PourCastHour
{
    /// <summary>
    /// The start of the hour.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// The temperature in °C.
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// The condition, as a name like PartlyCloudy.
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Whether the sun is up.
    /// </summary>
    public bool IsDay { get; set; }
}