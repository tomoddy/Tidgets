namespace Tidgets;

/// <summary>
/// How one widget on the home screen is set up.
/// </summary>
public class WidgetConfig
{
    /// <summary>
    /// The Android ID of the widget.
    /// </summary>
    public int WidgetId { get; set; }

    /// <summary>
    /// The ID of the source the widget shows.
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// The base URL of the site.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The API key, if the source uses one.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}