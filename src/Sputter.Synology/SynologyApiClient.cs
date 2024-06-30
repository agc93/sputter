using Flurl;
using Synology.Api.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sputter.Synology;

public class SynologyApiClient(SynologyConfiguration configuration) {

	protected SynologyClient Client => _client ??=
		new SynologyClient(configuration.Host);

	private SynologyClient? _client;
	
	protected static HttpClient Http => _http ??=
		new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) });

	private static HttpClient? _http;
	
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) {
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
		NumberHandling = JsonNumberHandling.AllowReadingFromString,
		WriteIndented = true,
	};

	public async Task<bool> Initialize() {
		var resp = await Client.InfoApi().QueryAsync();
		if (resp.InfoApi != null) {
			var otp = "";
			await Client.LoginAsync(configuration.User, configuration.Password, otp);
			return Client.IsLoggedIn;
		}
		return false;
	}

	public async Task<IEnumerable<SynologyDiskInfo>> GetDisks() {
		if ((!Client.IsLoggedIn) || string.IsNullOrWhiteSpace(Client.Session?.Sid)) {
			var otp = "";
			await Client.LoginAsync(configuration.User, configuration.Password, otp);
		}
		var u = new Url(new Uri(configuration.Host))
			.AppendPathSegments("webapi", "entry.cgi")
			.AppendQueryParam("api", "SYNO.Storage.CGI.Storage")
			.AppendQueryParam("version", "1")
			.AppendQueryParam("method", "load_info")
			.AppendQueryParam("_sid", Client.Session.Sid);
		
		var uri = new UriBuilder(new Uri(configuration.Host)) { Path = "/webapi/entry.cgi", };
		// var resp = await Http.GetFromJsonAsync<ApiResponse<SynologyDiskResponse>>(uri.ToString());
		var resp = await Http.GetStringAsync(u.ToString());
		var data = JsonSerializer.Deserialize<ApiResponse<SynologyDiskResponse>>(resp, JsonOptions);
		return data?.Data?.Disks ?? [];
	}
}