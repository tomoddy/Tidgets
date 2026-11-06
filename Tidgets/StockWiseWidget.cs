using Android.Appwidget;
using Android.Content;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The StockWise widget, listing expired items and items expiring soon.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_stockwise", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_stockwise")]
public class StockWiseWidget : SourceWidget
{
    /// <summary>
    /// The StockWise source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.StockWise;

    /// <summary>
    /// The StockWise layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_stockwise;

    /// <summary>
    /// Lists the expired items, then the ones expiring soon.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">StockWise's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        StockWiseData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.StockWiseData) ?? new StockWiseData();
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        views.RemoveAllViews(Resource.Id.widget_content);
        if (data.Expired.Count == 0 && data.Expiring.Count == 0)
        {
            WidgetViews.AddSection(context, views, Resource.Id.widget_content, "All Good", "good", []);
            return;
        }
        if (data.Expired.Count > 0)
        {
            WidgetViews.AddSection(context, views, Resource.Id.widget_content, $"Expired ({data.Expired.Count})", "bad", [.. data.Expired.Select(x => $"{x.Name} · {x.Location}")]);
        }
        if (data.Expiring.Count > 0)
        {
            WidgetViews.AddSection(context, views, Resource.Id.widget_content, $"Expiring ({data.Expiring.Count})", "warn", [.. data.Expiring.Select(x => $"{x.Name} · {When(x.Expiry.DayNumber - today.DayNumber)}")]);
        }
    }

    /// <summary>
    /// Describes how far away an expiry is.
    /// </summary>
    /// <param name="days">The days until expiry.</param>
    private static string When(int days)
    {
        return days <= 0 ? "Today" : days == 1 ? "Tomorrow" : $"In {days} Days";
    }
}