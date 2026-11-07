using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The HomeRun widget, listing today's tasks with a tick beside the ones that are done.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_homerun", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_homerun")]
public class HomeRunWidget : SourceWidget
{
    /// <summary>
    /// The HomeRun source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.HomeRun;

    /// <summary>
    /// The HomeRun layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_homerun;

    /// <summary>
    /// Lists today's tasks, ticked and greyed out when done.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">HomeRun's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        HomeRunData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.HomeRunData) ?? new HomeRunData();
        views.RemoveAllViews(Resource.Id.widget_tasks);
        foreach (HomeRunTask task in data.Tasks)
        {
            RemoteViews row = new(context.PackageName, Resource.Layout.widget_task);
            row.SetTextViewText(Resource.Id.task_check, task.Done ? "✓" : "○");
            row.SetTextColor(Resource.Id.task_check, Color.ParseColor(task.Done ? "#6BCB77" : "#B3FFFFFF"));
            row.SetTextViewText(Resource.Id.task_name, task.Name);
            row.SetTextColor(Resource.Id.task_name, Color.ParseColor(task.Done ? "#B36BCB77" : "#FFFFFF"));
            views.AddView(Resource.Id.widget_tasks, row);
        }
    }
}