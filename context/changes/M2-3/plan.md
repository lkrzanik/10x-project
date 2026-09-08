# Plan — M2-3 Widok korelacji

Data: 2026-09-08
Zmiana: `M2-3`

## Cel zmiany

Rozwinąć akcję `ClimateCorrelation/Correlation` z demonstracyjnego widoku dwóch tabel do kompletnego widoku analizy: wspólnej tabeli skorelowanych temperatur, wykresu obu serii oraz jasnych informacji o interpolacji i brakach danych.

## Stan wejściowy

- `ClimateDataController.Correlation` pobiera dane sensor/weather, wywołuje `IClimateCorrelationService` i przekazuje `CorrelationViewModel`.
- `ClimateCorrelationService` z `M2-1` wyrównuje serie bez ekstrapolacji i zwraca metadane.
- `Correlation.cshtml` pokazuje dwa niezależne zestawienia oraz wyniki ekstremów zintegrowane przez `M2-2`.
- Istnieją testy kontrolera korelacji, które należy rozszerzyć zamiast tworzyć równoległy harness.

## Decyzje projektowe

1. **Jedno źródło obliczeń** — kontroler nadal deleguje interpolację do `IClimateCorrelationService`; warstwa UI tylko scala i prezentuje wynik.
2. **Wspólna oś tabeli** — tabela jest budowana deterministycznie po timestampie, z wartościami sensor/weather w jednym wierszu, a nie przez indeksowanie dwóch list o różnych długościach.
3. **Jawne braki** — `null` jest renderowane jako brak wartości i przekazywane do wykresu jako luka; nie wolno zastępować go zerem.
4. **Responsywność** — tabela ma przewijanie poziome, a wykres ma stabilną wysokość i nie wypycha formularza poza viewport mobilny.
5. **Bezpieczna serializacja** — dane do JavaScript są przekazywane przez mechanizm serializacji Razor/JSON z właściwym kodowaniem, nie przez ręczne sklejanie skryptu.
6. **Zakres MVP** — temperatura i zakres dat; architektura ViewModelu może później przyjąć kolejne parametry bez implementowania ich teraz.

## Fazy implementacji

### Faza 1 — Kontrakt prezentacji i dane

Pliki docelowe:

- `app/Models/Correlation/CorrelationViewModel.cs`;
- ewentualnie `app/Models/Correlation/CorrelationTableRowViewModel.cs`;
- `app/Controllers/ClimateDataController.cs`;
- `app/Tests/10xPV.Tests/Controllers/ClimateDataCorrelationTests.cs`.

Zakres:

- Dodać jawny model wiersza wspólnej tabeli albo równoważny sposób scalania po timestampie.
- Ustalić kolejność sortowania oraz semantykę flag `IsInterpolated` dla obu źródeł.
- Zachować istniejące `SensorMetadata`, `WeatherMetadata`, `Extremes` i obsługę `from/to`.
- Dodać testy dla nierównych osi, pustych list i punktów poza zakresem.

Definition of Done:

- ViewModel nie opiera wspólnego wiersza na indeksie dwóch niezależnych list.
- Każdy timestamp i `null` ma jednoznaczną reprezentację.
- Dotychczasowe testy akcji `Correlation` pozostają zielone.

### Faza 2 — Tabela i wykres

Pliki docelowe:

- `app/Views/ClimateCorrelation/Correlation.cshtml`;
- `app/wwwroot/js/` — wybrany skrypt wykresu, jeśli potrzebny;
- `app/wwwroot/css/site.css` — minimalne style komponentu;
- ewentualnie `app/Views/Shared/_ValidationScriptsPartial.cshtml` tylko jeśli istniejący wzorzec tego wymaga.

Zakres:

- Zastąpić/uzupełnić obecne dwa zestawienia wspólną tabelą z timestampem, wartościami i flagami interpolacji.
- Zachować metadane kierunku sensor → weather i weather → sensor.
- Dodać wykres dwóch serii z legendą, opisem osi i obsługą luk `null`.
- Zapewnić dostępny tekstowy odpowiednik informacji z wykresu, aby tabela nie była tylko dodatkiem do wizualizacji.
- Pokazać poprawne komunikaty dla braku wyników i braku ekstremów.

Definition of Done:

- Widok jest czytelny na desktopie i urządzeniu mobilnym.
- Wykres nie zasłania formularza ani tabeli i nie jest jedynym nośnikiem danych.
- Dane użytkownika są kodowane i bezpiecznie przekazywane do JavaScript.

### Faza 3 — Testy i stabilizacja

Pliki docelowe:

- `app/Tests/10xPV.Tests/Controllers/ClimateDataCorrelationTests.cs`;
- ewentualne testy widoku/integracyjne w `app/Tests/10xPV.Tests/`;
- `context/changes/M2-3/change.md`.

Zakres:

- Pokryć poprawny wynik, brak danych, odwrócony zakres, pojedynczy punkt i dane poza zakresem.
- Zweryfikować, że filtr dat trafia do zapytań i wraca do formularza.
- Zweryfikować teksty/elementy dostępności widoku oraz obecność danych wykresu.
- Uruchomić pełne bramki jakości i uzupełnić status change.

Definition of Done:

- Testy regresyjne istnieją dla kontrolera i najważniejszych stanów UI.
- `dotnet build app/10xPV.sln` przechodzi bez nowych ostrzeżeń.
- `dotnet test app/Tests/10xPV.Tests/10xPV.Tests.csproj` przechodzi.

## Ryzyka i mitigacje

1. **Dwie listy mają różne timestampy** — scalać po kluczu czasu, nigdy po pozycji listy; dodać test nierównych osi.
2. **Wykres ukrywa braki** — zachować `null`, pokazać lukę i tabelę jako źródło prawdy.
3. **Zależność od biblioteki JS** — najpierw sprawdzić istniejące zasoby projektu; nie wprowadzać ciężkiej biblioteki bez potrzeby.
4. **Regresja istniejących ekstremów** — zachować sekcję ekstremów i test kontrolera z `M2-2`.
5. **Duży zakres danych** — ograniczyć ciężar payloadu lub użyć stabilnej tabeli; nie wykonywać dodatkowych zapytań dla każdego punktu.

## Kryteria akceptacji

- Użytkownik może wybrać zakres dat i zobaczyć skorelowane serie temperatury.
- Wspólna tabela poprawnie pokazuje timestampy obecne tylko w jednej osi oraz wartości brakujące.
- Interpolacja jest oznaczona osobno dla obu kierunków.
- Wykres przedstawia obie serie, nie sugeruje ekstrapolacji i ma tekstową alternatywę.
- Błędy zakresu i pusty wynik są obsługiwane bez wyjątku.
- Istniejące wyniki ekstremów nadal są dostępne w tym przepływie.

## Plan wykonania

1. `/10x-implement M2-3 phase 1` — kontrakt tabeli i mapowanie danych.
2. `/10x-implement M2-3 phase 2` — Razor, wykres i responsywność.
3. `/10x-implement M2-3 phase 3` — testy, przegląd dostępności i bramki jakości.
4. Po każdej fazie: testy odpowiedniego zakresu; na końcu pełny build i test projektu.

## Progress

- [x] Faza 1 — kontrakt prezentacji i dane
- [x] Faza 2 — tabela i wykres
- [x] Faza 3 — testy i stabilizacja

<!-- Created by /10x-new -->
