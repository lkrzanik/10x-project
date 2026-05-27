# /10x-impl-review — M1-4

Data: 2026-05-27
Zmiana: `context/changes/M1-4`
Reviewer: AI

## Wynik końcowy

**Status: REQUIRES TRIAGE (nie rekomenduję merge bez decyzji dla P1)**

Implementacja pokrywa większość zakresu (persistence + tabela + filtrowanie + paginacja + testy), ale wykryto istotną niespójność kontraktu i nawigacji źródła danych, która wpływa na główną ścieżkę UI (`Sensor`/`Weather`).

## Sprawdzone artefakty

- Plan: `context/changes/M1-4/plan.md`
- Kod:
  - `app/Services/ClimateImport/ClimateImportPersistenceAdapter.cs`
  - `app/Controllers/ClimateDataController.cs`
  - `app/Views/ClimateData/Index.cshtml`
  - `app/Views/Shared/_Layout.cshtml`
  - `app/Program.cs`
- Testy:
  - `app/Tests/10xPV.Tests/Services/ClimateImport/ClimateImportPersistenceAdapterTests.cs`
  - `app/Tests/10xPV.Tests/Controllers/ClimateDataControllerTests.cs`
- Kontrakty: `docs/reference/contract-surfaces.md`

## Triage ustaleń

| ID | Obszar | Ważność | Wpływ teraz | Ustalenie | Decyzja |
| --- | --- | --- | --- | --- | --- |
| F1 | UI/Endpoint contract | P1 (wysoka) | Wysoki | Przełączanie źródła danych w widoku i linki paginacji odwołują się do `ClimateData/Index` + `dataSource`, ale kontroler nie ma akcji `Index(DataSource, ...)`; `Index()` robi redirect do `Sensor`, ignorując `dataSource`. | **fix now** |
| F2 | Dokumentacja kontraktów | P2 (średnia) | Średni | `docs/reference/contract-surfaces.md` deklaruje seam `ClimateDataController.Index(DataSource, DateOnly?, DateOnly?, int, int, CancellationToken)`, a implementacja expose'uje `Sensor(...)` i `Weather(...)`. | **fix now** (razem z F1) |

## Dowody (evidence)

### F1 — niespójność przepływu `dataSource`

1. `app/Controllers/ClimateDataController.cs`
   - `Index()` tylko: `return RedirectToAction(nameof(Sensor));`
   - brak podpisu akcji `Index(DataSource dataSource, DateOnly? from, DateOnly? to, int page, ...)`
2. `app/Views/ClimateData/Index.cshtml`
   - formularz zawiera `<select name="dataSource" ...>`
   - paginacja linkuje do `asp-action="Index"` i przekazuje `asp-route-dataSource`
3. Efekt:
   - wybór `Weather` w filtrze/paginacji nie jest kontraktowo obsłużony przez `Index`, bo redirect zawsze kieruje na `Sensor`.

### F2 — kontrakt referencyjny != implementacja

- `docs/reference/contract-surfaces.md` deklaruje:
  - endpoint: `ClimateData/Index (GET)` jako wejście dla tabeli `Sensor/Weather`
  - seam: `ClimateDataController.Index(DataSource, DateOnly?, DateOnly?, int, int, CancellationToken)`
- Implementacja:
  - osobne akcje `Sensor(...)` i `Weather(...)`
  - `Index()` bez parametrów.

## Zakres i jakość (co jest dobre)

- Persistence adapter (`ClimateImportPersistenceAdapter`) realizuje deduplikację po `Timestamp` i zapis przez EF Core.
- Query używa `AsNoTracking()`, ma filtrowanie zakresu i paginację `Skip/Take`.
- Obsłużone są edge-case’y walidacji (`from > to`) i normalizacji strony.
- Testy jednostkowe pokrywają główne scenariusze (persistence + controller).

## Bramki jakości

- Build: **PASS** (`dotnet build`)
- Lint/Typecheck/Syntax: **PASS** (`get_errors` dla zmienionych plików)
- Tests: **PASS** (`dotnet test`, 21/21)

## Rekomendowany follow-up

1. Rozstrzygnąć F1/F2 jedną spójną decyzją architektoniczną:
   - **Opcja A (zgodna z planem/kontraktem):** dodać `Index(DataSource, from, to, page, ...)` jako główny endpoint i utrzymać `Sensor/Weather` jako aliasy (lub usunąć).
   - **Opcja B:** zostawić `Sensor/Weather`, ale wtedy poprawić widok (form action + paginacja) i kontrakty dokumentacyjne, aby nie wskazywały `Index(DataSource, ...)`.
2. Po decyzji dopisać test scenariusza przełączenia źródła (`Sensor -> Weather`) przez endpoint używany przez UI.
