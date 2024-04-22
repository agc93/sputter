using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sputter.Composition;
using Sputter.Core;
using Sputter.DBus;
using Sputter.HWMon;
using Sputter.Messaging;
using Sputter.MQTT;
using Sputter.MQTT.HomeAssistant;
using Sputter.Scrutiny;
using Sputter.Server;
using Sputter.Server.Configuration;
using Sputter.Server.Messaging;
using Sputter.Server.Workers;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(routeOptions =>
{
	routeOptions.SetParameterPolicy("adapter", typeof(AdapterRouteConstraint));
	//routeOptions.ConstraintMap.Add("adapter", typeof(AdapterRouteConstraint));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#if DEBUG
builder.Configuration.AddHomeAssistantAddOnConfiguration("addon.json");
#endif
builder.Configuration.AddHomeAssistantAddOnConfiguration();
builder.Configuration.AddJsonFile("sputter.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

var caching = builder.Configuration.GetSection("Caching").Get<CachingConfiguration>() ?? new CachingConfiguration();

builder.Services.AddSputterDefaults([typeof(DBusAdapter), typeof(HWMonAdapter), typeof(ScrutinyApiAdapter)]);

builder.Services.AddSputterPlugins([typeof(ISensorExtractor)]);

builder.Services.AddFusionCache(ScrutinyApiAdapter.AdapterName).TryWithAutoSetup().WithOptions(o => { }).WithDefaultEntryOptions(e => {
	e.Duration = TimeSpan.FromSeconds((builder.Configuration.GetSection("Scrutiny").Get<ScrutinyConfiguration>())?.CacheLifetime ?? 0);
	e.EagerRefreshThreshold = 0.8F;
});

//I'm hoping that loading the HA target before the publish target will fix weird behaviour around HA marking devices as Unavailable
builder.Services.AddScoped<IPublishTarget, HomeAssistantPublishTarget>();
builder.Services.AddScoped<IPublishTarget>(p => new MQTTPublishTarget(p.GetService<IOptionsSnapshot<MQTTConfiguration>>(), p.GetService<ILogger<MQTTPublishTarget>>()));

builder.Services.AddMediatR(m => {
	m.AddSputterComponents(s => s.EnableAggregation());
	m.AddBehavior<IPipelineBehavior<DriveMeasurementRequest, IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>>, FilterParsingBehaviour>();
	m.AddBehavior<IPipelineBehavior<DriveDiscoveryRequest, IEnumerable<DriveEntity>>, FilterParsingBehaviour>();
	if (caching.Lifetime > 0) {
		m.AddBehavior<IPipelineBehavior<DriveMeasurementRequest, IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>>, MeasurementCachingBehaviour>();
		m.AddBehavior<IPipelineBehavior<DriveDiscoveryRequest, IEnumerable<DriveEntity>>, MeasurementCachingBehaviour>();
	}
});

builder.Services.Configure<ServerConfiguration>(
	builder.Configuration.GetSection("Sputter"));

builder.Services.Configure<ScrutinyConfiguration>(
	builder.Configuration.GetSection("Scrutiny"));

var mqttOpts = builder.Configuration.GetSection("MQTT").Get<MQTTConfiguration>();
if (mqttOpts != null)
{
	Console.WriteLine("Enabling MQTT configuration!");
	builder.Services.Configure<MQTTConfiguration>(builder.Configuration.GetSection("MQTT"));
	
	// We don't need to conditionally load the target here since it no-ops if it's not enabled in the config
	// This way we can enable/disable HA integration without restarting the server
	//if (mqttOpts.EnableHomeAssistant == true)
	//{
	//    Console.WriteLine("Enabling Home Assistant Discovery integration!");
	//    builder.Services.AddScoped<IPublishTarget, HomeAssistantPublishTarget>();
	//}
} else
{
	Console.WriteLine("MQTT configuration not found, skipping MQTT setup!");
}



builder.Services.AddSingleton<DriveSpecificationReader>();
builder.Services.AddSingleton<FilterTemplateParser>();

builder.Services.AddFusionCache().TryWithAutoSetup().WithOptions(o => {}).WithDefaultEntryOptions(e => {
	e.Duration = TimeSpan.FromSeconds(caching.Lifetime);
	e.EagerRefreshThreshold = 0.8F;
});

builder.Services.AddHostedService<MQTTWorkerService>();
builder.Services.AddHostedService<MeasurementWorkerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
