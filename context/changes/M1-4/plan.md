# Plan implementacji — M1-4 Widok tabeli danych

## 1) Cel zmiany

Dostarczyć widok danych klimatycznych zapisanych po imporcie CSV, który pozwala użytkownikowi:
- przeglądać rekordy w tabeli,
- filtrować rekordy po zakresie dat,
- przechodzić między stronami wyników,
- przełączać źródło danych (`Sensor` / `Weather`) bez utraty spójności UI.

Równolegle domknąć warstwę persistence rozpoczętą w M1-3, tak aby importowane poprawne wiersze były trwale zapisywane w `AppDbContext` i widoczne w tabeli.

## 2) Zakres / poza zakresem

### W zakresie
- Implementacja adaptera persistence zamiast `NoOpClimateImportPersistenceAdapter`.
- Zapis poprawnych rekordów CSV do tabel `SensorReadings` i `WeatherReadings`.
- Endpoint MVC do renderowania tabeli danych z parametrami filtra/paginacji.
- ViewModel-e dla filtra i listy wyników (metadata paginacji).
- Widok Razor tabeli danych z filtrem daty i kontrolkami paginacji.
- Testy jednostkowe dla persistence i kontrolera widoku tabeli.
- Aktualizacja kontraktów (`docs/reference/contract-surfaces.md`) o nowe endpointy UI i seam query.

### Poza zakresem
- Sortowanie po wielu kolumnach i zaawansowane filtry (np. po temperaturze/wilgotności).
- Eksport tabeli do CSV/PDF.
- Wykresy i wizualizacje (zakres M2).
- Wieloużytkownikowość i role.

## 3) Zależności i bramki

- **Brama wejścia:**
  - E0-1: baza danych i migracje działają.
  - E0-2: uwierzytelnianie działa i wymusza dostęp tylko po zalogowaniu.
  - M1-3: endpoint importu + orchestrator + no-op persistence seam są gotowe.
- **Brama wyjścia:**
  - Poprawne rekordy importu są trwale zapisywane.
  - Użytkownik widzi rekordy w tabeli z filtrem dat i paginacją.
  - Build + testy przechodzą.

## 4) Kontrakty plików (planowane)

- `app/Services/ClimateImport/NoOpClimateImportPersistenceAdapter.cs`  
  Zastąpienie implementacją zapisującą do EF Core (lub podmiana w DI na nową klasę).
- `app/Services/ClimateImport/ClimateImportPersistenceAdapter.cs` *(nowy)*  
  Mapowanie poprawnych rekordów parsera na `SensorReading` / `WeatherReading`, deduplikacja po `Timestamp`, zapis batchowy.
- `app/Services/ClimateImport/IClimateImportPersistenceAdapter.cs`  
  Kontrakt bez zmian (lub minimalne doprecyzowanie dokumentacyjne).
- `app/Services/ClimateImport/ClimateImportOrchestrator.cs`  
  Utrzymanie przepływu parser -> persistence; ewentualne logowanie metryk zapisu.
- `app/Controllers/ClimateDataController.cs` *(nowy)*  
  Endpoint `Index` (GET) dla widoku tabeli danych, parametry: typ danych, zakres dat, numer strony.
- `app/Models/ClimateImport/ClimateDataFilterViewModel.cs` *(nowy)*  
  Dane filtra: typ źródła, data od, data do, page, pageSize.
- `app/Models/ClimateImport/ClimateDataRowViewModel.cs` *(nowy)*  
  Ujednolicony rekord do renderu w tabeli (timestamp, temperature, humidity, cloudCover).
- `app/Models/ClimateImport/ClimateDataPageViewModel.cs` *(nowy)*  
  Strona wynikowa: filtry, rekordy, metadane paginacji (totalCount, totalPages, hasPrev, hasNext).
- `app/Views/ClimateData/Index.cshtml` *(nowy)*  
  Tabela danych + formularz filtrów + kontrolki paginacji.
- `app/Views/Shared/_Layout.cshtml`  
  Link nawigacyjny do tabeli danych klimatycznych.
