#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY LinkMicroservice.DBMigration.csproj ./
RUN dotnet restore LinkMicroservice.DBMigration.csproj
COPY . .
RUN dotnet build LinkMicroservice.DBMigration.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish LinkMicroservice.DBMigration.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinkMicroservice.DBMigration.dll"]