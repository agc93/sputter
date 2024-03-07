---
weight: 200
title: "Quickstart"
description: "Getting started with setting up Sputter for yourself"
icon: "rocket_launch"
date: "2024-03-04T02:17:43+10:00"
lastmod: "2024-03-04T02:17:43+10:00"
draft: false
toc: true
---

Due to its flexibility and extensibility (and its complexity behind the scenes), there's a few ways of getting started with Sputter. The first choice is which tool you want to use: the **CLI** or the **Server**.

- **CLI**: Standalone command-line-only utility to gather drive temperatures from all the supported sources and outputs them to the console, optionally also publishing the measurements to an MQTT server.
- **Server**: Server application that runs a HTTP API that you can query to gather drive measurements and optionally publish those measurements to an MQTT server and/or to Home Assistant. Can also be configured to gather and publish measurements on a timer for automation.

{{< alert icon="💡" context="success" text="<strong>Both</strong> versions of Sputter will run on both Windows and Linux! Just download the right binary for your target platform." />}}


## Command-Line Interface (CLI)

Download the correct release from [GitHub Releases](https://github.com/agc93/sputter/releases), making sure to download the matching `sputter-console-*` file for your target platform/architecture of choice. Unzip the archive somewhere convenient and run `sputter -h`/`sputter.exe -h` to see the contextual help. 

Running the CLI without any arguments will (by default) output all measurements (and states/sensors) for any _local_ drives in a human-readable table format.

## Sputter Server

You can run the server a few different ways but to keep things brief here, we'll only cover the two simplest ones: Docker and natively.

### Docker

There is a pre-built Docker image available [on Quay.io](https://quay.io/repository/sputter/server). For example:

```bash
docker run -it --rm -p 8080:8080 quay.io/sputter/server:{{< latest-version >}}
```

If you now hit `http://localhost:8080/measurements`, Sputter will use the HWMon adapter to discover and measure any drives from your host system. 

{{< alert icon="❔" text="If you don't see any results, you might need to enable the DBus adapter! Check [the docs](./server/usage.md) for more info." />}}

### Native

To run the server, download the correct release from [GitHub Releases](https://github.com/agc93/sputter/releases), making sure to download the matching `sputter-server-*` file for your target platform/architecture of choice. Unzip the archive somewhere convenient, and run `Sputter.Server`/`Sputter.Server.exe` to start the server. Open your browser and hit `http://localhost:5000/measurements to get all the measurements for any local drives.