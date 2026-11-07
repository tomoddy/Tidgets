using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using System.Globalization;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The TzerDashboard widget, showing monthly hours progress, ticket, email and task counts, and the next calendar event.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_tzerdashboard", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_tzerdashboard")]
public class TzerDashboardWidget : SourceWidget
{
    /// <summary>
    /// The TzerDashboard source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.TzerDashboard;

    /// <summary>
    /// The TzerDashboard layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_tzerdashboard;

    /// <summary>
    /// Fills in the hours, the three counts and the next event.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">TzerDashboard's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        TzerDashboardData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.TzerDashboardData) ?? new TzerDashboardData();
        if (data.Hours is null)
        {
            views.SetTextViewText(Resource.Id.widget_hours, "--");
            views.SetTextViewText(Resource.Id.widget_target, string.Empty);
            views.SetTextViewText(Resource.Id.widget_today, string.Empty);
            views.SetTextViewText(Resource.Id.widget_pace, string.Empty);
            views.SetImageViewBitmap(Resource.Id.widget_bar, HoursBar.Create(0, 1, 0));
        }
        else
        {
            views.SetTextViewText(Resource.Id.widget_hours, Hours(data.Hours.Current));
            views.SetTextViewText(Resource.Id.widget_target, $"Month {Hours(data.Hours.Target)}h");
            views.SetTextViewText(Resource.Id.widget_today, $"Today {Hours(data.Hours.Pace)}h");
            views.SetTextViewText(Resource.Id.widget_pace, $"{Format(Math.Abs(data.Hours.DaysAhead))} Days {(data.Hours.DaysAhead >= 0 ? "Ahead" : "Behind")}");
            views.SetTextColor(Resource.Id.widget_pace, Color.ParseColor(data.Hours.DaysAhead >= 0 ? "#6BCB77" : "#FFC857"));
            views.SetImageViewBitmap(Resource.Id.widget_bar, HoursBar.Create(data.Hours.Current, data.Hours.Target, data.Hours.Pace));
        }
        views.SetTextViewText(Resource.Id.widget_tickets, Count(data.Tickets));
        views.SetTextViewText(Resource.Id.widget_unread, Count(data.Unread));
        views.SetTextViewText(Resource.Id.widget_tasks, Count(data.Tasks));
        views.SetTextViewText(Resource.Id.widget_event_time, data.NextEvent is null ? string.Empty : When(data.NextEvent.Start.ToLocalTime()));
        views.SetTextViewText(Resource.Id.widget_event_title, data.NextEvent is null ? "No Events" : data.NextEvent.Title);
    }

    /// <summary>
    /// Formats a number to one decimal place, dropping a trailing zero.
    /// </summary>
    /// <param name="value">The number.</param>
    private static string Format(double value)
    {
        return string.Create(CultureInfo.InvariantCulture, $"{value:0.#}");
    }

    /// <summary>
    /// Formats hours to two decimal places.
    /// </summary>
    /// <param name="value">The hours.</param>
    private static string Hours(double value)
    {
        return string.Create(CultureInfo.InvariantCulture, $"{value:0.00}");
    }

    /// <summary>
    /// Formats a count, or dashes if it couldn't be fetched.
    /// </summary>
    /// <param name="count">The count, or null.</param>
    private static string Count(int? count)
    {
        return count?.ToString(CultureInfo.InvariantCulture) ?? "--";
    }

    /// <summary>
    /// Formats an event start as the time, with the day if it isn't today.
    /// </summary>
    /// <param name="start">The start in local time.</param>
    private static string When(DateTimeOffset start)
    {
        return start.ToString(start.Date == DateTime.Today ? "HH:mm" : "ddd HH:mm", CultureInfo.InvariantCulture);
    }
}