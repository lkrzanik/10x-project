# Research — M2-1 Correlation Research Session

Data: 2026-05-28  
Zmiana: `M2-1`  
Cel: Zweryfikować, czy zaproponowana biblioteka jest właściwa do realizacji **Milestone 2 / M2-1 (Interpolacja liniowa między seriami)**.

## Executive summary

**Werdykt: GO** — zaproponowana biblioteka **`MathNet.Numerics`** jest właściwym wyborem dla M2-1 w tym repozytorium.

Powody:
- pokrywa dokładnie potrzebę M2-1 (interpolacja liniowa) przez `Interpolate.Linear` / `LinearSpline`;
- dobrze pasuje do aktualnej architektury ASP.NET Core + EF Core (serwisy domenowe, DI, testy xUnit);
- ma niski koszt integracji (1 pakiet NuGet + nowy serwis + testy);
- nie wymaga zmian w persistence modelu na start.

## Co sprawdzono w kodzie aplikacji (badania wewnętrzne)

### 1) Stan danych i modeli

- Istnieją dwa strumienie danych: `SensorReading` i `WeatherReading`:
  - `app/Models/SensorReading.cs`
  - `app/Models/WeatherReading.cs`
- Oba mają wspólną oś czasu (`Timestamp: DateTimeOffset`) i zestaw pól numerycznych (`Temperature`, `Humidity`), co naturalnie wspiera interpolację między seriami.

### 2) Jakość i normalizacja wejścia

- Walidacja danych istnieje i ogranicza skrajne/wadliwe wartości:
  - `app/Services/ClimateDataValidator.cs`
- Import utrwala timestampy w UTC i deduplikuje po `Timestamp`:
  - `app/Services/ClimateImport/ClimateImportPersistenceAdapter.cs`
  - `app/Services/DeduplicationService.cs`

Wniosek: pipeline już dziś zapewnia dobre prewarunki pod interpolację (spójny czas + redukcja duplikatów).

### 3) Architektura i punkty integracji

- Kontroler danych klimatycznych (`app/Controllers/ClimateDataController.cs`) obecnie renderuje surowe serie; brak logiki korelacji/interpolacji.
- DI jest zorganizowane przez interfejsy i serwisy (`app/Program.cs`), więc nowy serwis M2-1 można dodać bez naruszeń istniejącego wzorca.
- Zestaw testowy jest gotowy (xUnit + EF InMemory):
  - `app/Tests/10xPV.Tests/10xPV.Tests.csproj`
  - przykładowe testy kontrolera: `app/Tests/10xPV.Tests/Controllers/ClimateDataControllerTests.cs`

## Czy proponowana biblioteka jest właściwa?

## Biblioteka oceniana

- `MathNet.Numerics` (zgodnie z:
  - `context/changes/M2-1/correlation-research-session/correlation-library-research.md`
  - `context/changes/M2-1/correlation-research-session/MathNet.Numerics/README.md`
  - `context/changes/M2-1/correlation-research-session/MathNet.Numerics/02-interpolation-linear.md`)

## Ocena dopasowania do M2-1

### Pokrycie wymagań funkcjonalnych

- M2-1 wymaga interpolacji liniowej między seriami.
- `MathNet.Numerics` daje gotowe API dla interpolacji liniowej (`Interpolate.Linear`, `LinearSpline.InterpolateSorted`) — pełne pokrycie.

### Pokrycie wymagań niefunkcjonalnych

- Zgodność technologiczna z C#/.NET: tak.
- Dojrzałość i utrzymanie: wysokie.
- Ryzyko vendor lock-in: niskie (API można opakować własnym interfejsem domenowym).

### Koszt integracji

Niski:
1. dodać pakiet `MathNet.Numerics` do `app/10xPV.csproj`,
2. dodać serwis domenowy interpolacji (np. `IClimateCorrelationService` + implementacja),
3. zarejestrować serwis w `app/Program.cs`,
4. dodać testy jednostkowe.

## Minimalny kontrakt implementacyjny dla M2-1

Wejście:
- dwie serie punktów czasowych `(timestamp, value)` dla wybranego parametru (np. temperatura).

Wyjście:
- seria wyrównana do wspólnej osi czasu z wartościami interpolowanymi,
- metadane jakości (liczba interpolacji, liczba punktów odrzuconych, punkty poza zakresem).

Tryby błędów:
- pusta seria,
- pojedynczy punkt w serii,
- zapytania poza zakresem,
- brak wartości (`null`) w polach numerycznych.

Kryterium sukcesu M2-1:
- dla wspólnego zakresu czasu zwracane są stabilne, deterministyczne wartości interpolowane,
- zachowanie poza zakresem jest jawnie zdefiniowane (rekomendacja: brak ekstrapolacji i zwracanie `null`).

## Edge cases do obowiązkowego pokrycia testami

1. Puste dane wejściowe po jednej stronie.
2. Tylko jeden punkt w serii (brak odcinka do interpolacji).
3. Nierównomierne odstępy czasu między pomiarami.
4. Punkt docelowy poza min/max czasu serii źródłowej.
5. Duplikaty czasu i wartości `null` na wejściu.

## Ryzyka i decyzje

### Ryzyka

- Największe ryzyko nie dotyczy biblioteki, tylko polityki domenowej:
  - czy dopuszczać ekstrapolację,
  - jak raportować jakość interpolacji,
  - który strumień jest referencyjny przy wyrównaniu osi czasu.

### Decyzja

- **Przyjąć `MathNet.Numerics` jako bibliotekę bazową dla M2-1.**
- **Nie dodawać** `MathNet.Numerics.MKL.Win-x64` na MVP (rozważyć tylko przy potwierdzonym bottlenecku wydajnościowym).

## Następny krok (gotowy do /10x-plan)

- Zaplanować implementację M2-1 w 2 fazach:
  1. serwis interpolacji + testy jednostkowe,
  2. endpoint/widok korelacji wykorzystujący serwis.
