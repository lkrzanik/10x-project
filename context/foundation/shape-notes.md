---
checkpoint:
  current_phase: completed
  phases_completed:
    - vision
    - persona_and_access
    - mvp_scope
    - functional_requirements
    - business_logic_and_data
    - stack_openness_sketch
  frs_drafted:
    - FR-001
    - FR-002
    - FR-003
    - FR-004
    - FR-005
    - FR-006
    - FR-007
    - FR-008
  quality_check_status:
    empty_crud:
      status: pass
      notes: "Wykryto realną regułę domenową: klasyfikacja przydatności pomieszczenia oraz rekomendacja technologii zestawu."
    mvp_too_big:
      status: warning
      notes: "Zakres MVP zawiera integrację pogodową + wizualizację + raport AI. Ryzyko przekroczenia ~1 tygodnia pracy po godzinach."
      override: true
      mitigation:
        - "Wersja MVP v1: raport regułowy (bez LLM), AI jako v1.1"
        - "Jedna integracja pogodowa i jeden format CSV"
        - "Jeden widok wykresu bez zaawansowanych filtrów"
---

# 10xFotowoltaika — Shape Notes

## 1) Vision

- **Problem użytkownika:** trudno ocenić, czy konkretne pomieszczenie nadaje się pod instalację fotowoltaiki i magazynu energii na podstawie długoterminowych warunków środowiskowych.
- **Wartość MVP:** użytkownik importuje pomiary (CSV), system automatycznie dołącza warunki pogodowe, pokazuje anomalie i daje rekomendację „nadaje się / nie nadaje się” z uzasadnieniem.
- **Outcome:** szybsza, bardziej obiektywna prekwalifikacja pomieszczenia bez ręcznego sklejenia danych.

## 2) Persona i dostęp

### Główna persona

- **Instalator / doradca OZE** (użytkownik jednoosobowy, lokalna praca na jednym stanowisku).

### Potrzeby persony

- szybki import danych pomiarowych,
- automatyczna korelacja z pogodą,
- czytelny wgląd w ekstremalne odczyty,
- raport końcowy wspierający decyzję techniczną.

### Dostęp i bezpieczeństwo (MVP)

- aplikacja lokalna, bez kont i bez wielodostępu,
- dane przechowywane lokalnie,
- brak wymagań multi-tenant i brak uprawnień ról w MVP.

## 3) MVP scope

### In scope

1. Import CSV z temperaturą i wilgotnością (interwał 30 minut).
2. Synchronizacja danych pogodowych z zewnętrznego serwisu i zapis skorelowanych danych.
3. Dodawanie notatek tekstowych do rekordu pomiarowego.
4. Wykres zależności danych pomieszczenia i danych pogodowych.
5. Wyróżnienie odczytów ekstremalnych.
6. Raport końcowy: decyzja + czynniki dyskwalifikujące + propozycja technologii inwerter/magazyn.

### Out of scope

- zdalny dostęp/web,
- zaawansowana edycja danych,
- wiele lokalizacji,
- wielu użytkowników,
- pełna analiza zapotrzebowania energetycznego i ilości paneli.

### Socratic challenge (czy MVP nie jest za szerokie?)

- **Ryzyko:** raport AI + integracja z API pogodowym + wykresy w jednej iteracji to wysoki koszt implementacyjny.
- **Kontrpropozycja zawężenia:**
  - MVP v1: raport oparty na regułach domenowych (deterministyczny),
  - MVP v1.1: rozszerzenie o komponent AI generujący narrację raportu,
  - jedna lokalizacja, jeden dostawca pogody, minimalny zestaw wizualizacji.

## 4) Functional Requirements (draft)

