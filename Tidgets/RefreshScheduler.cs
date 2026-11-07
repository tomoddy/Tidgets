using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using JavaClass = Java.Lang.Class;

namespace Tidgets;

/// <summary>
/// Schedules the repeating alarm that refreshes every widget, since the system's own widget timer is unreliable.
/// </summary>
public static class RefreshScheduler
{
    /// <summary>
    /// Starts, or restarts, the repeating refresh using the interval in the refresh_minutes resource.
    /// </summary>
    /// <param name="context">The context.</param>
    public static void Schedule(Context context)
    {
        AlarmManager? alarms = (AlarmManager?)context.GetSystemService(Context.AlarmService);
        if (alarms is null)
        {
            return;
        }
        long interval = context.Resources!.GetInteger(Resource.Integer.refresh_minutes) * 60000L;
        alarms.SetInexactRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + interval, interval, Create(context));
    }

    /// <summary>
    /// Stops the repeating refresh if no widgets are left.
    /// </summary>
    /// <param name="context">The context.</param>
    public static void CancelIfUnused(Context context)
    {
        if (WidgetSources.All.Any(x => GetWidgetIds(context, x).Length > 0))
        {
            return;
        }
        AlarmManager? alarms = (AlarmManager?)context.GetSystemService(Context.AlarmService);
        alarms?.Cancel(Create(context));
    }

    /// <summary>
    /// Gets the Android IDs of the widgets placed for a source.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="source">The source.</param>
    public static int[] GetWidgetIds(Context context, WidgetSource source)
    {
        return AppWidgetManager.GetInstance(context)?.GetAppWidgetIds(new ComponentName(context, JavaClass.FromType(source.ProviderType))) ?? [];
    }

    /// <summary>
    /// Creates the pending intent that wakes the refresh receiver.
    /// </summary>
    /// <param name="context">The context.</param>
    private static PendingIntent Create(Context context)
    {
        return PendingIntent.GetBroadcast(context, 0, new Intent(context, typeof(RefreshReceiver)), PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
    }
}