# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY GymAngel.sln .
COPY GymAngel.Api/GymAngel.Api.csproj GymAngel.Api/
COPY GymAngel.Business/GymAngel.Business.csproj GymAngel.Business/
COPY GymAngel.Data/GymAngel.Data.csproj GymAngel.Data/
COPY GymAngel.Domain/GymAngel.Domain.csproj GymAngel.Domain/

# Restore dependencies
RUN dotnet restore GymAngel.Api/GymAngel.Api.csproj

# Copy source code
COPY GymAngel.Api/ GymAngel.Api/
COPY GymAngel.Business/ GymAngel.Business/
COPY GymAngel.Data/ GymAngel.Data/
COPY GymAngel.Domain/ GymAngel.Domain/

# Build
WORKDIR /src/GymAngel.Api
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "GymAngel.Api.dll"]
