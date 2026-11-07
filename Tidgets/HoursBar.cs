using Android.Graphics;

namespace Tidgets;

/// <summary>
/// Draws the monthly hours progress bar.
/// </summary>
public static class HoursBar
{
    /// <summary>
    /// Draws a bar filled to the hours logged, green if on pace or amber if behind, with a marker where pace is today.
    /// </summary>
    /// <param name="current">The hours logged so far.</param>
    /// <param name="target">The hours targeted for the month.</param>
    /// <param name="pace">The hours expected by today.</param>
    public static Bitmap Create(double current, double target, double pace)
    {
        int width = 600;
        int height = 20;
        Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888!)!;
        using Canvas canvas = new(bitmap);
        using Paint paint = new(PaintFlags.AntiAlias);
        paint.Color = new Color(unchecked((int)0x33FFFFFF));
        canvas.DrawRoundRect(0, 0, width, height, height / 2f, height / 2f, paint);
        double range = Math.Max(target, 1);
        float fill = (float)(width * Math.Clamp(current / range, 0, 1));
        paint.Color = Color.ParseColor(current >= pace ? "#6BCB77" : "#FFC857");
        canvas.DrawRoundRect(0, 0, Math.Max(fill, height), height, height / 2f, height / 2f, paint);
        float marker = (float)(width * Math.Clamp(pace / range, 0, 1));
        paint.Color = Color.White;
        canvas.DrawRect(Math.Clamp(marker - 2, 0, width - 4), 0, Math.Clamp(marker + 2, 4, width), height, paint);
        return bitmap;
    }
}