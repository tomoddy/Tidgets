using Android.Appwidget;
using Android.Content;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The PourCast weather widget.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_pourcast", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_pourcast")]
public class PourCastWidget : SourceWidget
{
    /// <summary>
    /// The PourCast source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.PourCast;

    /// <summary>
    /// The PourCast layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_pourcast;

    /// <summary>
    /// Shows the place, temperature and conditions.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">PourCast's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        PourCastData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.PourCastData) ?? new PourCastData();
        views.SetTextViewText(Resource.Id.widget_title, data.Title);
        views.SetTextViewText(Resource.Id.widget_value, data.Value);
        views.SetTextViewText(Resource.Id.widget_subtitle, data.Subtitle);
    }
}