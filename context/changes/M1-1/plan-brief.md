# Plan Brief: M1-1

## One-liner
Dwie niezależne encje (SensorReading, WeatherReading) w osobnych tabelach, walidacja, deduplikacja po timestamp, migracja EF Core.

## Phases (3)
1. Encje + EF Core config + unique indexes (brak dziedziczenia, osobne tabele)
2. Validator + DeduplicationService + DI
3. EF Core migration

## Key contracts
- `SensorReading` — tabela `SensorReadings`, unique(Timestamp)
- `WeatherReading` — tabela `WeatherReadings`, unique(Timestamp)
- `IClimateDataValidator.Validate()` → `ValidationResult`
- `IDeduplicationService.ExistsAsync(timestamp, source)` → bool

## Blockers
- Wymaga E0-1 (AppDbContext)