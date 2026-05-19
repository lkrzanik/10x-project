---
project: "10xFotowoltaika"
context_type: greenfield
created: 2026-05-19
updated: 2026-05-19
checkpoint:
  current_phase: 8
  phases_completed: [1, 2, 3, 4, 5, 6]
  gray_areas_resolved:
    - topic: "ingest danych pogodowych"
      decision: "Obsługa dwóch ścieżek: API pogodowe lub import CSV pobrany ręcznie."
    - topic: "interwały czasowe danych"
      decision: "Korelacja przez normalizację osi czasu do interwałów pomiarowych urządzenia (30 min)."
    - topic: "model dostępu"
      decision: "MVP jako aplikacja dla pojedynczego użytkownika; brak wieloużytkownikowości i ról."
    - topic: "zakres rekomendacji PV"
      decision: "Raport AI rekomenduje technologię zestawu (inwerter + magazyn), bez doboru liczby paneli i analizy pełnego zapotrzebowania energetycznego."
    - topic: "horyzont danych wejściowych"
      decision: "Dane do analizy są zbierane przez minimum 1 rok."
    - topic: "tryb raportu AI"
      decision: "Raport AI jest generowany online i udostępniany w formie pliku PDF."
    - topic: "tryb realizacji projektu"
      decision: "Projekt realizowany po godzinach (after-hours-only)."
    - topic: "definicja ekstremalnego odczytu"
      decision: "Ekstremum wyznaczane na podstawie wartości minimalnych i maksymalnych zdefiniowanych w ustawieniach aplikacji."
    - topic: "szczegółowość rekomendacji technologii"
      decision: "Rekomendacja dotyczy ogólnej klasy urządzeń, nie konkretnych modeli."
    - topic: "budżet czasowy MVP"
      decision: "MVP planowane na 6 tygodni pracy po godzinach."
  frs_drafted: 9
  quality_check_status: warned
---

## Vision & Problem Statement

Właściciel pomieszczenia potrzebuje wiarygodnej odpowiedzi, czy dane miejsce nadaje się pod zestaw fotowoltaiczny z magazynem energii, ale dziś musi ręcznie łączyć długoterminowe pomiary temperatury/wilgotności z osobnymi danymi pogodowymi. To wydłuża analizę, utrudnia wychwycenie ekstremów i zwiększa ryzyko błędnych decyzji przy wyborze technologii.

Wartość produktu wynika z automatycznej korelacji dwóch strumieni danych o różnych interwałach oraz przekształcenia ich w czytelny wniosek decyzyjny (nadaje się / nie nadaje się + dlaczego + rekomendacja technologii zestawu).

## User & Persona

### Primary Persona
- **Nazwa robocza:** Właściciel / opiekun techniczny pomieszczenia
- **Rola:** Osoba analizująca warunki środowiskowe pod inwestycję PV
- **Moment użycia:** Po zebraniu danych z urządzenia pomiarowego i przed decyzją o doborze technologii
- **Aktualny koszt:** Ręczne porównywanie danych z wielu źródeł i brak spójnego raportu decyzyjnego

## Access Control

Single user; no auth; data processed in one local workspace for one operator in MVP.

## Success Criteria

### Primary
- Wszystkie rekordy pomiarowe z urządzenia są skorelowane z danymi pogodowymi mimo różnic interwałów czasowych.
- Użytkownik może wygenerować raport AI zawierający: decyzję „nadaje się/nie nadaje się”, czynniki dyskwalifikujące (jeśli są) oraz rekomendowaną technologię zestawu inwerter + magazyn energii.

### Secondary
- Użytkownik może dodawać notatki tekstowe do każdego rekordu pomiarowego i uwzględniać je podczas interpretacji wyników.
- Użytkownik otrzymuje wykres zależności danych pomiarowych i pogodowych do szybkiego przeglądu trendów.

