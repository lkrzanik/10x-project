# Plan — M2-1 Correlation Research Session

Data: 2026-05-28
Zmiana: `M2-1`
Źródła dowodowe:
- `context/changes/M2-1/correlation-research-session/research.md`
- `context/changes/M2-1/correlation-research-session/correlation-library-research.md`
- `context/changes/M2-1/correlation-research-session/MathNet.Numerics/README.md`
- `context/changes/M2-1/correlation-research-session/MathNet.Numerics/02-interpolation-linear.md`

## Status realizacji planu

- ✅ Phase 1 — zakończona (`serwis + DI + testy jednostkowe`)
- ⏳ Phase 2 — pending (`MVC endpoint/widok + testy kontrolera`)
- ⏳ Phase 3 — pending (`appsettings + IOptions + fail-fast walidacja`)

Aktualizacja 2026-05-29:
- Review phase 1: `PASS`
- Quality gates: `dotnet build` PASS, `dotnet test` PASS (29/29)

## Cel zmiany

Dostarczyć interpolację liniową pomiędzy seriami klimatycznymi (`SensorReading`, `WeatherReading`) jako fundament pod korelację danych w Milestone 2.

## Decyzje projektowe (na podstawie research)

1. **Biblioteka bazowa: `MathNet.Numerics`** (GO).
2. **Brak ekstrapolacji** w MVP — punkty poza zakresem zwracają `null`.
3. **Opakowanie biblioteki interfejsem domenowym**, aby ograniczyć coupling i ryzyko vendor lock-in.
4. **Brak `MathNet.Numerics.MKL.Win-x64`** na MVP — tylko jeśli potwierdzi się bottleneck wydajności.

## Kontrakt implementacyjny

### Wejście

- Dwie serie czasowe punktów `(DateTimeOffset Timestamp, double? Value)` dla wybranego parametru (np. temperatura).
- Dane po walidacji i deduplikacji (już realizowane przez istniejący pipeline importu).

### Wyjście

- Seria wyrównana do wspólnej osi czasu.
- Wartości interpolowane dla brakujących punktów w granicach zakresu źródła.
- Metadane jakości:
  - `InterpolatedCount`
  - `DroppedCount`
  - `OutOfRangeCount`

### Tryby błędów / zachowania graniczne

- Pusta seria wejściowa → wynik pusty + metadane diagnostyczne.
- Seria z 1 punktem → brak interpolacji (wynik `null` poza punktem źródłowym).
- Punkty poza zakresem → `null` (bez ekstrapolacji).
- Wartości `null` i duplikaty czasu → odfiltrowanie przed budową interpolatora.

### Kryterium sukcesu

- Dla wspólnego zakresu czasu wynik jest **deterministyczny** i stabilny numerycznie.
- Polityka poza zakresem jest jawna i jednolita (`null`, bez ekstrapolacji).
- Pokrycie testami obejmuje happy path + edge cases.

## Zakres i pliki docelowe

### Faza 1 — Serwis interpolacji + testy jednostkowe

Zakres:
- Dodać pakiet NuGet `MathNet.Numerics` do `app/10xPV.csproj`.
- Dodać kontrakt serwisu domenowego, np.:
  - `app/Services/Correlation/IClimateCorrelationService.cs`
- Dodać implementację opartą o `MathNet.Numerics`, np.:
  - `app/Services/Correlation/ClimateCorrelationService.cs`
- Dodać model(e) wynikowe dla odpowiedzi i metadanych (folder `app/Models/Correlation/` lub `app/Services/Correlation/Models/`, zgodnie z istniejącą konwencją po implementacji).
- Rejestracja DI w `app/Program.cs`.
- Testy jednostkowe:
  - `app/Tests/10xPV.Tests/Services/Correlation/ClimateCorrelationServiceTests.cs`

Definition of Done (Faza 1):
- Serwis zwraca poprawne wartości interpolowane dla osi czasu referencyjnej.
- Wszystkie testy jednostkowe dla serwisu przechodzą.

### Faza 2 — Integracja MVC (endpoint/widok korelacji)

Zakres:
- Rozszerzyć `app/Controllers/ClimateDataController.cs` o akcję prezentującą korelację/interpolację (zgodnie z routingiem konwencjonalnym MVC).
- Dodać/rozszerzyć ViewModel(e) dla danych korelacyjnych (folder `app/Models/ClimateImport/` lub dedykowany folder dla ClimateData — wybrać zgodnie z aktualnym wzorcem w kodzie).
- Dodać/rozszerzyć widok Razor w `app/Views/ClimateData/`.
- Dodać testy kontrolera (xUnit) dla nowej akcji i scenariuszy brzegowych.

