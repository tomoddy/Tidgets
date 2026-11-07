namespace Tidgets;

/// <summary>
/// What HomeRun returns for its widget.
/// </summary>
public class HomeRunData
{
    /// <summary>
    /// Today's tasks.
    /// </summary>
    public List<HomeRunTask> Tasks { get; set; } = [];
}