- **FR-001 — Import pomiarów:** system importuje plik CSV z polami: znacznik czasu, temperatura, wilgotność.
- **FR-002 — Walidacja danych:** system odrzuca rekordy z brakami krytycznymi lub błędnym formatem daty.
- **FR-003 — Korelacja pogodowa:** dla każdego rekordu pomiarowego system przypina odpowiadające dane pogodowe.
- **FR-004 — Trwałość danych:** dane pomiarowe i pogodowe są zapisywane lokalnie.
- **FR-005 — Notatki użytkownika:** użytkownik może dodać/edytować notatkę tekstową do pojedynczego rekordu.
- **FR-006 — Wizualizacja:** użytkownik może wyświetlić wykres zależności pomiarów lokalnych i danych pogodowych.
- **FR-007 — Ekstrema:** system oznacza rekordy przekraczające progi ekstremalne temperatury/wilgotności.
- **FR-008 — Raport końcowy:** system generuje raport zawierający: decyzję przydatności, listę dyskwalifikatorów, rekomendację technologii inwerter + magazyn.

## 5) Logika biznesowa i dane

### Reguła domenowa (1 zdanie)

System klasyfikuje przydatność pomieszczenia do instalacji OZE na podstawie stabilności i ekstremów warunków środowiskowych oraz kontekstu pogodowego, a następnie mapuje wynik na rekomendowaną technologię zestawu.

### Kluczowe reguły biznesowe (draft)

1. **Reguła kompletności:** decyzja końcowa jest możliwa tylko przy minimalnym oknie danych (np. co najmniej N dni pomiarów).
2. **Reguła ekstremów:** przekroczenie progów krytycznych przez określony udział próbek może dyskwalifikować pomieszczenie.
3. **Reguła stabilności:** duża zmienność temperatury/wilgotności obniża ocenę przydatności.
4. **Reguła mapowania technologii:** wynik klasyfikacji i profil warunków mapują się do ograniczonego katalogu rekomendacji technologicznych.

### Model danych (draft)

- **MeasurementRecord**
  - id
  - measured_at
  - temperature_c
  - humidity_pct
  - source_file_id
  - user_note
- **WeatherRecord**
  - id
  - measured_at
  - outdoor_temp_c
  - outdoor_humidity_pct
  - cloud_cover_pct
  - weather_code
  - provider
- **CorrelatedSnapshot**
  - id
  - measurement_record_id
  - weather_record_id
  - extreme_flags[]
  - suitability_score
- **FinalReport**
  - id
  - generated_at
  - verdict (fit / unfit / conditional)
  - disqualifiers[]
  - recommendation
  - explanation

### Edge cases do obsłużenia

- luki czasowe i duplikaty w CSV,
- brak danych pogodowych dla części osi czasu,
- niespójna strefa czasowa między źródłami,
- skrajne wartości odstające (awaria czujnika),
- zbyt mało danych do wiarygodnej rekomendacji.

## 6) Szkic otwartości stosu (binding for next step)

- **product_type:** desktop/local-first analytical app
- **tech_preferences.language_family:** do decyzji w 10x-tech-stack-selector (preferowane: rodzina języków z dobrym wsparciem CSV + wizualizacji)
- Brak decyzji o frameworku, bazie danych i hostingu na etapie shape (celowo).

## 7) Kryteria sukcesu (doprecyzowane)

1. 100% poprawnie zaimportowanych rekordów waliduje się i trafia do korelacji albo jest jawnie odrzucone z powodem.
2. Co najmniej 95% rekordów pomiarowych otrzymuje sparowany rekord pogodowy (lub status braku pary).
3. Użytkownik otrzymuje raport końcowy z czytelnym werdyktem i listą czynników wpływających na decyzję.

## 8) Open questions do przeniesienia do PRD

1. Jaki minimalny horyzont danych (`N` dni) jest wymagany do decyzji?
2. Jakie dokładne progi ekstremów temperatury i wilgotności obowiązują?
3. Który zewnętrzny dostawca pogody jest źródłem referencyjnym?
4. Czy raport AI ma być obowiązkowy w MVP, czy może być etapem v1.1?
5. Czy rekomendacja technologii ma używać zamkniętego katalogu, czy wolnego tekstu?