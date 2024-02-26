using MediatR;
using Microsoft.AspNetCore.Routing.Template;
using Sputter.Core;
using Sputter.Messaging;

namespace Sputter.Server.Messaging;

public class FilterParsingBehaviour(IServiceProvider services, ILogger<FilterParsingBehaviour> logger) : IPipelineBehavior<DriveMeasurementRequest, IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>>, IPipelineBehavior<DriveDiscoveryRequest, IEnumerable<DriveEntity>> {
    public async Task<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> Handle(DriveMeasurementRequest request, RequestHandlerDelegate<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> next, CancellationToken cancellationToken) {
		var parser = services.GetService<FilterTemplateParser>();
		if (parser != null && !(request.Drives ?? []).Any() && !string.IsNullOrWhiteSpace(request.DriveFilter) && request.EnableDriveDiscovery) {
			logger.LogDebug("Parsing filter as template");
			var template = parser.ParseTemplate(request.DriveFilter);
			if (template == null) {
				// filter parsing failed
				logger?.LogWarning("Failed to process filter as template, falling back to legacy behaviour!");
			} else {
				// filter parsing succeeded, we can use the parsed template instead
				request.DriveFilter = null;
				request.FilterTemplates = [template];
			}
		}
		return await next();
	}

    public async Task<IEnumerable<DriveEntity>> Handle(DriveDiscoveryRequest request, RequestHandlerDelegate<IEnumerable<DriveEntity>> next, CancellationToken cancellationToken) {
		var parser = services.GetService<FilterTemplateParser>();
		if (parser != null && !string.IsNullOrWhiteSpace(request.DriveFilter)) {
			logger.LogDebug("Parsing filter as template");
			var template = parser.ParseTemplate(request.DriveFilter);
			if (template == null) {
				// filter parsing failed
				logger?.LogWarning("Failed to process filter as template, falling back to legacy behaviour!");
			} else {
				// filter parsing succeeded, we can use the parsed template instead
				request.DriveFilter = null;
				request.Templates.Add(template);
			}
		}
		return await next();
    }
}
