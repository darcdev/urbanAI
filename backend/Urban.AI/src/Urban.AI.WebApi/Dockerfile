# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/Urban.AI.WebApi/Urban.AI.WebApi.csproj", "Urban.AI.WebApi/"]
COPY ["src/Urban.AI.Infrastructure/Urban.AI.Infrastructure.csproj", "Urban.AI.Infrastructure/"]
COPY ["src/Urban.AI.Application/Urban.AI.Application.csproj", "Urban.AI.Application/"]
COPY ["src/Urban.AI.Domain/Urban.AI.Domain.csproj", "Urban.AI.Domain/"]

# Restore dependencies
RUN dotnet restore "Urban.AI.WebApi/Urban.AI.WebApi.csproj"

# Copy all source code
COPY src/ .

# Build
WORKDIR "/src/Urban.AI.WebApi"
RUN dotnet build "Urban.AI.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Urban.AI.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Urban.AI.WebApi.dll"]