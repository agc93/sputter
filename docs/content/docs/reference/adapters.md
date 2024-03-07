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
