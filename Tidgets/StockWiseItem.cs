namespace Tidgets;

/// <summary>
/// One expired or expiring item from StockWise.
/// </summary>
public class StockWiseItem
{
    /// <summary>
    /// The item's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Where the item is stored.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// The item's expiry date.
    /// </summary>
    public DateOnly Expiry { get; set; }
}