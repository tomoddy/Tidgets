using Android.App;
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
    /// Hands the refresh to the refresh job, which waits for a connection if there isn't one, or stops the alarm if no widgets are left.
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
        if (WidgetSources.All.Any(x => RefreshScheduler.GetWidgetIds(context, x).Length > 0))
        {
            RefreshJob.Schedule(context);
        }
        if (intent?.Action == Intent.ActionBootCompleted)
        {
            RefreshScheduler.Schedule(context);
        }
    }
}