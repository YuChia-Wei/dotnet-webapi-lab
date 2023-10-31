FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:7.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["dotnet.WebApi/dotnet.WebApi.csproj", "dotnet.WebApi/"]
COPY ["dotnet.WebApi.Observability/dotnet.WebApi.Observability.csproj", "dotnet.WebApi.Observability/"]
COPY ["dotnet.WebApi.Common/dotnet.WebApi.Common.csproj", "dotnet.WebApi.Common/"]
COPY ["dotnet.WebApi.Repository/dotnet.WebApi.Repository.csproj", "dotnet.WebApi.Repository/"]
COPY ["dotnet.WebApi.Repository.Db.SampleDb/dotnet.WebApi.Repository.Db.SampleDb.csproj", "dotnet.WebApi.Repository.Db.SampleDb/"]
COPY ["dotnet.WebApi.Service/dotnet.WebApi.Service.csproj", "dotnet.WebApi.Service/"]
RUN dotnet restore "dotnet.WebApi/dotnet.WebApi.csproj"
COPY . .
WORKDIR "/src/dotnet.WebApi"
RUN dotnet build "dotnet.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotnet.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet.WebApi.dll"]