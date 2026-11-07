using System.Text.Json.Serialization;

namespace Tidgets;

/// <summary>
/// Source-generated JSON metadata, so deserialising still works when the app is trimmed.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(PourCastData))]
[JsonSerializable(typeof(StockWiseData))]
[JsonSerializable(typeof(WakeBoardData))]
[JsonSerializable(typeof(ForgettableData))]
[JsonSerializable(typeof(HomeRunData))]
[JsonSerializable(typeof(TzerDashboardData))]
public partial class WidgetJsonContext : JsonSerializerContext
{
}