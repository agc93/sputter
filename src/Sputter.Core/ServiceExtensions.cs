using Microsoft.Extensions.DependencyInjection;
using Sputter.Composition;

namespace Sputter.Core
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddSputterPlugins(
			this IServiceCollection services,
			IEnumerable<Type>? sharedTypes = null) {
				sharedTypes ??= [];
				var allSharedTypes = new Type[] { typeof(IDriveSensorAdapter), typeof(IPublishTarget) }.Concat(sharedTypes);
			services.AddPlugins<IPlugin>(c => c.ShareTypes(allSharedTypes.ToArray()));
			return services;
		}
	}
}