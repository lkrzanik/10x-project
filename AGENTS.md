# Repository Guidelines — 10xPV

## Project overview

Internal single-user ASP.NET Core MVC web app for long-term climate data analysis and photovoltaic technology recommendations. Polish-language UI. One authenticated user, no multi-tenancy.

## Tech stack

- .NET 10, C# with nullable enabled and implicit usings
- ASP.NET Core MVC (conventional routing: `{controller}/{action}/{id?}`)
- Root namespace: `_10xPV`
- Target: Azure App Service + Azure SQL Database
- AI: OpenAI SDK for .NET
- PDF: QuestPDF
- Frontend: Razor views + Bootstrap 5 + jQuery

## Repository layout

```
app/                → ASP.NET Core MVC project (10xPV.sln)
context/            → Decision artifacts (PRD, tech-stack, lessons, changes)
docs/reference/     → Contract surfaces and reference docs
```

All application code lives under `app/`. Do not place source files outside `app/`.

## Build & run

```powershell
cd app
dotnet build
dotnet run
```

No custom scripts. No Makefile. Standard `dotnet` CLI.

## Conventions

### Code style
- File-scoped namespaces (`namespace _10xPV.Controllers;`)
- One class per file; filename matches class name (PascalCase)
- Shared views/partials in `Views/Shared/`

### Routing
- Conventional MVC routing only (no attribute routing unless endpoint requires it)
- Controller actions: PascalCase (`Index`, `Privacy`, `Error`)

### Configuration
- `appsettings.json` for defaults, `appsettings.Development.json` for dev overrides
- Secrets via user-secrets or environment variables — never commit secrets

### Commits
- Short lowercase messages prefixed with module/lesson marker (e.g., `m1l3`)
- No conventional-commits enforced yet

## Key decisions (from PRD)

- Single-user auth via ASP.NET Identity (not yet implemented)
- CSV import: predefined formats, validation, deduplication
- Data correlation via linear interpolation
- Extremes thresholds configurable in app settings
- AI report generation on-demand → PDF export (text only)
- Language: Polish (UI and reports)

## Pitfalls

- Root namespace is `_10xPV` (underscore prefix because project name starts with a digit) — use this consistently
- `context/archive/` is immutable — never write to it
- No EF Core DbContext exists yet — when adding, place in `app/Data/`
- No authentication middleware yet — when adding, register before `UseAuthorization()`

## Canonical references

- @context/foundation/prd.md — full product requirements
- @context/foundation/tech-stack.md — stack rationale and deployment target
- @context/foundation/lessons.md — accumulated project lessons
- @docs/reference/contract-surfaces.md — naming registry
