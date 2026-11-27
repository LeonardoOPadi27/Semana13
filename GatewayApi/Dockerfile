# Se construye desde la raiz del repositorio: docker build -f GatewayApi/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY GatewayApi/ GatewayApi/
RUN dotnet publish GatewayApi/GatewayApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
# En el contenedor el entorno es Production: se carga ocelot.json y luego ocelot.Production.json
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "GatewayApi.dll"]
