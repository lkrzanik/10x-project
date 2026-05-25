# Plan Brief: E0-1

3 phases: (1) Add EF Core packages + empty AppDbContext + DI registration, (2) Configure connection strings for Dev/Azure, (3) Generate initial migration and verify database creation.

Key contract: `_10xPV.Data.AppDbContext` registered via `AddDbContext` using `ConnectionStrings:DefaultConnection`.