---
weight: 9999
title: "FAQ"
description: "Frequently Asked Questions"
icon: "quiz"
date: "2024-03-04T02:17:43+10:00"
lastmod: "2024-03-04T02:17:43+10:00"
draft: false
toc: true
---

This is just a random selection of questions that might not be answered elsewhere!

## How do I update the Server configuration?

If you're using the configuration file (i.e. `sputter.json` or `appsettings.json`), you can generally update the file in-place and the server will automatically reload the configuration. In particular, updating the MQTT broker details, auto measurement intervals, or drive filters will apply the changes without restarting the server.

That being said, if you weren't already using the configuration file, or are using the server in limited environments, you might need to restart the server.

## How do I change the Server listening port?

If you're running in Docker, the easiest way is to just use the port mapping in Docker, so you can update the `-p 8080:8080` from the quickstart to (for example) `-p 9000:8080` to use port 9000 on your host.

If you're running it natively, you need to set an environment variable when running the server. You can set the `ASPNETCORE_URLS` variable to set the listening URL:

{{< tabs tabTotal="3">}}
{{% tab tabName="Bash" %}}

```bash
export ASPNETCORE_URLS="http://localhost:5001;https://localhost:5002"
```

{{% /tab %}}
{{% tab tabName="PowerShell" %}}

```powershell
$Env:ASPNETCORE_URLS = "http://localhost:5001"
```

{{% /tab %}}
{{% tab tabName="cmd.exe" %}}

```bat
setx ASPNETCORE_URLS "http://localhost:5001"
```

{{% /tab %}}
{{< /tabs >}}

## What's with the weird name?

Because I'm a sucker for a) weird project names, and b) funny words. [Sputtering](https://en.wikipedia.org/wiki/Sputter_deposition) is a physical process used heavily in the manufacturing of hard disks, and "sputter" is just such a fun word I had to use it.

## How does this relate/compare to Scrutiny?

First off, I ***love*** [Scrutiny](https://github.com/AnalogJ/scrutiny)! It's an awesome project that I run an instance of myself. That's actually why Sputter supports gathering drive temperatures from Scrutiny. 

So what's the difference? Scrutiny is much more capable at a lower level than Sputter is, while Sputter is more of an API-first gateway with a narrow focus designed for automation. Basically, Sputter is not a replacement for Scrutiny, it's just a higher-level API gateway to provide one metric (drive temperatures) from any number of sources (including but not limited to Scrutiny).

{{< details "Read More" >}}

{{% alert icon="ℹ" %}}

Scrutiny requires direct low-level access to disk devices to gather all the SMART information that it provides. This makes it a little trickier to set up, but much more capable at doing drive diagnostics (beyond just temperature). It's also designed to gather and store drive information, enabling long-term metrics and such but at the cost of complexity.

Conversely, Sputter is designed to run without any elevated access (on Linux, at least), provides a simplifed stateless API with basic drive information and current sensor values, and has a more flexible model for gathering data (from adapters) and publishing that data (with publish targets). This makes it easier to use with [MQTT](./server/configuration.md#mqtt) and [Home Assistant](./server/home-assistant/). Sputter also supports plugins so that you can integrate it more easily with whatever weird setups you might have.

###### Integrating with Scrutiny

If you want, you can even use them both. Set up Sputter to pull drive information from Scrutiny (using the [Scrutiny support](./server/configuration.md#scrutiny-support)) and you can use Sputter to publish that information to MQTT/Home Assistant, or any other target. You can also (as I do) use Sputter to gather temperatures for local drives, merge that with Scrutiny-gathered temperatures on a remote system, and make them all available as a single set of sensors.

{{% /alert %}}

{{< alert icon="💡" context="success" text="You can use the Scrutiny support to fetch drives from a running Scrutiny server with both the Sputter [CLI](../command-line.md) and [Server](../server/)!" />}}
{{< /details >}}