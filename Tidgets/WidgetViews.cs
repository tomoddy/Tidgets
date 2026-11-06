using Android.Content;
using Android.Graphics;

namespace Tidgets;

/// <summary>
/// Helpers for drawing lists on widgets.
/// </summary>
public static class WidgetViews
{
    /// <summary>
    /// Adds a coloured heading and its lines to a container, cutting a long list short with a count of the rest.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="views">The widget's views.</param>
    /// <param name="containerId">The ID of the container to add to.</param>
    /// <param name="title">The heading.</param>
    /// <param name="status">The heading's colour: good, warn or bad, or null for white.</param>
    /// <param name="items">The lines under the heading.</param>
    /// <param name="maxItems">The most lines to show.</param>
    public static void AddSection(Context context, RemoteViews views, int containerId, string title, string? status, IReadOnlyList<string> items, int maxItems = 4)
    {
        RemoteViews header = new(context.PackageName, Resource.Layout.widget_section_header);
        header.SetTextViewText(Resource.Id.section_title, title);
        header.SetTextColor(Resource.Id.section_title, Color.ParseColor(status switch { "bad" => "#FF6B6B", "warn" => "#FFC857", "good" => "#6BCB77", _ => "#FFFFFF" }).ToArgb());
        views.AddView(containerId, header);
        List<string> shown = [.. items.Take(maxItems)];
        if (items.Count > maxItems)
        {
            shown.Add($"+{items.Count - maxItems} More");
        }
        foreach (string item in shown)
        {
            RemoteViews row = new(context.PackageName, Resource.Layout.widget_row);
            row.SetTextViewText(Resource.Id.row_text, item);
            views.AddView(containerId, row);
        }
    }
}