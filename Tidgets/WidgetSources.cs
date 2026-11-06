namespace Tidgets;

/// <summary>
/// The predefined sources a widget can be made from. Add a new site here.
/// </summary>
public static class WidgetSources
{
    /// <summary>
    /// Every available source.
    /// </summary>
    public static IReadOnlyList<WidgetSource> All { get; } = [new WidgetSource { Id = "pourcast", Name = "PourCast", DefaultBaseUrl = "https://pc.tzer0m.co.uk", UsesLocation = true }, new WidgetSource { Id = "stockwise", Name = "StockWise", DefaultBaseUrl = "https://sw.tzer0m.co.uk", UsesApiKey = true }];

    /// <summary>
    /// Finds a source by its ID.
    /// </summary>
    /// <param name="id">The source ID.</param>
    public static WidgetSource? Find(string id)
    {
        return All.FirstOrDefault(x => x.Id == id);
    }
}