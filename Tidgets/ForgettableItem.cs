namespace Tidgets;

/// <summary>
/// The next item due in Forgettable.
/// </summary>
public class ForgettableItem
{
    /// <summary>
    /// The item type's friendly name.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The date the item is due.
    /// </summary>
    public DateOnly DueDate { get; set; }
}