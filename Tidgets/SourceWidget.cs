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
    /// The layout resource for this widget. It needs a view with the ID widget_root, and widget_status too if the widget shows its status.
    /// </summary>
    protected abstract int Layout { get; }

    /// <summary>
    /// Whether the widget has a status line for problems such as No Connection. Widgets one cell tall leave it out, as it doesn't fit.
    /// </summary>
    protected virtual bool ShowsStatus => true;

    /// <summary>
    /// Fills in the widget's views from the JSON the site returned.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">The site's response.</param>
    protected abstract void Draw(Context context, RemoteViews views, string json);

    /// <summary>
    /// Hands the update to the refresh job, which runs once there's a network connection and isn't cut short like a broadcast.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="appWidgetManager">The widget manager.</param>
    /// <param name="appWidgetIds">The IDs of the widgets to update.</param>
    public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
    {
        base.OnUpdate(context, appWidgetManager, appWidgetIds);
        if (context is null)
        {
            return;
        }
        RefreshScheduler.Schedule(context);
        RefreshJob.Schedule(context);
    }

    /// <summary>
    /// Stops the repeating refresh when the last widget of any source is removed.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnDisabled(Context? context)
    {
        base.OnDisabled(context);
        if (context is not null)
        {
            RefreshScheduler.CancelIfUnused(context);
        }
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
    /// Fetches and draws every placed widget of this source. Called by the refresh job.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>True if any widget couldn't reach its site, so the job should try again later.</returns>
    public async Task<bool> UpdateAllAsync(Context context)
    {
        int[] ids = RefreshScheduler.GetWidgetIds(context, Source);
        AppWidgetManager? manager = AppWidgetManager.GetInstance(context);
        if (ids.Length == 0 || manager is null)
        {
            return false;
        }
        SettingsStore settings = new(context);
        LocationService locations = new(context, settings);
        bool failed = false;
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
                bool fetchFailed;
                (json, status, fetchFailed) = await FetchAsync(config, json, locations, settings);
                failed |= fetchFailed;
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
            if (ShowsStatus)
            {
                views.SetTextViewText(Resource.Id.widget_status, status ?? string.Empty);
                views.SetViewVisibility(Resource.Id.widget_status, status is null ? ViewStates.Gone : ViewStates.Visible);
            }
            if (config is not null)
            {
                views.SetOnClickPendingIntent(Resource.Id.widget_root, PendingIntent.GetActivity(context, id, new Intent(Intent.ActionView, AndroidUri.Parse(config.BaseUrl)), PendingIntentFlags.Immutable));
            }
            manager.UpdateAppWidget(id, views);
        }
        return failed;
    }

    /// <summary>
    /// Fetches the widget's data, retrying once. On failure it returns the last good data, why it failed and when it last worked.
    /// </summary>
    /// <param name="config">The widget's setup.</param>
    /// <param name="cached">The last good response, if any.</param>
    /// <param name="locations">The location service.</param>
    /// <param name="settings">The settings store, for remembering the response.</param>
    private async Task<(string? Json, string? Status, bool Failed)> FetchAsync(WidgetConfig config, string? cached, LocationService locations, SettingsStore settings)
    {
        WidgetSource source = Source;
        (double Latitude, double Longitude)? location = source.UsesLocation ? locations.GetLocation() : null;
        if (source.UsesLocation && location is null)
        {
            return (cached, "Location Unavailable", false);
        }
        Exception? failure = null;
        for (int attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                string json = await WidgetClient.GetAsync(source, config, location, TimeZoneInfo.Local.Id, CancellationToken.None);
                settings.SaveData(config.WidgetId, json);
                return (json, null, false);
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
        DateTime? updated = settings.GetUpdated(config.WidgetId);
        string status = updated is null ? Describe(failure!) : $"{Describe(failure!)} · {(updated.Value.Date == DateTime.Today ? updated.Value.ToString("HH:mm") : updated.Value.ToString("ddd HH:mm"))}";
        return (cached, status, true);
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