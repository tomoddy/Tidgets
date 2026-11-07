namespace Tidgets;

/// <summary>
/// A predefined site a widget can show, and what it needs to be set up.
/// </summary>
public class WidgetSource
{
    /// <summary>
    /// The stable ID stored with each widget.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name shown in setup.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The base URL filled in by default, which can be changed per widget.
    /// </summary>
    public string DefaultBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The path of the site's widget endpoint.
    /// </summary>
    public string Path { get; set; } = "/api/widget";

    /// <summary>
    /// Whether the endpoint needs the device's location.
    /// </summary>
    public bool UsesLocation { get; set; }

    /// <summary>
    /// Whether the endpoint needs an API key.
    /// </summary>
    public bool UsesApiKey { get; set; }

    /// <summary>
    /// The header the API key is sent in.
    /// </summary>
    public string ApiKeyHeader { get; set; } = "X-Api-Key";

    /// <summary>
    /// The widget provider class that draws this source.
    /// </summary>
    public Type ProviderType { get; set; } = typeof(object);

    /// <summary>
    /// Creates the widget class that draws this source, so the refresh job can update its widgets.
    /// </summary>
    public Func<SourceWidget> CreateWidget { get; set; } = () => throw new InvalidOperationException("No widget class set for this source.");
}