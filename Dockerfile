# FASE 1: Construcción (usando SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solo el .csproj primero para aprovechar caché de Docker
COPY ["AttendanceApi.csproj", "."]

# Restaura paquetes NuGet
RUN dotnet restore "AttendanceApi.csproj"

# Copia todo el código fuente
COPY . .

# Publica la aplicación en modo Release
RUN dotnet publish "AttendanceApi.csproj" -c Release -o /app/publish --no-restore

# FASE 2: Producción (usando Runtime ligero)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia solo los archivos publicados desde la fase de construcción
COPY --from=build /app/publish .

# Exponer el puerto que Railway usará (80)
EXPOSE 80

# Iniciar la aplicación con el DLL generado
ENTRYPOINT ["dotnet", "AttendanceApi.dll"]