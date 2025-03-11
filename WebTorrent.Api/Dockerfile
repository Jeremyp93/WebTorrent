FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Install aria2 torrent client
RUN apt-get update && apt-get install -y aria2

COPY ["WebTorrent.Api.csproj", "./"]
RUN dotnet restore "./WebTorrent.Api.csproj"
COPY . ./
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WebTorrent.Api.dll"]