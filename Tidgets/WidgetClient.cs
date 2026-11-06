using System.Globalization;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// Fetches a widget's summary from its site.
/// </summary>
public static class WidgetClient
{
    /// <summary>
    /// The HTTP client shared by all requests.
    /// </summary>
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(20) };

    /// <summary>
    /// Gets the summary for a widget, sending the location and API key only if its source uses them.
    /// </summary>
    /// <param name="source">The source the widget shows.</param>
    /// <param name="config">The widget's setup.</param>
    /// <param name="location">The device's location, if the source needs it.</param>
    /// <param name="timeZone">The IANA time zone of the device.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<WidgetData> GetAsync(WidgetSource source, WidgetConfig config, (double Latitude, double Longitude)? location, string timeZone, CancellationToken cancellationToken)
    {
        string url = $"{config.BaseUrl.TrimEnd('/')}{source.Path}";
        if (source.UsesLocation && location is not null)
        {
            url = string.Create(CultureInfo.InvariantCulture, $"{url}?latitude={location.Value.Latitude}&longitude={location.Value.Longitude}&timeZone={Uri.EscapeDataString(timeZone)}");
        }
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        if (source.UsesApiKey && !string.IsNullOrWhiteSpace(config.ApiKey))
        {
            request.Headers.Add(source.ApiKeyHeader, config.ApiKey);
        }
        using HttpResponseMessage response = await Client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync(stream, WidgetJsonContext.Default.WidgetData, cancellationToken) ?? throw new InvalidOperationException("The site returned no data.");
    }
}