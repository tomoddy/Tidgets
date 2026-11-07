namespace Tidgets;

/// <summary>
/// One of today's HomeRun tasks.
/// </summary>
public class HomeRunTask
{
    /// <summary>
    /// The task name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether the task is done today.
    /// </summary>
    public bool Done { get; set; }
}