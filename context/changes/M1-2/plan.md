# Plan implementacji — M1-2 CSV parser (predefiniowane formaty)

## 1) Cel zmiany

Dostarczyć usługę parsera CSV dla dwóch predefiniowanych formatów (`Sensor`, `Weather`) z:
- walidacją nagłówków i typów danych,
- walidacją wierszy,
- zebraniem błędów per wiersz,
- kontraktem gotowym do podpięcia pod UI importu (M1-3) i zapis do bazy (M1-1).

## 2) Zakres / poza zakresem

### W zakresie
- Kontrakty domenowe parsera (wynik, rekordy, błędy).
- Implementacja parsera dla 2 schematów CSV.
- Walidacja struktury pliku i pól.
- Testy jednostkowe parsera i walidacji.

### Poza zakresem
- UI uploadu (`M1-3`).
- Persistencja do DB (`M1-1`, zależna od E0-1).
- Obsługa wielu użytkowników / autoryzacji (E0-2).

## 3) Zależności i bramki

- **Brama wejścia:** E0-1/E0-2 mogą być jeszcze w toku, ale parser ma być niezależny od DB i auth.
- **Brama wyjścia:** Kontrakt parsera ma umożliwiać późniejsze podpięcie do kontrolera importu i mapowania encji M1-1 bez zmiany API usługi.

## 4) Kontrakty plików (planowane)

- `app/Services/Csv/ICsvImportService.cs`  
  Interfejs usługi parsera.
- `app/Services/Csv/CsvImportService.cs`  
  Implementacja parsera (wybór schematu + parse + walidacja).
- `app/Services/Csv/Contracts/CsvSchemaType.cs`  
  Enum: `Sensor`, `Weather`.
- `app/Services/Csv/Contracts/CsvImportResult.cs`  
  Wynik: poprawne rekordy + błędy + metryki.
- `app/Services/Csv/Contracts/CsvRowError.cs`  
  Błąd per wiersz (nr linii, pole, kod, komunikat).
- `app/Services/Csv/Contracts/SensorCsvRow.cs`  
  DTO rekordu `Sensor`.
- `app/Services/Csv/Contracts/WeatherCsvRow.cs`  
  DTO rekordu `Weather`.
- `app/Tests/Services/Csv/CsvImportServiceTests.cs`  
  Testy parsera i walidacji.

> Jeśli istnieje już projekt testowy, użyć jego struktury; w przeciwnym razie utworzyć standardowy projekt testów `dotnet new xunit`.

## 4.1) Decyzje kontraktowe (zamrożone na M1-2)

- Delimiter CSV: `,`
- Kodowanie: UTF-8
- Pierwszy wiersz: nagłówki wymagane (match case-insensitive, trimowane)
- Format daty wejściowej: ISO 8601 (`yyyy-MM-dd` lub `yyyy-MM-ddTHH:mm:ss`)
- Liczby: `CultureInfo.InvariantCulture` (separator dziesiętny: `.`)
- Puste wiersze: ignorowane
- Brak deduplikacji w M1-2 (deduplikacja pozostaje w M1-1)

### Wymagane nagłówki (MVP)

- `Sensor`: `Timestamp, SensorId, Value, Unit`
- `Weather`: `Timestamp, TemperatureC, WindSpeedMs, IrradianceWm2`

## 5) Fazy realizacji

## Faza 1 — Kontrakt parsera i modele wejścia/wyjścia
- Zdefiniować enum schematu i DTO wyniku.
- Zdefiniować standard błędów walidacji (kody, komunikaty PL).
- Ustalić minimalny kontrakt wejścia: `Stream + schemaType` (bez zależności od MVC).
- **Zamrozić katalog kodów błędów już w F1**:
  - `CSV_EMPTY_FILE`
  - `CSV_HEADER_MISSING`
  - `CSV_HEADER_INVALID`
  - `CSV_ROW_FIELD_REQUIRED`
  - `CSV_ROW_DATETIME_INVALID`
  - `CSV_ROW_NUMBER_INVALID`
  - `CSV_ROW_NUMBER_NAN_OR_INF`

### Kryteria akceptacji F1
- Kompilacja przechodzi.
- Kontrakty nie zależą od EF/MVC/UI.
- Jest miejsce na listę błędów per wiersz i metryki importu.
- **Kody błędów i nagłówki są zamrożone i użyte w testach.**

## Faza 2 — Implementacja parsera `Sensor` i `Weather`
- Dodać parsowanie CSV z walidacją nagłówków.
- Dodać walidację pól:
  - wymagane pola,
  - parse `DateTime`,
  - parse wartości numerycznych (kultura invariant),
  - zakresy logiczne (np. brak NaN/Infinity).
- Dodać zbieranie błędów bez przerywania całego importu (best-effort per wiersz).
- Zwracać podsumowanie: `total`, `valid`, `invalid`.

### Kryteria akceptacji F2
- Dwa formaty CSV są obsługiwane.
- Błędny wiersz nie blokuje przetworzenia kolejnych.
- Wynik zawiera rekordy poprawne i pełną listę błędów.

## Faza 3 — Testy i gotowość pod integrację M1-3/M1-1
- Testy:
  - poprawny plik `Sensor`,
  - poprawny plik `Weather`,
  - niepoprawne nagłówki,
  - mieszane poprawne/błędne wiersze,
  - pusty plik / tylko nagłówek.
- Ustalić stabilne kody błędów do użycia w UI.
- Krótka notatka integracyjna (jak wywołać usługę z kontrolera).

### Kryteria akceptacji F3
- Testy przechodzą lokalnie.
- Kontrakt jest stabilny i gotowy do użycia przez M1-3.
- Brak regresji builda.

## 6) Weryfikacja

Uruchomienia lokalne (Windows/PowerShell):
- `cd app`
- `dotnet build`
- `dotnet test`

## 7) Ryzyka i decyzje robocze

- **Różne formaty CSV:** MVP ogranicza się do 2 znanych schematów.
- **Kultura liczb/dat:** parser używa jawnych reguł (`InvariantCulture` + uzgodniony format daty).
- **Duże pliki:** w tej zmianie brak optymalizacji strumieniowej ponad podstawowe czytanie liniowe.

## 8) Definition of Done

- Zaimplementowany parser `Sensor`/`Weather`.
- Walidacja struktury i danych z błędami per wiersz.
- Testy jednostkowe zielone.
- Planowane punkty integracji do UI i persistence nie wymagają zmiany kontraktu parsera.

## Progress

- [ ] F1 — Kontrakt parsera i modele
- [ ] F2 — Implementacja parsera + walidacja
- [ ] F3 — Testy i gotowość integracyjna

<!-- Updated by /10x-implement: phase status, commit SHA -->