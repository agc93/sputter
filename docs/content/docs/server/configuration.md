---
weight: 910
title: "Configuration"
description: "Customizing the functionality of your Sputter server"
icon: folder_managed
lead: ""
date: 2023-01-21T16:13:15+00:00
lastmod: 2023-01-21T16:13:15+00:00
draft: false
images: []
---

The default behaviour of Sputter should work fine for many use cases, but Sputter is designed to be super-configurable to adapt to whatever weird use case you can come up with.

## Configuration

The Sputter server reads its configuration from a few different places, but we recommend using environment variables if you just want to tweak one or two things, or use a configuration file if you want to customize things in more detail.

#### Configuration File

The Sputter server will load configuration from either an `appsettings.json` or `sputter.json` file in the directory you run the server from, if they are present. These files must be valid JSON files but can include only the configuration parts you want to adjust, and the server will fall back to defaults for any configuration you don't include.

<!-- > Generally speaking, you don't need to restart the Sputter server when updating the configuration file! If you're having issues though, try restarting the server. -->

{{< alert text="Generally speaking, you don't need to restart the Sputter server when updating the configuration file! If you're having issues though, try restarting the server." />}}

#### Environment Variables

Any configuration option can be specified with either a configuration file or with environment variables. This can be particularly convenient when using the Docker container, especially if you're only changing one or two options.

## Drive Filters

