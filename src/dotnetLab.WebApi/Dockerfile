FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:9.0 AS base
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnetLab.WebApi/dotnetLab.WebApi.csproj", "dotnetLab.WebApi/"]
COPY ["dotnetLab.Domain/dotnetLab.Domain.csproj", "dotnetLab.Domain/"]
COPY ["dotnetLab.BuildingBlocks.Domain/dotnetLab.BuildingBlocks.Domain.csproj", "dotnetLab.BuildingBlocks.Domain/"]
COPY ["dotnetLab.CrossCutting.Observability/dotnetLab.CrossCutting.Observability.csproj", "dotnetLab.CrossCutting.Observability/"]
COPY ["dotnetLab.Persistence.Metadata.SampleDb/dotnetLab.Persistence.Metadata.SampleDb.csproj", "dotnetLab.Persistence.Metadata.SampleDb/"]
COPY ["dotnetLab.Infrastructure/dotnetLab.Infrastructure.csproj", "dotnetLab.Infrastructure/"]
COPY ["dotnetLab.Application/dotnetLab.Application.csproj", "dotnetLab.Application/"]
RUN dotnet restore "dotnetLab.WebApi/dotnetLab.WebApi.csproj"
COPY . .
WORKDIR "/src/dotnetLab.WebApi"
RUN dotnet build "./dotnetLab.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./dotnetLab.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnetLab.WebApi.dll"]
