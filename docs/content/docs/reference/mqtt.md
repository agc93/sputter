---
weight: 1115
title: "MQTT"
icon: forum
description: "More details on Sputter's MQTT support."
lead: ""
date: 2023-01-21T16:16:15+00:00
lastmod: 2023-01-21T16:16:15+00:00
draft: false
images: []
toc: true
---

{{< alert icon="💡" context="success" text="Both the CLI and Server support publishing drive measurements to an MQTT broker!" />}}

## MQTT Messages

### Topics

When Sputter publishes drive measurements to MQTT, it will publish measurements to a separate topic per-drive. The topic format will be in the format `sputter/drv-<SERIAL-NUMBER>` with a JSON payload. For example:

{{< table "table-hover table-responsive" >}}
| Topic | Message |
|---------|--------|
| `sputter/drv-Z52CBFRL` | `{"sensors":{"temperature":39},"states":{"healthy":"False","source":"scrutiny"}}` |
{{< /table >}}

## Home Assistant Integration

As outlined in the [HA Integration docs](../server/home-assistant/integration.md), Sputter can automatically create sensor entitiies in Home Assistant for measured drives. It does this using Home Assistant's [MQTT Discovery](https://www.home-assistant.io/integrations/mqtt/#mqtt-discovery) capabilities.

This consists of publishing messages to the `homeassistant/sensor/<drive-id>/drive-temperature/config` topic (for any discovered drives) where the exact message body will be controlled by the [Home Assistant configuration](../server/home-assistant/integration.md#configuration).