# M1-2 — Notatka integracyjna (M1-3 / M1-1)

## Cel

Podłączyć parser CSV (`ICsvImportService`) do kontrolera importu w M1-3 i przygotować mapowanie rekordów pod zapis encji w M1-1 bez zmiany kontraktu parsera.

## Kontrakt usługi

- Interfejs: `ICsvImportService`
- Metoda: `Task<CsvImportResult> ImportAsync(Stream csvStream, CsvSchemaType schemaType, CancellationToken cancellationToken = default)`
- Rejestracja DI już istnieje w `app/Program.cs`:
  - `builder.Services.AddScoped<ICsvImportService, CsvImportService>();`

## Minimalny przepływ w kontrolerze importu (M1-3)

1. Odbierz plik CSV (`IFormFile`) i typ schematu (`Sensor` / `Weather`).
2. Otwórz strumień z pliku i wywołaj `ImportAsync`.
3. Jeśli `result.Errors.Any()`, pokaż błędy per wiersz w UI (line/field/code/message).
4. Jeśli są poprawne rekordy, przekaż je do warstwy zapisu (M1-1).

## Przykładowy szkic użycia

- `Sensor`: użyj `result.SensorRows`
- `Weather`: użyj `result.WeatherRows`
- Metryki do UI: `TotalRows`, `ValidRows`, `InvalidRows`

## Mapowanie pod encje (M1-1)

### SensorCsvRow -> SensorReading

- `Timestamp` -> `RecordedAtUtc` / `Timestamp` (zgodnie z nazwą docelowego modelu)
- `SensorId` -> `SensorId`
- `Value` -> `Value`
- `Unit` -> `Unit`

### WeatherCsvRow -> WeatherReading

- `Timestamp` -> `RecordedAtUtc` / `Timestamp`
- `TemperatureC` -> `TemperatureC`
- `WindSpeedMs` -> `WindSpeedMs`
- `IrradianceWm2` -> `IrradianceWm2`

## Stabilne kody błędów dla UI

- `CSV_EMPTY_FILE`
- `CSV_HEADER_MISSING`
- `CSV_HEADER_INVALID`
- `CSV_ROW_FIELD_REQUIRED`
- `CSV_ROW_DATETIME_INVALID`
- `CSV_ROW_NUMBER_INVALID`
- `CSV_ROW_NUMBER_NAN_OR_INF`

Te kody powinny być traktowane jako stabilny kontrakt komunikacji parser -> UI.
