# AGENTS.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

ASP.NET Core 10 Web API for motorcycle (Moto) management. Uses Entity Framework Core with SQL Server.

## Build and Run Commands

```powershell
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run (starts on http://localhost:5280 or https://localhost:7227)
dotnet run

# Run with hot reload
dotnet watch run
```

## Database

- SQL Server on `localhost:1433`, database `MotosDb`
- Connection string in `appsettings.json` (update password before running)
- On startup, `Program.cs` drops and recreates the database via `EnsureDeleted()` + `Migrate()`

```powershell
# Add a migration
dotnet ef migrations add <MigrationName>

# Apply migrations manually (usually not needed - handled on startup)
dotnet ef database update
```

## API Documentation

Swagger UI available at `/swagger` when running. JWT Bearer authentication is configured but not enforced.

## Architecture

- **Models/**: Entity classes (e.g., `Moto`)
- **Dtos/**: Data transfer objects for API requests (e.g., `CreateMotoDto` - currently unused)
- **Controllers/**: API endpoints with standard CRUD operations
- **Data/**: `AppDbContext` for EF Core database access

Controllers inject `AppDbContext` directly. Consider using the DTOs in `Dtos/` for create/update operations.
