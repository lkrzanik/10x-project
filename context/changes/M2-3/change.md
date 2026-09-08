# Change: M2-3 — Widok korelacji

## Source

Roadmap Milestone 2, element M2-3 ([context/foundation/roadmap.md](context/foundation/roadmap.md))

## Scope

- Domknięcie użytkowego widoku korelacji danych klimatycznych dla wybranego zakresu dat.
- Prezentacja skorelowanych wartości temperatury dla osi czujnika i osi pogody w czytelnej tabeli.
- Wizualizacja przebiegu obu serii na wykresie, z rozróżnieniem wartości źródłowych, interpolowanych i punktów poza zakresem.
- Zachowanie metadanych obliczeń: liczby punktów interpolowanych, odrzuconych i poza zakresem.
- Czytelne stany pustych danych, błędnego zakresu dat i wartości `null` bez udawania kompletności serii.
- Testy kontrolera, mapowania ViewModelu i podstawowych stanów widoku.

## Status

Done — phases 1–3 implemented

## Progress

- [x] F1 — Kontrakt prezentacji i dane dla widoku
- [x] F2 — Tabela oraz wykres korelacji
- [x] F3 — Testy, dostępność i stabilizacja UX

### Validation note

- Test Playwright `M2-3: correlation view exposes accessible filters and result state` został dodany, ale nie uruchomiono go w tej sesji, ponieważ środowisko wymaga lokalnych `AdminEmail` i `AdminPassword`.

## Key decisions

1. Właścicielem przepływu pozostaje `ClimateDataController.Correlation`; nie powstaje drugi endpoint dla tego samego widoku.
2. Serwis `IClimateCorrelationService` z `M2-1` pozostaje źródłem obliczeń. `M2-3` nie zmienia interpolacji ani polityki braku ekstrapolacji.
3. MVP pokazuje temperaturę, bo to jedyny parametr obecny w istniejącym kontrakcie korelacji; wilgotność i zachmurzenie pozostają osobnym rozszerzeniem.
4. Wykres korzysta z danych już przygotowanych przez kontroler/ViewModel, bez zapytań z przeglądarki i bez zapisywania wyników korelacji w bazie.
5. Punkt poza zakresem pozostaje pusty (`null`) i jest oznaczony w tabeli/legendzie jako brak wartości, zamiast być ekstrapolowany.
6. Daty wybrane przez użytkownika pozostają w formularzu po błędzie lub ponownym obliczeniu.

## Open questions

- Czy wykres ma używać biblioteki JavaScript, czy lekkiego rozwiązania opartego na istniejącym stosie frontendowym projektu?
- Czy w MVP potrzebne są eksport obrazu/danych i wybór parametru, czy wystarczy temperatura oraz zakres dat?
- Czy docelowo korelacja ma prezentować jedną wspólną serię punktów, czy dwa kierunki wyrównania pozostają równorzędnymi seriami?

## Expected files

- `app/Models/Correlation/CorrelationViewModel.cs` lub dedykowany model wiersza wykresu/tabeli.
- `app/Controllers/ClimateDataController.cs`.
- `app/Controllers/ClimateCorrelationController.cs`.
- `app/Views/ClimateCorrelation/Correlation.cshtml`.
- `app/wwwroot/js/` — skrypt wykresu, jeśli wybrana biblioteka wymaga osobnego modułu.
- `app/wwwroot/css/site.css` — tylko style niezbędne do czytelnej, responsywnej prezentacji.
- `app/Tests/10xPV.Tests/Controllers/ClimateDataCorrelationTests.cs`.
- testy modelu/mapowania lub testy widoku, jeśli repozytorium ma dla nich ustalony harness.

<!-- Created by /10x-new -->
