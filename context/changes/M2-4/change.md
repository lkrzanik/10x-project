# Change: M2-4 — Widok ekstremów

## Source

Roadmap Milestone 2, element M2-4 ([context/foundation/roadmap.md](context/foundation/roadmap.md))

## Scope

- Prezentacja wyników wykrywania ekstremów dla danych klimatycznych w czytelnym widoku użytkowym.
- Wybór zakresu dat i, opcjonalnie, filtrowanie po parametrze lub kierunku przekroczenia.
- Tabela z timestampem, parametrem, wartością, progami i kierunkiem ekstremum.
- Stan pusty dla brakujących danych lub dla zakresu bez przekroczeń.
- Minimalna integracja z istniejącym przepływem analizy i testy kontrolera / widoku.

## Status

Planned

## Progress

- [ ] F1 — Kontrakt widoku i przygotowanie danych z serwisu ekstremów
- [ ] F2 — Razor + filtry + stan pusty + UX
- [ ] F3 — Testy kontrolera, widoku i walidacja przepływu

## Key decisions

1. Zmiana korzysta z istniejącego kontraktu `ClimateExtreme` z M2-2; nie dodaje nowej, trwałej warstwy zapisu.
2. Widok pozostaje prostym, tabelarycznym MVP: najpierw czytelna lista ekstremów, bez dodatkowych wykresów.
3. Przepływ generowania wyników pozostaje w kontrolerze danych klimatycznych lub w dedykowanym kontrolerze korelacji, ale bez tworzenia osobnego źródła prawdy dla tej samej analizy.
4. Brak ekstremów jest traktowany jako poprawny stan aplikacji, a nie błędny warunek operacyjny.
5. Filtry dat są zachowane po ponownym renderowaniu i po błędnej walidacji, aby UX był przewidywalny.

## Open questions

- Czy widok ekstremów ma być częścią strony korelacji, czy osobnym podwidokiem w `ClimateData`?
- Czy MVP ma oferować tylko listę z sortowaniem chronologicznym, czy też dodatkowe filtry po parametrze / kierunku?
- Czy docelowo potrzeba będzie agregacji sumarycznej (liczba ekstremów, najostrzejszy przypadek) czy wystarczy tabela detaliczna?

## Expected files

- `app/Models/Correlation/ClimateExtremeViewModel.cs` (lub podobny model widoku)
- `app/Controllers/ClimateDataController.cs`
- `app/Controllers/ClimateCorrelationController.cs` (jeśli wątek będzie rozdzielony)
- `app/Views/ClimateData/Extremes.cshtml` lub odpowiedni widok w `ClimateCorrelation/`
- `app/Services/Correlation/` — ewentualny adapter mapujący wyniki z M2-2 do widoku
- `app/Tests/10xPV.Tests/Controllers/ClimateDataExtremeTests.cs`
- `app/Tests/10xPV.Tests/Services/Correlation/` — testy dla stanu pustego i sortowania

<!-- Created by /10x-new -->
