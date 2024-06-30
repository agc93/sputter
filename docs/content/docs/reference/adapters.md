---
weight: 1105
title: "Adapters"
icon: query_stats
description: "Available data sources for disk temperatures."
lead: ""
date: 2023-01-21T16:16:15+00:00
lastmod: 2023-01-21T16:16:15+00:00
draft: false
images: []
toc: true
---

## About Adapters

**Adapters** are the method Sputter uses to discover available drives and gather measurements (including the temperature). In short, Sputter itself (either the CLI or the Server) will poll any available adapters looking for drives, gather measurements ([sensors and states](./sensors.md)) from those drives, aggregate measurements together to prevent duplicates, then return them as appropriate. 

These adapters are loosely coupled to the server to make it easier to add new data sources to enable measuring drives in more contexts.

## Bundled Adapters

Sputter currently includes three default adapters: DBus, HWMon, and Scrutiny.

### DBus

The DBus adapter polls the local DBus system bus for all local drives that expose a SMART temperature sensor. Note that due to limitations in DBus, this will _generally_ only identify SATA drives as DBus only exposes temperatures in the `Ata` object so NVMe drives may not be included.

### HWMon

The HWMon/`sysfs`/`/sys` adapter uses the support for drive temperature sensors integrated into the HWMon subsystem of more recent Linux kernels. This will often discover more drives than the DBus adapter, but relies on your system having a sufficiently recent Linux kernel. The HWMon adapter will use the special device files in the `sysfs`/`/sys` file system to read the drive information for any drives with HWMon-supported sensors. Note that Sputter will only load drives that have temperature sensors, so there might be more devices in `sysfs` that won't be read by Sputter.

### Scrutiny

The Scrutiny adapter uses the API in [Scrutiny](https://github.com/AnalogJ/scrutiny) to collect drives and drive temperatures from a running Scrutiny instance. Unless you have [drive filters](../server/configuration.md#drive-filters) set up, Sputter will collect **any** drives in your Scrutiny instance and return the drives and temperatures just like any locally-discovered drives.

## Plugins

### LibreHardwareMonitor

{{< alert context="info" text="This adapter is distributed only as a plugin and is not bundled with the default server!" />}}

The LibreHardwareMonitor plugin uses the excellent [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) project to read the temperature sensor of local drives on Windows. Note that you don't need the app itself installed, the plugin just uses the same library as the LHM desktop app to detect drives.

With the plugin installed, using any of the usual Sputter requests (CLI or HTTP API) will poll the local computer for any drives with supported temperature sensors and include those in measurements.

### Synology API

{{% alert context="warning" %}}
This adapter is distributed only as a plugin and is not bundled with the default server! It is also considered in a preview state which means:

- You might run into more issues and bugs than usual!
- It is currently only compatible with the Server, not the CLI!

Please report any issues you run into with this adapter.
{{% /alert %}}

The Synology API plugin connects to a Synology NAS running DSM to read the temperature of drives installed in a compatible NAS.

Once you've installed the plugin, you'll need to add some details to your configuration file to allow Sputter to connect to your NAS:

```json
{
  //trimmed for brevity
  "Synology": [
    {
      "Host": "http://<NAS-hostname-or-IP>:5000",
      "User": "<user-with-DSM-access>",
      "Password": "<password>"
    }
  ]
}
```

Once configured, the next time Sputter discovers available drives, it will authenticate as the given user to DSM (assuming single-factor login is available), and include any drives installed in your NAS in measurements. 