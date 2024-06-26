FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ARG package_version
LABEL \
  io.hass.name="Sputter" \
  io.hass.description="Run the Sputter measurement server inside Home Assistant" \
  io.hass.version="${package_version}" \
  io.hass.type="addon" \
  io.hass.arch="armhf|aarch64|amd64"

COPY publish/Sputter.Server/dotnet-any/ .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sputter.Server.dll"]