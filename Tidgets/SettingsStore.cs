using Android.Content;
using System.Globalization;

namespace Tidgets;

/// <summary>
/// Stores each widget's setup and the last known location.
/// </summary>
/// <param name="context">The context used to reach shared preferences.</param>
public class SettingsStore(Context context)
{
    /// <summary>
    /// The shared preferences everything is stored in.
    /// </summary>
    private readonly ISharedPreferences Preferences = context.GetSharedPreferences("tidgets", FileCreationMode.Private)!;

    /// <summary>
    /// Gets a widget's setup, or null if it hasn't been set up.
    /// </summary>
    /// <param name="widgetId">The Android ID of the widget.</param>
    public WidgetConfig? GetConfig(int widgetId)
    {
        string? sourceId = Preferences.GetString($"widget_{widgetId}_source", null);
        if (sourceId is null)
        {
            return null;
        }
        return new WidgetConfig { WidgetId = widgetId, SourceId = sourceId, BaseUrl = Preferences.GetString($"widget_{widgetId}_url", null) ?? string.Empty, ApiKey = Preferences.GetString($"widget_{widgetId}_key", null) ?? string.Empty };
    }

    /// <summary>
    /// Saves a widget's setup.
    /// </summary>
    /// <param name="config">The setup to save.</param>
    public void SaveConfig(WidgetConfig config)
    {
        Preferences.Edit()?.PutString($"widget_{config.WidgetId}_source", config.SourceId)?.PutString($"widget_{config.WidgetId}_url", config.BaseUrl)?.PutString($"widget_{config.WidgetId}_key", config.ApiKey)?.Apply();
    }

    /// <summary>
    /// Removes a widget's setup.
    /// </summary>
    /// <param name="widgetId">The Android ID of the widget.</param>
    public void DeleteConfig(int widgetId)
    {
        Preferences.Edit()?.Remove($"widget_{widgetId}_source")?.Remove($"widget_{widgetId}_url")?.Remove($"widget_{widgetId}_key")?.Remove($"widget_{widgetId}_title")?.Remove($"widget_{widgetId}_value")?.Remove($"widget_{widgetId}_subtitle")?.Apply();
    }

    /// <summary>
    /// Remembers what a widget last showed successfully.
    /// </summary>
    /// <param name="widgetId">The Android ID of the widget.</param>
    /// <param name="data">The data to remember.</param>
    public void SaveData(int widgetId, WidgetData data)
    {
        Preferences.Edit()?.PutString($"widget_{widgetId}_title", data.Title)?.PutString($"widget_{widgetId}_value", data.Value)?.PutString($"widget_{widgetId}_subtitle", data.Subtitle)?.Apply();
    }

    /// <summary>
    /// Gets what a widget last showed successfully, or null if it never has.
    /// </summary>
    /// <param name="widgetId">The Android ID of the widget.</param>
    public WidgetData? GetData(int widgetId)
    {
        string? value = Preferences.GetString($"widget_{widgetId}_value", null);
        if (value is null)
        {
            return null;
        }
        return new WidgetData { Title = Preferences.GetString($"widget_{widgetId}_title", null) ?? string.Empty, Value = value, Subtitle = Preferences.GetString($"widget_{widgetId}_subtitle", null) ?? string.Empty };
    }

    /// <summary>
    /// Remembers a location, for when the device can't provide one.
    /// </summary>
    /// <param name="latitude">The latitude to remember.</param>
    /// <param name="longitude">The longitude to remember.</param>
    public void SaveLocation(double latitude, double longitude)
    {
        Preferences.Edit()?.PutString("latitude", latitude.ToString(CultureInfo.InvariantCulture))?.PutString("longitude", longitude.ToString(CultureInfo.InvariantCulture))?.Apply();
    }

    /// <summary>
    /// Gets the remembered location, or null if there isn't one.
    /// </summary>
    public (double Latitude, double Longitude)? GetSavedLocation()
    {
        if (double.TryParse(Preferences.GetString("latitude", null), CultureInfo.InvariantCulture, out double latitude) && double.TryParse(Preferences.GetString("longitude", null), CultureInfo.InvariantCulture, out double longitude))
        {
            return (latitude, longitude);
        }
        return null;
    }
}