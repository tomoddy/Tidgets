namespace Tidgets;

/// <summary>
/// What TzerDashboard returns for its widget. Any part it couldn't fetch is null.
/// </summary>
public class TzerDashboardData
{
    /// <summary>
    /// This month's billable hours progress.
    /// </summary>
    public TzerDashboardHours? Hours { get; set; }

    /// <summary>
    /// The number of open tickets that aren't waiting on the customer.
    /// </summary>
    public int? Tickets { get; set; }

    /// <summary>
    /// The number of unread inbox threads.
    /// </summary>
    public int? Unread { get; set; }

    /// <summary>
    /// The number of tasks due today or overdue.
    /// </summary>
    public int? Tasks { get; set; }

    /// <summary>
    /// The next timed calendar event, or null if there isn't one.
    /// </summary>
    public TzerDashboardEvent? NextEvent { get; set; }
}