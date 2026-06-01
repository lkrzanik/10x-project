# Research: tp-unit-csv

## Odkrycia

### 1. Serwisy do przetestowania

| Serwis | Plik | Odpowiedzialność |
|--------|------|------------------|
| `CsvImportService` | `app/Services/Csv/CsvImportService.cs` | Parsowanie CSV (comma-separated, quoted fields), walidacja nagłówków, mapowanie wierszy Sensor/Weather |
| `ClimateDataValidator` | `app/Services/ClimateDataValidator.cs` | Walidacja zakresów (temp -60..60, humidity 0..100, cloud 0..100), timestamp UTC i nie-przyszły |
| `DeduplicationService` | `app/Services/DeduplicationService.cs` | Sprawdzenie czy rekord o danym timestamp+source już istnieje w DB |

### 2. Istniejące testy

Plik `Tests/10xPV.Tests/Services/Csv/CsvImportServiceTests.cs` (162 linii) — **już istnieje** z 7 testami:
- Valid Sensor row, Valid Weather row, Weather date with slash
- Invalid headers → error
- Mixed rows (valid + invalid date + NaN) → partial success
- Only header + empty rows → zero rows
- Empty file → EmptyFile error

### 3. Luki w pokryciu (R1, R3)

**R1 — Ciche zapisanie śmieciowych danych:**
- ❌ Brak testu: semicolon separator zamiast comma (CSV polski format) — system powinien zwrócić błąd nagłówka
- ❌ Brak testu: za mało kolumn w wierszu danych (np. brakuje Humidity)
- ❌ Brak testu: dodatkowe kolumny (nadmiarowe dane)
- ❌ Brak testu: Sensor schema z 4 kolumnami Weather → HeaderInvalid
- ❌ Brak testu: wartość pusta w polu (np. `2026-01-01,,63.0`)
- ❌ Brak testu: BOM w pliku UTF-8
- ❌ Brak testu: quoted fields z przecinkami wewnątrz
- ❌ Brak testu: Infinity w polu numerycznym
- ❌ Brak testu: `ClimateDataValidator` — temperature out of range, humidity out of range, future timestamp

**R3 — Deduplikacja:**
- ❌ Brak testów `DeduplicationService` — wymaga in-memory DbContext
- Scenariusze: ExistsAsync=true dla istniejącego timestamp, false dla nowego, poprawne rozróżnienie Sensor vs Weather

### 4. Stos testowy — stan

- Projekt testowy: xUnit + Moq (nie NSubstitute jak w test-plan §4 — **rozbieżność**)
- Brak FluentAssertions w csproj (test-plan §4 je zakłada)
- Jest `Microsoft.EntityFrameworkCore.InMemory` — gotowe do testów deduplikacji
- Brak coverlet w konfiguracji (jest pakiet, ale brak ustawień zbierania)

### 5. Kontrakt parsera CSV

- Separator: **comma** (nie semicolon) — `ParseCsvLine` rozdziela po `,`
- Obsługuje quoted fields (podwójne cudzysłowy, escaped `""`)
- Nagłówki case-insensitive
- Formaty dat: `MM/dd/yyyy`, `yyyy-MM-dd`, `yyyy/MM/dd`, warianty z czasem, ISO format T
- Waliduje: empty file, missing headers, invalid headers, required fields, invalid date, invalid number, NaN/Infinity
- Pomija puste linie (whitespace-only)

### 6. Model danych deduplikacji

- `DataSource` enum: `Sensor`, `Weather`
- Klucz deduplikacji: **timestamp** (bez dodatkowych pól)
- Sprawdza `AnyAsync` na odpowiedniej tabeli DbContext

## Wnioski dla planu

1. **CsvImportService** — potrzeba ~8-10 dodatkowych testów na edge cases (separator, brakujące kolumny, quoted fields, BOM, schema mismatch)
2. **ClimateDataValidator** — potrzeba ~6-8 testów (valid sensor, valid weather, out-of-range każdego pola, future timestamp, non-UTC offset)
3. **DeduplicationService** — potrzeba ~4 testy z in-memory DB (exists sensor, not exists sensor, exists weather, wrong source returns false)
4. **Decyzja stosu**: Moq już w projekcie — użyć Moq (nie NSubstitute). FluentAssertions opcjonalne — istniejące testy używają `Assert.*`.
5. Istniejące testy przechodzą — nowe testy **rozszerzają**, nie zastępują.
