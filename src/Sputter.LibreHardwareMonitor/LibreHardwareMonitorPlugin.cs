using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sputter.Composition;
using Sputter.Core;
using ZiggyCreatures.Caching.Fusion;

namespace Sputter.LibreHardwareMonitor;

public class LibreHardwareMonitorPlugin : IPlugin {
	public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration? configuration) {
		services.AddTransient<IDriveSensorAdapter, LibreHardwareMonitorAdapter>();
		services.AddFusionCache(LibreHardwareMonitorAdapter.AdapterName).TryWithAutoSetup().WithOptions(o => { }).WithDefaultEntryOptions(e => {
			e.Duration = TimeSpan.FromSeconds(60);
			e.EagerRefreshThreshold = 0.8F;
		});
		return services;
	}
}
