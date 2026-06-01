# Test Plan — 10xPV

> Umowa jakościowa oparta na ryzyku. Utworzono: 2026-06-01.

---

## §1 Mapa ryzyka

| # | Scenariusz awarii | Wpływ | Prawdop. | Źródło |
|---|---|---|---|---|
| R1 | Użytkownik importuje CSV z subtelnymi błędami formatu (np. zamieniony separator, brakujące kolumny) i system cicho zapisuje śmieciowe dane | Krytyczny | Wysokie | PRD FR-2/FR-3, roadmap M1-2 |
| R2 | Interpolacja liniowa produkuje błędne wartości skorelowane gdy serie czasowe mają duże luki lub odwróconą kolejność timestampów | Krytyczny | Średnie | PRD FR-4, roadmap M2-1 |
| R3 | Deduplikacja przy ponownym imporcie nie wykrywa duplikatów lub kasuje prawidłowe rekordy | Wysoki | Średnie | PRD FR-2 "walidacja duplikatów", roadmap M1-1 |
| R4 | Raport AI zawiera halucynacje lub brak wymaganej struktury (ocena nadaje się/nie nadaje się + rekomendacja) | Wysoki | Średnie | PRD FR-8, metryka sukcesu #2 |
| R5 | Eksport PDF nie generuje się lub produkuje uszkodzony plik przy dużym zbiorze danych | Średni | Niskie | PRD FR-8, roadmap M3-3 |
| R6 | Progi ekstremów zmienione w ustawieniach nie wpływają natychmiast na oznaczenie rekordów | Średni | Średnie | PRD FR-7/FR-9, US-8 |
| R7 | Nieautoryzowany dostęp do danych — sesja nie wygasa lub brak middleware auth | Wysoki | Niskie | PRD FR-1, roadmap E0-2 |

---

## §2 Uzasadnienie ryzyk (dowody)

- **R1**: PRD wymaga walidacji formatu i feedbacku błędów. Dwa różne schematy CSV (Sensor vs Weather) zwiększają powierzchnię błędu.
- **R2**: PRD wymaga interpolacji liniowej dla niedopasowanych interwałów. Metryka sukcesu #1 ("100% rekordów skorelowanych") oznacza, że ciche błędy interpolacji są niewidoczne.
- **R3**: PRD explicite wymaga "walidacja duplikatów". Ponowny import tego samego pliku to naturalny scenariusz użycia.
- **R4**: PRD wymaga konkretnej struktury raportu. GPT może nie przestrzegać schematu odpowiedzi bez odpowiedniej walidacji.
- **R5**: QuestPDF przy dużych danych może przekroczyć limity pamięci lub timeout 60s (metryka #3).
- **R6**: PRD US-8: "Zmiana progów natychmiast wpływa na oznaczenie ekstremów" — wymaga przeliczenia w locie.
- **R7**: PRD FR-1: "Sesja z timeoutem". Brak middleware = pełny dostęp bez logowania.

---

## §3 Phased Rollout

| Faza | Change ID | Zakres | Pokrywa ryzyka | Status |
|------|-----------|--------|----------------|--------|
| 1 | tp-unit-csv | Testy jednostkowe: parsery CSV, walidacja, deduplikacja | R1, R3 | planned |
| 2 | tp-unit-correlation | Testy jednostkowe: interpolacja liniowa, korelacja | R2 | not started |
| 3 | tp-unit-extremes | Testy jednostkowe: detekcja ekstremów, progi | R6 | not started |
| 4 | tp-integration-import | Testy integracyjne: pełny przepływ importu CSV → baza | R1, R3 | not started |
| 5 | tp-unit-ai-report | Testy jednostkowe: serwis raportu AI (mock OpenAI) + walidacja struktury | R4 | not started |
| 6 | tp-unit-pdf | Testy jednostkowe: generowanie PDF (mock danych) | R5 | not started |
| 7 | tp-integration-auth | Testy integracyjne: middleware auth, sesja, timeout | R7 | not started |

---

## §4 Stos testowy

| Warstwa | Narzędzie | Rola | checked |
|---------|-----------|------|---------|
| Unit | xUnit (`Assert.*`) | Asercje, testy parametryzowane | 2026-06-01 |
| Mocking | Moq | Mocki serwisów i zależności | 2026-06-01 |
| Integration | Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory) | Testy HTTP, in-memory DB | 2026-06-01 |
| In-memory DB | Microsoft.EntityFrameworkCore.InMemory | Izolacja testów integracyjnych | 2026-06-01 |
| Coverage | coverlet + ReportGenerator | Raport pokrycia (lokalnie) | 2026-06-01 |

