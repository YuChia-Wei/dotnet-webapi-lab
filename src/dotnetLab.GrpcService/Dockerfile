FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:9.0-bookworm-slim AS base
#FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
USER $APP_UID
WORKDIR /app

# root port
EXPOSE 80
EXPOSE 443

# rootless port (dotnet 8 default)
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["dotnetLab.SharedKernel/dotnetLab.SharedKernel.csproj", "dotnetLab.SharedKernel/"]
COPY ["dotnetLab.CrossCutting.Observability/dotnetLab.CrossCutting.Observability.csproj", "dotnetLab.CrossCutting.Observability/"]

COPY ["dotnetLab.Persistence.Metadata.SampleDb/dotnetLab.Persistence.Metadata.SampleDb.csproj", "dotnetLab.Persistence.Metadata.SampleDb/"]
COPY ["dotnetLab.Persistence.Repositories/dotnetLab.Persistence.Repositories.csproj", "dotnetLab.Persistence.Repositories/"]

COPY ["dotnetLab.UseCases/dotnetLab.UseCases.csproj", "dotnetLab.UseCase/"]
COPY ["dotnetLab.Domains/dotnetLab.Domains.csproj", "dotnetLab.DomainEntity/"]

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
