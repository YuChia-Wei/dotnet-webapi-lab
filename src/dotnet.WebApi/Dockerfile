FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:8.0-bookworm-slim AS base
#FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["dotnet.WebApi.Common/dotnet.WebApi.Common.csproj", "dotnet.WebApi.Common/"]
COPY ["dotnet.WebApi.Observability/dotnet.WebApi.Observability.csproj", "dotnet.WebApi.Observability/"]
COPY ["dotnet.WebApi.Database.SampleDb/dotnet.WebApi.Database.SampleDb.csproj", "dotnet.WebApi.Database.SampleDb/"]
COPY ["dotnet.WebApi.Repository/dotnet.WebApi.Repository.csproj", "dotnet.WebApi.Repository/"]
COPY ["dotnet.WebApi.Service/dotnet.WebApi.Service.csproj", "dotnet.WebApi.Service/"]
COPY ["dotnet.WebApi/dotnet.WebApi.csproj", "dotnet.WebApi/"]
RUN dotnet restore "dotnet.WebApi/dotnet.WebApi.csproj"
COPY . .
WORKDIR "/src/dotnet.WebApi"
RUN dotnet build "dotnet.WebApi.csproj" \
    -c Release \
    -o /app/build

FROM build AS publish
RUN dotnet publish "dotnet.WebApi.csproj" \
    -c Release \
    -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet.WebApi.dll"]