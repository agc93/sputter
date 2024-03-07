---
weight: 995
title: "Home Assistant Integration"
description: "Integrate Sputter with Home Assistant automatically"
icon: integration_instructions
lead: ""
date: 2023-01-21T16:13:15+00:00
lastmod: 2023-01-21T16:13:15+00:00
draft: false
images: []
---

Sputter comes bundled with optional Home Assistant support that can seamlessly create new entities in Home Assistant for discovered drives. Once enabled, any time Sputter publishes measurements (after requests, or on a schedule) you'll be able to immediately see updated values right in Home Assistant.

## Configuration

To enable the Home Assistant support, you'll need to first set up [MQTT support](../configuration.md#mqtt).

{{< alert icon="⚠" context="warning" text="If you don't have MQTT configured correctly, the HA integration will not work!" />}}

Once you have MQTT configured, enable the Home Assistant integration with the `EnableHomeAssistant` option:

{{< tabs tabTotal="2">}}
{{% tab tabName="Config File (JSON)" %}}

```json
{
    "MQTT": {
        // MQTT configuration trimmed for brevity
        "EnableHomeAssistant": true
    }
}
```

{{% /tab %}}
{{% tab tabName="Env Variable" %}}

```bash
# bash
export MQTT__EnableHomeAssistant="true"
```

{{% /tab %}}
{{< /tabs >}}

You can then customize the HA behaviour with a few extra options in the `HomeAssistant` object:

{{< tabs tabTotal="2">}}
{{% tab tabName="Config File (JSON)" %}}

```json
{
    "MQTT": {
        "HomeAssistant": {
            "DeviceArea": "Office",
            "ExtendedDriveInfo": true,
            "SingleDeviceMode": true,
            "OverrideManufacturer": true,
            "EnableAllSensors": true,
            "ExpireAfter": 600
        }
    }
}
```

{{% /tab %}}
{{% tab tabName="Env Variable" %}}

{{< alert icon="🛈" text="Setting advanced HA options with environment variables is pretty rough, so we recommend using a configuration file" />}}

```bash
# bash
export MQTT__HomeAssistant__DeviceArea="Office"
export MQTT__HomeAssistant__ExtendedDriveInfo="true"
export MQTT__HomeAssistant__SingleDeviceMode="true"
export MQTT__HomeAssistant__OverrideManufacturer="true"
export MQTT__HomeAssistant__EnableAllSensors="true"
export MQTT__HomeAssistant__ExpireAfter="600"
```

{{% /tab %}}
{{< /tabs >}}

Since that's a lot, here's a quick guide on what each of these options means in practice:

{{< table "table-hover table-responsive" >}}
| Option | Default Value | Notes |
|--------|---------------|-------|
| `DeviceArea` | `null` | If you set this value, all the sensors created will be auto-assigned to the specified area in Home Assistant. If you don't set this, you can still set the device's area in Home Assistant itself.|
| `ExtendedDriveInfo` | `false` | Enabling this option will also **attempt** to include any additional information about the drive in the sensor in Home Assistant. For example, this will include the manufacturer name and software version if available. |
| `SingleDeviceMode` | `false` | By default, Sputter will create a device in Home Assistant for each monitored drive. If you turn this on, it will instead create a single "Sputter" device and add all the drives as sensors on  that one device instead. |
| `OverrideManufacturer` | `false` | If enabled, this will replace the "Manufacturer" of the created HA sensor with `Sputter` instead of the drive manufacturer (or nothing, if not detected) |
| `EnableAllSensors` | `false` | By default, only the temperature sensor will be auto-enabled in Home Assistant. If you enable this option, other [sensors/states](../../reference/sensors.md) (like the Healthy state) will also be enabled by default. |
| `ExpireAfter` | `null` | If set, this is the number of seconds after which Home Assistant will mark a sensor entity as "Unavailable". Note that this can cause oddities when Sputter first starts up/takes its first measurement, but should "level out" |
{{< /table >}}
