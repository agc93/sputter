---
weight: 300
title: "Command-Line/CLI"
description: "Get more familiar with the Sputter Command-Line Interface (CLI)"
icon: "terminal"
date: "2024-03-04T02:17:43+10:00"
lastmod: "2024-03-04T02:17:43+10:00"
draft: false
toc: true
---

{{< alert icon="💡" context="success" text="The Sputter CLI supports both Windows and Linux! Just download the right binary for your target platform." />}}

## Getting Help

You can use the `-h`/`--help` option to get help for the available commands and options when using the CLI:

```
USAGE:
    sputter.exe [filter] [OPTIONS]

ARGUMENTS:
    [filter]

OPTIONS:
    -h, --help                Prints help information
    -v, --version             Prints version information
        --scrutiny-api
        --mqtt-server
        --mqtt-credentials
```

The default command will discover any local drives using the [default adapters](./reference/adapters.md) and gather measurements from all of them.

## Drive Filter

If you only want to gather measurements from a subset of drives you can provide a _filter_. Filters can work a little different for each adapter but the usual behaviour is a basic wildcard match against detected drive's serial number and model number. That is, if you provide a filter like `"Z52*"`, Sputter will only gather measurements for drives where the serial number or model number starts with `Z52`

## Scrutiny Support

{{< alert icon="⚠" context="warning" text="For this to work you'll need a Scrutiny instance already running and set up! Check the [Scrutiny project](https://github.com/AnalogJ/scrutiny) for information on getting Scrutiny set up." />}}

You can add the API address of a Scrutiny server to gather measurements from a remote Scrutiny server (in addition to any local drives). Specify the full URL to the **API** address of your Scrutiny server:

```powershell
# For example:
sputter.exe --scrutiny-api "http://192.168.0.100:8080/api/"
# or
sputter.exe --scrutiny-api "https://scrutiny.domain.tld/api/"
```

## MQTT

You can also publish any measurements the CLI finds (both local drives and from Scrutiny, if configured) to an MQTT broker. Note that this will be a one-time/"fire-and-forget" publish. To enable MQTT, you need to at least provide the `--mqtt-server` option:

```bash
sputter --mqtt-server my-mqtt-server:1337
# or
sputter --mqtt-server "192.168.0.90"
```

If your MQTT server also requires authentication, you can provide credentials with the `--mqtt-credentials` option. For example:

```bash
sputter --mqtt-server my-mqtt-server:1337 --mqtt-credentials "user:password"
```

Running this will still display any gathered measurements as usual, but now results will also be published to your MQTT broker.

## Plugins

If you are using any Sputter plugins, unpack them into folders in a `plugins` folder in the same directory as the binary. For example, for the LibreHardwareMonitor plugin:

```text
│   sputter.exe
└───plugins
	└───Sputter.LibreHardwareMonitor
		│   // trimmed for brevity
		│   Sputter.Composition.dll
		│   Sputter.Core.dll
		│   Sputter.LibreHardwareMonitor.deps.json
		│   Sputter.LibreHardwareMonitor.dll
		└───runtimes
			└─── // trimmed
```

{{< alert text="The plugin folder **must** be named the same as the plugin assembly (i.e. `Sputter.LibreHardwareMonitor` in the example above)!" />}}