#FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:8.0-bookworm-slim AS base
WORKDIR /app

# root port
EXPOSE 80
EXPOSE 443

# rootless port (dotnet 8 default)
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["dotnetLab.Common/dotnetLab.Common.csproj", "dotnetLab.Common/"]
COPY ["dotnetLab.Observability/dotnetLab.Observability.csproj", "dotnetLab.Observability/"]

COPY ["dotnetLab.Database.SampleDb/dotnetLab.Database.SampleDb.csproj", "dotnetLab.Database.SampleDb/"]
COPY ["dotnetLab.Repository/dotnetLab.Repository.csproj", "dotnetLab.Repository/"]

COPY ["dotnetLab.Application/dotnetLab.Application.csproj", "dotnetLab.Application/"]
COPY ["dotnetLab.DomainEntities/dotnetLab.DomainEntities.csproj", "dotnetLab.DomainEntities/"]

COPY ["dotnetLab.GrpcService/dotnetLab.GrpcService.csproj", "dotnetLab.GrpcService/"]

RUN dotnet restore "dotnetLab.GrpcService/dotnetLab.GrpcService.csproj"
COPY . .
WORKDIR "/src/dotnetLab.GrpcService"
RUN dotnet build "dotnetLab.GrpcService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotnetLab.GrpcService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnetLab.GrpcService.dll"]
