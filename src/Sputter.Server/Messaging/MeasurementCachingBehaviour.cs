using MediatR;
using Sputter.Core;
using Sputter.Messaging;
using ZiggyCreatures.Caching.Fusion;

namespace Sputter.Server.Messaging;

public class MeasurementCachingBehaviour : IPipelineBehavior<DriveMeasurementRequest, IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>>, IPipelineBehavior<DriveDiscoveryRequest, IEnumerable<DriveEntity>> {
    private readonly IFusionCache _cache;
    private readonly ILogger<MeasurementCachingBehaviour> _logger;

    public MeasurementCachingBehaviour(IFusionCache fusionCache, ILogger<MeasurementCachingBehaviour> logger) {
        _cache = fusionCache;
        _logger = logger;
    }
    public async Task<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> Handle(DriveMeasurementRequest request, RequestHandlerDelegate<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> next, CancellationToken cancellationToken) {
        var checksum = request.GetChecksum();
        var res = await _cache.GetOrSetAsync(checksum, async _ => {
            _logger?.LogDebug("Could not get measurements from cache for '{checksum}'!", checksum);
            return await next();
        }, token: cancellationToken) ?? await next();
        return res;
    }

    public async Task<IEnumerable<DriveEntity>> Handle(DriveDiscoveryRequest request, RequestHandlerDelegate<IEnumerable<DriveEntity>> next, CancellationToken cancellationToken) {
        var driveCheck = request.GetChecksum();
        var drives = await _cache.GetOrSetAsync(driveCheck, async (_) => {
            _logger?.LogDebug("Could not get matching drives from cache for '{checksum}'!", driveCheck);
            var drives = await next();
            return drives;
        }, token: cancellationToken);
        if (drives == null) {
            _logger.LogWarning("Cache failed to retrieve drives, falling back to uncached request");
            return await next();
        }
        return drives;
    }
}
