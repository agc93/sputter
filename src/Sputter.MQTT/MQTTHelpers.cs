using Sputter.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sputter.MQTT;

public static class MQTTHelpers {
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = true,
    };

    public static JsonSerializerOptions JsonContextOptions =>
        new(JsonOptions) {
            TypeInfoResolver = SourceGenerationContext.Default
        };

    public static string GetNamedObjectId(this UniqueId id, string name) {
        return $"drv-{id.SerialNumber}-{name.ToObjectId()}";
    }

    public static string ToObjectId(this string input) {
        return string.Join("-", input.Split(" ").Select(w => w.ToLower()));
    }
}
