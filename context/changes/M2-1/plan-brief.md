# M2-1 — plan-brief

## Cel
Dostarczyć interpolację liniową pomiędzy seriami klimatycznymi (`SensorReading`, `WeatherReading`) jako fundament pod korelację danych w Milestone 2.

## Decyzje projektowe
- Biblioteka bazowa: `MathNet.Numerics`.
- MVP bez ekstrapolacji: punkty poza zakresem zwracają `null`.
- Opakowanie biblioteki interfejsem domenowym dla ograniczenia coupling.
- Bez `MathNet.Numerics.MKL.Win-x64` na MVP.

## Kontrakt implementacyjny

### Wejście
- Dwie serie czasowe punktów `(DateTimeOffset Timestamp, double? Value)`.
- Dane po walidacji i deduplikacji z istniejącego pipeline importu.

### Wyjście
- Seria wyrównana do wspólnej osi czasu.
- Wartości interpolowane dla brakujących punktów w granicach zakresu źródła.
- Metadane jakości: `InterpolatedCount`, `DroppedCount`, `OutOfRangeCount`.

### Zachowania graniczne
- Pusta seria wejściowa → wynik pusty + metadane diagnostyczne.
- Seria z 1 punktem → brak interpolacji (wynik `null` poza punktem źródłowym).
- Punkty poza zakresem → `null`.
- Wartości `null` i duplikaty czasu → odfiltrowanie przed budową interpolatora.

## Fazy
1. **Serwis interpolacji + testy jednostkowe**
	- Dodać pakiet NuGet `MathNet.Numerics` do `app/10xPV.csproj`.
	- Dodać kontrakt i implementację serwisu korelacji/interpolacji.
	- Dodać modele wynikowe i metadane jakości.
	- Zarejestrować DI w `app/Program.cs`.
	- Dodać testy jednostkowe serwisu.
2. **Integracja MVC (endpoint/widok korelacji)**
	- Rozszerzyć `app/Controllers/ClimateDataController.cs` o akcję korelacji/interpolacji.
	- Dodać/rozszerzyć ViewModel(e) dla danych korelacyjnych.
	- Dodać/rozszerzyć widok w `app/Views/ClimateData/`.
	- Dodać testy kontrolera dla nowej akcji.
3. **Konfiguracja progów (alignment z roadmap `M2-1`)**
	- Dodać sekcję konfiguracji korelacji/interpolacji w `app/appsettings.json`.
	- Dodać klasę opcji domenowych i bindowanie przez `IOptions<TOptions>`.
	- Dodać walidację progów przy starcie aplikacji (fail-fast dla wartości spoza zakresu).
	- Ustawić wartości domyślne (robocze):
		- `MinTemperatureC = -40`
		- `MaxTemperatureC = 60`
		- `MinHumidityPercent = 0`
		- `MaxHumidityPercent = 100`

## Kryteria sukcesu
- Interpolacja zwraca deterministyczne wyniki dla danych wewnątrz zakresu czasu.
- Polityka poza zakresem jest jawna i jednolita (`null`, bez ekstrapolacji).
- Wynik zawiera metadane jakości (`InterpolatedCount`, `DroppedCount`, `OutOfRangeCount`).
- Pokrycie testami obejmuje happy path i edge cases (unit + controller).

## Strategia testów (minimalna)

### Unit (serwis)
1. Happy path: nierównomierne odstępy czasu i poprawna interpolacja liniowa.
2. Pusta seria po jednej stronie.
3. Pojedynczy punkt w serii źródłowej.
4. Punkt docelowy poza zakresem min/max.
5. Duplikaty timestamp i wartości `null`.

### Integracyjne / kontrolerowe
1. Akcja zwraca widok z danymi korelacji dla poprawnych danych.
2. Akcja obsługuje brak danych bez wyjątku.
3. Akcja zwraca metadane jakości w modelu widoku.

## Ryzyka i mitigacje
- Niejednoznaczna oś referencyjna czasu → jawny parametr/konwencja + testy kontraktowe.
- Niejasna polityka filtrowania danych wejściowych → wydzielony krok normalizacji + testy dla duplikatów/null.
- Regresja warstwy MVC → testy kontrolera + brak zmian w istniejących akcjach poza rozszerzeniem.

## Non-goals (M2-1)
- Zaawansowane metody interpolacji (spline, wielomianowa, regresje wielowymiarowe).
- Optymalizacje natywne MKL.
- Eksport raportu PDF dla wyników korelacji.

## Open question
- Docelowe wartości progów klimatycznych i ekstremów pozostają do potwierdzenia z użytkownikiem.
- Decyzja tymczasowa: użyć wartości domyślnych z Fazy 3 (konfiguracja progów).

## Granica zakresu względem roadmapy
- Główny zakres zmiany to `M2-1` (interpolacja liniowa).
- Plan obejmuje minimalną integrację MVC (akcja + widok) jako pionowy wycinek demonstracyjny.
- To świadome, minimalne wyprzedzenie części `M2-3` (prezentacja tabelaryczna), bez zaawansowanej wizualizacji.

## Komendy weryfikacyjne
- `cd app`
- `dotnet build`
- `dotnet test`

## Progress
- [ ] F1
- [ ] F2
- [ ] F3
