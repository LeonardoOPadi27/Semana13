@echo off
rem Abre los tres servicios de la Semana 15, cada uno en su propia ventana.
cd /d "%~dp0"
start "Microservicio 1 - Gimnasio.Api (5091)" cmd /k dotnet run --project Gimnasio.Api --launch-profile http
start "Microservicio 2 - EntrenadoresApi (5092)" cmd /k dotnet run --project EntrenadoresApi --launch-profile http
start "API Gateway - Ocelot (5100)" cmd /k dotnet run --project GatewayApi --launch-profile http
