---
weight: 905
title: "Usage"
description: "Setting up the Sputter Server and running it."
icon: rocket_launch
lead: ""
date: 2023-01-21T16:13:15+00:00
lastmod: 2023-01-21T16:13:15+00:00
draft: false
images: []
---

As outlined in [the quickstart docs](../quickstart.md), you can get started with the Sputter server using either the native binaries for your platform of choice, or a pre-built Docker image from Quay.io.

{{< alert icon="💡" context="info" text="There is also a highly experimental Home Assistant add-on available, which is covered in more detail [here](./home-assistant/addon.md)." />}}

<!-- > There is also a HA add-on available, which is documented in the [Home Assistant section](./home-assistant/) -->

## Installation

### Native Binaries

To run the server, download the correct release from [GitHub Releases](https://github.com/agc93/sputter/releases), making sure to download the matching `sputter-server-*` file for your target platform/architecture of choice. Unzip the archive somewhere convenient, and run `Sputter.Server`/`Sputter.Server.exe` to start the server. Open your browser and hit `http://localhost:5000/measurements to get all the measurements for any local drives.

The default server comes configured with the DBus and HWMon adapters so the server will poll the local DBus system bus and the local `/sys`/`sysfs` filesystem for drives with temperature sensors. You can enable the Scrutiny adapter using the configuration file, as outlined in the [Configuration guide](./configuration.md).

### Docker

There is a pre-built Docker image available [on Quay.io](https://quay.io/repository/sputter/server). For example:

```bash
docker run -it --rm -p 8080:8080 quay.io/sputter/server:{{< latest-version >}}
```

Just like the native binaries the container will automatically look for drives using the default DBus and HWMon adapters. Since DBus won't be available, Sputter will use the `sysfs`/`/sys` filesystem to read the drives from your host system. You can now access `http://localhost:8080/measurements` to discover and measure drives from your host system.

If you want to enable DBus in the container, we can add the `/var/run/dbus` socket as a bind mount in the command above:

```bash
docker run -it --rm -p 8080:8080 -v /var/run/dbus:/var/run/dbus quay.io/sputter/server:{{< latest-version >}}
```

If you now hit `http://localhost:8080/measurements`, Sputter will use _both_ HWMon _and_ the bind-mounted DBus system bus to discover and measure any drives from your host system. 

{{< alert icon="💡" context="success" text="You can obviously change the <code>-p</code> argument to map a different host port to the container port that Sputter runs on (<code>8080</code> by default)." />}}


## Listening Port

If you're running in Docker, the easiest way is to just use the port mapping in Docker, so you can update the `-p 8080:8080` from the quickstart to (for example) `-p 9000:8080` to use port 9000 on your host.

If you're running it natively, you need to set an environment variable when running the server. You can set the `ASPNETCORE_URLS` variable to set the listening URL:

{{< tabs tabTotal="3">}}
{{% tab tabName="Bash" %}}

```bash
export ASPNETCORE_URLS="http://localhost:5001"
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