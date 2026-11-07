using Android.App;
using Android.Appwidget;
using Android.Content;

namespace Tidgets;

/// <summary>
/// Refreshes every widget when the repeating alarm fires, and restarts the alarm after the phone reboots.
/// </summary>
[BroadcastReceiver(Exported = false)]
[IntentFilter([Intent.ActionBootCompleted])]
public class RefreshReceiver : BroadcastReceiver
{
    /// <summary>
    /// Asks each placed widget to update, or stops the alarm if there are none.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="intent">The alarm or boot intent.</param>
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null)
        {
            return;
        }
        RefreshScheduler.CancelIfUnused(context);
        foreach (WidgetSource source in WidgetSources.All)
        {
            int[] ids = RefreshScheduler.GetWidgetIds(context, source);
            if (ids.Length == 0)
            {
                continue;
            }
            Intent update = new(context, source.ProviderType);
            update.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            update.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
            context.SendBroadcast(update);
        }
        if (intent?.Action == Intent.ActionBootCompleted)
        {
            RefreshScheduler.Schedule(context);
        }
    }
}