- `app/Tests/10xPV.Tests/Services/ClimateImport/ClimateImportPersistenceAdapterTests.cs` *(nowy)*  
  Testy mapowania i deduplikacji zapisu.
- `app/Tests/10xPV.Tests/Controllers/ClimateDataControllerTests.cs` *(nowy)*  
  Testy akcji GET: domyślny widok, filtr dat, paginacja skrajna.
- `docs/reference/contract-surfaces.md`  
  Rejestr kontraktu endpointu `ClimateData/Index` i seam query/listingu.

## 4.1) Kontrakt wejścia/wyjścia (MVP)

### Input (GET `ClimateData/Index`)
- `dataSource` — enum (`Sensor`, `Weather`), domyślnie `Sensor`.
- `from` — opcjonalna data początkowa (`DateOnly?`).
- `to` — opcjonalna data końcowa (`DateOnly?`).
- `page` — numer strony, min 1.
- `pageSize` — rozmiar strony, MVP: stały 50.

### Walidacja wejścia
- Jeśli `from > to`: walidacja użytkownika i powrót do widoku bez wyjątku.
- Jeśli `page < 1`: normalizacja do `1`.
- Jeśli `page` wykracza poza `totalPages`: normalizacja do ostatniej dostępnej strony (lub pusty wynik na stronie 1 przy `totalCount == 0`).

### Output (UI)
- Tabela rekordów (kolumny MVP):
  - `Timestamp`
  - `Temperature`
  - `Humidity`
  - `CloudCover` (dla `Weather`; dla `Sensor` wartość `-`)
- Podsumowanie listy:
  - `TotalCount`
  - `CurrentPage`
  - `TotalPages`
  - `PageSize`

### Error modes
- Błąd walidacji filtra: ten sam widok + komunikat `ModelState`.
- Błąd odczytu/zapisu DB: bezpieczny komunikat użytkownika + log po stronie serwera.

## 5) Fazy realizacji

## Faza 1 — Persistence adapter (domknięcie import -> DB)
- Dodać implementację adaptera persistence opartą o `AppDbContext`.
- Zmapować poprawne rekordy parsera na encje domenowe.
- Dodać regułę deduplikacji po `Timestamp` z uwzględnieniem unikalnych indeksów DB.
- Podpiąć implementację w DI zamiast no-op.

### Kryteria akceptacji F1
- Import poprawnych rekordów skutkuje zapisem do DB.
- Brak duplikatów przy ponownym imporcie tych samych znaczników czasu.
- `ClimateImportController` nie wymaga zmiany kontraktu.

## Faza 2 — Query + endpoint tabeli z filtrem i paginacją
- Dodać kontroler `ClimateDataController` z akcją `Index` (GET).
- Zaimplementować query z `AsNoTracking()`, filtrem dat i paginacją po stronie SQL (`Skip/Take`).
- Dodać mapowanie encji do `ClimateDataRowViewModel`.
- Dodać walidację parametrów filtra/paginacji.

### Kryteria akceptacji F2
- Użytkownik widzi dane sensor/weather w tabeli.
- Filtr dat zwraca tylko rekordy z zadanego zakresu.
- Paginacja działa poprawnie dla pierwszej, środkowej i ostatniej strony.

## Faza 3 — UI, testy i stabilizacja kontraktu
- Zbudować widok Razor z formularzem filtra i kontrolkami paginacji.
- Dodać link nawigacyjny w layoucie.
- Dodać testy kontrolera i persistence adaptera (happy path + edge cases).
- Zaktualizować `docs/reference/contract-surfaces.md`.

### Kryteria akceptacji F3
- Widok jest czytelny i spójny z obecnym UI Bootstrap.
- Testy nowych komponentów przechodzą.
- `dotnet build` i `dotnet test` przechodzą bez regresji.

## 6) Ryzyka i decyzje robocze

- **Ryzyko konfliktów unikalnego indeksu `Timestamp`:**
  - Decyzja: deduplikować przed zapisem i/lub bezpiecznie obsługiwać conflict przy `SaveChangesAsync`.
