FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:9.0-bookworm-slim AS base
#FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
USER $APP_UID
WORKDIR /app

# root port
EXPOSE 80
EXPOSE 443

# rootless port (dotnet 9 default)
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["dotnetLab.SharedKernel/dotnetLab.SharedKernel.csproj", "dotnetLab.SharedKernel/"]
COPY ["dotnetLab.CrossCutting.Observability/dotnetLab.CrossCutting.Observability.csproj", "dotnetLab.CrossCutting.Observability/"]

COPY ["dotnetLab.Persistence.Metadata.SampleDb/dotnetLab.Persistence.Metadata.SampleDb.csproj", "dotnetLab.Persistence.Metadata.SampleDb/"]
COPY ["dotnetLab.Persistence.Repositories/dotnetLab.Persistence.Repositories.csproj", "dotnetLab.Persistence.Repositories/"]

COPY ["dotnetLab.UseCases/dotnetLab.UseCases.csproj", "dotnetLab.UseCase/"]
COPY ["dotnetLab.Domains/dotnetLab.Domains.csproj", "dotnetLab.DomainEntity/"]

COPY ["dotnetLab.WebApi/dotnetLab.WebApi.csproj", "dotnetLab.WebApi/"]
RUN dotnet restore "dotnetLab.WebApi/dotnetLab.WebApi.csproj"
COPY . .
WORKDIR "/src/dotnetLab.WebApi"
RUN dotnet build "dotnetLab.WebApi.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "dotnetLab.WebApi.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnetLab.WebApi.dll"]