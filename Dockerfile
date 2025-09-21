# Usa la imagen de .NET Runtime 8.0 (ligera, para producción)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Usa la imagen de .NET SDK 8.0 para construir
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AttendanceApi/AttendanceApi.csproj", "AttendanceApi/"]
RUN dotnet restore "AttendanceApi/AttendanceApi.csproj"
COPY . .
WORKDIR "/src/AttendanceApi"
RUN dotnet publish "AttendanceApi.csproj" -c Release -o /app/publish --no-restore

# Copia el resultado al entorno de ejecución
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AttendanceApi.dll"]