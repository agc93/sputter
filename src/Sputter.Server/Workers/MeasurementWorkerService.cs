
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

	public Task StartAsync(CancellationToken cancellationToken) {
		logger.LogInformation("Timed Hosted Service running.");
		_cancellationToken = cancellationToken;
		if (serverConfig.CurrentValue.AutoMeasureInterval != null && serverConfig.CurrentValue.AutoMeasureInterval.Value > 0) {
			var delay = serverConfig.CurrentValue.AutoMeasureInterval.Value;
			logger.LogDebug("Configuring initial timer for interval of {Interval}s", delay);
			_timer = new Timer(RunMeasurementInterval, 
				state: null, 
				TimeSpan.FromSeconds(Math.Max(20, delay*0.2)),
				TimeSpan.FromSeconds(delay));
		}
		_monitor = serverConfig.OnChangeDedup((c) => {
				logger.LogInformation("Configuration change detected, reloading measurement timer!");
				_timer?.Dispose();
				if (c.AutoMeasureInterval != null && c.AutoMeasureInterval.Value > 0) {
					var delay = c.AutoMeasureInterval.Value;
					logger.LogDebug("Configuring timer for new interval of {Interval}s", delay);
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
		logger.LogTrace("Running scheduled measurement from worker service");
		var driveReader = services.GetService<DriveSpecificationReader>();
		await using var scope = services.CreateAsyncScope();
		var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
		if (driveReader != null) {
			logger.LogTrace("Using template reader to read drive specifications");
			var templates = driveReader.GetSpecifications();
			logger.LogDebug("Timed measurement worker loaded {TemplateCount} templates", templates.Count);
			var _ = await mediator.MeasureDrives(templates, _cancellationToken, templates != null && templates.Count > 0);
		} else if (serverConfig.CurrentValue.AllowPublishingAll) {
			logger.LogDebug("Timed measurement worker publishing all discovered drives...");
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
