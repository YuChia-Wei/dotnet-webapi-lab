FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# if you need downgrade to TLS 1.0
# RUN sed -i '1i openssl_conf = default_conf' /etc/ssl/openssl.cnf && echo -e "\n[ default_conf ]\nssl_conf = ssl_sect\n[ssl_sect]\nsystem_default = system_default_sect\n[system_default_sect]\nMinProtocol = TLSv1\nCipherString = DEFAULT:@SECLEVEL=1" >> /etc/ssl/openssl.cnf

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
# if you need downgrade to TLS 1.0
# RUN sed -i '1i openssl_conf = default_conf' /etc/ssl/openssl.cnf && echo -e "\n[ default_conf ]\nssl_conf = ssl_sect\n[ssl_sect]\nsystem_default = system_default_sect\n[system_default_sect]\nMinProtocol = TLSv1\nCipherString = DEFAULT:@SECLEVEL=1" >> /etc/ssl/openssl.cnf
ARG BuildConfiguration=Debug
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["dotnet.WebApi/dotnet.WebApi.csproj", "dotnet.WebApi/"]
COPY ["dotnet.WebApi.AopComponent/dotnet.WebApi.AopComponent.csproj", "dotnet.WebApi.AopComponent/"]
COPY ["dotnet.WebApi.Common/dotnet.WebApi.Common.csproj", "dotnet.WebApi.Common/"]
COPY ["dotnet.WebApi.Repository/dotnet.WebApi.Repository.csproj", "dotnet.WebApi.Repository/"]
COPY ["dotnet.WebApi.Repository.Db.SampleDb/dotnet.WebApi.Repository.Db.SampleDb.csproj", "dotnet.WebApi.Repository.Db.SampleDb/"]
COPY ["dotnet.WebApi.Service/dotnet.WebApi.Service.csproj", "dotnet.WebApi.Service/"]
RUN dotnet restore "dotnet.WebApi/dotnet.WebApi.csproj"
COPY . .
WORKDIR "/src/dotnet.WebApi"
RUN dotnet build "dotnet.WebApi.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration=Debug
RUN dotnet publish "dotnet.WebApi.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet.WebApi.dll"]