### Guardrails
- Import i korelacja danych nie mogą pomijać rekordów bez jawnego oznaczenia braków.
- Ekstremalne odczyty muszą być wizualnie wyróżnione i łatwe do odfiltrowania.
- Zakres MVP nie obejmuje analizy liczby paneli ani pełnego modelu zapotrzebowania energetycznego.

## Functional Requirements

### Data Ingestion & Correlation
- FR-001: Użytkownik może zaimportować plik CSV z urządzenia pomiarowego zawierający temperaturę i wilgotność w interwałach 30-minutowych. Priority: must-have
  > Sokrates: Rozważono kontrargument: "format CSV z urządzenia może się zmieniać i import będzie kruchy". Rozwiązanie: zachowano FR; parser w MVP wspiera jeden jawnie opisany format wejściowy, a odchylenia są raportowane jako błąd walidacji.

- FR-002: Użytkownik może dostarczyć dane pogodowe dla lokalizacji przez API zewnętrznego serwisu pogodowego lub przez import CSV pobrany ręcznie. Priority: must-have
  > Sokrates: Rozważono kontrargument: "dwie ścieżki importu zwiększają złożoność MVP". Rozwiązanie: zachowano FR; obie ścieżki są krytyczne dla ciągłości pracy przy braku dostępu do API.

- FR-003: System może automatycznie skorelować dane pomiarowe i pogodowe nawet gdy dane pogodowe mają inny interwał czasowy niż 30 minut. Priority: must-have
  > Sokrates: Rozważono kontrargument: "agregacja/normalizacja czasu może zniekształcić wyniki". Rozwiązanie: zachowano FR; metoda korelacji i poziom dopasowania czasu muszą być jawnie raportowane użytkownikowi.

### Analysis & Insight
- FR-004: Użytkownik może dodać tekstową notatkę do każdego rekordu danych pomiarowych. Priority: must-have
  > Sokrates: Rozważono kontrargument: "notatki na rekord mogą być kosztowne i rzadko używane". Rozwiązanie: zachowano FR jako must-have, ponieważ notatki są potrzebne do kontekstu anomalii.

- FR-005: Użytkownik może wyświetlić wykres zależności między danymi z urządzenia i danymi pogodowymi na wspólnej osi czasu. Priority: must-have
  > Sokrates: Rozważono kontrargument: "wykres może nie wnosić wartości ponad tabelę". Rozwiązanie: zachowano FR; szybka interpretacja trendu jest kluczowa dla decyzji inwestycyjnej.

- FR-006: System może wykryć i wyróżnić ekstremalne odczyty z urządzenia pomiarowego w widocznym miejscu. Priority: must-have
  > Sokrates: Rozważono kontrargument: "brak definicji ekstremum może dawać mylące alarmy". Rozwiązanie: zachowano FR; ekstremum definiowane przez konfigurowalne wartości min/max w ustawieniach aplikacji.

### AI Report
- FR-007: Użytkownik może wygenerować online raport AI w formacie PDF oparty na zebranych danych skorelowanych i notatkach. Priority: must-have
  > Sokrates: Rozważono kontrargument: "jakość raportu AI będzie niestabilna bez dodatkowego nadzoru". Rozwiązanie: zachowano FR; raport jest wsparciem decyzyjnym i zawiera uzasadnienie.

- FR-008: Raport AI może jednoznacznie stwierdzić, czy pomieszczenie nadaje się do rozwiązania PV (tak/nie). Priority: must-have
  > Sokrates: Rozważono kontrargument: "decyzja binarna może upraszczać realny stan". Rozwiązanie: zachowano FR; decyzja binarna jest wymagana, ale musi być uzupełniona sekcją uzasadnienia.

- FR-009: Raport AI może wskazać czynniki dyskwalifikujące i zaproponować ogólną klasę technologii zestawu inwertera z magazynem energii. Priority: must-have
  > Sokrates: Rozważono kontrargument: "rekomendacja technologii bez modelu energii może być zbyt ogólna". Rozwiązanie: zachowano FR; zakres rekomendacji dotyczy typu technologii, nie ilości paneli ani pełnego sizingu instalacji.

## User Stories

