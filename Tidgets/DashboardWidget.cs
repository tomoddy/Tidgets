using Android.Appwidget;
using Android.Content;
using Android.Util;
using Android.Views;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The Dashboard widget, a single row showing the homelab's online and offline totals followed by each host's counts.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_dashboard", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_dashboard")]
public class DashboardWidget : SourceWidget
{
    /// <summary>
    /// The Dashboard source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.Dashboard;

    /// <summary>
    /// The Dashboard layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_dashboard;

    /// <summary>
    /// Leaves out the status line, as it doesn't fit in a widget one cell tall.
    /// </summary>
    protected override bool ShowsStatus => false;

    /// <summary>
    /// Shows the totals, then one cell per host. A count with anything offline is shown as online over offline, smaller and stacked.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">The Dashboard's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        DashboardData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.DashboardData) ?? new DashboardData();
        int fullSize = context.Resources!.GetInteger(Resource.Integer.dashboard_count_sp);
        int stackedSize = context.Resources!.GetInteger(Resource.Integer.dashboard_stacked_sp);
        SetCounts(views, Resource.Id.widget_online, Resource.Id.widget_offline, data.Online, data.Offline, fullSize, stackedSize);
        views.RemoveAllViews(Resource.Id.widget_hosts);
        foreach (DashboardHost host in data.Hosts)
        {
            RemoteViews cell = new(context.PackageName, Resource.Layout.widget_dashboard_host);
            cell.SetTextViewText(Resource.Id.host_short, host.Short);
            SetCounts(cell, Resource.Id.host_online, Resource.Id.host_offline, host.Online, host.Offline, fullSize, stackedSize);
            views.AddView(Resource.Id.widget_hosts, cell);
        }
    }

    /// <summary>
    /// Shows an online count on its own at full size, or online over offline at the smaller stacked size when anything is offline.
    /// </summary>
    /// <param name="views">The views holding the two counts.</param>
    /// <param name="onlineId">The ID of the online count.</param>
    /// <param name="offlineId">The ID of the offline count.</param>
    /// <param name="online">The online count.</param>
    /// <param name="offline">The offline count.</param>
    /// <param name="fullSize">The text size, in sp, of a count on its own.</param>
    /// <param name="stackedSize">The text size, in sp, of stacked counts.</param>
    private static void SetCounts(RemoteViews views, int onlineId, int offlineId, int online, int offline, int fullSize, int stackedSize)
    {
        bool stacked = offline > 0;
        views.SetTextViewText(onlineId, online.ToString());
        views.SetTextViewTextSize(onlineId, (int)ComplexUnitType.Sp, stacked ? stackedSize : fullSize);
        views.SetTextViewText(offlineId, offline.ToString());
        views.SetTextViewTextSize(offlineId, (int)ComplexUnitType.Sp, stackedSize);
        views.SetViewVisibility(offlineId, stacked ? ViewStates.Visible : ViewStates.Gone);
    }
}