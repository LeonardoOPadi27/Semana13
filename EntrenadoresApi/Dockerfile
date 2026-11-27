# Se construye desde la raiz del repositorio: docker build -f EntrenadoresApi/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY EntrenadoresApi/ EntrenadoresApi/
RUN dotnet publish EntrenadoresApi/EntrenadoresApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "EntrenadoresApi.dll"]
