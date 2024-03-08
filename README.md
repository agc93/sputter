# Sputter

Sputter is a flexible and configurable API server designed to gather hard drive and SSD temperatures from a variety of sources and make them available through a CLI, HTTP API or MQTT.

## Getting Started

You should start with the docs [available here](https://agc93.github.io/sputter)! The guides included in the documentation should give you enough information to get your own Sputter server running easily.

## Building

Building Sputter locally should be pretty simple. Ensure you have the .NET 8 SDK installed, and `dotnet` available in your `PATH`. 

First, restore required build tools:

```bash
dotnet tool restore
```

Now you can run a build with the Cake script in the repo:

```bash
dotnet cake
# or to build all artifacts
dotnet cake --target=Publish
```

This will build all the component projects and (assuming you used the `Publish` target) create all relevant build artifacts in the `dist/` folder.