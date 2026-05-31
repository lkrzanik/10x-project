# Change: M2-1 — Interpolacja liniowa między seriami

## Source

Roadmap Milestone 2, element M2-1 ([context/foundation/roadmap.md](context/foundation/roadmap.md))

## Scope

- Interpolacja liniowa między seriami klimatycznymi (`SensorReading`, `WeatherReading`) jako fundament korelacji danych
- Implementacja oparta o `MathNet.Numerics` z opakowaniem domenowym (interfejs + implementacja)
- Jawna polityka „bez ekstrapolacji” w MVP (`null` poza zakresem)
- Normalizacja danych wejściowych przed interpolacją (filtrowanie `null` i duplikatów czasu)
- Konfiguracja progów klimatycznych przez `appsettings.json` + `IOptions<TOptions>` + walidacja fail-fast przy starcie
- Integracja MVC: endpoint/widok korelacji + testy kontrolera

## Status

🟡 In progress (F1 completed)

## Progress

- [x] F1 — Serwis interpolacji + DI + testy jednostkowe
- [ ] F2 — Integracja MVC (akcja + widok) + testy kontrolera
- [ ] F3 — Konfiguracja progów (`appsettings` + `IOptions` + walidacja fail-fast)

### Latest update

- 2026-05-29: F1 zakończona. Dodano i zweryfikowano testy `ClimateCorrelationService` (w tym guard clauses dla `null`); quality gates: `dotnet build` PASS, `dotnet test` PASS (29/29).

## Key decisions (from plan)

- GO: `MathNet.Numerics` jako biblioteka bazowa
- Brak ekstrapolacji w MVP (`null` poza zakresem)
- Brak `MathNet.Numerics.MKL.Win-x64` na MVP (tylko przy potwierdzonym bottlenecku)

## Threshold defaults (working)

- `MinTemperatureC = 0`
- `MaxTemperatureC = 40`

## Open question

- Docelowe wartości progów klimatycznych i ekstremów są nadal otwarte i wymagają potwierdzenia z użytkownikiem.

## Scope boundary vs roadmap

- Główny zakres tej zmiany to `M2-1` (interpolacja liniowa).
- Minimalna integracja MVC (akcja + widok) jest celowym pionowym wycinkiem demonstracyjnym.
- To świadome, ograniczone wyprzedzenie części `M2-3` tylko dla prezentacji tabelarycznej; zaawansowana wizualizacja pozostaje poza zakresem.

<!-- Updated by /10x-implement -->
