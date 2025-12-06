# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Urban.AI.WebApi/Urban.AI.WebApi.csproj", "src/Urban.AI.WebApi/"]
COPY ["src/Urban.AI.Infrastructure/Urban.AI.Infrastructure.csproj", "src/Urban.AI.Infrastructure/"]
COPY ["src/Urban.AI.Application/Urban.AI.Application.csproj", "src/Urban.AI.Application/"]
COPY ["src/Urban.AI.Domain/Urban.AI.Domain.csproj", "src/Urban.AI.Domain/"]
RUN dotnet restore "./src/Urban.AI.WebApi/Urban.AI.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Urban.AI.WebApi"
RUN dotnet build "./Urban.AI.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Urban.AI.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Urban.AI.WebApi.dll"]