- **Ryzyko mieszania stref czasowych:**
  - Decyzja: traktować dane jako `DateTimeOffset` i filtrować po dacie w strefie UTC (jawnie udokumentować format filtra).
- **Ryzyko dużych wolumenów danych:**
  - Decyzja: obowiązkowa paginacja server-side, brak pobierania całej tabeli do pamięci.

## 6.1) Edge cases do pokrycia

- Brak danych po imporcie (pusta tabela).
- `from` bez `to`, `to` bez `from`.
- `from > to`.
- Żądanie strony większej niż `totalPages`.
- Ponowny import tych samych danych (duplikaty timestamp).
- Rekordy z `null` w `Temperature`/`Humidity`/`CloudCover`.

## 7) Weryfikacja

Uruchomienia lokalne (Windows/PowerShell):
- `cd app`
- `dotnet build`
- `dotnet test`

## 8) Definition of Done

- Poprawne rekordy z importu trafiają do DB bez duplikatów.
- Dostępny widok tabeli danych z filtrem daty i paginacją.
- UI dostępne z nawigacji aplikacji.
- Testy jednostkowe nowych elementów przechodzą.
- Build/test regresji przechodzą.

## Progress

- [x] F1 — Persistence adapter (domknięcie import -> DB)
- [x] F2 — Query + endpoint tabeli z filtrem i paginacją
- [x] F3 — UI, testy i stabilizacja kontraktu

### 2026-05-26 — F1 implementacja

- Dodano `ClimateImportPersistenceAdapter` (EF Core) i podpięto go w DI zamiast `NoOpClimateImportPersistenceAdapter`.
- Dodano deduplikację po `Timestamp` dla `Sensor` i `Weather` (duplikaty w bazie + duplikaty w bieżącym batchu).
- Kontrakt `ClimateImportController` i orchestratora pozostawiono bez zmian.
- Weryfikacja: `dotnet build` ✅, `dotnet test .\Tests\10xPV.Tests\10xPV.Tests.csproj` ✅ (14/14).

### 2026-05-26 — F2 implementacja

- Dodano `ClimateDataController` z akcją `Index` (GET) obsługującą `dataSource`, `from`, `to`, `page`, `pageSize`.
- Zaimplementowano zapytania `AsNoTracking()` dla `SensorReadings` i `WeatherReadings` z filtrem zakresu dat oraz paginacją server-side (`Skip/Take`).
- Dodano walidację wejścia `from > to` (bez wyjątku) oraz normalizację paginacji (`page < 1`, `page > totalPages`).
- Dodano nowe ViewModel-e: `ClimateDataFilterViewModel`, `ClimateDataRowViewModel`, `ClimateDataPageViewModel`.
- Dodano testy `ClimateDataControllerTests` pokrywające: domyślny widok, filtr daty, paginację skrajną i błąd walidacji zakresu dat.
- Weryfikacja: `dotnet build` ✅, `dotnet test .\Tests\10xPV.Tests\10xPV.Tests.csproj` ✅ (18/18).

### 2026-05-26 — F3 implementacja

- Dodano widok `Views/ClimateData/Index.cshtml` z formularzem filtra (`dataSource`, `from`, `to`), tabelą wyników i kontrolkami paginacji.
- Dodano link nawigacyjny `Dane klimatyczne` w `Views/Shared/_Layout.cshtml`.
- Rozszerzono testy `ClimateDataControllerTests` o edge-case: normalizacja `page < 1` oraz filtr z samą granicą `to`.
- Zaktualizowano `docs/reference/contract-surfaces.md` o endpoint `ClimateData/Index` i seam query kontrolera.
- Weryfikacja: `dotnet build` ✅, `dotnet test .\Tests\10xPV.Tests\10xPV.Tests.csproj` ✅ (20/20), `get_errors` ✅ (brak błędów).
- Uwaga: pozostało istniejące ostrzeżenie zależności `NU1903` dla `Microsoft.Build.Tasks.Core` 17.7.2 (poza zakresem tej fazy).

<!-- Updated by /10x-plan -->