### US-01: Import i korelacja danych środowiskowych

- **Given** użytkownik posiada plik CSV z urządzenia pomiarowego i źródło danych pogodowych (API lub CSV)
- **When** uruchamia import obu źródeł i zleca korelację
- **Then** otrzymuje jednolity zestaw danych z przypisanymi danymi pogodowymi do rekordów pomiarowych

#### Acceptance Criteria
- Rekordy pomiarowe bez możliwego dopasowania pogodowego są jawnie oznaczone
- Różne interwały czasowe są normalizowane według jawnej reguły dopasowania czasu
- Użytkownik widzi liczbę rekordów wejściowych i skorelowanych

### US-02: Analiza anomalii i kontekstu

- **Given** dane zostały poprawnie skorelowane
- **When** użytkownik przegląda wyniki na wykresie i dodaje notatki do rekordów
- **Then** może szybko wskazać ekstremalne odczyty i opisać ich kontekst

#### Acceptance Criteria
- Ekstremalne odczyty są wyraźnie odróżnione wizualnie od pozostałych
- Notatka może zostać przypisana do pojedynczego rekordu pomiarowego

### US-03: Raport decyzyjny AI dla pomieszczenia

- **Given** użytkownik ma skorelowane dane i ewentualne notatki
- **When** wybiera generowanie raportu AI
- **Then** dostaje ocenę nadaje się/nie nadaje się, czynniki dyskwalifikujące oraz rekomendację technologii inwertera i magazynu

#### Acceptance Criteria
- Raport zawiera sekcję decyzji binarnej oraz krótkie uzasadnienie
- Raport wyraźnie oddziela wnioski od danych wejściowych
- Raport jest dostępny jako plik PDF wygenerowany online
- Raport nie obejmuje doboru liczby paneli PV

## Business Logic

Aplikacja klasyfikuje przydatność pomieszczenia pod zestaw PV poprzez połączenie długoterminowych pomiarów środowiskowych z danymi pogodowymi i wyprowadzenie decyzji wraz z uzasadnieniem.

Reguła konsumuje dane czasowe (temperatura, wilgotność, zachmurzenie) oraz notatki kontekstowe użytkownika, a wynikiem jest decyzja binarna i rekomendacja technologii zestawu inwerter + magazyn energii. Użytkownik styka się z tą regułą podczas generowania raportu AI po etapie korelacji danych.

## Non-Functional Requirements

- Import i korelacja danych powinny zwrócić wynik dla standardowego zestawu MVP bez zauważalnego opóźnienia blokującego pracę użytkownika.
- Aplikacja powinna zapewnić pełną transparentność braków danych (żaden brak nie może zostać „ukryty”).
- Raport AI powinien być reprodukowalny dla tego samego zestawu danych wejściowych i tej samej konfiguracji modelu.
- Raport AI powinien być możliwy do wygenerowania online i eksportu do formatu PDF.

## Non-Goals

- Brak zdalnego dostępu i hostowanej wersji webowej w MVP.
- Brak zaawansowanej edycji danych pomiarowych i pogodowych w MVP.
- Brak obsługi wielu lokalizacji pomiarowych.
- Brak obsługi wielu użytkowników i ról.
- Brak analizy zapotrzebowania energetycznego i brak doboru liczby paneli PV.

## Open Questions

1. **Czy projekt ma hard deadline biznesowy (konkretna data kalendarzowa)?** — Owner: user. Block: no (MVP: 6 tygodni po godzinach — confirmed).

## Quality cross-check

- ⚠️ Nie zebrano jeszcze hard deadline (konkretnej daty kalendarzowej); budżet MVP 6 tygodni po godzinach został potwierdzony.
- ⚠️ Część wymagań NFR wymaga doprecyzowania metryk liczbowych (SLA/czas odpowiedzi).
- ✅ Zakres MVP i non-goals są rozdzielone oraz spójne z notatką wejściową.
- ✅ Zidentyfikowano główną regułę domenową (uniknięto pustego CRUD).
