namespace Tidgets;

/// <summary>
/// What StockWise returns for its widget.
/// </summary>
public class StockWiseData
{
    /// <summary>
    /// Items past their expiry date.
    /// </summary>
    public List<StockWiseItem> Expired { get; set; } = [];

    /// <summary>
    /// Items expiring soon.
    /// </summary>
    public List<StockWiseItem> Expiring { get; set; } = [];
}