# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
COPY GymAngel.Frontend/GymAngel.Frontend.csproj GymAngel.Frontend/

# Restore dependencies
RUN dotnet restore GymAngel.Frontend/GymAngel.Frontend.csproj

# Copy source code
COPY GymAngel.Frontend/ GymAngel.Frontend/

# Build
WORKDIR /src/GymAngel.Frontend
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5034
ENTRYPOINT ["dotnet", "GymAngel.Frontend.dll"]
