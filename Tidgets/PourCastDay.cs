namespace Tidgets;

/// <summary>
/// One day in PourCast's daily forecast.
/// </summary>
public class PourCastDay
{
    /// <summary>
    /// The date.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// The day's main condition, as a name like PartlyCloudy.
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// The lowest temperature in °C.
    /// </summary>
    public double? Low { get; set; }

    /// <summary>
    /// The highest temperature in °C.
    /// </summary>
    public double? High { get; set; }
}