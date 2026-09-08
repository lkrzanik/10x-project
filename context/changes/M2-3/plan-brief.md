# M2-3 — plan-brief

## Cel

Dostarczyć użytkownikowi jeden spójny widok, w którym może ocenić skorelowane serie klimatyczne: zobaczyć wartości w tabeli, szybko rozpoznać interpolację i braki poza zakresem oraz odczytać przebieg obu serii na wykresie.

## Kontrakt implementacyjny

### Wejście

- Opcjonalny zakres `from` / `to` z istniejącego formularza `ClimateCorrelation/Correlation`.
- Dane `SensorReading` i `WeatherReading` odczytane bez śledzenia.
- Wyniki `IClimateCorrelationService.AlignToReferenceTimeline` z `M2-1`.

### Wyjście

- Model widoku zawierający zakres dat, dwie serie wyrównanych punktów i metadane obu kierunków.
- Tabela z timestampem, temperaturą czujnika, temperaturą pogody oraz informacją, czy wartość jest interpolowana.
- Wykres z dwiema seriami i jawnym rozróżnieniem wartości brakujących/interpolowanych.
- Komunikaty dla braku danych, nieprawidłowego zakresu oraz poprawnego wyniku bez ekstremów.

### Zachowania graniczne

- `from > to` nie uruchamia zapytań ani obliczeń i zwraca formularz z błędem.
- Puste źródło lub brak wspólnego zakresu czasu daje pusty stan, bez wyjątku i bez ekstrapolacji.
- `null` pozostaje `null` w modelu oraz na wykresie.
- Daty i stan filtrów są zachowane po walidacji błędnej lub ponownej wysyłce formularza.
- Duża liczba punktów nie może powodować rozszerzania układu poza kontener; tabela pozostaje przewijalna poziomo.

## Fazy

1. **Kontrakt prezentacji i przygotowanie danych**
   - Ustalić kształt ViewModelu dla wspólnej tabeli i danych wykresu.
   - Ograniczyć powielanie mapowania w kontrolerze.
   - Zachować istniejące metadane i kontrakt serwisu korelacji.
2. **Tabela i wykres**
   - Rozszerzyć widok Razor o skanowalną tabelę skorelowanych wartości.
   - Dodać responsywny wykres z etykietami, legendą i obsługą pustych wartości.
   - Nie ładować danych z nieufnego HTML bez odpowiedniego kodowania/serializacji.
3. **Testy i stabilizacja**
   - Dodać testy kontrolera dla wyników, pustych danych, zakresu odwróconego i wartości poza zakresem.
   - Sprawdzić renderowanie stanu pustego oraz tabeli/wykresu.
   - Uruchomić build i pełny zestaw testów projektu.

## Kryteria sukcesu

- Użytkownik widzi oba kierunki korelacji w jednym widoku i może odróżnić wartości interpolowane od źródłowych.
- Wartości poza zakresem nie są przedstawiane jako interpolowane ani ekstrapolowane.
- Widok działa dla pustego zbioru, pojedynczego punktu i nierównych osi czasu.
- Filtr dat działa tak samo jak w istniejących widokach danych.
- `dotnet build` i `dotnet test` przechodzą bez nowych ostrzeżeń.

## Strategia testów

1. Poprawny zakres z nierównymi osiami czasu: tabela zawiera obie serie i metadane.
2. Daty `from == to` oraz `from > to`.
3. Puste sensor/weather i brak wspólnego zakresu.
4. Wartości interpolowane, dokładne oraz `null` poza zakresem.
5. Zachowanie filtrów po błędzie walidacji.
6. Widok zawiera dostępne nagłówki tabeli, legendę i komunikat stanu pustego.

## Non-goals

- Zmiana algorytmu interpolacji lub dodawanie ekstrapolacji.
- Statystyczny współczynnik korelacji Pearsona.
- Korelacja wilgotności i zachmurzenia w tym samym zakresie.
- Trwałe zapisywanie wyników ani eksport CSV/PDF.
- Widok listy ekstremów jako osobna funkcja M2-4.

## Komendy weryfikacyjne

- `dotnet build app/10xPV.sln`
- `dotnet test app/Tests/10xPV.Tests/10xPV.Tests.csproj`
