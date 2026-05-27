# /10x-impl-review — M1-1

Data: 2026-05-27
Zmiana: `context/changes/M1-1`
Reviewer: AI

## Wynik końcowy

**Status: REQUIRES TRIAGE (nie rekomenduję merge bez decyzji dla F1)**

Faza 1 i 3 są dostarczone zgodnie z planem (dwie niezależne tabele, indeksy unikalne, migracja), ale kluczowy kontrakt fazy 2 nie jest domknięty w ścieżce runtime importu: walidator i deduplikacja zostały zdefiniowane, lecz nie są użyte przez pipeline zapisu.

## Sprawdzone artefakty

- Plan: `context/changes/M1-1/plan.md`
- Kod:
  - `app/Models/DataSource.cs`
  - `app/Models/SensorReading.cs`
  - `app/Models/WeatherReading.cs`
  - `app/Models/ClimateValidationResult.cs`
  - `app/Data/Configurations/SensorReadingConfiguration.cs`
  - `app/Data/Configurations/WeatherReadingConfiguration.cs`
  - `app/Data/AppDbContext.cs`
  - `app/Services/IClimateDataValidator.cs`
  - `app/Services/ClimateDataValidator.cs`
  - `app/Services/IDeduplicationService.cs`
  - `app/Services/DeduplicationService.cs`
  - `app/Services/ClimateImport/ClimateImportOrchestrator.cs`
  - `app/Services/ClimateImport/ClimateImportPersistenceAdapter.cs`
  - `app/Program.cs`
  - `app/Migrations/3_AddClimateModels.cs`
- Weryfikacja runtime:
  - `dotnet build`
  - `dotnet test --no-build`
  - `dotnet ef migrations script --idempotent`

## Triage ustaleń

| ID | Obszar | Ważność | Wpływ teraz | Ustalenie | Decyzja |
| --- | --- | --- | --- | --- | --- |
| F1 | Runtime contract (Phase 2) | P1 (wysoka) | Wysoki | `IClimateDataValidator` i `IDeduplicationService` istnieją, ale nie są używane w ścieżce importu (`ClimateImportOrchestrator`/`ClimateImportPersistenceAdapter`). Kontrakt „deduplikacja sprawdza PRZED insertem przez dedup service” oraz walidacja zakresów/timestamp na danych domenowych nie są egzekwowane przez dedykowane serwisy. | **fix now** |
| F2 | Spójność kontraktu typu wyniku walidacji | P3 (niska) | Niski | Plan wskazuje `ClimateValidationResult(bool IsValid, List<string> Errors)`, implementacja ma `IReadOnlyList<string>` + factory methods. To zmiana korzystna (niemutowalność API), ale odstępstwo od zapisu planu. | **disagree** (zaakceptować jako lepszy wariant) |

## Dowody (evidence)

### F1 — serwisy fazy 2 nie są wpięte do przepływu importu

1. `Program.cs` rejestruje oba serwisy w DI:
   - `AddScoped<IClimateDataValidator, ClimateDataValidator>()`
   - `AddScoped<IDeduplicationService, DeduplicationService>()`
2. `ClimateImportOrchestrator` korzysta wyłącznie z:
   - `ICsvImportService`
   - `IClimateImportPersistenceAdapter`
3. `ClimateImportPersistenceAdapter` wykonuje własną deduplikację zapytaniem po `Timestamp`, ale **nie używa** `IDeduplicationService`.
4. Analiza użyć symboli pokazuje brak konsumentów biznesowych:
   - `IClimateDataValidator` i `IDeduplicationService` użyte tylko w definicji + rejestracji DI (oraz klasie implementacji), bez wywołań z importerów/kontrolerów.

### F2 — kontrakt typu `Errors`

- Plan: `List<string>`
- Kod: `IReadOnlyList<string>`
- Ocena: bezpieczne i bardziej restrykcyjne API; brak negatywnego wpływu funkcjonalnego.

## Zakres i jakość (co jest dobre)

- Dwie niezależne encje i tabele (`SensorReadings`, `WeatherReadings`) bez dziedziczenia.
- `Timestamp` jako `DateTimeOffset`, indeksy unikalne `IX_SensorReadings_Timestamp` i `IX_WeatherReadings_Timestamp`.
- `Id` ustawione jako `ValueGeneratedNever()` i tworzone po stronie aplikacji (`Guid.NewGuid()`).
- Migracja `3_AddClimateModels` poprawnie tworzy obie tabele i indeksy.
- Aplikacja przechodzi build i testy.

## Bramki jakości

- Build: **PASS** (`dotnet build`)
- Lint/Typecheck/Syntax: **PASS** (brak błędów w analizie)
- Tests: **PASS** (`dotnet test --no-build`, 21/21)

## Rekomendowany follow-up

1. Domknąć F1 jedną z dwóch dróg (preferowana A):
   - **A (zgodnie z planem):** wpiąć `IClimateDataValidator` i `IDeduplicationService` do pipeline importu, tak by każdy rekord był walidowany i sprawdzany na duplikat przed insertem.
   - **B (jeśli świadoma zmiana architektury):** zaktualizować plan/kontrakty, że deduplikacja i walidacja domenowa są realizowane innym seamem (adapter), i usunąć martwe serwisy.
2. Dodać testy jednostkowe pokrywające użycie seamów z fazy 2 (minimum: 1 happy path + 2 edge case’y: future timestamp, duplicate timestamp).
