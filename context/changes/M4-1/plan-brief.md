# Plan Brief — M4-1 Usuwanie wszystkich danych klimatycznych

## Cel

Akcja "reset" usuwająca wszystkie rekordy z tabel `SensorReading` i `WeatherReading`, wywołana przez zalogowanego użytkownika z potwierdzeniem w UI.

## Kluczowe decyzje

- Dedykowany serwis `IClimateDataResetService` z metodą `DeleteAllAsync`
- Potwierdzenie przez modal Bootstrap przed wysłaniem POST
- Akcja HTTP POST (`[Authorize]`) → przekierowanie z komunikatem TempData

## Fazy

| Faza | Zakres | Quality gate |
|------|--------|--------------|
| F1 | `IClimateDataResetService` + impl + rejestracja DI + testy jednostkowe | `dotnet build` PASS, `dotnet test` PASS |
| F2 | Akcja MVC `ResetAllData` (GET/POST) + widok potwierdzenia + testy kontrolera | `dotnet build` PASS, `dotnet test` PASS |
