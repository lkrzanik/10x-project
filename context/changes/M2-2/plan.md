# Plan — M2-2 Detekcja ekstremów

Data: 2026-09-08
Zmiana: `M2-2`

## Cel zmiany

Umożliwić wykrywanie ekstremalnych wartości w odczytach klimatycznych na podstawie konfigurowalnych progów, bez zmiany danych zapisanych w bazie. Wynik ma być deterministyczny, wyjaśnialny i możliwy do pokazania w istniejącym przepływie analizy.

## Decyzje projektowe

1. **Porównanie progowe** — ekstremum występuje wyłącznie dla wartości mniejszej od minimum lub większej od maksimum.
2. **Brak mutacji danych** — serwis zwraca wynik analizy, ale nie zapisuje flagi na `SensorReading` ani `WeatherReading`.
3. **Aktualna konfiguracja** — progi są dostarczane przez `IOptions<ExtremeDetectionOptions>` i walidowane przy starcie.
4. **Wspólny kontrakt** — sensor i weather są mapowane do wspólnego modelu wejściowego, aby jedna reguła działała dla obu źródeł.
5. **Jawny wynik** — każdy rekord zawiera parametr, kierunek przekroczenia, wartość i próg użyty w klasyfikacji.

## Kontrakt domenowy

### Parametry i progi

Minimalny zakres MVP obejmuje:

- `Temperature`: próg minimalny i maksymalny, z wartościami roboczymi `0` i `40` °C odziedziczonymi z M2-1;
- `Humidity`: niezależny zakres konfiguracyjny;
- `CloudCover`: niezależny zakres konfiguracyjny dla rekordów weather.

Zakresy muszą spełniać `minimum < maximum` oraz dopuszczalne limity domenowe. Dokładne wartości robocze dla parametrów innych niż temperatura wymagają potwierdzenia przed implementacją F1.

### Wynik

Model `ClimateExtreme` powinien zawierać co najmniej:

- `Timestamp`;
- `Parameter`;
- `Value`;
- `Direction` (`BelowMinimum` / `AboveMaximum`);
- `Threshold`;
- `Source` (`Sensor` / `Weather`), jeżeli jest potrzebne w UI.

## Fazy implementacji

### Faza 1 — Model, opcje i serwis

Pliki docelowe:

- `app/Models/Correlation/ClimateExtreme.cs`;
- `app/Services/Correlation/IClimateExtremeDetectionService.cs`;
- `app/Services/Correlation/ClimateExtremeDetectionService.cs`;
- `app/Services/Correlation/ExtremeDetectionOptions.cs`.

Zakres:

- Zdefiniować input/output bez zależności od EF Core.
- Zaimplementować pomijanie `null` i klasyfikację ścisłym porównaniem.
- Zachować deterministyczną kolejność wyników: timestamp, parametr, źródło.

Definition of Done:

- Serwis ma testowalny kontrakt i nie wymaga bazy danych.
- Wszystkie przypadki graniczne są opisane testami jednostkowymi.

### Faza 2 — Konfiguracja, DI i testy

Pliki docelowe:

- `app/appsettings.json`;
- `app/appsettings.Development.json`, jeśli lokalne nadpisanie jest potrzebne;
- `app/Program.cs`;
- `app/Tests/10xPV.Tests/Services/Correlation/ClimateExtremeDetectionServiceTests.cs`;
- testy walidacji opcji.

Zakres:

- Zbindować sekcję `ExtremeDetection` przez `IOptions`.
- Włączyć walidację adnotacyjną i krzyżową przez `ValidateOnStart()`.
- Zarejestrować serwis w DI.
- Przetestować zmianę progów i odrzucenie błędnej konfiguracji.

Definition of Done:

- Aplikacja nie startuje z nieprawidłowym zakresem progów.
- `dotnet build` i testy jednostkowe przechodzą.

### Faza 3 — Integracja MVC

Pliki docelowe do potwierdzenia po wyborze właściciela przepływu:

- `app/Controllers/ClimateDataController.cs`;
- istniejący ViewModel korelacji lub nowy model dla ekstremów;
- widok `app/Views/ClimateData/`;
- testy kontrolera.

Zakres:

- Pobrać dane bez śledzenia i zmapować je do kontraktu serwisu.
- Pokazać wyniki oraz komunikat braku ekstremów/braku danych.
- Nie zmieniać istniejącej akcji korelacji bardziej niż to konieczne.

Definition of Done:

- Użytkownik może zobaczyć wyniki ekstremów dla danych klimatycznych.
- Kontroler ma test dla wyników i pustego zbioru.

## Ryzyka i mitigacje

1. **Niepotwierdzone progi** — oznaczyć wartości robocze i nie zamykać decyzji produktowej jako finalnej.
2. **Niejednoznaczne mapowanie parametrów** — użyć jawnego enumu/kontraktu zamiast nazw kolumn przekazywanych jako string.
3. **Błędna semantyka granic** — testy dla równości progu muszą być obowiązkowe.
4. **Regresja korelacji M2-1** — nowy serwis pozostawić niezależnym od istniejącego serwisu interpolacji i uruchomić pełny test projektu.

## Kryteria akceptacji

- Wszystkie przekroczenia są wykrywane dokładnie raz i mają poprawny kierunek.
- Wartości równe progom oraz `null` nie są zgłaszane.
- Progi są konfigurowalne i walidowane przy starcie.
- Wynik jest deterministyczny i zawiera dane potrzebne do wyjaśnienia klasyfikacji.
- Testy jednostkowe i kontrolerowe przechodzą.

## Plan wykonania

1. `/10x-implement M2-2 phase 1` — kontrakt, opcje i serwis.
2. `/10x-implement M2-2 phase 2` — konfiguracja, DI i testy.
3. `/10x-implement M2-2 phase 3` — integracja MVC po potwierdzeniu właściciela widoku.
4. Po każdej fazie: `dotnet build` oraz testy odpowiedniego zakresu; na końcu pełny test projektu.

## Progress

- [x] Faza 1 — kontrakt, opcje i serwis; testy jednostkowe (6/6)
- [x] Faza 2 — konfiguracja, DI i testy (3/3)
- [x] Faza 3 — integracja MVC i testy kontrolera (7/7)

<!-- Created by /10x-new -->
