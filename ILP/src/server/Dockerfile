FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ILP/src/server/server.csproj", "ILP/src/server/"]
RUN dotnet restore "ILP/src/server/server.csproj"
COPY . .
WORKDIR "/src/ILP/src/server"
RUN dotnet build "server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "server.dll"]
