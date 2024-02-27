ARG BASE_IMAGE_TAG=latest
FROM sputter/server:${BASE_IMAGE_TAG}
ARG package_version
LABEL \
  io.hass.name="Sputter" \
  io.hass.description="Run the RemoteHub server inside Home Assistant" \
  io.hass.version="${package_version}" \
  io.hass.type="addon" \
  io.hass.arch="amd64"
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY publish/dotnet-any/ .
ENTRYPOINT ["dotnet", "RemoteHub.dll"]

