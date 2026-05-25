# Roadmap — 10xPV

## North Star

Pojedynczy użytkownik importuje lokalne dane klimatyczne, otrzymuje skorelowaną analizę z wykrywaniem ekstremów i generuje raport AI z rekomendacjami technologii fotowoltaicznej — eksportowany jako PDF — w jednym spójnym przepływie.

## Main Goal

Dostarczyć działającą aplikację webową, która w jednym przepływie pozwala: zaimportować dane klimatyczne z CSV, przeprowadzić korelację i wykryć ekstrema, a następnie wygenerować raport AI z rekomendacjami technologii PV i wyeksportować go jako PDF — dla jednego uwierzytelnionego użytkownika, wdrożoną na Azure.

## Top Blocker

Brak persistence layer (EF Core DbContext + Azure SQL) i mechanizmu uwierzytelniania — żaden pionowy wycinek nie może wystartować bez bazy danych i zalogowanego użytkownika. Milestone 0 musi być zamknięty jako pierwszy.

## Podejście

Pionowe wycinki (vertical-first). Każdy kamień milowy dostarcza widoczny dla użytkownika przepływ end-to-end.

---

## Milestone 0 — Enabler: Persistence & Auth Skeleton

**Cel:** Odblokować M1 (import CSV wymaga bazy danych i zalogowanego użytkownika).

| ID | Element | Zakres |
|----|---------|--------|
| E0-1 | EF Core DbContext + Azure SQL migration pipeline | `app/Data/`, connection string config |
| E0-2 | ASP.NET Identity — single-user auth | Registration seed, login/logout, middleware |
| E0-3 | Bazowy layout Razor z nawigacją (PL) | `Views/Shared/_Layout.cshtml`, Bootstrap 5 |

**Odblokowuje:** M1

---

## Milestone 1 — Import i podgląd danych klimatycznych

**Cel:** Użytkownik importuje CSV, widzi dane w tabeli.

| ID | Element | Zakres |
|----|---------|--------|
| M1-1 | Model danych klimatycznych + migracja | Encje, walidacja, deduplikacja |
| M1-2 | CSV parser (predefiniowane formaty) | Service, walidacja wierszy, feedback błędów |
| M1-3 | UI importu CSV | Formularz upload, progress, podsumowanie |
| M1-4 | Widok tabeli danych | Paginacja, filtrowanie po dacie |

**Zależności:** E0-1, E0-2
**Ryzyka:** Różnorodność formatów CSV — ograniczyć do 2-3 znanych schematów w MVP.

---

## Milestone 2 — Korelacja i analiza danych

**Cel:** Użytkownik widzi skorelowane dane i ekstrema.

| ID | Element | Zakres |
|----|---------|--------|
| M2-1 | Interpolacja liniowa między seriami | Service z konfigurowalnymi progami |
| M2-2 | Detekcja ekstremów | Configurable thresholds (appsettings) |
| M2-3 | Widok korelacji | Tabela/wykres skorelowanych wartości |
| M2-4 | Widok ekstremów | Lista przekroczeń z filtrami |

**Zależności:** M1
**Niewiadome:** Dokładne progi ekstremów — do ustalenia z użytkownikiem.

---

## Milestone 3 — Raport AI + eksport PDF

**Cel:** Użytkownik generuje raport AI z rekomendacjami PV i eksportuje do PDF.

| ID | Element | Zakres |
|----|---------|--------|
| M3-1 | Integracja OpenAI SDK | Service, prompt engineering, konfiguracja klucza |
| M3-2 | Generowanie raportu on-demand | UI trigger, progress indicator, zapis wyniku |
| M3-3 | Eksport PDF (QuestPDF) | Szablon raportu, tekst PL, download |

**Zależności:** M2
**Ryzyka:** Koszty API OpenAI; rate limiting. Cachowanie wyników raportu.

---

## Pola przekazania do backlogu

Każdy element (`E0-x`, `M1-x`, …) staje się kandydatem na `context/changes/<change-id>/` tworzony via `/10x-new` w momencie rozpoczęcia implementacji.

## Zasady

- Brak pracy horyzontalnej bez nazwanego kamienia milowego, który odblokowuje.
- Brak dat ani story points — to nie jest plan kalendarzowy.
- Mapa drogowa ewoluuje wraz z `lessons.md`.