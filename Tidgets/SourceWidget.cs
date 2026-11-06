using Android.Appwidget;
using Android.Content;
using Android.Util;
using Android.Views;
using System.Text.Json;
using AndroidUri = Android.Net.Uri;

namespace Tidgets;

/// <summary>
/// Base for the home screen widgets, one per source. It handles setup, fetching, retries and failures, so each widget only draws its own data.
/// </summary>
public abstract class SourceWidget : AppWidgetProvider
{
    /// <summary>
    /// The source this widget shows.
    /// </summary>
    protected abstract WidgetSource Source { get; }

    /// <summary>
    /// The layout resource for this widget. It needs views with the IDs widget_root and widget_status.
    /// </summary>
    protected abstract int Layout { get; }

    /// <summary>
    /// Fills in the widget's views from the JSON the site returned.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">The site's response.</param>
    protected abstract void Draw(Context context, RemoteViews views, string json);

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
    /// Asks one widget to update now.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="source">The source the widget shows.</param>
    /// <param name="widgetId">The Android ID of the widget.</param>
    public static void Refresh(Context context, WidgetSource source, int widgetId)
    {
        int[] ids = [widgetId];
        Intent intent = new(context, source.ProviderType);
        intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
        intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
        context.SendBroadcast(intent);
    }

    /// <summary>
    /// Fetches and draws each widget, showing the last good data with a status line if it can't.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="manager">The widget manager.</param>
    /// <param name="ids">The IDs of the widgets to update.</param>
    private async Task UpdateAsync(Context context, AppWidgetManager manager, int[] ids)
    {
        SettingsStore settings = new(context);
        LocationService locations = new(context, settings);
        foreach (int id in ids)
        {
            WidgetConfig? config = settings.GetConfig(id);
            string? json = settings.GetData(id);
            string? status = null;
            if (config is null)
            {
                status = "Not Set Up";
            }
            else
            {
                (json, status) = await FetchAsync(config, json, locations, settings);
            }
            RemoteViews views = new(context.PackageName, Layout);
            if (json is not null)
            {
                try
                {
                    Draw(context, views, json);
                }
                catch (JsonException exception)
                {
                    Log.Warn("Tidgets", $"Couldn't read the data for {Source.Name}: {exception}");
                    status = "Bad Data";
                }
            }
            views.SetTextViewText(Resource.Id.widget_status, status ?? string.Empty);
            views.SetViewVisibility(Resource.Id.widget_status, status is null ? ViewStates.Gone : ViewStates.Visible);
            if (config is not null)
            {
                views.SetOnClickPendingIntent(Resource.Id.widget_root, PendingIntent.GetActivity(context, id, new Intent(Intent.ActionView, AndroidUri.Parse(config.BaseUrl)), PendingIntentFlags.Immutable));
            }
            manager.UpdateAppWidget(id, views);
        }
    }

    /// <summary>
    /// Fetches the widget's data, retrying once. On failure it returns the last good data and why it failed.
    /// </summary>
    /// <param name="config">The widget's setup.</param>
    /// <param name="cached">The last good response, if any.</param>
    /// <param name="locations">The location service.</param>
    /// <param name="settings">The settings store, for remembering the response.</param>
    private async Task<(string? Json, string? Status)> FetchAsync(WidgetConfig config, string? cached, LocationService locations, SettingsStore settings)
    {
        WidgetSource source = Source;
        (double Latitude, double Longitude)? location = source.UsesLocation ? locations.GetLocation() : null;
        if (source.UsesLocation && location is null)
        {
            return (cached, "Location Unavailable");
        }
        Exception? failure = null;
        for (int attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                string json = await WidgetClient.GetAsync(source, config, location, TimeZoneInfo.Local.Id, CancellationToken.None);
                settings.SaveData(config.WidgetId, json);
                return (json, null);
            }
            catch (Exception exception)
            {
                failure = exception;
                Log.Warn("Tidgets", $"Attempt {attempt} for {source.Name} failed: {exception}");
                if (attempt == 1)
                {
                    await Task.Delay(3000);
                }
            }
        }
        return (cached, Describe(failure!));
    }

    /// <summary>
    /// Describes a failure in a few words for the widget.
    /// </summary>
    /// <param name="exception">The failure.</param>
    private static string Describe(Exception exception)
    {
        return exception switch
        {
            TaskCanceledException => "Timed Out",
            HttpRequestException { StatusCode: { } code } => $"Server Error {(int)code}",
            HttpRequestException => "No Connection",
            _ => "Update Failed"
        };
    }
}