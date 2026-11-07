using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using System.Globalization;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The Forgettable widget, showing the next item due.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_forgettable", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_forgettable")]
public class ForgettableWidget : SourceWidget
{
    /// <summary>
    /// The Forgettable source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.Forgettable;

    /// <summary>
    /// The Forgettable layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_forgettable;

    /// <summary>
    /// Leaves out the status line, as it doesn't fit in a widget one cell tall.
    /// </summary>
    protected override bool ShowsStatus => false;

    /// <summary>
    /// Shows the next item's type and how long is left, coloured by urgency.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">Forgettable's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        ForgettableData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.ForgettableData) ?? new ForgettableData();
        if (data.Next is null)
        {
            views.SetTextViewText(Resource.Id.widget_type, "All Clear");
            views.SetTextViewText(Resource.Id.widget_due, string.Empty);
            return;
        }
        int days = data.Next.DueDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber;
        views.SetTextViewText(Resource.Id.widget_type, data.Next.Type);
        views.SetTextViewText(Resource.Id.widget_due, When(days));
        views.SetTextColor(Resource.Id.widget_due, Color.ParseColor(days < 0 ? "#FF6B6B" : days <= 14 ? "#FFC857" : "#6BCB77"));
    }

    /// <summary>
    /// Describes the time left in the most readable unit, matching Forgettable's own format, e.g. 5d, 1.9w, 3.5m, or -2w when overdue.
    /// </summary>
    /// <param name="days">The days until the due date, negative if overdue.</param>
    private static string When(int days)
    {
        if (days == 0)
        {
            return "Today";
        }
        int size = Math.Abs(days);
        string amount = size < 7 ? Unit(size, "d") : size < 30 ? Unit(size / 7.0, "w") : size < 365 ? Unit(size / 30.44, "m") : Unit(size / 365.25, "y");
        return days < 0 ? $"-{amount}" : amount;
    }

    /// <summary>
    /// Formats an amount to one decimal place followed by its unit letter.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="unit">The unit letter.</param>
    private static string Unit(double amount, string unit)
    {
        return string.Create(CultureInfo.InvariantCulture, $"{amount:0.#}{unit}");
    }
}