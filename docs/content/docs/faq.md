---
weight: 9999
title: "FAQ"
description: "Frequently Asked Questions"
icon: "quiz"
date: "2024-03-04T02:17:43+10:00"
lastmod: "2024-03-04T02:17:43+10:00"
draft: true
toc: true
---



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