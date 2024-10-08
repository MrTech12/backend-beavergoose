#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY LinkMicroservice.csproj ./
RUN dotnet restore LinkMicroservice.csproj
COPY . .
RUN dotnet build LinkMicroservice.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish LinkMicroservice.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 7000
ENV ASPNETCORE_URLS=http://+:7000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "LinkMicroservice.dll"]