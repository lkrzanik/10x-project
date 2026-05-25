# Plan: E0-2 — ASP.NET Identity — single-user auth

## Goal

Single authenticated user can log in and out. All app routes require authentication. One admin account is seeded on first run. No public registration.

## Phases

### Phase 1 — Identity packages, DbContext integration & middleware

**Files:**
- `app/10xPV.csproj` — add Identity packages
- `app/Data/AppDbContext.cs` — inherit from `IdentityDbContext`
- `app/Program.cs` — register Identity services, add `UseAuthentication()` before `UseAuthorization()`

**Actions:**
1. Add `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
2. Change `AppDbContext` base class to `IdentityDbContext<IdentityUser>`
3. Register Identity: `builder.Services.AddDefaultIdentity<IdentityUser>(opts => { opts.SignIn.RequireConfirmedAccount = false; }).AddEntityFrameworkStores<AppDbContext>()`
4. Add `app.UseAuthentication()` before `app.UseAuthorization()`
5. Add `[Authorize]` globally via `builder.Services.AddControllersWithViews(opts => opts.Filters.Add(new AuthorizeFilter()))`

**Success:** `dotnet build` passes. Navigating to any page redirects to `/Identity/Account/Login`.

### Phase 2 — Migration for Identity tables

**Files:**
- `app/Data/Migrations/` — new migration

**Actions:**
1. `dotnet ef migrations add AddIdentity --project app`
2. `dotnet ef database update --project app`

**Success:** Database contains AspNet* tables. Exit code 0.

### Phase 3 — Seed admin user & login/logout UI

**Files:**
- `app/Data/SeedData.cs` — static method to seed admin user
- `app/Program.cs` — call seeder after `app` built
- `app/Views/Shared/_LoginPartial.cshtml` — login/logout nav (Polish)
- `app/Views/Shared/_Layout.cshtml` — include `_LoginPartial`

**Actions:**
1. Create `SeedData.EnsureAdminAsync(IServiceProvider)` — creates user `admin@10xpv.local` / password from config (`AdminPassword` in user-secrets)
2. Call seeder in `Program.cs` using scoped `UserManager<IdentityUser>`
3. Scaffold `_LoginPartial.cshtml` with Polish labels ("Zaloguj", "Wyloguj", display email)
4. Add `@await Html.PartialAsync("_LoginPartial")` to `_Layout.cshtml` nav

**Success:** App starts, admin user exists in DB, login with seeded credentials works, logout returns to login page.

## Contracts

| Symbol | Type | Location |
|--------|------|----------|
| `AppDbContext : IdentityDbContext<IdentityUser>` | class | `app/Data/AppDbContext.cs` |
| `SeedData.EnsureAdminAsync` | static method | `app/Data/SeedData.cs` |
| `AdminPassword` | user-secret key | `secrets.json` |
| `_LoginPartial.cshtml` | partial view | `app/Views/Shared/` |

## Risks

- Scaffolded Identity UI pages may conflict with custom layout — use only Login/Logout, skip registration pages
- Password in user-secrets must be set manually on each dev machine

## Progress

<!-- Updated by /10x-implement -->
- [x] Phase 1 — Identity packages, DbContext integration & middleware → `28b60a5`
- [x] Phase 2 — Migration for Identity tables → `db07fd9`