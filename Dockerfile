# syntax=docker/dockerfile:1

# --- Runtime image ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Default HTTP port inside container
EXPOSE 8080

# ASP.NET Core will listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# --- Build image ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["ProjectsWebApp/ProjectsWebApp.csproj", "ProjectsWebApp/"]
COPY ["ProjectsWebApp.DataAccsess/ProjectsWebApp.DataAccsess.csproj", "ProjectsWebApp.DataAccsess/"]
COPY ["ProjectsWebApp.Models/ProjectsWebApp.Models.csproj", "ProjectsWebApp.Models/"]
COPY ["ProjectsWebApp.Utility/ProjectsWebApp.Utility.csproj", "ProjectsWebApp.Utility/"]
COPY ["Dto/Dto.csproj", "Dto/"]

RUN dotnet restore "ProjectsWebApp/ProjectsWebApp.csproj"

# Now copy the full source and build
COPY . .
RUN dotnet publish "ProjectsWebApp/ProjectsWebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Final image ---
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

# Default to Production; override via ASPNETCORE_ENVIRONMENT if needed
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ProjectsWebApp.dll"]
