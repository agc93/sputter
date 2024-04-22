using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sputter.Composition;

public interface IPluginBuilder<T> {
	T AddSearchPath(string path);
	T AddSearchPaths(IEnumerable<string> paths);
	T UseConfiguration(IConfiguration configuration);
	T UseConfiguration(IConfigurationSection section);
	T UseLogger(ILogger logger);
	IEnumerable<PluginLoader> Build();
}
