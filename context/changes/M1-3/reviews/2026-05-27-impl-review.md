# /10x-impl-review — M1-3

Data: 2026-05-27
Zmiana: `context/changes/M1-3`
Plan referencyjny: `context/changes/M1-3/plan.md`

## Werdykt

**Status:** ✅ Zatwierdzone warunkowo do triage  
**Podsumowanie:** Implementacja spełnia główny cel M1-3 (UI importu CSV, walidacja formularza, integracja parsera, prezentacja podsumowania i błędów, podstawowy UX „w trakcie”). Brak krytycznych błędów bezpieczeństwa i stabilności.

Do decyzji triage pozostają 2 obserwacje: jedna o średnim wpływie (dyscyplina zakresu), jedna o niskim/średnim wpływie (pokrycie testów edge-case).

## Kontrola zgodności z planem

### Kontrakt i zakres

- ✅ Endpoint MVC GET/POST z anty-forgery: `app/Controllers/ClimateImportController.cs`
- ✅ ViewModel-e formularza/wyniku: `app/Models/ClimateImport/*`
- ✅ Widok Razor (formularz, summary, tabela błędów): `app/Views/ClimateImport/Index.cshtml`
- ✅ UX „w trakcie” (disable submit + komunikat): `app/Views/ClimateImport/Index.cshtml`
- ✅ Integracja parsera przez orchestrator: `app/Services/ClimateImport/ClimateImportOrchestrator.cs`
- ✅ Link nawigacyjny: `app/Views/Shared/_Layout.cshtml`
- ✅ Testy kontrolera (happy path + walidacja + wyjątki): `app/Tests/10xPV.Tests/Controllers/ClimateImportControllerTests.cs`

### Bezpieczeństwo i obsługa błędów

- ✅ CSRF: `[ValidateAntiForgeryToken]`
- ✅ Bezpieczne komunikaty użytkownika dla wyjątków i anulowania
- ✅ Brak stack trace do UI, logowanie po stronie serwera
- ✅ Limit rozmiaru pliku i podstawowa walidacja rozszerzenia `.csv`

### Architektura / gotowość pod M1-4

- ✅ Warstwa rozszerzalności istnieje (`IClimateImportOrchestrator`, `IClimateImportPersistenceAdapter`)
- ⚠️ Implementacja persistence została realnie podłączona wcześniej niż deklarowany zakres M1-3 (szczegół w sekcji „Findings”)

## Findings i triage

### 1) Scope drift: aktywna persistencja DB w M1-3

- **Severity:** Medium
- **Impact now:** Medium
- **Dowód:**
  - Rejestracja DI: `app/Program.cs` używa `IClimateImportPersistenceAdapter -> ClimateImportPersistenceAdapter`
  - Adapter zapisuje dane do DB: `app/Services/ClimateImport/ClimateImportPersistenceAdapter.cs`
- **Dlaczego to problem:** Plan M1-3 deklaruje persistence jako poza zakresem (NoOp + punkt rozszerzenia pod M1-4). Aktualna implementacja wykonuje faktyczny zapis i deduplikację już w M1-3.
- **Triage:** **fix** (napraw inaczej / doprecyzuj)
- **Opcje decyzji:**
  1. Przywrócić zgodność zakresu: przełączyć DI na `NoOpClimateImportPersistenceAdapter` w M1-3.
  2. Albo zaakceptować rozszerzenie zakresu i zaktualizować plan/DoD/ryzyka, by formalnie odzwierciedlały stan implementacji.

### 2) Niepełne pokrycie testowe walidacji wejścia

- **Severity:** Low
- **Impact now:** Low-Medium
- **Dowód:** `ClimateImportControllerTests` nie obejmuje jawnie scenariuszy:
  - plik 0 B,
  - niepoprawne rozszerzenie,
  - rozmiar > 10 MB.
- **Dlaczego to problem:** Edge-case’y są opisane w planie jako do pokrycia implementacją/testami; obecne pokrycie jest dobre, ale niepełne dla kontraktu walidacji wejścia.
- **Triage:** **skip** (świadomie odroczyć) lub **fix** (jeśli zależy nam na pełnej zgodności z planem F3)
- **Rekomendacja:** Dodać 2–3 krótkie testy jednostkowe kontrolera (niski koszt, wysoka czytelność kontraktu).

## Quality gates (delta)

- **Build:** PASS (`dotnet build`)
- **Tests:** PASS (`dotnet test`, 21/21)
- **Lint/Typecheck:** PASS (brak błędów diagnostycznych w workspace)

## Rekomendacja końcowa

Zmiana jest funkcjonalnie gotowa i jakościowo stabilna dla celu UI importu.  
Przed finalnym zamknięciem M1-3 warto podjąć świadomą decyzję triage dla scope drift persistence (fix lub formalna akceptacja ryzyka) oraz ewentualnie domknąć brakujące testy walidacji wejścia.
