---
weight: 100
title: "Introduction"
description: "An introduction to the Sputter project"
icon: "circle"
date: "2024-03-04T02:17:43+10:00"
lastmod: "2024-03-04T02:17:43+10:00"
draft: false
toc: true
---

Sputter is a flexible and highly configurable API server to retrieve hard drive temperatures, and optionally publish them via MQTT. 

In short, Sputter will gather up hard drive temperatures from a number of sources either on-demand or on a timer, and then return those temperatures while optionally also publishing those values over MQTT. Sputter also includes first-class support for Home Assistant so it can publish those temperatures as entities in Home Assistant.

> If you just want to get started with setting Sputter up for yourself, you can probably skip the rest of this topic and move on to the [quickstart](./quickstart.md)!

## Design

Sputter is designed as a set of (at times over-complicated) interlocking components to make it as flexible as possible. There's two tools you can use when working with Sputter: the CLI and the Server. 

- The Sputter CLI is a lightweight simple CLI to quickly get drive measurements on demand, print the measurement results and optionally publish them via MQTT as a "one-time" operation
- The Sputter Server is a self-hosted API server that exposes many additional features compared to the CLI allowing to set up a self-managed measurement server that's simple to integrate with other applications.

### Sources

Sputter has to draw the drive temperatures from _somewhere_ and in our case it's from any number of sources (called adapters internally) including any combination of:

- **DBus**: The local DBus system bus.
- **HWMon** (aka `sysfs`): The local device nodes in the `sysfs`/`/sys` file system.
- **Scrutiny**: Sputter can also pull drive temperatures from a running [Scrutiny](https://github.com/AnalogJ/scrutiny) instance.

Allowing any combination of these adapters allows you to mix-and-match drive measurements from any number of compatible sources in one place. This also makes it easier for different deployment scenarios where different methods might be more difficult than others.

## Why "Sputter"

Because I'm a sucker for a) weird project names, and b) funny words. [Sputtering](https://en.wikipedia.org/wiki/Sputter_deposition) is a physical process used heavily in the manufacturing of hard disks, and "sputter(ing)" is just such a fun word I had to use it.
