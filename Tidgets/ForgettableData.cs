namespace Tidgets;

/// <summary>
/// What Forgettable returns for its widget.
/// </summary>
public class ForgettableData
{
    /// <summary>
    /// The next item due, or null if nothing is due.
    /// </summary>
    public ForgettableItem? Next { get; set; }
}