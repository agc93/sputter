---
weight: 1130
title: "Scrutiny"
description: "How Sputter and Scrutiny compare, and how to integrate them."
icon: query_stats
lead: ""
date: 2023-01-21T16:13:15+00:00
lastmod: 2023-01-21T16:13:15+00:00
draft: true
images: []
---

If you haven't already heard of it, [Scrutiny](https://github.com/AnalogJ/scrutiny) is an awesome tool to track and monitor the SMART status of drives including temperature.

## Sputter vs Scrutiny

So what's the difference? Scrutiny is much more capable at a lower level than Sputter is, while Sputter is more of an API-first gateway with a narrow focus designed for automation. 

Scrutiny requires direct low-level access to disk devices to gather all the SMART information that it provides. This makes it a little trickier to set up, but much more capable at doing drive diagnostics (beyond just temperature). It's also designed to gather and store drive information, enabling long-term metrics and such but at the cost of complexity.

Conversely, Sputter is designed to run without any elevated access (on Linux, at least), provides a simplifed stateless API with basic drive information and current sensor values, and has a more flexible model for gathering data (from adapters) and publishing that data (with publish targets). This makes it easier to use with [MQTT](./server/configuration.md#mqtt) and [Home Assistant](./server/home-assistant/). Sputter also supports plugins so that you can integrate it more easily with whatever weird setups you might have.

## Integrating with Scrutiny

If you want, you can even use them both. Set up Sputter to pull drive information from Scrutiny (using the [Scrutiny support](./server/configuration.md#scrutiny-support)) and you can use Sputter to publish that information to MQTT/Home Assistant, or any other target. You can also (as I do) use Sputter to gather temperatures for local drives, merge that with Scrutiny-gathered temperatures on a remote system, and make them all available as a single set of sensors.

{{< alert icon="💡" context="success" text="You can use the Scrutiny support to fetch drives from a running Scrutiny server with both the Sputter [CLI](../command-line.md) and [Server](../server/)!" />}}