# Plan — M2-4 Widok ekstremów

Data: 2026-09-08
Zmiana: `M2-4`

## Cel zmiany

Umożliwić użytkownikowi przegląd i analizę ekstremalnych odczytów klimatycznych w czytelnym widoku, wykorzystując istniejący kontrakt `ClimateExtreme` z M2-2. Celem jest szybka identyfikacja przekroczeń progów bez rozbudowywania modelu danych lub oddzielnego mechanizmu persistence.

## Decyzje projektowe

1. **Współdzielenie wyniku M2-2** — widok odwołuje się do istniejącego serwisu ekstremów i nie wprowadza nowego źródła danych.
2. **MVP tylko jako tabela** — w praktyce najpierw czytelna, przewijalna lista rekordów; wykres i agregacja są poza zakresem.
3. **Deterministyczne sortowanie** — rekordy są porządkowane według timestampu, a przy równych datach według parametru / kierunku.
4. **Aplikacja bez błędu dla pustych wyników** — stan pusty to poprawny wynik UX, nie wyjątek.
5. **Minimalna zależność od widoku korelacji** — M2-4 nie rozwiązuje problemu korelacji statystycznej ani nie zmienia logiki M2-1/M2-3.

## Kontrakt domenowy

### Typ wyniku

Model `ClimateExtreme` z M2-2 powinien wystarczyć do prezentacji:

- `Timestamp`
- `Parameter`
- `Value`
- `Direction`
- `Threshold`
- `Source` (jeżeli konieczne w UI)

### Widok

W modelu widoku powinny znaleźć się minimum:

- zakres dat wejściowych
- lista ekstrema
- stan pusty / komunikat
- odczyt progów dla bieżącego kontekstu

## Fazy implementacji

### Faza 1 — Kontrakt prezentacji i mapowanie danych

Pliki docelowe:

- `app/Models/Correlation/ClimateExtremeViewModel.cs`
- `app/Controllers/ClimateDataController.cs`
- `app/Controllers/ClimateCorrelationController.cs` (jeżeli akcja jest rozdzielona)

Zakres:

- Przygotować model widoku z listą ekstremów i stanem pustym.
- Zmapować wynik z `IClimateExtremeDetectionService` do prostego kontraktu prezentacyjnego.
- Zdefiniować wejście dla zakresu dat i weryfikację `from > to`.

Definition of Done:

- Kontroler ma przewidywalny kontrakt dla danych wejściowych i pustego zbioru.
- Wartości z M2-2 są gotowe do renderowania bez dodatkowej obróbki w widoku.

### Faza 2 — Widok Razor i UX

Pliki docelowe:

- `app/Views/ClimateData/Extremes.cshtml` lub equivalent w `ClimateCorrelation/`
- `app/wwwroot/css/site.css` (jeśli wymagane tylko dla czytelności)

Zakres:

- Dodać tabelę z datą, parametrem, wartością, progiem i kierunkiem.
- Pokazać komunikat dla pustego zbioru i braków danych.
- Zachować user experience zgodny z resztą aplikacji: bootstrapowy layout, czytelne nagłówki, responsywność.

Definition of Done:

- Użytkownik może ze zrozumieniem odczytać wyniki ekstremów bez dodatkowych wyjaśnień.
- Widok ma przejrzysty stan pusty i nie zawiera niejawnych błędów walidacji.

### Faza 3 — Testy i stabilizacja

Pliki docelowe:

- `app/Tests/10xPV.Tests/Controllers/ClimateDataExtremeTests.cs`
- `app/Tests/10xPV.Tests/Services/Correlation/` — testy warstwy prezentacyjnej lub mapowania

Zakres:

- Test dla poprawnego zakresu z rekordami ekstremalnymi.
- Test dla pustego zbioru i dla danych bez przekroczeń.
- Test dla błędnego zakresu dat.
- Test sprawdzający stabilność sortowania i mapowania kolumn.

Definition of Done:

- Testy kontrolera i mapowania przechodzą bez regresji.
- Projekt zbuduje się i przejdzie testy jednostkowe.

## Ryzyka i mitigacje

1. **Niejednoznaczny właściciel widoku** — założyć jedną akcję i jeden widok, a nie rozpraszać przepływ po kilku kontrolerach.
2. **Nadmierna funkcjonalność w MVP** — zredukować scope do tabeli i stanu pustego; filtr wymaga osobnego odrębnego potwierdzenia.
3. **Niejasne pomijanie `null`** — jawnie określić, że brak wartości nie tworzy rekordu ekstremum.
4. **Regresja M2-2 i M2-3** — utrzymać kontrakt w M2-2 bez modyfikacji jego logiki; testy dotyczące korelacji pozostają nienaruszone.

## Kryteria akceptacji

- Rekordy ekstremum są prezentowane w prostym i czytelnym formacie.
- Kierunek przekroczenia jest jednoznacznie widoczny.
- Brak danych i brak ekstremów są obsługiwane jako poprawne stany.
- Filtr zakresu dat działa bez skutków ubocznych w UI.
- Build i testy całego projektu przechodzą.

## Plan wykonania

1. `/10x-implement M2-4 phase 1` — kontrakt widoku i mapowanie danych.
2. `/10x-implement M2-4 phase 2` — widok Razor i komunikaty stanu pustego.
3. `/10x-implement M2-4 phase 3` — testy, walidacja i finalna weryfikacja.
4. Po każdej fazie: `dotnet build` i testy odpowiedniego zakresu; na końcu pełny test projektu.

## Progress

- [x] Faza 1 — kontrakt prezentacji i mapowanie danych
- [x] Faza 2 — widok Razor i UX
- [x] Faza 3 — testy i weryfikacja

<!-- Created by /10x-new -->
