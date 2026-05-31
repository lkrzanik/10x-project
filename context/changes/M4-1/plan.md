# Plan — M4-1 Usuwanie wszystkich danych klimatycznych

Data: 2026-05-31
Zmiana: `M4-1`

## Status realizacji planu

- ⏳ Phase 1 — pending (`serwis + DI + testy jednostkowe`)
- ⏳ Phase 2 — pending (`MVC akcja + widok potwierdzenia + testy kontrolera`)

## Cel zmiany

Umożliwić użytkownikowi zresetowanie aplikacji poprzez usunięcie wszystkich danych klimatycznych — wszystkich rekordów z tabel `SensorReading` i `WeatherReading` — z poziomu UI, z wymaganym potwierdzeniem przed wykonaniem operacji.

## Decyzje projektowe

1. **Operacja nieodwracalna** — UI musi wyraźnie komunikować, że dane zostaną trwale usunięte.
2. **Potwierdzenie przez modal Bootstrap** — przed wysłaniem żądania POST.
3. **Dedykowany serwis** `IClimateDataResetService` — enkapsulacja logiki czyszczenia, łatwy do mockowania w testach.
4. **POST, nie GET** — akcja czyszczenia wywoływana przez formularz HTTP POST (ochrona przed przypadkowym wywołaniem).
5. **Dostęp tylko dla zalogowanego użytkownika** (`[Authorize]`).

## Kontrakt implementacyjny

### Wejście

Żądanie HTTP POST z potwierdzenia w UI (brak dodatkowych parametrów).

### Wyjście

Przekierowanie do widoku importu lub strony głównej z komunikatem sukcesu (TempData).

## Fazy implementacji

### Phase 1 — Serwis + DI + testy jednostkowe

**Pliki do utworzenia/modyfikacji:**
- `app/Services/IClimateDataResetService.cs` — interfejs
- `app/Services/ClimateDataResetService.cs` — implementacja (`DeleteAllAsync` dla obu encji)
- `app/Program.cs` — rejestracja DI
- `app/Tests/10xPV.Tests/Services/ClimateDataResetServiceTests.cs` — testy jednostkowe (mock DbContext)

**Quality gate:** `dotnet build` PASS, `dotnet test` PASS

### Phase 2 — Integracja MVC + widok + testy kontrolera

**Pliki do utworzenia/modyfikacji:**
- `app/Controllers/ClimateDataController.cs` — nowa akcja `ResetAllData` (GET + POST)
- `app/Views/ClimateData/ResetAllData.cshtml` — widok potwierdzenia z modalem Bootstrap
- `app/Tests/10xPV.Tests/Controllers/ClimateDataControllerResetTests.cs` — testy kontrolera

**Quality gate:** `dotnet build` PASS, `dotnet test` PASS
