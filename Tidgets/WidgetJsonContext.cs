using System.Text.Json.Serialization;

namespace Tidgets;

/// <summary>
/// Source-generated JSON metadata, so deserialising still works when the app is trimmed.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(WidgetData))]
public partial class WidgetJsonContext : JsonSerializerContext
{
}