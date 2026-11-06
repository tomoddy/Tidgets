using JavaClass = Java.Lang.Class;

namespace Tidgets;

/// <summary>
/// The predefined sources, one per widget. Add a new site here and give it its own widget class.
/// </summary>
public static class WidgetSources
{
    /// <summary>
    /// The PourCast weather source.
    /// </summary>
    public static WidgetSource PourCast { get; } = new() { Id = "pourcast", Name = "PourCast", DefaultBaseUrl = "https://pc.tzer0m.co.uk", UsesLocation = true, ProviderType = typeof(PourCastWidget) };

    /// <summary>
    /// The StockWise expiry source.
    /// </summary>
    public static WidgetSource StockWise { get; } = new() { Id = "stockwise", Name = "StockWise", DefaultBaseUrl = "https://sw.tzer0m.co.uk", UsesApiKey = true, ProviderType = typeof(StockWiseWidget) };

    /// <summary>
    /// Every available source.
    /// </summary>
    public static IReadOnlyList<WidgetSource> All { get; } = [PourCast, StockWise];

    /// <summary>
    /// Finds the source whose widget class has the given Java class name.
    /// </summary>
    /// <param name="className">The Java class name of the widget provider.</param>
    public static WidgetSource? FindByProvider(string className)
    {
        return All.FirstOrDefault(x => JavaClass.FromType(x.ProviderType).Name == className);
    }
}