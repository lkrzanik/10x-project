# Change: M1-3 — UI importu CSV

## Source

Roadmap Milestone 1, element M1-3 ([context/foundation/roadmap.md](context/foundation/roadmap.md))

## Scope

- Formularz uploadu pliku CSV (single file, max MVP)
- Wybór schematu importu (`Sensor` / `Weather`)
- Uruchomienie parsera CSV i prezentacja podsumowania importu
- Czytelna prezentacja błędów walidacji (per wiersz)
- Integracja z istniejącymi usługami importu/walidacji/deduplikacji na poziomie orchestration

## Status

🟡 In progress

## Progress

- 2026-05-26 — F1 zakończona (`198cac3`): endpoint MVC importu, modele strony, walidacja formularza i testy kontrolera.
- 2026-05-26 — F2 zakończona: integracja parsera CSV w kontrolerze, mapowanie wyniku importu na UI oraz prezentacja podsumowania i błędów per wiersz; weryfikacja `dotnet build` + `dotnet test` (11/11).

<!-- Updated by /10x-implement -->