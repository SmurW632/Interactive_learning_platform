FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ILP/src/server/server.csproj ./ILP/src/server/
RUN dotnet restore ./ILP/src/server/server.csproj

# Копируем только нужные файлы, игнорируя node_modules и другие большие папки
COPY ILP/src/server/. ./ILP/src/server/

RUN dotnet publish ./ILP/src/server/server.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Устанавливаем необходимые библиотеки
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

# Отключаем отслеживание файлов
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "server.dll"]
