FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

RUN dotnet tool install --global dotnet-ef --version 9.0.7

COPY --from=build /app/out .

EXPOSE 8080

ENTRYPOINT ["/bin/sh", "-c", "dotnet ef database update --project UserManagementApp.csproj && dotnet UserManagementApp.dll"]
