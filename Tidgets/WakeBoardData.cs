namespace Tidgets;

/// <summary>
/// What WakeBoard returns for its widget.
/// </summary>
public class WakeBoardData
{
    /// <summary>
    /// The number of machines that are online.
    /// </summary>
    public int Online { get; set; }

    /// <summary>
    /// The number of machines that are offline.
    /// </summary>
    public int Offline { get; set; }
}