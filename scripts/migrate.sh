#!/usr/bin/env bash
set -euo pipefail

echo "[migrator] Starting EF Core migration run..."
cd /src

# Ensure connection string exists
: "${ConnectionStrings__DefaultConnection:?Missing ConnectionStrings__DefaultConnection environment variable}"

# Install dotnet-ef if missing
if ! command -v dotnet-ef >/dev/null 2>&1; then
  echo "[migrator] Installing dotnet-ef..."
  dotnet tool install --global dotnet-ef --version 8.*
fi
export PATH="$PATH:/root/.dotnet/tools"

echo "[migrator] Restore..."
dotnet restore "ProjectsWebApp/ProjectsWebApp.csproj"

echo "[migrator] Build (Release)..."
dotnet build "ProjectsWebApp/ProjectsWebApp.csproj" -c Release --no-restore

echo "[migrator] Apply migrations..."
dotnet ef database update \
  --project "ProjectsWebApp.DataAccsess/ProjectsWebApp.DataAccsess.csproj" \
  --startup-project "ProjectsWebApp/ProjectsWebApp.csproj" \
  --configuration Release \
  --verbose

echo "[migrator] Done."
 