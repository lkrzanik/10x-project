# Change: E0-1 — EF Core DbContext + Azure SQL migration pipeline

## Source

Roadmap Milestone 0, element E0-1

## Scope

- Create `AppDbContext` in `app/Data/`
- Configure connection string in `appsettings.json` / `appsettings.Development.json`
- Add EF Core SQL Server provider + tools packages
- Create initial migration
- Ensure `dotnet ef database update` pipeline works locally and targets Azure SQL

## Status

✅ Done

## Progress

- [x] Phase 1 — NuGet packages & DbContext skeleton → `b145236`
- [x] Phase 2 — Connection string configuration → included in `b145236`
- [x] Phase 3 — Initial migration & verification → `bcb3674`