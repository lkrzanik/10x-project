# Plan Brief: E0-2

3 phases: (1) Add Identity packages, change DbContext to IdentityDbContext, register middleware + global [Authorize], (2) Generate Identity migration, (3) Seed admin user + Polish login/logout partial in nav.

Key contracts: `AppDbContext : IdentityDbContext<IdentityUser>`, `SeedData.EnsureAdminAsync`, admin credentials via user-secrets.