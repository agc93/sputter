using System.Text.Json.Serialization;

namespace Sputter.Scrutiny;

public class ApiResponse<T> where T : class {
    [JsonPropertyName("data")]
    public required T Value { get; set; }
}
