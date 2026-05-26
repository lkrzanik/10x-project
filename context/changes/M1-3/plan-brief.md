# M1-3 — plan-brief

## Cel
Dostarczyć UI importu CSV dla zalogowanego użytkownika: upload pliku, wybór schematu, uruchomienie parsera i prezentacja podsumowania + błędów.

## Fazy
1. **Endpoint + ViewModel**: `ClimateImportController` (GET/POST) i model strony importu.
2. **Integracja parsera**: wywołanie `ICsvImportService`, mapowanie wyniku, render metryk i błędów.
3. **Gotowość pod M1-4**: testy kontrolera i punkt rozszerzenia pod persistence.

## Kryteria sukcesu
- Formularz uploadu działa i waliduje brak pliku/schematu.
- Walidacja uploadu obejmuje: pusty plik, rozszerzenie inne niż `.csv`, limit 10 MB.
- Wynik importu pokazuje `TotalRows`, `ValidRows`, `InvalidRows` i błędy per wiersz.
- UI nie wymaga zmiany kontraktu parsera.
- Kontroler ma jawne zachowanie dla anulowania requestu i błędów nieoczekiwanych.
- `dotnet build` i `dotnet test` przechodzą.

## Ryzyko krytyczne
- Aktualne kontrakty parsera nie mapują się 1:1 na encje `SensorReading`/`WeatherReading`; w M1-3 dostarczamy UI + feedback importu i zostawiamy mapping strategy jako jawny punkt integracyjny.

## Komendy weryfikacyjne
- `cd app`
- `dotnet build`
- `dotnet test`

## Progress
- [ ] F1
- [ ] F2
- [ ] F3