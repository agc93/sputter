namespace Sputter.Synology;

public class ApiError {
	public int Code { get; set; }
	public List<ErrorCode> Errors { get; set; } = [];
}