FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:9.0 AS base
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnetLab.GrpcService/dotnetLab.GrpcService.csproj", "dotnetLab.GrpcService/"]
COPY ["dotnetLab.UseCases/dotnetLab.UseCases.csproj", "dotnetLab.UseCases/"]
COPY ["dotnetLab.CrossCutting.Observability/dotnetLab.CrossCutting.Observability.csproj", "dotnetLab.CrossCutting.Observability/"]
COPY ["dotnetLab.Domains/dotnetLab.Domains.csproj", "dotnetLab.Domains/"]
COPY ["dotnetLab.BuildingBlocks/dotnetLab.BuildingBlocks.csproj", "dotnetLab.BuildingBlocks/"]
COPY ["dotnetLab.Persistence.Repositories/dotnetLab.Persistence.Repositories.csproj", "dotnetLab.Persistence.Repositories/"]
COPY ["dotnetLab.Persistence.Metadata.SampleDb/dotnetLab.Persistence.Metadata.SampleDb.csproj", "dotnetLab.Persistence.Metadata.SampleDb/"]
RUN dotnet restore "dotnetLab.GrpcService/dotnetLab.GrpcService.csproj"
COPY . .
WORKDIR "/src/dotnetLab.GrpcService"
RUN dotnet build "./dotnetLab.GrpcService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./dotnetLab.GrpcService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnetLab.GrpcService.dll"]
