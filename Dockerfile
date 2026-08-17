# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Eight.API/Eight.API.csproj", "Eight.API/"]
COPY ["Eight.Application/Eight.Application.csproj", "Eight.Application/"]
COPY ["Eight.Domain/Eight.Domain.csproj", "Eight.Domain/"]
COPY ["Eight.Infrastructure/Eight.Infrastructure.csproj", "Eight.Infrastructure/"]
RUN dotnet restore "./Eight.API/Eight.API.csproj"
COPY . .
WORKDIR "/src/Eight.API"
RUN dotnet build "./Eight.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Eight.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eight.API.dll"]