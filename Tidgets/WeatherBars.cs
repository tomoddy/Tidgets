using Android.Graphics;

namespace Tidgets;

/// <summary>
/// Draws the temperature range bars for the daily forecast.
/// </summary>
public static class WeatherBars
{
    /// <summary>
    /// The temperatures and colours the bar's gradient passes through.
    /// </summary>
    private static readonly (double Temperature, int Red, int Green, int Blue)[] Stops = [(-5, 0, 176, 255), (5, 0, 229, 255), (10, 0, 230, 118), (15, 255, 234, 0), (20, 255, 145, 0), (28, 255, 61, 0)];

    /// <summary>
    /// Draws a bar showing a day's low to high within the week's range, coloured by temperature.
    /// </summary>
    /// <param name="low">The day's low in °C.</param>
    /// <param name="high">The day's high in °C.</param>
    /// <param name="min">The lowest temperature in the week.</param>
    /// <param name="max">The highest temperature in the week.</param>
    public static Bitmap Create(double low, double high, double min, double max)
    {
        int width = 400;
        int height = 16;
        Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888!)!;
        using Canvas canvas = new(bitmap);
        using Paint paint = new(PaintFlags.AntiAlias);
        paint.Color = new Color(unchecked((int)0x33FFFFFF));
        canvas.DrawRoundRect(0, 0, width, height, height / 2f, height / 2f, paint);
        double range = Math.Max(max - min, 1);
        float start = (float)(width * (low - min) / range);
        float end = Math.Max((float)(width * (high - min) / range), start + height);
        if (end > width)
        {
            start -= end - width;
            end = width;
        }
        paint.SetShader(new LinearGradient(start, 0, end, 0, ColorFor(low), ColorFor(high), Shader.TileMode.Clamp!));
        canvas.DrawRoundRect(start, 0, end, height, height / 2f, height / 2f, paint);
        return bitmap;
    }

    /// <summary>
    /// Gets the colour for a temperature, blending between the stops.
    /// </summary>
    /// <param name="temperature">The temperature in °C.</param>
    private static Color ColorFor(double temperature)
    {
        if (temperature <= Stops[0].Temperature)
        {
            return Color.Rgb(Stops[0].Red, Stops[0].Green, Stops[0].Blue);
        }
        for (int index = 1; index < Stops.Length; index++)
        {
            if (temperature <= Stops[index].Temperature)
            {
                double fraction = (temperature - Stops[index - 1].Temperature) / (Stops[index].Temperature - Stops[index - 1].Temperature);
                return Color.Rgb((int)(Stops[index - 1].Red + (Stops[index].Red - Stops[index - 1].Red) * fraction), (int)(Stops[index - 1].Green + (Stops[index].Green - Stops[index - 1].Green) * fraction), (int)(Stops[index - 1].Blue + (Stops[index].Blue - Stops[index - 1].Blue) * fraction));
            }
        }
        return Color.Rgb(Stops[^1].Red, Stops[^1].Green, Stops[^1].Blue);
    }
}