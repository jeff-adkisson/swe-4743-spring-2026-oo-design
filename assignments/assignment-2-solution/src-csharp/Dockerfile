# Use the official .NET runtime as a base image for the final stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the solution file and project files to restore dependencies
COPY ["TeaShop.sln", "./"]
COPY ["TeaShop/TeaShop.csproj", "TeaShop/"]
COPY ["TeaShop.UnitTest/TeaShop.UnitTest.csproj", "TeaShop.UnitTest/"]

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/TeaShop"
RUN dotnet build "TeaShop.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TeaShop.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: copy the published output and set the entry point
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeaShop.dll"]