### Warstwa AI-native

Nie stosowana w tym projekcie. Uzasadnienie: single-user internal tool, klasyczne mocki dla OpenAI SDK wystarczają do walidacji struktury odpowiedzi i obsługi błędów. Warstwa wizualna PDF weryfikowana ręcznie.

---

## §5 Bramki jakości

| Bramka | Typ | Kiedy wymagana |
|--------|-----|----------------|
| `dotnet build` — zero warnings (nullable) | Wymagana | Każdy commit (lokalnie) |
| `dotnet test` — 100% pass | Wymagana | Każdy commit (lokalnie) |
| Pokrycie ryzyk R1–R3 ≥ 90% linii w serwisach CSV/korelacji | Wymagana | Po §3 Faza 2 |
| Pokrycie ryzyk R4–R5 ≥ 80% linii w serwisach AI/PDF | Wymagana | Po §3 Faza 6 |
| CI pipeline (lint + test + build) | Zalecana (przyszłość) | Nie planowana na razie |
| Hook post-edycji: `dotnet test` | Zalecany lokalnie | Po §3 Faza 1 |

---

## §6 Cookbook

### 6.1 Testy jednostkowe CSV (Faza 1)

- **Lokalizacja**:
	- `app/Tests/10xPV.Tests/Services/Csv/CsvImportServiceTests.cs`
	- `app/Tests/10xPV.Tests/Services/ClimateDataValidatorTests.cs`
	- `app/Tests/10xPV.Tests/Services/DeduplicationServiceTests.cs`
- **Nazewnictwo**: `Method_Scenario_ExpectedResult`
- **Test referencyjny**: edge case parsera z błędnym separatorem (`;`) powinien kończyć się błędem walidacji nagłówka
- **Komenda uruchomienia**: `dotnet test app/Tests/10xPV.Tests/10xPV.Tests.csproj`
- **Zakres sygnału**: R1 (format + walidacja danych) i R3 (deduplikacja po timestamp + source)

### 6.2 Testy jednostkowe korelacji (Faza 2)
TBD — zobacz §3 Faza 2

### 6.3 Testy jednostkowe ekstremów (Faza 3)
TBD — zobacz §3 Faza 3

### 6.4 Testy integracyjne importu (Faza 4)
TBD — zobacz §3 Faza 4

### 6.5 Testy jednostkowe AI (Faza 5)
TBD — zobacz §3 Faza 5

### 6.6 Testy jednostkowe PDF (Faza 6)
TBD — zobacz §3 Faza 6

### 6.7 Testy integracyjne auth (Faza 7)
TBD — zobacz §3 Faza 7

---

## §7 Przestrzeń negatywna (czego celowo NIE testujemy)

| Element | Powód |
|---------|-------|
| UI Razor views (renderowanie HTML) | Single-user, weryfikacja manualna wystarczy |
| Wizualizacja wykresów (Chart.js) | Frontend JS poza zakresem testów backend |
| Migracje EF Core | Testowane przez `dotnet ef database update` w dev |
| Konfiguracja Azure App Service | Infrastruktura, nie logika aplikacji |
| Jakość odpowiedzi GPT (semantyczna) | Mock na granicy SDK; treść walidowana manualnie |
| Stylowanie PDF (layout, fonty) | Weryfikacja manualna; brak ROI z automatyzacji |
