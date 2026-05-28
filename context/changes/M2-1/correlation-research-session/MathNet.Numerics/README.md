# MathNet.Numerics — dokumentacja do M2-1

Data przygotowania: 2026-05-28
Cel: materiały źródłowe potrzebne do implementacji `M2-1` (interpolacja liniowa między seriami) oraz przygotowania pod kolejne kroki analityczne Milestone 2.

## Zawartość folderu

- `01-installation-and-compatibility.md` — instalacja, pakiety, kompatybilność, licencja.
- `02-interpolation-linear.md` — API interpolacji liniowej (`Interpolate.Linear`, `LinearSpline`).
- `03-correlation-api.md` — API korelacji (`Correlation.Pearson`, warianty).

## Szybka ściąga dla implementacji M2-1

- Pakiet bazowy: `MathNet.Numerics`
- Namespace do interpolacji:
  - `MathNet.Numerics`
  - `MathNet.Numerics.Interpolation`
- Najważniejsze metody dla M2-1:
  - `Interpolate.Linear(points, values)`
  - `LinearSpline.InterpolateSorted(x, y)` (gdy dane posortowane)
  - `IInterpolation.Interpolate(t)`

## Uwaga implementacyjna

Dla serii czasowych klimatycznych warto przed interpolacją:
- wyrównać oś czasu,
- uporządkować punkty rosnąco po czasie,
- jawnie zdecydować jak traktować punkty poza zakresem (brak ekstrapolacji vs kontrolowana ekstrapolacja).
