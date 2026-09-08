# M2-4 — plan-brief

## Cel

Dostarczyć użytkownikowi czytelny widok ekstremów z danych klimatycznych, tak aby szybko sprawdzić, które rekordy przekroczyły konfigurację progów, w jakim kierunku i z jakimi wartościami progowymi.

## Kontrakt implementacyjny

### Wejście

- Zakres dat z formularza analizy danych.
- Wyniki z `IClimateExtremeDetectionService` z M2-2.
- Parametry z konfiguracji `appsettings.json` dla temperatury, wilgotności i zachmurzenia.

### Wyjście

- Tabela z datą, parametrem, wartością, kierunkiem przekroczenia i progami.
- Komunikat dla pustego wyniku lub dla braku danych wejściowych.
- Opcjonalne filtry po parametrze / kierunku, jeśli nie wydłużą zbytnio zakresu pracy.

### Zachowania graniczne

- Brak ekstremów nie jest błędem i nie powoduje wyjątku.
- `from > to` nie wykonuje obliczeń i zwraca formularz z błędem walidacji.
- Dla `null` lub puste dane wejściowe widok pokazuje pusty stan bez artefaktów UI.
- Sortowanie ma być deterministyczne: rosnąco po dacie, a w razie potrzeby po parametrze.

## Fazy

1. **Kontrakt prezentacji i mapowanie danych**
   - Zdefiniować ViewModel dla tabeli ekstremów.
   - Ustalić własny przepływ kontrolera i zachować kontrakt M2-2.
2. **Widok i UX**
   - Dodać tabelę z nagłówkami, formatem daty i odczytem progów.
   - Wprowadzić stany pusty i komunikaty, bez zbytecznego udziwniania layoutu.
3. **Testy i stabilizacja**
   - Dodać testy kontrolera dla wyników, pustych danych i błędnego zakresu.
   - Sprawdzić, że widok nie łamie istniejącyh ścieżek `ClimateData` / `ClimateCorrelation`.

## Kryteria sukcesu

- Użytkownik widzi wszystkie przekroczenia w jednym miejscu.
- Kierunek przekroczenia jest jednoznacznie rozróżniony i podpisany.
- Widok działa zarówno dla pustego zbioru, jak i dla danych z wieloma ekstremami.
- `dotnet build` i `dotnet test` przechodzą bez regresji.

## Strategia testów

1. Poprawny zakres danych z kilkoma ekstremami.
2. Pusty zbiór i brak wspólnego zakresu dat.
3. `from > to` oraz `from == to`.
4. Sortowanie chronologiczne i mapowanie progów.
5. Widok z komunikatem stanu pustego i bez braków w układzie.

## Non-goals

- Dodawanie nowych typów danych lub segmentacji w bazie.
- Zmiana algorytmu wykrywania ekstremów z M2-2.
- Wykresy, eksport do PDF lub persystowanie wyników w bazie.
- Rozszerzanie zakresu o statystykę agregowaną w tej samej zmienia.

## Komendy weryfikacyjne

- `dotnet build app/10xPV.sln`
- `dotnet test app/Tests/10xPV.Tests/10xPV.Tests.csproj`
