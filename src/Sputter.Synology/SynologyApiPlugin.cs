using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sputter.Composition;
using Sputter.Core;

namespace Sputter.Synology;

public class SynologyApiPlugin : IPlugin {
	public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration? configuration) {
		if (configuration != null) {
			services.Configure<List<SynologyConfiguration>>(configuration.GetSection("Synology"));
		}
		//TODO: this should potentially be a singletone
		services.AddTransient<IDriveSensorAdapter, SynologyApiAdapter>();
		// services.AddFusionCache(SynologyApiAdapter.AdapterName).TryWithAutoSetup().WithOptions(o => { }).WithDefaultEntryOptions(e => {
		// 	e.Duration = TimeSpan.FromSeconds(60);
		// 	e.EagerRefreshThreshold = 0.8F;
		// });
		return services;
	}
}