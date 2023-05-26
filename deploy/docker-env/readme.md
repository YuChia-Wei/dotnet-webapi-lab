# docker run sample command

```shell
docker run -d --name dotnet-webapi \
 --restart always \
 -p 8080:80 \
 --hostname dotnet-webapi-docker-server-1 \
 --env-file /deploy/docker-env/dotnet-webapi/env-production/docker-run-env.env \
 ghcr.io/yuchia-wei/dotnet-webapi-lab:main
```