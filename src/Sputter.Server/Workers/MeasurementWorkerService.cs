
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sputter.Core;
using Sputter.Server.Configuration;

namespace Sputter.Server.Workers;

public class MeasurementWorkerService(IServiceProvider services, ILogger<MeasurementWorkerService> logger, IOptionsMonitor<ServerConfiguration> serverConfig) : IHostedService, IDisposable {
    private bool disposedValue;
    private Timer? _timer;
    private CancellationToken _cancellationToken;
    private IDisposable? _monitor;
    //private Debouncer _debouncer = new Debouncer(TimeSpan.FromSeconds(3));

    public Task StartAsync(CancellationToken cancellationToken) {
        logger.LogInformation("Timed Hosted Service running.");
        _cancellationToken = cancellationToken;
        if (serverConfig.CurrentValue.AutoMeasureInterval != null && serverConfig.CurrentValue.AutoMeasureInterval.Value > 0) {
            var delay = serverConfig.CurrentValue.AutoMeasureInterval.Value;
            _timer = new Timer(RunMeasurementInterval, 
                state: null, 
                TimeSpan.FromSeconds(Math.Max(20, delay*0.2)),
                TimeSpan.FromSeconds(delay));
        }
        _monitor = serverConfig.OnChangeDedup((c) => {
                logger?.LogInformation("Configuration change detected, reloading measurement timer!");
                _timer?.Dispose();
                if (c.AutoMeasureInterval != null && c.AutoMeasureInterval.Value > 0) {
                    var delay = c.AutoMeasureInterval.Value;
                    logger?.LogDebug("Configuring timer for new interval of {Interval}s", delay);
                    _timer = new Timer(RunMeasurementInterval,
                        state: null,
                        TimeSpan.FromSeconds(Math.Max(20, delay * 0.2)),
                        TimeSpan.FromSeconds(delay));
                }
        });
        return Task.CompletedTask;
    }

    private void RunMeasurementInterval(object? state) {
        // Discard the result
        _ = RunMeasurement();
    }

    private async Task RunMeasurement() {
        var driveReader = services.GetService<DriveSpecificationReader>();
        await using var scope = services.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        if (driveReader != null) {
            var templates = driveReader.GetSpecifications();
            var _ = await mediator.MeasureDrives(templates, _cancellationToken, templates != null && templates.Count > 0);
        } else if (serverConfig.CurrentValue.AllowPublishingAll) {
            var _ = await mediator.MeasureDrives("*", _cancellationToken, publishMeasurements: true);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                _timer?.Dispose();
                _monitor?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
