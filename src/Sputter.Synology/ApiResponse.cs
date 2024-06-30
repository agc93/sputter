using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Sputter.Synology;

public class ApiResponse<T> {
	[JsonPropertyName("success")]
	public bool IsSuccess { get; set; }
	public ApiError? Error { get; set; }
	public T? Data { get; set; }
}