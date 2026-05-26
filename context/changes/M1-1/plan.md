# Plan: M1-1 — Model danych klimatycznych + migracja

## Goal

Zdefiniować dwie niezależne encje EF Core (`SensorReading`, `WeatherReading`) w osobnych tabelach — bez wspólnej tabeli bazowej. Dodać walidację, deduplikację po timestamp i wygenerować migrację.

## Dependencies

- **E0-1** (DbContext + migration pipeline) — `AppDbContext` w `app/Data/` musi istnieć.

## Phases

### Phase 1 — Encje i konfiguracja EF Core

**Pliki:**
| Plik | Operacja | Opis |
|------|----------|------|
| `app/Models/DataSource.cs` | CREATE | Enum: `Sensor`, `Weather` |
| `app/Models/SensorReading.cs` | CREATE | Osobna encja: Id, Timestamp, Temperature, Humidity |
| `app/Models/WeatherReading.cs` | CREATE | Osobna encja: Id, Timestamp, Temperature, Humidity, CloudCover |
| `app/Data/Configurations/SensorReadingConfiguration.cs` | CREATE | Unique index (Timestamp), typy kolumn |
| `app/Data/Configurations/WeatherReadingConfiguration.cs` | CREATE | Unique index (Timestamp), typy kolumn |
| `app/Data/AppDbContext.cs` | MODIFY | Dodać `DbSet<SensorReading>`, `DbSet<WeatherReading>`, `ApplyConfigurationsFromAssembly` |

**Kontrakt:**
- **Brak dziedziczenia** — dwie w pełni niezależne tabele (`SensorReadings`, `WeatherReadings`)
- `Timestamp` — `DateTimeOffset`, NOT NULL, przechowywany jako UTC
- Unique index `IX_{Table}_Timestamp` per tabela
- Pomiary opcjonalne: `double?`
- `Id` — `Guid`, generowany po stronie klienta

**Kryterium sukcesu:** `dotnet build` przechodzi; DbContext rejestruje oba DbSety.

---

### Phase 2 — Walidacja i deduplikacja

**Pliki:**
| Plik | Operacja | Opis |
|------|----------|------|
| `app/Models/ClimateValidationResult.cs` | CREATE | `record ClimateValidationResult(bool IsValid, List<string> Errors)` |
| `app/Services/IClimateDataValidator.cs` | CREATE | `ClimateValidationResult Validate(SensorReading)`, `Validate(WeatherReading)` |
| `app/Services/ClimateDataValidator.cs` | CREATE | Zakresy dla SensorReading: temperature_c -60..+60, humidity_pct 0..100, timestamp ≤ now; Zakresy dla WeatherReading: temperature_c -60..+60, humidity_pct 0..100, cloud_cover_pct 0..100, timestamp ≤ now |
| `app/Services/IDeduplicationService.cs` | CREATE | `Task<bool> ExistsAsync(DateTimeOffset timestamp, DataSource source)` |
| `app/Services/DeduplicationService.cs` | CREATE | Query po unique index |
| `app/Program.cs` | MODIFY | DI: `AddScoped` dla obu serwisów |

**Kontrakt:**
- Walidator jest synchroniczny (pure logic, no DB)
- Deduplikacja: osobne query per `DataSource` enum → odpowiedni DbSet
- Deduplikacja sprawdza PRZED insertem

**Kryterium sukcesu:** dotnet build przechodzi; serwisy rejestrują się w DI bez błędów runtime.

---

### Phase 3 — Migracja EF Core

**Pliki:**
| Plik | Operacja | Opis |
|------|----------|------|
| `app/Data/Migrations/*_AddClimateModels.cs` | CREATE (generated) | `dotnet ef migrations add AddClimateModels` |

**Kryterium sukcesu:** `dotnet ef migrations script` generuje SQL z dwiema tabelami i unique indexes.

---

## Risks & Mitigations

| Ryzyko | Mitigacja |
|--------|-----------|
| E0-1 niegotowe | Sprawdzić `AppDbContext` przed Phase 1 |
| Przyszła korelacja wymaga JOIN | Korelacja w M2 użyje LINQ join po Timestamp — osobne tabele nie blokują |
| Timezone drift | Wymuszenie UTC na walidacji (`Timestamp.Offset == TimeSpan.Zero`) |

## Progress

<!-- Updated by /10x-implement -->
- [ ] Phase 1 — Encje i konfiguracja EF Core → —
- [ ] Phase 2 — Walidacja i deduplikacja → —
- [ ] Phase 3 — Migracja EF Core → —
