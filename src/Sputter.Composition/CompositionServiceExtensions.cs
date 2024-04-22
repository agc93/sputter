// ReSharper disable once CheckNamespace
using Microsoft.Extensions.Configuration;
using Sputter.Composition;

namespace Microsoft.Extensions.DependencyInjection; 

public static class CompositionServiceExtensions {
	public static IServiceCollection AddPlugins<TPlugin>(this IServiceCollection services,
		Func<PluginBuilder<TPlugin>, PluginBuilder<TPlugin>>? func = null) where TPlugin : IPlugin {
		var builder = new PluginBuilder<TPlugin>();
		if (func != null) {
			builder = func(builder);
		}

		var loaders = builder.BuildServices(services);
		return loaders;
	}

	public static IServiceCollection AddPlugins<TPlugin, TOption>(this IServiceCollection services,
		IConfiguration config, Func<PluginBuilder<TPlugin, TOption>, PluginBuilder<TPlugin, TOption>>? func = null)
		where TPlugin : IPlugin where TOption : IPluginOptions, new() {
		var builder = new PluginBuilder<TPlugin, TOption>();
		if (func != null) {
			builder = func(builder);
		}

		var loaders = builder.BuildServices(services, config);
		return loaders;
	}
}