namespace Tidgets;

/// <summary>
/// What the Dashboard returns for its widget.
/// </summary>
public class DashboardData
{
    /// <summary>
    /// The number of monitors online across every host.
    /// </summary>
    public int Online { get; set; }

    /// <summary>
    /// The number of monitors offline across every host.
    /// </summary>
    public int Offline { get; set; }

    /// <summary>
    /// Each host's counts, ordered by host name.
    /// </summary>
    public List<DashboardHost> Hosts { get; set; } = [];
}