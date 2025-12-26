#!/usr/bin/env bash
set -euo pipefail

# Simple EF Core migration runner for PromptCoach
# Uses ConnectionStrings__DefaultConnection from environment (injected by docker-compose)

echo "[migrator] NOTE: EF Core migrations are now applied automatically by the web app on startup."
echo "[migrator] This container does nothing and exits successfully."
