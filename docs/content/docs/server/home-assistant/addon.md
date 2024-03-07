---
weight: 998
title: "Home Assistant Add-on"
description: "Run Sputter inside Home Assistant"
icon: add_home
lead: ""
date: 2023-01-21T16:13:15+00:00
lastmod: 2023-01-21T16:13:15+00:00
draft: false
images: []
---

{{< alert icon="⚠" context="danger" text="The Home Assistant add-on is in **extremely** early preview and has not been tested much. Use with caution!" />}}

We provide a Home Assistant add-on for running Sputter inside supported Home Assistant installations. 

This is particularly useful when you are running Home Assistant on the same hardware as the drives you're trying to monitor, or when using the [Scrutiny integration](../configuration.md#scrutiny-support).

## Installation

From the *Settings > Add-ons > Add-on Store* menu, click the overflow menu in the top-right, choose *Repositories* and add `https://github.com/agc93/sputter`. Back in the `Add-on Store` screen, you should now have the option to install Sputter.

## Configuration

From the *Settings > Add-ons > Sputter* screen, click over to the *Configuration* tab, and fill out the options relevant to your setup. Make sure to toggle on the **Show unused optional configuration options** option at the bottom to see all the optional configuration options.

### Network Ports

Since Sputter doesn't (currently) have a proper web UI, we deliberately expose the API port through a "host port" from the add-on. You can change what port on your host to expose the API on using the *Network* section at the bottom of the Configuration page.