# Change: M4-1 — Usuwanie wszystkich danych klimatycznych

## Source

Roadmap Milestone 4, element M4-1 ([context/foundation/roadmap.md](context/foundation/roadmap.md))

## Scope

- Akcja "reset" czyszcząca wszystkie rekordy z tabel `SensorReading` i `WeatherReading`
- Potwierdzenie operacji w UI (modal lub dedykowana strona) przed wykonaniem
- Kontroler MVC + serwis + testy

## Status

✅ Done

## Progress

- [x] F1 — Serwis resetowania danych + DI + testy jednostkowe
- [x] F2 — Integracja MVC (akcja + widok potwierdzenia) + testy kontrolera

## Key decisions

1. Przycisk "Wyczyść wszystkie dane" umieszczony na stronie Import (`ClimateImport/Index`), nie na osobnej stronie.
2. Potwierdzenie przez modal Bootstrap przed wysłaniem POST.
3. Dedykowany serwis `IClimateDataResetService` / `ClimateDataResetService` z metodą `DeleteAllAsync()` usuwającą oba zbiory danych jednocześnie (`ExecuteDeleteAsync`).
4. Akcja POST `ResetAllData` w `ClimateImportController` z atrybutami `[Authorize]` + `[ValidateAntiForgeryToken]`.
5. Feedback użytkownika przez TempData (alert success/error) po przekierowaniu na stronę importu.

## Files changed

- `app/Services/IClimateDataResetService.cs` — interfejs
- `app/Services/ClimateDataResetService.cs` — implementacja
- `app/Program.cs` — rejestracja DI
- `app/Controllers/ClimateImportController.cs` — akcja `ResetAllData` (POST)
- `app/Views/ClimateImport/Index.cshtml` — przycisk + modal + alerty TempData
- `app/Tests/10xPV.Tests/Services/ClimateDataResetServiceTests.cs` — testy serwisu
- `app/Tests/10xPV.Tests/Controllers/ClimateImportControllerTests.cs` — aktualizacja CreateSut
- `app/Tests/10xPV.Tests/10xPV.Tests.csproj` — dodano pakiet Moq
