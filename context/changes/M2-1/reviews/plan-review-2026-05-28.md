# Plan Review — M2-1 (`plan.md`)

Data review: 2026-05-29
Recenzowany plik: `context/changes/M2-1/plan.md`

## TL;DR

Plan jest **dobry i implementowalny** (ocena: **8.5/10**).
Werdykt: **GO warunkowe** — po doprecyzowaniu kilku punktów kontraktowych, które ograniczą ryzyko rozjazdu podczas `/10x-implement`.

## Co działa bardzo dobrze

1. **Jasny kontrakt funkcjonalny**
   - Wejście/wyjście, zachowania brzegowe i kryteria sukcesu są zdefiniowane.
2. **Trafne decyzje techniczne dla MVP**
   - `MathNet.Numerics`, brak ekstrapolacji, opakowanie biblioteki interfejsem domenowym.
3. **Sensowna strategia testów**
   - Happy path + kluczowe edge cases dla serwisu i MVC.
4. **Dobra struktura faz**
   - Serwis → integracja MVC → konfiguracja progów.
5. **Świadoma granica zakresu**
   - Minimalne wejście w `M2-3` opisane jako pionowy wycinek demonstracyjny.

## Luki / doprecyzowania przed implementacją

### 1) Oś referencyjna czasu wymaga twardej reguły

Plan wspomina „wspólną oś czasu”, ale bez jednej, wiążącej polityki MVP.
Należy dopisać decyzję, np.:
- oś serii docelowej, albo
- oś parametryzowana (jawny parametr kontraktu).

### 2) Metryki jakości wymagają ścisłych definicji

Doprecyzować semantykę:
- `InterpolatedCount` — liczba punktów osi, dla których wartość została wyznaczona interpolacją,
- `OutOfRangeCount` — liczba punktów osi poza zakresem czasu serii źródłowej,
- `DroppedCount` — liczba rekordów odrzuconych w normalizacji wejścia (np. `null`, duplikat timestamp).

### 3) Lokalizacje plików powinny być jednoznaczne

W planie są warianty „lub/albo” dla modeli i view modeli.
Przed implementacją warto wskazać jeden docelowy układ katalogów, aby zachować spójność architektury.

### 4) Testy kontrolera — wskazać rozszerzenie istniejącego suite

W repo istnieje już testowy kontekst dla `ClimateDataController`; plan powinien preferować jego rozszerzenie zamiast tworzenia równoległej, rozproszonej struktury testów.

## Go / No-Go

- **GO warunkowe** — można startować `/10x-implement M2-1 phase 1`.
- Zalecenie: przed startem dopisać 4 doprecyzowania powyżej bez zmiany zakresu funkcjonalnego.

## Proponowane następne kroki

1. Uzupełnić `context/changes/M2-1/plan.md` o twardą politykę osi czasu i definicje metryk.
2. Zamrozić docelowe ścieżki plików (modele/view modele/testy).
3. Uruchomić `/10x-implement M2-1 phase 1`.
