namespace Tidgets;

/// <summary>
/// The summary a site returns for the widget to show.
/// </summary>
public class WidgetData
{
    /// <summary>
    /// The widget title, usually the place name.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The main value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The supporting line.
    /// </summary>
    public string Subtitle { get; set; } = string.Empty;
}