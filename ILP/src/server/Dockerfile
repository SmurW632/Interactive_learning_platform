FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ILP/src/server/server.csproj ./ILP/src/server/
RUN dotnet restore ./ILP/src/server/server.csproj

COPY . .

RUN dotnet publish ./ILP/src/server/server.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "server.dll"]
