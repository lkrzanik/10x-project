# Plan: tp-unit-csv

## Cel

Dostarczyć tani i wiarygodny sygnał dla ryzyk **R1** (cichy import błędnych danych) oraz **R3** (błędna deduplikacja) przez rozszerzenie testów jednostkowych warstwy importu CSV, walidacji danych i deduplikacji.

## Wejścia

- `context/changes/tp-unit-csv/change.md`
- `context/changes/tp-unit-csv/research.md`
- `context/foundation/test-plan.md` (§3 Faza 1)

## Założenia robocze

1. Pozostajemy przy aktualnym stosie testowym repo (`xUnit` + `Moq` + `Assert.*`), bez migracji na `NSubstitute`.
2. Implementujemy wyłącznie testy (bez zmian logiki produkcyjnej), chyba że test ujawni krytyczny błąd blokujący uruchomienie.

## Zakres fazy

1. Rozszerzenie testów `CsvImportService` o brakujące przypadki brzegowe (separator, schemat, brak/nadmiar kolumn, wartości puste, BOM, quoted fields, Infinity).
2. Dodanie testów `ClimateDataValidator` dla zakresów i czasu (out-of-range, future timestamp, offset/non-UTC).
3. Dodanie testów `DeduplicationService` z in-memory EF Core (`Sensor`/`Weather`, istniejący/nieistniejący rekord).
4. Weryfikacja uruchamiania testów i aktualizacja cookbook w `test-plan.md`.

## Poza zakresem

- Testy integracyjne importu (`tp-integration-import`, Faza 4).
- Zmiany CI/CD, hooków i konfiguracji pipeline.
- Testy e2e/UI.

## Strategia koszt × sygnał

| Obszar | Warstwa | Sygnał | Koszt | Decyzja |
|---|---|---|---|---|
| Parser CSV (`CsvImportService`) | Unit | Wysoki (bezpośrednia walidacja kontraktu parsera) | Niski | Implementujemy szeroki zestaw edge cases |
| Walidacja danych (`ClimateDataValidator`) | Unit | Wysoki (ochrona jakości danych wejściowych) | Niski | Implementujemy testy granic i czasu |
| Deduplikacja (`DeduplicationService`) | Unit + InMemory DB | Średnio-wysoki (regresja klucza timestamp+source) | Niski/Średni | Implementujemy minimalny zestaw scenariuszy źródło+czas |
| End-to-end import | Integration | Najwyższy, ale droższy | Średni/Wysoki | Odkładamy do Fazy 4 zgodnie z §3 |

## Plan implementacji

### Krok 1 — Parser CSV (R1)

- Rozszerzyć `Tests/10xPV.Tests/Services/Csv/CsvImportServiceTests.cs` o przypadki:
  - zły separator (`;`) i nagłówek niezgodny z kontraktem,
  - brakujące kolumny / nadmiar kolumn,
  - mismatch schema Sensor vs Weather,
  - puste wartości wymagane,
  - BOM UTF-8 na wejściu,
  - quoted fields z przecinkiem,
  - wartości `Infinity`.
- Zachować spójny styl testów i nazewnictwo (`Method_Scenario_Expected`).

### Krok 2 — Walidator klimatu (R1)

- Dodać nowy plik testów (np. `Tests/10xPV.Tests/Services/ClimateDataValidatorTests.cs`).
- Pokryć:
  - valid sensor/weather,
  - temperatury poza zakresem,
  - wilgotność i zachmurzenie poza zakresem,
  - timestamp z przyszłości,
  - timestamp z offsetem (normalizacja/dozwolone zachowanie wg obecnej implementacji).

### Krok 3 — Deduplikacja (R3)

- Dodać testy `DeduplicationService` z `Microsoft.EntityFrameworkCore.InMemory`.
- Scenariusze minimalne:
  - `ExistsAsync` zwraca true dla istniejącego `Sensor`,
  - false dla nieistniejącego `Sensor`,
  - true dla istniejącego `Weather`,
  - false dla tego samego timestamp, ale innego źródła.

### Krok 4 — Weryfikacja i domknięcie fazy

- Uruchomić testy projektu `10xPV.Tests`.
- Upewnić się, że brak regresji istniejących testów.
- Uzupełnić `context/foundation/test-plan.md` §6.1 o:
  - lokalizację testów,
  - konwencję nazewnictwa,
  - test referencyjny,
  - komendę uruchomienia.

## Kryteria akceptacji

1. Dla obszarów `CsvImportService`, `ClimateDataValidator`, `DeduplicationService` istnieją testy pokrywające główne scenariusze błędów z research.
2. `dotnet test` dla `app/Tests/10xPV.Tests/10xPV.Tests.csproj` przechodzi w 100%.
3. `context/foundation/test-plan.md` ma zaktualizowane §6.1 (cookbook) i status Fazy 1 ustawiony na `planned`.
4. `plan.md` zawiera checklistę postępu gotową do realizacji w `/10x-implement`.

## Ryzyka realizacyjne

- Rozbieżność dokumentacji stosu (`NSubstitute`) vs kodu (`Moq`) może powodować niespójne przykłady — utrwalamy decyzję o `Moq` na potrzeby tej fazy.
- Część edge cases może ujawnić defekty logiki produkcyjnej; jeśli to nastąpi, odnotować i otworzyć osobną zmianę naprawczą.

## Progress

- [ ] Krok 1: dopisane testy parsera CSV
- [ ] Krok 2: dopisane testy `ClimateDataValidator`
- [ ] Krok 3: dopisane testy `DeduplicationService`
- [ ] Krok 4: testy uruchomione i zielone
- [ ] Cookbook §6.1 zaktualizowany
- [ ] Faza 1 oznaczona jako `complete` po implementacji
