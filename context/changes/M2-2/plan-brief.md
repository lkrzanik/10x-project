# M2-2 — plan-brief

## Cel

Dostarczyć deterministyczną detekcję ekstremów dla danych klimatycznych, sterowaną progami z konfiguracji i gotową do użycia przez widok analizy.

## Kontrakt implementacyjny

### Wejście

- Odczyty `SensorReading` i `WeatherReading` albo ujednolicone punkty analizy.
- Parametr: temperatura, wilgotność lub zachmurzenie.
- Aktualne progi minimalne i maksymalne z `IOptions<ExtremeDetectionOptions>`.

### Wyjście

Lista wyników zawierających:

- `Timestamp`;
- nazwę parametru;
- wartość;
- kierunek: `BelowMinimum` albo `AboveMaximum`;
- próg, który został przekroczony;
- źródło odczytu, jeśli jest potrzebne do rozróżnienia sensor/weather.

### Zachowania graniczne

- `null` jest pomijany.
- Wartość równa minimum lub maksimum nie jest ekstremum.
- Jedna wartość może wygenerować najwyżej jeden wynik dla danego parametru.
- Pusta kolekcja zwraca pusty wynik bez wyjątku.
- Nieprawidłowa konfiguracja (`minimum >= maximum`, zakres spoza dopuszczalnych granic) zatrzymuje start aplikacji przez walidację opcji.

## Fazy

1. **Model i serwis detekcji**
   - Dodać model wyniku ekstremum i kontrakt serwisu.
   - Zdefiniować ujednolicony input mapping dla obu typów odczytów.
   - Zaimplementować klasyfikację poniżej minimum / powyżej maksimum.
2. **Konfiguracja, DI i testy jednostkowe**
   - Dodać sekcję progów do `appsettings.json` i `appsettings.Development.json`, jeśli wymagana.
   - Zarejestrować opcje z walidacją `ValidateOnStart()` oraz serwis w DI.
   - Pokryć testami progi, wartości graniczne, `null`, pusty input i wiele parametrów.
3. **Integracja MVC**
   - Udostępnić wyniki w istniejącym kontrolerze lub dedykowanej akcji analizy.
   - Rozszerzyć ViewModel i widok tabeli o oznaczenie ekstremów.
   - Dodać testy kontrolera dla wyników i braku danych.

## Kryteria sukcesu

- Dla każdej wartości poniżej lub powyżej skonfigurowanego progu powstaje dokładnie jeden poprawnie opisany wynik.
- Wartości graniczne nie są błędnie oznaczane.
- Zmiana progów w konfiguracji zmienia klasyfikację po ponownym uruchomieniu bez zmiany kodu.
- `dotnet build` i `dotnet test` przechodzą bez nowych ostrzeżeń.

## Strategia testów

1. Temperatura poniżej minimum.
2. Temperatura powyżej maksimum.
3. Wartości dokładnie na obu progach.
4. `null` i pusta kolekcja.
5. Wilgotność i zachmurzenie z niezależnymi progami.
6. Odczyty z obu źródeł w tej samej osi czasu.
7. Nieprawidłowa konfiguracja odrzucona podczas walidacji opcji.
8. Kontroler przekazuje wyniki do ViewModelu i obsługuje brak danych.

## Non-goals

- Automatyczne wyznaczanie progów statystycznych.
- Predykcja anomalii i uczenie maszynowe.
- Trwałe zapisywanie wyników ekstremów.
- Wykresy i pełny widok ekstremów, jeśli wymaga to osobnego zakresu M2-4.

## Komendy weryfikacyjne

- `dotnet build app/10xPV.sln`
- `dotnet test app/Tests/10xPV.Tests/10xPV.Tests.csproj`
