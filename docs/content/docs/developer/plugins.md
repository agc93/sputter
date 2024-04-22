---
weight: 1105
title: "Plugins"
description: "Building plugins for Sputter"
icon: integration_instructions
lead: ""
date: 2023-01-21T16:13:15+00:00
lastmod: 2023-01-21T16:13:15+00:00
draft: false
images: []
---

## Introduction

Sputter supports a plugin system that allows for loading new data sources (i.e. [adapters](../reference/adapters.md)) and publish targets. To build a plugin, create a new class library, add a reference to `Sputter.Core`, and add a class implementing the `IPlugin` interface (from `Sputter.Composition`). In the `ConfigureServices` method, you can register any services your plugin needs, and also register any types for Sputter to load.

## Adapters

### Adapter Implementation

At its simplest, a new data source is just an implementation of `IDriveSensorAdapter` which looks like this:

```csharp
int Priority { get; }
string Name { get; }
Task<IEnumerable<DriveEntity>> DiscoverDrives(string? filter);
Task<DriveMeasurement?> MeasureDrive(DriveEntity drive);
Task<DriveEntity?> IdentifyDrive(string pathSpec, bool exactMatch = true);
```

Let's look at this in more detail:

##### `Priority`

The `Priority` is only used for aggregating measurements where more than one adapter reads the same drive (where lowest priority wins). The "default" priority is 20, so we recommend using that unless you have a special case.

##### Name

The `Name` property is an identifier used in a few places to uniquely identify your adapter:

- **Routing**: Sputter's HTTP API includes a route to fetch all measurements for an adapter by its `Name`
- **Filters**: Sputter Server includes support for [drive filtering](../server/configuration.md#drive-filters) which can filter drives by which adapter discovers it
- **Source State**: Sputter adds a Source state to every measurement to identify which adapters contributed to a drive measurement

Based on these, the name should be: *URI-safe*, *short* and *only use letters* where possible. For example, the bundled adapters have names of `dbus`, `sysfs`, and `scrutiny`.

##### `DiscoverDrives`

The `DiscoverDrives` method is called by Sputter in several places to gather available drives from all adapters *before* taking any measurements. As such, your adapter should return a collection of discovered drives **that can be measured**. Your adapter can return an empty collection if there are no drives available (for any reason).

{{< alert icon="💡" text="While you can return a `DriveEntity` object, you can also use subclasses if your adapter's discovery process gathers additional information that might be useful later. For example, the DBus adapter has a `DBusEntity` type you can use as a guide." />}}

Your adapter should only return drives that match the given `filter` **if one is provided**. The usual logic is to return drives with either a serial number or model number that matches the given `filter`.

##### `MeasureDrive`

This is the "main" method that Sputter calls after drive discovery has been completed for all adapters. Once Sputter has completed discovery, filtered as necessary, and is ready to measure the temperature of the discovered drives, it will call `MeasureDrive` for **each** "eligible" drive for **each** adapter. This way, multiple adapters can contribute data to the same drive, if applicable.

As such, your adapter's `MeasureDrive` method may be called with a `DriveEntity` that didn't originally come from your adapter, and if your adapter can't find/doesn't match/doesn't understand the `DriveEntity` it is passed, it should fail out quickly by returning `null`.

If your adapter can gather temperatures for the provided `DriveEntity`, it should do so and return a `DriveMeasurement` object for the given drive with any relevant sensor data. Don't worry about uniqueness: Sputter, by default, will aggregate and deduplicate measurements.

Since this method is called **once per discovered drive**, it should be as quick as possible and if your adapter doesn't support the concept of gathering sensor data for only one drive at a time, we recommend caching results with a short lifetime to improve performance (the Scrutiny and LibreHardwareMonitor adapters both do this).

##### `IdentifyDrive`

> This is not currently used by Sputter.

The idea of this API is to be given a drive "specification"/filter/template and deterministically match it to a `DriveEntity` if available. This is part of a feature that's been in occasional development for a while and you can safely throw `NotImplementedException`/`NotSupportedException` or return `null` for now.

### Registration 

Once you've built out your `IDriveSensorAdapter` implementation in a new type, register that type in your plugin type. For example, here is the relevant service registration from the *LibreHardwareMonitor* plugin:

```csharp
public class LibreHardwareMonitorPlugin : IPlugin {
	public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) {
		services.AddTransient<IDriveSensorAdapter, LibreHardwareMonitorAdapter>();
		// trimmed for brevity
		return services;
	}
}
```

## Publish Target

{{< alert context="warning" text="The Publish Target API is not well documented and is not guaranteed stable. Use with caution!" />}}

The Publish Target API is intended for "publishing" results of drive measurements. For example, the [MQTT](../reference/mqtt.md) integration and [Home Assistant](../server/home-assistant/) integration are both implemented as Publish Targets.

Publish Targets are called **after** all the measurements are gathered and will be called with an aggregated set of drive measurements at the end of the measurement process. Note that there is no guarantee of order or measurement recency for publish targets.

Publish Targets are ideal for things like notifications, automations, or for storing drive measurements in another application.

To create a publish target, implement the `IPublishTarget` interface:

```csharp
public class ExamplePublishTarget : IPublishTarget {
	public Task<Result> PublishMeasurement(MeasurementResult result) => throw new NotImplementedException();
}
```

{{< alert text="If your target throws a `NotImplementedException` it will be ignored entirely, other exceptions will be logged as errors." />}}

For Sputter to load your target, you also need to register it in your plugin type:

```csharp
public class ExampleTargetPlugin : IPlugin {
	public IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) {
		services.AddScoped<IPublishTarget, ExamplePublishTarget>();
		return services;
	}
}
```