using Android.Appwidget;
using Android.Content;
using System.Globalization;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The MMDb widget, showing how many films have been watched.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_mmdb", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_mmdb")]
public class MMDbWidget : SourceWidget
{
    /// <summary>
    /// The MMDb source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.MMDb;

    /// <summary>
    /// The MMDb layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_mmdb;

    /// <summary>
    /// Shows the number of films watched.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">MMDb's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        MMDbData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.MMDbData) ?? new MMDbData();
        views.SetTextViewText(Resource.Id.widget_films, data.Films.ToString("N0", CultureInfo.InvariantCulture));
    }
}