# Этап 1: Сборка
# Используем .NET SDK для компиляции и публикации приложения.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Копируем файл .csproj и восстанавливаем зависимости, чтобы кэшировать их.
COPY *.csproj ./
RUN dotnet restore

# Копируем остальной код приложения.
COPY . .

# Публикуем приложение в режиме Release.
RUN dotnet publish -c Release -o out

# ---
# Этап 2: Запуск
# Используем ASP.NET Core Runtime (легкий образ) для запуска.
# Это лучший выбор для production, так как он намного меньше, чем SDK.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Копируем все опубликованные файлы из этапа сборки.
COPY --from=build /app/out .

# Открываем порт для прослушивания веб-трафика.
EXPOSE 8080

# Команда для запуска приложения. Это стандартный способ,
# который просто запускает скомпилированный UserManagementApp.dll.
# Теперь Dockerfile выполняет только эту команду при запуске контейнера.
CMD ["dotnet", "UserManagementApp.dll"]
