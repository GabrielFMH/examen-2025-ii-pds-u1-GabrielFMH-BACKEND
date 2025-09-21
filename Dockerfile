# Usa la imagen de .NET SDK 8.0 para construir
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AttendanceApi.csproj", "."]
RUN dotnet restore "AttendanceApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet publish "AttendanceApi.csproj" -c Release -o /app/publish --no-restore

# Usa la imagen de .NET Runtime 8.0 para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AttendanceApi.dll"]