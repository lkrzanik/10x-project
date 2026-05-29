# Impl Review — M2-1 Phase 1

Data: 2026-05-29  
Zmiana: `M2-1`  
Zakres: `/10x-implement M2-1 phase 1`

## Werdykt

✅ **Zgodne** z planem fazy 1.

Implementacja serwisu interpolacji została dostarczona i działa zgodnie z kontraktem, a wymagane testy jednostkowe `ClimateCorrelationService` są zaimplementowane i przechodzą.

## Dowody zgodności z planem (phase 1)

### 1) NuGet i integracja techniczna

- ✅ `MathNet.Numerics` dodane do `app/10xPV.csproj` (`Version=5.0.0`).
- ✅ Brak `MathNet.Numerics.MKL.Win-x64` (zgodnie z decyzją MVP).

### 2) Serwis domenowy i kontrakt

- ✅ Interfejs: `app/Services/Correlation/IClimateCorrelationService.cs`
- ✅ Implementacja: `app/Services/Correlation/ClimateCorrelationService.cs`
- ✅ Rejestracja DI: `app/Program.cs`
  - `builder.Services.AddScoped<IClimateCorrelationService, ClimateCorrelationService>();`

### 3) Modele wynikowe

- ✅ `app/Models/Correlation/ClimateSeriesPoint.cs`
- ✅ `app/Models/Correlation/ClimateCorrelationAlignedPoint.cs`
- ✅ `app/Models/Correlation/ClimateCorrelationMetadata.cs`
- ✅ `app/Models/Correlation/ClimateCorrelationResult.cs`

### 4) Zgodność zachowania serwisu z kontraktem

Potwierdzone w kodzie `ClimateCorrelationService`:
- ✅ Brak ekstrapolacji: punkty poza zakresem zwracają `null` i zwiększają `OutOfRangeCount`.
- ✅ Filtrowanie wartości `null` przed budową interpolatora.
- ✅ Dedup timestamp (zliczane w `DroppedCount`).
- ✅ Obsługa serii z 1 punktem: tylko punkt źródłowy zwraca wartość, pozostałe `null`.
- ✅ Metadane jakości: `InterpolatedCount`, `DroppedCount`, `OutOfRangeCount`.

## Status testów jednostkowych serwisu (wymagane przez phase 1)

- ✅ Obecny plik:
  - `app/Tests/10xPV.Tests/Services/Correlation/ClimateCorrelationServiceTests.cs`
- ✅ Pokrycie przypadków obowiązkowych z planu:
  1. happy path (nierównomierne odstępy),
  2. pusta seria,
  3. pojedynczy punkt,
  4. punkt poza zakresem,
  5. duplikaty + `null`.
- ✅ Dodatkowe testy kontraktowe:
  - `ArgumentNullException` dla `sourceSeries == null`
  - `ArgumentNullException` dla `referenceTimeline == null`

## Quality gates (stan na review)

- **Build:** ✅ PASS  
  - `dotnet build` zakończony sukcesem.
- **Lint/Typecheck:** ✅ PASS  
  - Brak osobnej konfiguracji lint; kompilacja C# bez błędów typów.
- **Tests:** ✅ PASS (29/29).

## Ocena Definition of Done (phase 1)

- `Serwis zwraca poprawne wartości interpolowane...` → ✅ Spełnione na poziomie implementacji.
- `Wszystkie testy jednostkowe dla serwisu przechodzą` → ✅ Spełnione.

## Rekomendacje (opcjonalne, poza DoD phase 1)

1. Dodać test dla pustej osi referencyjnej (`referenceTimeline` empty) z oczekiwaniem pustych punktów i metadanych bez błędów.
2. Rozważyć test z nieposortowaną osią referencyjną (zachowanie kolejności wejścia), jeśli to stanie się kontraktem publicznym dla warstwy MVC.

## Finalna decyzja review

**Status:** `PASS`  
**Powód:** wszystkie elementy kontraktu fazy 1 są obecne i zweryfikowane; quality gates zielone.
