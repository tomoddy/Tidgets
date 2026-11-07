namespace Tidgets;

/// <summary>
/// This month's billable hours progress from TzerDashboard.
/// </summary>
public class TzerDashboardHours
{
    /// <summary>
    /// Billable hours logged so far this month.
    /// </summary>
    public double Current { get; set; }

    /// <summary>
    /// Billable hours targeted for the whole month.
    /// </summary>
    public double Target { get; set; }

    /// <summary>
    /// Billable hours expected by today if exactly on pace.
    /// </summary>
    public double Pace { get; set; }

    /// <summary>
    /// How many days ahead (positive) or behind (negative) pace.
    /// </summary>
    public double DaysAhead { get; set; }
}