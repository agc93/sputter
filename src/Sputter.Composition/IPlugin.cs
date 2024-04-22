using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sputter.Composition;

public interface IPlugin {
	IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration? configuration);
}
