---
weight: 1110
title: "Sensors and States"
icon: forum
description: "An introduction on how Sputter measures drive attributes"
lead: ""
date: 2023-01-21T16:16:15+00:00
lastmod: 2023-01-21T16:16:15+00:00
draft: false
images: []
toc: true
---

When Sputter measures drives, it gathers collections of two primitives for each drive measures: **sensors** and **states**. 

This design is specifically to keep Sputter flexible and extensible, but it can make things a little daunting at first. REad more below, but it's really not that complex once you get into it.

When Sputter runs a measurement (either on-demand or on a schedule) it polls all the available adapters for drives, filters them (if applicable) then runs each available drive through all the available adapters to gather sensors and states. That means what measurements are returned for a drive will depend on which adapters ran (and what the drive provides).

## Sensors

Sensors are the "main" metric that Sputter is designed for, with particular attention to one specific sensor: temperature. Sputter's drive temperature measurements are (internally) just a sensor with the name of `Temperature`. As of the initial release, the only sensor that Sputter gathers by default is the temperature, but with this design we could potentially expose other values as sensors. Each adapter has its own method of gathering drive temperature, but measurements will always include a temperature sensors.

## States

"States" are the primitive for any details about a drive measurement that can't be measured numerically, but which might change over time. Each adapter can add as many states as it wants (or add none). For example, a common state is the `Healthy` state, which both the DBus and Scrutiny adapters will populate based on whether a drive is failing its SMART checks.

{{< alert icon="💡" context="info" text="Sputter automatically adds a state called `Source` with the names of any adapters that measured each specific drive. " />}}