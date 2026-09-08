# Change: M2-2 — Detekcja ekstremów

## Source

Roadmap Milestone 2, element M2-2 ([context/foundation/roadmap.md](context/foundation/roadmap.md))

## Scope

- Wykrywanie ekstremalnych wartości w danych klimatycznych na podstawie progów konfigurowanych w `appsettings.json`.
- Serwis domenowy działający dla temperatury, wilgotności i zachmurzenia, z rozróżnieniem minimum i maksimum.
- Wynik zawierający timestamp, parametr, wartość, kierunek przekroczenia i użyty próg.
- Testy jednostkowe logiki progów oraz minimalna integracja z istniejącym przepływem MVC.

## Status

Done

## Progress

- [x] F1 — Kontrakt wyniku, opcje progów i serwis detekcji
- [x] F2 — Rejestracja DI, konfiguracja i testy jednostkowe
- [x] F3 — Integracja z widokiem danych / endpointem oraz testy kontrolera

## Key decisions

1. Wartości równe progowi nie są ekstremami; ekstremum oznacza wartość `< minimum` albo `> maksimum`.
2. Brak wartości (`null`) jest pomijany i nie tworzy wyniku ekstremum.
3. Detekcja jest obliczana w locie z aktualnych opcji; rekordy źródłowe nie są modyfikowane ani wzbogacane o stan trwały.
4. Każdy wynik zachowuje parametr i próg użyty do oceny, aby UI i przyszły raport mogły wyjaśnić klasyfikację.
5. Zakres M2-2 nie obejmuje korelacji statystycznej ani wizualizacji wykresowej z M2-3.

## Open questions

- Docelowe wartości progów temperatury pozostają do potwierdzenia z użytkownikiem; wartości robocze z M2-1 (`0` i `40` °C) są używane do czasu decyzji.
- Czy progi wilgotności i zachmurzenia mają być dodane w tej zmianie, czy dopiero wraz z widokiem ekstremów?

## Expected files

- `app/Models/Correlation/ClimateExtreme.cs`
- `app/Services/Correlation/IClimateExtremeDetectionService.cs`
- `app/Services/Correlation/ClimateExtremeDetectionService.cs`
- `app/Services/Correlation/ExtremeDetectionOptions.cs` lub rozszerzenie istniejących opcji
- `app/Program.cs`
- `app/appsettings.json`
- `app/Tests/10xPV.Tests/Services/Correlation/ClimateExtremeDetectionServiceTests.cs`

<!-- Created by /10x-new -->

<!-- Updated by /10x-implement M2-2 phase 1 -->

<!-- Updated by /10x-implement M2-2 phase 2 -->
