# Plan: E0-1 — EF Core DbContext + Azure SQL migration pipeline

## Goal

Deliver a working EF Core DbContext with SQL Server provider, connection string configuration, and an initial migration that can be applied locally and on Azure SQL.

## Phases

### Phase 1 — NuGet packages & DbContext skeleton

**Files:**
- `app/10xPV.csproj` — add EF Core packages
- `app/Data/AppDbContext.cs` — empty DbContext

**Actions:**
1. Add packages: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.EntityFrameworkCore.Design`
2. Create `AppDbContext` class in namespace `_10xPV.Data` inheriting `DbContext`
3. Register `AppDbContext` in `Program.cs` via `builder.Services.AddDbContext<AppDbContext>()`

**Success:** `dotnet build` passes; DI resolves `AppDbContext`.

### Phase 2 — Connection string configuration

**Files:**
- `app/appsettings.json` — add `ConnectionStrings:DefaultConnection` placeholder for Azure SQL
- `app/appsettings.Development.json` — add LocalDB/SQL Express connection string

**Actions:**
1. Add connection string entries
2. Reference `"DefaultConnection"` in `AddDbContext` call

**Success:** App starts without connection errors in Development (LocalDB).

### Phase 3 — Initial migration & verification

**Files:**
- `app/Data/Migrations/` — generated migration files

**Actions:**
1. Run `dotnet ef migrations add InitialCreate --project app`
2. Run `dotnet ef database update --project app`
3. Verify empty database created successfully

**Success:** `dotnet ef database update` completes with exit code 0; database exists with `__EFMigrationsHistory` table.

## Contracts

| Symbol | Type | Location |
|--------|------|----------|
| `AppDbContext` | class | `app/Data/AppDbContext.cs` |
| `ConnectionStrings:DefaultConnection` | config key | `appsettings.json` |

## Risks

- LocalDB not installed → fallback: use SQL Server Express or Docker SQL container
- .NET 10 EF Core package version alignment — pin to latest stable preview

## Progress

<!-- Updated by /10x-implement -->