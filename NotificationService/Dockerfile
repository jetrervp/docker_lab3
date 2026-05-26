FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем ТОЛЬКО csproj (НЕ Dockerfile)
COPY NotificationService.csproj NotificationService.csproj
RUN dotnet restore NotificationService.csproj

# Копируем ВСЁ КРОМЕ Dockerfile
COPY --exclude=Dockerfile . .
# Если --exclude не поддерживается, используем .dockerignore

RUN dotnet publish NotificationService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]