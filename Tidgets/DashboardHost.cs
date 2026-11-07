namespace Tidgets;

/// <summary>
/// One host's counts in the Dashboard widget.
/// </summary>
public class DashboardHost
{
    /// <summary>
    /// The host's short name, such as Ty for Tyrion.
    /// </summary>
    public string Short { get; set; } = string.Empty;

    /// <summary>
    /// The number of the host's monitors that are online.
    /// </summary>
    public int Online { get; set; }

    /// <summary>
    /// The number of the host's monitors that are offline.
    /// </summary>
    public int Offline { get; set; }
}