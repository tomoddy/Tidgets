using Android.Appwidget;
using Android.Content;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The WakeBoard widget, showing how many machines are online and offline.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_wakeboard", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_wakeboard")]
public class WakeBoardWidget : SourceWidget
{
    /// <summary>
    /// The WakeBoard source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.WakeBoard;

    /// <summary>
    /// The WakeBoard layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_wakeboard;

    /// <summary>
    /// Shows the online and offline counts.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">WakeBoard's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        WakeBoardData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.WakeBoardData) ?? new WakeBoardData();
        views.SetTextViewText(Resource.Id.widget_online, data.Online.ToString());
        views.SetTextViewText(Resource.Id.widget_offline, data.Offline.ToString());
    }
}