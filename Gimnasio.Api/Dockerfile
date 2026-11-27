# Se construye desde la raiz del repositorio: docker build -f Gimnasio.Api/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Gimnasio.Entidades/ Gimnasio.Entidades/
COPY Gimnasio.Datos/ Gimnasio.Datos/
COPY Gimnasio.Negocio/ Gimnasio.Negocio/
COPY Gimnasio.Api/ Gimnasio.Api/
RUN dotnet publish Gimnasio.Api/Gimnasio.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
# Render envia el trafico al puerto 10000 por defecto
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "Gimnasio.Api.dll"]