When the server runs a measurement (either from a HTTP request or [auto measurement](./configuration.md#auto-measure-interval)), by default it will collect measurements for **all** available drives from all the enabled adapters. Sometimes, you might not want that, so Sputter supports filtering the discovered drives to only gather temperatures for some drives. Filters can be specified per-request (when using the HTTP API), but if you don't specify one it will use any filters from the configuration file next.

### Syntax

The syntax for filters is pretty simple: a wildcard match applied to the drive's serial number _and_ model number, optionally prefixed by an adapter name to limit matches to only drives found with that adapter. For example, if you provide a filter like `"Z52*"`, Sputter will only gather measurements for drives where the serial number or model number starts with `Z52`. Similarly, if you provide a filter like `dbus:ST8000VN004*` will only include drives discovered from DBus where the serial or model number starts with `ST8000VN004`.

### Filter Configuration

You can either provide filters with your request (over HTTP) or in the configuration file. When specified in the configuration file, the filter will be applied with any request that doesn't specify a filter, or when running on a schedule.

{{< tabs tabTotal="2">}}
{{% tab tabName="Config File (JSON)" %}}

```json
{
    "Sputter": {
        "Drives": [
            "dbus:ST8000VN004*",
            "*WD20EFRX*"
        ]
    }
}
```

{{% /tab %}}
{{% tab tabName="HTTP Request" %}}

```http
http://localhost:8080/measurements/WDC*
```

```http
http://localhost:8080/measurements/sysfs:*SN730*
```

{{% /tab %}}
{{< /tabs >}}

## Auto Measure Interval

You can set an interval (in seconds) and Sputter will automatically run that often, measuring any available drives (taking into account any [filters](./configuration.md#drive-filters)). This is most useful if you have [MQTT](./configuration.md#mqtt) configured so you can regularly publish your drive temperatures over MQTT.

{{< tabs tabTotal="2">}}
{{% tab tabName="Config File (JSON)" %}}

```json
{
    "Sputter": {
        "AutoMeasureInterval": 60
    }
}
```

{{% /tab %}}
{{% tab tabName="Env Variable" %}}

```bash
# bash
export SPUTTER__AUTOMEASUREINTERVAL=60
```

{{% /tab %}}
{{< /tabs >}}

When this is enabled, the first measurement on server startup will be taken after 20% of the interval you have set (down to a minimum of 20s)

#### Allow Publishing All

By default, Sputter will not publish measurements on a schedule unless you _either_ [set a drive filter](#drive-filters) or set the `AllowPublishingAll` option in your configuration. This is to prevent possibly saturating your MQTT broker.

{{< tabs tabTotal="2">}}
{{% tab tabName="Config File (JSON)" %}}

```json
{
    "Sputter": {
        "AllowPublishingAll": true
    }
}
```

{{% /tab %}}
{{% tab tabName="Env Variable" %}}

```bash
# bash
export SPUTTER__ALLOWPUBLISHINGALL="true"
```

{{% /tab %}}
{{< /tabs >}}

## MQTT

Sputter supports publishing drive measurements to an MQTT broker by adding your broker details to the configuration. Once your MQTT configuration is added, Sputter will automatically publish every measurement (either on-demand or scheduled) to MQTT. You can see more details on Sputter's MQTT traffic in the [MQTT Reference docs](../reference/mqtt.md).

{{< alert icon="💡" context="info" text="If your MQTT broker doesn't require authentication, skip the username/password options below" />}}

{{< tabs tabTotal="2">}}
{{% tab tabName="Config File (JSON)" %}}

```json
{
    "MQTT": {
        "Server": "192.168.0.100",
        "UserName": "user",
        "Password": "password",
        "Port": 1338
    }
}
```

{{% /tab %}}
{{% tab tabName="Env Variable" %}}

{{< alert icon="🛈" text="Setting MQTT details with environment variables is pretty rough, so we recommend using a configuration file" />}}

```bash
# bash
export MQTT__SERVER="mqtt-server-name:1338"
export MQTT__USERNAME="user"
export MQTT__PASSWORD="password"
```

{{% /tab %}}
{{< /tabs >}}

## Scrutiny Support

{{< alert icon="⚠" context="warning" text="For this to work you'll need a Scrutiny instance already running and set up! Check the [Scrutiny project](https://github.com/AnalogJ/scrutiny) for information on getting Scrutiny set up." />}}

You can add the API address of a Scrutiny server to gather measurements from a remote Scrutiny server (in addition to any local drives). Specify the full URL to the **API** address of your Scrutiny server:

{{< tabs tabTotal="2">}}
{{% tab tabName="Config File (JSON)" %}}

```json
{
    "Scrutiny": {
        "ApiBaseAddress": "http://192.168.0.100:8080/api/"
    }
}
```

or

```json
{
    "Scrutiny": {
        "ApiBaseAddress": "https://scrutiny.domain.tld/api/"
    }
}
```

{{% /tab %}}
{{% tab tabName="Env Variable" %}}

Set the `Scrutiny__ApiBaseAddress` to the full URL:

```bash
# bash
export SCRUTINY__APIBASEADDRESS="http://192.168.0.100:8080/api/"
```

```bash
# Docker
docker run -it --rm -p 8080:8080 -e SCRUTINY_APIBASEADDRESS="https://scrutiny.domain.tld/api/" quay.io/sputter/server:{{< latest-version >}}
```


{{% /tab %}}
{{< /tabs >}}

Once you've set this up, whenever Sputter gathers drive measurements it will also grab any available drives (and their temperatures) from your Scrutiny server. These will be included in the measurements along with any locally discovered drives.

## Caching

{{< alert icon="⚠" context="warning" text="Changing the cache lifetime (or disabling caching) requires restarting Sputter" />}}

By default, Sputter caches measurement results for a short time. This is mostly to prevent adding too much system load if requests stack up or anything like that. The default behaviour is to cache measurements for 120 seconds. You can adjust this to any interval (in seconds), or set it to 0 to disable measurement caching entirely.

{{< tabs tabTotal="2">}}
{{% tab tabName="Config File (JSON)" %}}

```json
{
    "Caching": {
        "Lifetime": 60
    }
}
```

{{% /tab %}}
{{% tab tabName="Env Variable" %}}

```bash
# bash
export CACHING__LIFETIME=60
```

{{% /tab %}}
{{< /tabs >}}
