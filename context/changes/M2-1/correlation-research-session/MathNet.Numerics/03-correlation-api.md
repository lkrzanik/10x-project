# MathNet.Numerics — API korelacji (pod M2+)

## Źródła

- https://numerics.mathdotnet.com/api/MathNet.Numerics.Statistics/Correlation.htm

## Namespace i typ

- Namespace: `MathNet.Numerics.Statistics`
- Typ: `Correlation`

## Najważniejsze metody

- `Correlation.Pearson(IEnumerable<double> dataA, IEnumerable<double> dataB)`
  - Współczynnik korelacji Pearsona między dwoma zbiorami danych.

- `Correlation.WeightedPearson(IEnumerable<double> dataA, IEnumerable<double> dataB, IEnumerable<double> weights)`
  - Wersja ważona współczynnika Pearsona.

- `Correlation.Spearman(IEnumerable<double> dataA, IEnumerable<double> dataB)`
  - Korelacja rang Spearmana.

- `Correlation.PearsonMatrix(...)` / `Correlation.SpearmanMatrix(...)`
  - Macierze korelacji dla wielu wektorów.

- `Correlation.Auto(...)`
  - Autokorelacja (ACF).

## Znaczenie dla Milestone 2

Chociaż `M2-1` skupia się na interpolacji liniowej, ten zestaw metod:
- umożliwia walidację jakości skorelowania serii po interpolacji,
- przygotowuje grunt pod późniejsze analizy w `M2-2` / `M2-3`.

## Wskazówki jakości danych

Przed liczeniem korelacji:
- upewnić się, że serie są wyrównane w czasie,
- usunąć lub jawnie obsłużyć `null`/NaN,
- sprawdzić minimalną liczebność próbki.

## Minimalny przykład koncepcyjny (bez kodu projektu)

1. Po interpolacji uzyskaj dwie listy `double` o tej samej długości.
2. Użyj `Correlation.Pearson(seriesA, seriesB)`.
3. Zapisz wynik w modelu widoku analitycznym jako metrykę jakości korelacji.
