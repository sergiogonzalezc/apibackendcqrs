#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8082
ENV ASPNETCORE_URLS=http://*:8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BackEndProducts.Api/BackEndProducts.Api.csproj", "BackEndProducts.Api/"]
COPY ["BackEndProducts.Logging/BackEndProducts.Logging.csproj", "BackEndProducts.Logging/"]
COPY ["BackEndProducts.Common/BackEndProducts.Common.csproj", "BackEndProducts.Common/"]
COPY ["BackEndProducts.Application/BackEndProducts.Application.csproj", "BackEndProducts.Application/"]
COPY ["BackEndProducts.Infrasctructure/BackEndProducts.Infrasctructure.csproj", "BackEndProducts.Infrasctructure/"]
COPY ["BackEndProducts.Sql/BackEndProducts.Sql.csproj", "BackEndProducts.Sql/"]
RUN dotnet restore "./BackEndProducts.Api/BackEndProducts.Api.csproj"
COPY . .
WORKDIR "/src/BackEndProducts.Api"
RUN dotnet build "./BackEndProducts.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BackEndProducts.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackEndProducts.Api.dll"]