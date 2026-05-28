# Correlation Library Research (M2-1)

Data: 2026-05-28
Zakres: wybór bibliotek do implementacji **M2-1 (Interpolacja liniowa między seriami)** z roadmapy, zgodnych z `context/foundation/tech-stack.md`.

## Kontekst wymagań

Z `context/foundation/roadmap.md`:
- M2-1: *Interpolacja liniowa między seriami*.
- M2-2/M2-3 sugerują dalszą analizę statystyczną i prezentację wyników.

Z `context/foundation/tech-stack.md`:
- ASP.NET Core MVC, C#, .NET, Azure App Service + Azure SQL.
- Preferowane biblioteki: dobrze udokumentowane, typowane, popularne, agent-friendly.

## Rekomendacja główna

## 1) MathNet.Numerics (REKOMENDOWANE)

**Status:** wybrać jako bibliotekę bazową do M2-1.

### Dlaczego pasuje do M2-1
- Posiada gotową interpolację liniową (`Interpolate.Linear`, `LinearSpline.InterpolateSorted`).
- Posiada gotowe metody korelacji/statystyki (`Correlation.Pearson`, macierze korelacji), co ogranicza przyszły koszt M2-2/M2-3.
- Stabilny i dojrzały projekt open source.

### Zgodność ze stackiem
- C# / .NET (wspiera nowoczesne wersje .NET).
- Bez konfliktu z ASP.NET Core MVC, Azure SQL, Azure App Service.
- Licencja MIT (bezpieczna dla projektu wewnętrznego).

### Ryzyko
- Niskie. Biblioteka jest szeroko używana i dobrze udokumentowana.

## Opcjonalny dodatek wydajnościowy

## 2) MathNet.Numerics.MKL.Win-x64 (OPCJONALNE)

**Status:** nie dodawać na start; rozważyć tylko przy realnym bottlenecku wydajnościowym.

### Uzasadnienie
- Przyspiesza część obliczeń numerycznych, ale zwiększa złożoność deploymentu.
- Dla MVP i M2-1 zwykle zbędne.

## Alternatywy odrzucone (na ten etap)

- **Numerics.NET (komercyjne):** technicznie mocne, ale wprowadza koszt/licensing overhead bez potrzeby na MVP.
- **Niszowe biblioteki time-series z małą adopcją:** wyższe ryzyko utrzymania i mniejsza przewidywalność wsparcia.

## Decyzja

Do implementacji M2-1 przyjąć:
- **Must-have:** `MathNet.Numerics`
- **Optional (później):** `MathNet.Numerics.MKL.Win-x64` tylko jeśli pomiary pokażą potrzebę.

## Źródła (research z EXA)

1. Math.NET Numerics — repozytorium + README (zakres: statistics/interpolation, licencja MIT)
   - https://github.com/mathnet/mathnet-numerics
2. Math.NET API — Interpolate
   - https://numerics.mathdotnet.com/api/MathNet.Numerics/Interpolate.htm
3. Math.NET API — Correlation
   - https://numerics.mathdotnet.com/api/MathNet.Numerics.Statistics/Correlation.htm
4. Przegląd alternatyw (w tym komercyjnych i niszowych) przez EXA
   - Numerics.NET docs
   - Syncfusion / ComponentOne / dotnetCHARTING (UI charting; głównie komercyjne)
   - Thinksharp.TimeFlow, Chrono.TimeSeries (niszowe na obecnym etapie)

## Następny krok implementacyjny

W M2-1 dodać pakiet `MathNet.Numerics` do `app/10xPV.csproj` i zaimplementować serwis interpolacji liniowej z testami jednostkowymi (happy path + brakujące punkty + skrajne zakresy czasu).
