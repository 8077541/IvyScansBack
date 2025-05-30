FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["IvyScans.API/IvyScans.API.csproj", "IvyScans.API/"]
RUN dotnet restore "IvyScans.API/IvyScans.API.csproj"
COPY . .
WORKDIR "/src/IvyScans.API"
RUN dotnet build "IvyScans.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IvyScans.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IvyScans.API.dll"]
