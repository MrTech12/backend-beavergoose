#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 6000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY FileMicroservice.csproj ./
RUN dotnet restore FileMicroservice.csproj
COPY . .
RUN dotnet build FileMicroservice.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish FileMicroservice.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://localhost:6000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FileMicroservice.dll"]