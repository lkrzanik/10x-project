# Contract Surfaces

Rejestr nazw i „powierzchni kontraktowych” projektu (API, zdarzenia, payloady, moduły, granice integracji).

## Cel

- Utrzymać spójne nazewnictwo między dokumentacją i kodem.
- Ułatwić zmianę implementacji bez łamania kontraktów.
- Wspierać szybkie wdrożenie nowych osób do projektu.

## Kategorie

### API / Endpoints

| Name | Direction | Owner | Status | Notes |
| --- | --- | --- | --- | --- |
| `ClimateImport/Index` (GET) | UI -> MVC | `_10xPV.Controllers.ClimateImportController` | active | Renderuje formularz importu CSV i ostatni wynik.
| `ClimateImport/Index` (POST) | UI -> MVC | `_10xPV.Controllers.ClimateImportController` | active | Upload pliku `.csv`, walidacja, uruchomienie importu, prezentacja podsumowania i błędów.
| `ClimateData/Index` (GET) | UI -> MVC | `_10xPV.Controllers.ClimateDataController` | active | Renderuje tabelę danych (`Sensor`/`Weather`) z filtrem `from/to` i paginacją server-side (`page`, `pageSize=50`).

### Events / Messages

| Name | Producer | Consumer | Payload Schema | Status | Notes |
| --- | --- | --- | --- | --- | --- |
| _TBD_ | _TBD_ | _TBD_ | _TBD_ | proposed | |

### Domain Contracts

| Name | Type | Invariants | Status | Notes |
| --- | --- | --- | --- | --- |
| `IClimateImportOrchestrator.ImportAsync(Stream, CsvSchemaType, CancellationToken)` | Application service | Nie zmienia kontraktu UI; zwraca `CsvImportResult`; deleguje parser i persistence adapter | active | Stabilny punkt wejścia dla kontrolera importu.
| `IClimateImportPersistenceAdapter.PersistValidRowsAsync(CsvImportResult, CancellationToken)` | Persistence seam | Zachowuje kontrakt parsera (`CsvImportResult`), zapisuje tylko poprawne rekordy, deduplikuje po `Timestamp` | active | W M1-4 podpięte `ClimateImportPersistenceAdapter` (EF Core) zamiast `NoOp`.
| `ClimateDataController.Index(DataSource, DateOnly?, DateOnly?, int, int, CancellationToken)` | Query/UI seam | Domyślnie `Sensor`; walidacja `from <= to`; normalizacja `page`; `pageSize` ograniczone do MVP=50; mapowanie do `ClimateDataPageViewModel` | active | Stabilny kontrakt listowania danych klimatycznych dla UI Razor.

## Notes

- Nie zapisuj tu implementacji ani wyborów frameworków.
- Skup się na stabilnych nazwach i granicach odpowiedzialności.
