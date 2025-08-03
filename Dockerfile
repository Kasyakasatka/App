# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Копируем .csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем остальной код
COPY . .

# Удаляем команду миграции с этапа сборки
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
# Используем образ SDK, чтобы dotnet ef был доступен на этапе запуска
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

# Устанавливаем dotnet-ef tool на этапе выполнения
RUN dotnet tool install --global dotnet-ef --version 9.0.7

COPY --from=build /app/out .

EXPOSE 8080

# Выполняем миграции перед запуском приложения
ENTRYPOINT ["/bin/sh", "-c", "/root/.dotnet/tools/dotnet-ef database update --project UserManagementApp.csproj && dotnet UserManagementApp.dll"]
