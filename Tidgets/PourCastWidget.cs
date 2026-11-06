using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using System.Globalization;
using System.Text.Json;

namespace Tidgets;

/// <summary>
/// The PourCast weather widget, with the current conditions, the next few hours and the next few days.
/// </summary>
[BroadcastReceiver(Label = "@string/widget_pourcast", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_info_pourcast")]
public class PourCastWidget : SourceWidget
{
    /// <summary>
    /// The PourCast source.
    /// </summary>
    protected override WidgetSource Source => WidgetSources.PourCast;

    /// <summary>
    /// The PourCast layout.
    /// </summary>
    protected override int Layout => Resource.Layout.widget_pourcast;

    /// <summary>
    /// Draws the header, the hourly strip and the daily forecast.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="json">PourCast's response.</param>
    protected override void Draw(Context context, RemoteViews views, string json)
    {
        PourCastData data = JsonSerializer.Deserialize(json, WidgetJsonContext.Default.PourCastData) ?? new PourCastData();
        views.SetTextViewText(Resource.Id.widget_place, data.Place);
        views.SetTextViewText(Resource.Id.widget_temperature, WeatherFormat.Degrees(data.Temperature));
        views.SetTextViewText(Resource.Id.widget_icon, WeatherFormat.Icon(data.Condition, data.IsDay));
        views.SetTextViewText(Resource.Id.widget_condition, WeatherFormat.Words(data.Condition));
        views.SetTextViewText(Resource.Id.widget_highlow, $"H:{WeatherFormat.Degrees(data.High)} L:{WeatherFormat.Degrees(data.Low)}");
        views.SetTextViewText(Resource.Id.widget_rain, WeatherFormat.Rain(data.IsRaining, data.RainStart, data.RainEnd));
        views.RemoveAllViews(Resource.Id.widget_hourly);
        for (int index = 0; index < Math.Min(data.Hourly.Count, 6); index++)
        {
            PourCastHour hour = data.Hourly[index];
            RemoteViews item = new(context.PackageName, Resource.Layout.widget_weather_hour);
            item.SetTextViewText(Resource.Id.hour_time, index == 0 ? "Now" : hour.Time.ToLocalTime().ToString("h tt", CultureInfo.InvariantCulture));
            item.SetTextViewText(Resource.Id.hour_icon, WeatherFormat.Icon(hour.Condition, hour.IsDay));
            item.SetTextViewText(Resource.Id.hour_temperature, WeatherFormat.Degrees(hour.Temperature));
            views.AddView(Resource.Id.widget_hourly, item);
        }
        views.RemoveAllViews(Resource.Id.widget_daily);
        List<PourCastDay> days = [.. data.Daily.Where(x => x.Low is not null && x.High is not null)];
        if (days.Count == 0)
        {
            return;
        }
        double min = days.Min(x => x.Low!.Value);
        double max = days.Max(x => x.High!.Value);
        foreach (PourCastDay day in days)
        {
            RemoteViews item = new(context.PackageName, Resource.Layout.widget_weather_day);
            item.SetTextViewText(Resource.Id.day_name, day.Date.ToString("ddd", CultureInfo.InvariantCulture));
            item.SetTextViewText(Resource.Id.day_icon, WeatherFormat.Icon(day.Condition, true));
            item.SetTextViewText(Resource.Id.day_low, WeatherFormat.Degrees(day.Low));
            item.SetTextViewText(Resource.Id.day_high, WeatherFormat.Degrees(day.High));
            item.SetTextViewText(Resource.Id.day_uv, WeatherFormat.Uv(day.UvIndex));
            item.SetTextColor(Resource.Id.day_uv, Color.ParseColor(WeatherFormat.UvColor(day.UvIndex)));
            item.SetImageViewBitmap(Resource.Id.day_bar, WeatherBars.Create(day.Low!.Value, day.High!.Value, min, max));
            views.AddView(Resource.Id.widget_daily, item);
        }
    }
}