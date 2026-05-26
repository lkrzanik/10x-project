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

### Events / Messages

| Name | Producer | Consumer | Payload Schema | Status | Notes |
| --- | --- | --- | --- | --- | --- |
| _TBD_ | _TBD_ | _TBD_ | _TBD_ | proposed | |

### Domain Contracts

| Name | Type | Invariants | Status | Notes |
| --- | --- | --- | --- | --- |
| `IClimateImportOrchestrator.ImportAsync(Stream, CsvSchemaType, CancellationToken)` | Application service | Nie zmienia kontraktu UI; zwraca `CsvImportResult`; deleguje parser i persistence adapter | active | Stabilny punkt wejścia dla kontrolera importu.
| `IClimateImportPersistenceAdapter.PersistValidRowsAsync(CsvImportResult, CancellationToken)` | Persistence seam | Side-effect optional; może być no-op; nie wpływa na wynik walidacji parsera | active | W M1-3 podpięte `NoOpClimateImportPersistenceAdapter`, docelowo implementacja DB w M1-4.

## Notes

- Nie zapisuj tu implementacji ani wyborów frameworków.
- Skup się na stabilnych nazwach i granicach odpowiedzialności.
