FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SmartHomeScenes.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENV SCENE_DB_PATH=/data/smart-home-scenes.db
VOLUME ["/data"]

ENTRYPOINT ["dotnet", "SmartHomeScenes.dll"]
