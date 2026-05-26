# M1-4 — plan-brief

## Cel
Dostarczyć tabelaryczny podgląd danych klimatycznych z paginacją i filtrowaniem po dacie oraz domknąć zapis poprawnych rekordów importu do bazy danych.

## Fazy
1. **Persistence adapter**: zastąpić no-op implementacją EF Core i zapisywać poprawne rekordy bez duplikatów po `Timestamp`.
2. **Endpoint + query**: dodać `ClimateDataController/Index` z filtrem dat i paginacją server-side.
3. **UI + testy**: widok tabeli, nawigacja, testy kontrolera i persistence, aktualizacja kontraktów.

## Kryteria sukcesu
- Import zapisuje poprawne rekordy do `SensorReadings`/`WeatherReadings`.
- Duplikaty timestamp nie powodują duplikowania danych.
- Widok `ClimateData/Index` obsługuje filtr `from/to` i paginację.
- UI pokazuje poprawne metadane listy (`TotalCount`, `CurrentPage`, `TotalPages`).
- `dotnet build` i `dotnet test` przechodzą.

## Ryzyko krytyczne
- Konflikty unikalnych indeksów po `Timestamp` i poprawna normalizacja zakresu dat (`from > to`, strefy czasowe).

## Komendy weryfikacyjne
- `cd app`
- `dotnet build`
- `dotnet test`

## Progress
- [ ] F1
- [ ] F2
- [ ] F3
