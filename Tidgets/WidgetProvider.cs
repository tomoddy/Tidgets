using Android.Appwidget;
using Android.Content;
using AndroidUri = Android.Net.Uri;
using JavaClass = Java.Lang.Class;

namespace Tidgets;

/// <summary>
/// The home screen widget, showing the summary from whichever source each widget was set up with.
/// </summary>
[BroadcastReceiver(Label = "@string/app_name", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info")]
public class WidgetProvider : AppWidgetProvider
{
    /// <summary>
    /// Updates the widgets, finishing the work after the broadcast returns.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="appWidgetManager">The widget manager.</param>
    /// <param name="appWidgetIds">The IDs of the widgets to update.</param>
    public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
    {
        base.OnUpdate(context, appWidgetManager, appWidgetIds);
        if (context is null || appWidgetManager is null || appWidgetIds is null)
        {
            return;
        }
        PendingResult? pending = GoAsync();
        Task.Run(async () =>
        {
            try
            {
                await UpdateAsync(context, appWidgetManager, appWidgetIds);
            }
            finally
            {
                pending?.Finish();
            }
        });
    }

    /// <summary>
    /// Forgets the setup of widgets that were removed.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="appWidgetIds">The IDs of the removed widgets.</param>
    public override void OnDeleted(Context? context, int[]? appWidgetIds)
    {
        base.OnDeleted(context, appWidgetIds);
        if (context is null || appWidgetIds is null)
        {
            return;
        }
        SettingsStore settings = new(context);
        foreach (int id in appWidgetIds)
        {
            settings.DeleteConfig(id);
        }
    }

    /// <summary>
    /// Asks every widget on the home screen to update now.
    /// </summary>
    /// <param name="context">The context.</param>
    public static void Refresh(Context context)
    {
        int[]? ids = AppWidgetManager.GetInstance(context)?.GetAppWidgetIds(new ComponentName(context, JavaClass.FromType(typeof(WidgetProvider))));
        if (ids is null || ids.Length == 0)
        {
            return;
        }
        Intent intent = new(context, typeof(WidgetProvider));
        intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
        intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
        context.SendBroadcast(intent);
    }

    /// <summary>
    /// Fetches each widget's summary from its own source and draws it.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="manager">The widget manager.</param>
    /// <param name="ids">The IDs of the widgets to update.</param>
    private static async Task UpdateAsync(Context context, AppWidgetManager manager, int[] ids)
    {
        SettingsStore settings = new(context);
        LocationService locations = new(context, settings);
        foreach (int id in ids)
        {
            WidgetConfig? config = settings.GetConfig(id);
            WidgetData data = await GetDataAsync(config, locations);
            RemoteViews views = new(context.PackageName, Resource.Layout.widget);
            views.SetTextViewText(Resource.Id.widget_title, data.Title);
            views.SetTextViewText(Resource.Id.widget_value, data.Value);
            views.SetTextViewText(Resource.Id.widget_subtitle, data.Subtitle);
            if (config is not null)
            {
                views.SetOnClickPendingIntent(Resource.Id.widget_root, PendingIntent.GetActivity(context, id, new Intent(Intent.ActionView, AndroidUri.Parse(config.BaseUrl)), PendingIntentFlags.Immutable));
            }
            manager.UpdateAppWidget(id, views);
        }
    }

    /// <summary>
    /// Gets what a widget should show, or a short message if it can't.
    /// </summary>
    /// <param name="config">The widget's setup, or null if it hasn't been set up.</param>
    /// <param name="locations">The location service.</param>
    private static async Task<WidgetData> GetDataAsync(WidgetConfig? config, LocationService locations)
    {
        WidgetSource? source = config is null ? null : WidgetSources.Find(config.SourceId);
        if (config is null || source is null)
        {
            return new WidgetData { Title = "Tidgets", Value = "--", Subtitle = "Not Set Up" };
        }
        (double Latitude, double Longitude)? location = source.UsesLocation ? locations.GetLocation() : null;
        if (source.UsesLocation && location is null)
        {
            return new WidgetData { Title = source.Name, Value = "--", Subtitle = "Location Unavailable" };
        }
        try
        {
            return await WidgetClient.GetAsync(source, config, location, TimeZoneInfo.Local.Id, CancellationToken.None);
        }
        catch (Exception)
        {
            return new WidgetData { Title = source.Name, Value = "--", Subtitle = "Update Failed" };
        }
    }
}