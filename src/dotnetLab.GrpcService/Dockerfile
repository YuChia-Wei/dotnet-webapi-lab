# syntax=docker/dockerfile:1
FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /workspace
COPY ["global.json", "nuget.config", "./"]
COPY ["src/Directory.Build.targets", "src/"]
COPY ["src/dotnetLab.GrpcService/dotnetLab.GrpcService.csproj", "src/dotnetLab.GrpcService/"]
COPY ["src/dotnetLab.Domain/dotnetLab.Domain.csproj", "src/dotnetLab.Domain/"]
COPY ["src/dotnetLab.CrossCutting.Observability/dotnetLab.CrossCutting.Observability.csproj", "src/dotnetLab.CrossCutting.Observability/"]
COPY ["src/dotnetLab.Persistence.Metadata.SampleDb/dotnetLab.Persistence.Metadata.SampleDb.csproj", "src/dotnetLab.Persistence.Metadata.SampleDb/"]
COPY ["src/dotnetLab.Infrastructure/dotnetLab.Infrastructure.csproj", "src/dotnetLab.Infrastructure/"]
COPY ["src/dotnetLab.Application/dotnetLab.Application.csproj", "src/dotnetLab.Application/"]
RUN --mount=type=secret,id=nuget_config,required=true dotnet restore "src/dotnetLab.GrpcService/dotnetLab.GrpcService.csproj" --configfile /run/secrets/nuget_config
COPY src/ src/
RUN dotnet build "src/dotnetLab.GrpcService/dotnetLab.GrpcService.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "src/dotnetLab.GrpcService/dotnetLab.GrpcService.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnetLab.GrpcService.dll"]
