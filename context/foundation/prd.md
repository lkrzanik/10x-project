---
project: "10xFotowoltaika"
version: 1
status: draft
created: 2026-05-19
context_type: greenfield
product_type: desktop
target_scale: "# TODO: target_scale — see Open Questions"
timeline_budget: "# TODO: timeline_budget — see Open Questions"
---

## Vision & Problem Statement

Długoterminowa analiza temperatury i wilgotności pomieszczenia oraz warunków pogodowych w celu zaproponowania optymalnego zestawu paneli fotowoltaicznych, inwertera i magazynu energii.

Trudno ocenić, czy konkretne pomieszczenie nadaje się pod instalację fotowoltaiki i magazynu energii na podstawie długoterminowych warunków środowiskowych. Użytkownik potrzebuje jednego procesu, który łączy pomiary z urządzenia z danymi pogodowymi i przekłada je na decyzję wraz z uzasadnieniem.

## User & Persona

### Primary persona

- Instalator / doradca OZE (użytkownik jednoosobowy, lokalna praca na jednym stanowisku).
- Potrzebuje szybkiego importu danych pomiarowych, automatycznej korelacji z pogodą, widocznych ekstremów i raportu końcowego wspierającego decyzję techniczną.

## Success Criteria

### Primary

- Wszystkie dane z urządzenia pomiarowego są skorelowane z danymi pogodowymi.
- Zgromadzone dane pozwalają na propozycję optymalnego zestawu paneli fotowoltaicznych, inwertera i magazynu energii.

### Secondary

- Użytkownik może dodać własne uwagi do rekordów pomiarowych.
- Użytkownik może w dowolnym momencie wyświetlić wykres zależności między danymi pomiarowymi i pogodowymi.

### Guardrails

- Produkt pozostaje aplikacją lokalną (bez zdalnego dostępu webowego) w zakresie MVP.
- MVP nie wprowadza obsługi wielu użytkowników ani wielu lokalizacji.

## User Stories

# TODO: user stories with Given/When/Then — see Open Questions

## Functional Requirements

- FR-001: User can import a CSV file with timestamp, temperature, and humidity readings at fixed intervals. Priority: must-have
- FR-002: User can receive validation feedback when imported CSV rows are incomplete or malformed. Priority: must-have
- FR-003: User can correlate imported indoor measurements with weather data from an external service and persist synchronized results. Priority: must-have
- FR-004: User can add and edit a text note for each measurement record. Priority: must-have
- FR-005: User can view a chart showing relationships between indoor measurements and correlated weather data. Priority: must-have
- FR-006: User can identify extreme readings highlighted in the interface. Priority: must-have
- FR-007: User can generate a final report that states suitability, disqualifying factors, and a recommended inverter + storage technology. Priority: must-have
- FR-008: User can receive suitability decisions only after the minimum required data horizon is met. Priority: must-have

## Non-Functional Requirements

# TODO: non-functional measurable targets — see Open Questions

## Business Logic

System klasyfikuje przydatność pomieszczenia do instalacji OZE na podstawie stabilności i ekstremów warunków środowiskowych oraz kontekstu pogodowego, a następnie mapuje wynik na rekomendowaną technologię zestawu.

Decyzja końcowa jest możliwa tylko przy minimalnym oknie danych (co najmniej N dni pomiarów). Przekroczenie progów krytycznych przez określony udział próbek może dyskwalifikować pomieszczenie. Duża zmienność temperatury i wilgotności obniża ocenę przydatności i wpływa na mapowanie rekomendacji technologii.

## Access Control

Single user; no auth; data lives on-device only.

## Non-Goals

- Zdalny dostęp (aplikacja nie musi być stroną internetową) — aby utrzymać prosty, lokalny zakres MVP.
- Zaawansowana edycja danych — aby ograniczyć MVP do importu, korelacji i analizy.
- Obsługa wielu lokalizacji pomiarowych — aby skupić się na jednym kontekście pomiarowym.
- Obsługa wielu użytkowników — aby uprościć model dostępu i przechowywania.
- Analiza zapotrzebowania na energię i ilości potrzebnych paneli fotowoltaicznych — poza zakresem pierwszego wydania.

## Open Questions

1. **What is `target_scale` (`users`, `qps`, `data_volume`) for this product?** — Owner: user. Block: no.
2. **What is `timeline_budget` (`mvp_weeks`, `hard_deadline`, `after_hours_only`)?** — Owner: user. Block: no.
3. **Please provide at least 2–3 MVP user stories in Given/When/Then format.** — Owner: user. Block: yes.
4. **What measurable non-functional targets are required (e.g., latency, data retention, reliability, compatibility)?** — Owner: user. Block: yes.
5. **Jaki minimalny horyzont danych (`N` dni) jest wymagany do decyzji?** — Owner: user. Block: yes.
6. **Jakie dokładne progi ekstremów temperatury i wilgotności obowiązują?** — Owner: user. Block: yes.
7. **Który zewnętrzny dostawca pogody jest źródłem referencyjnym?** — Owner: user. Block: yes.
8. **Czy raport AI ma być obowiązkowy w MVP, czy może być etapem v1.1?** — Owner: user. Block: yes.
9. **Czy rekomendacja technologii ma używać zamkniętego katalogu, czy wolnego tekstu?** — Owner: user. Block: no.