#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY DeleteFileApp.csproj ./
RUN dotnet restore DeleteFileApp.csproj
COPY . .
RUN dotnet build DeleteFileApp.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish DeleteFileApp.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 4000
ENV ASPNETCORE_URLS=http://+:4000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "DeleteFileApp.dll"]