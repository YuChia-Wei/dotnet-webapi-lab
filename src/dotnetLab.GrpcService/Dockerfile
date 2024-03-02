FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["dotnet.GrpcService/dotnet.GrpcService.csproj", "dotnet.GrpcService/"]
RUN dotnet restore "dotnet.GrpcService/dotnet.GrpcService.csproj"
COPY . .
WORKDIR "/src/dotnet.GrpcService"
RUN dotnet build "dotnet.GrpcService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotnet.GrpcService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet.GrpcService.dll"]
