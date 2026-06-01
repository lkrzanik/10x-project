# Plan Brief: tp-unit-csv

## Cel

Zmniejszyć ryzyka **R1** (cichy import błędnych danych) i **R3** (błędna deduplikacja) przez rozszerzenie testów jednostkowych dla CSV, walidacji i deduplikacji.

## Zakres (skrót)

- Rozszerzenie testów `CsvImportService` o kluczowe edge cases (separator, schema mismatch, brak/nadmiar kolumn, puste wartości, BOM, quoted fields, Infinity).
- Dodanie testów `ClimateDataValidator` (granice zakresów, przyszły timestamp, przypadki czasu).
- Dodanie testów `DeduplicationService` na `InMemory` (Sensor/Weather, exists/not exists, rozróżnienie source).

## Poza zakresem

- Testy integracyjne importu (Faza 4).
- Zmiany CI/pipeline/hooków.
- Zmiany kodu produkcyjnego (chyba że testy ujawnią blocker).

## Kroki implementacyjne

1. Dopisać testy parsera CSV w `CsvImportServiceTests`.
2. Dodać `ClimateDataValidatorTests`.
3. Dodać `DeduplicationServiceTests`.
4. Uruchomić testy projektu i upewnić się, że nie ma regresji.

## Kryteria akceptacji

- Testy pokrywają wskazane luki dla R1 i R3.
- `dotnet test app/Tests/10xPV.Tests/10xPV.Tests.csproj` przechodzi w 100%.
- Cookbook §6.1 i status fazy są zsynchronizowane z planem.

## Następny krok

`/10x-implement tp-unit-csv phase 1`
