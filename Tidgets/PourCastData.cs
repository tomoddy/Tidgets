namespace Tidgets;

/// <summary>
/// What PourCast returns for its widget.
/// </summary>
public class PourCastData
{
    /// <summary>
    /// The place name.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The current temperature.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The condition with the day's high and low.
    /// </summary>
    public string Subtitle { get; set; } = string.Empty;
}