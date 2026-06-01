# Change: tp-unit-csv

## Opis

Testy jednostkowe dla parserów CSV, walidacji formatu i deduplikacji — pokrycie ryzyk R1 (cichy import śmieciowych danych) i R3 (błędna deduplikacja).

## Źródło

- `context/foundation/test-plan.md` §3 Faza 1
- Ryzyka: R1, R3

## Zakres

- Testy parsera CSV dla formatu Sensor (semicolon-separated, specyficzne kolumny)
- Testy parsera CSV dla formatu Weather (inny schemat kolumn)
- Testy walidatora: brakujące kolumny, zły separator, puste wiersze, nieprawidłowe typy danych
- Testy deduplikacji: wykrywanie duplikatów po timestamp+source, brak kasowania unikatów

## Ograniczenia

- Nie implementujemy testów integracyjnych (to Faza 4)
- Nie konfigurujemy CI (poza zakresem)
- Nie piszemy kodu produkcyjnego — tylko testy

## Status

`change opened` — następny krok: `/10x-research`