Definition of Done (Faza 2):
- Użytkownik może wyświetlić wynik korelacji/interpolacji dla wybranego zakresu danych.
- Integracja z serwisem działa end-to-end w warstwie MVC.
- Testy kontrolera dla nowej funkcjonalności przechodzą.

### Faza 3 — Konfiguracja progów (alignment z roadmap `M2-1`)

Zakres:
- Dodać sekcję konfiguracji korelacji/interpolacji w `app/appsettings.json`.
- Dodać klasę opcji domenowych i bindowanie przez `IOptions<TOptions>`.
- Dodać walidację wartości progów przy starcie aplikacji (fail-fast dla wartości spoza dopuszczalnego zakresu).
- Ustawić wartości domyślne (robocze):
  - `MinTemperatureC = -40`
  - `MaxTemperatureC = 60`
  - `MinHumidityPercent = 0`
  - `MaxHumidityPercent = 100`

Definition of Done (Faza 3):
- Progi są konfigurowalne przez `appsettings.json` i poprawnie zbindowane do opcji.
- Aplikacja zatrzymuje start dla nieprawidłowych wartości progów.
- Wartości domyślne są ustawione i udokumentowane jako robocze dla M2-1.

## Strategia testów (minimalna)

### Unit (serwis)

Obowiązkowe przypadki:
1. Happy path: nierównomierne odstępy czasu, poprawna interpolacja liniowa.
2. Pusta seria po jednej stronie.
3. Pojedynczy punkt w serii źródłowej.
4. Punkt docelowy poza zakresem min/max.
5. Duplikaty timestamp i wartości `null`.

### Integracyjne / kontrolerowe

1. Akcja zwraca widok z danymi korelacji dla poprawnych danych.
2. Akcja obsługuje brak danych bez wyjątku.
3. Akcja zwraca metadane jakości w modelu widoku.

## Ryzyka i mitigacje

1. **Niejednoznaczna oś referencyjna czasu**
   - Mitigacja: jawny parametr/konwencja (np. oś serii docelowej) + testy kontraktowe.
2. **Niejasna polityka filtrowania danych wejściowych**
   - Mitigacja: wydzielony krok normalizacji danych wejściowych i testy dla duplikatów/null.
3. **Regresja warstwy MVC**
   - Mitigacja: testy kontrolera + brak zmian w istniejących akcjach poza rozszerzeniem.

## Open questions / decyzje do potwierdzenia

1. **Docelowe wartości progów klimatycznych i ekstremów**
  - Status: **otwarte** (roadmap: „Dokładne progi ekstremów — do ustalenia z użytkownikiem”).
  - Decyzja tymczasowa: użyć wartości domyślnych z sekcji „Konfiguracja progów” i oznaczyć je jako robocze.
  - Krok domykający: potwierdzenie progów z użytkownikiem przed zamknięciem całego Milestone 2.

## Granica zakresu względem roadmapy

- Główny zakres tej zmiany to `M2-1` (interpolacja liniowa).
- Plan zawiera także minimalną integrację MVC (akcja + widok) jako pionowy wycinek demonstracyjny.
- To **świadome wyprzedzenie części `M2-3`** tylko w minimalnym zakresie prezentacji wyniku (preferowana forma MVP: tabela skorelowanych wartości). Zaawansowana wizualizacja wykresowa pozostaje poza zakresem tej zmiany.

## Non-goals (M2-1)

- Zaawansowane metody interpolacji (spline, wielomianowa, regresje wielowymiarowe).
- Optymalizacje natywne MKL.
- Eksport raportu PDF dla wyników korelacji (może być osobnym krokiem później).

## Plan wykonania (/10x-implement)

1. `/10x-implement M2-1 phase 1` — serwis + DI + testy jednostkowe.
2. `/10x-implement M2-1 phase 2` — kontroler + widok + testy MVC.
3. `/10x-implement M2-1 phase 3` — konfiguracja progów (`appsettings` + `IOptions` + walidacja).
4. Po każdej fazie: `dotnet build` + testy odpowiedniego scope, a finalnie pełne testy projektu.

## Kryteria akceptacji końcowej

- Interpolacja liniowa działa dla danych klimatycznych zgodnie z polityką „bez ekstrapolacji”.
- Kod jest osadzony w istniejącym wzorcu architektonicznym (interfejs + implementacja + DI + testy).
- Funkcjonalność jest dostępna z poziomu MVC i zachowuje się stabilnie dla edge cases.
