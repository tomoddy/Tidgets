namespace Tidgets;

/// <summary>
/// The next calendar event from TzerDashboard.
/// </summary>
public class TzerDashboardEvent
{
    /// <summary>
    /// The event's title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// When the event starts.
    /// </summary>
    public DateTimeOffset Start { get; set; }
}