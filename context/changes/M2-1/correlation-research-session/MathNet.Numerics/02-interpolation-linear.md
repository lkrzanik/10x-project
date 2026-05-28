# MathNet.Numerics — interpolacja liniowa (M2-1)

## Źródła

- https://numerics.mathdotnet.com/api/MathNet.Numerics/Interpolate.htm
- https://numerics.mathdotnet.com/api/MathNet.Numerics.Interpolation/LinearSpline.htm

## Namespace i typy

- Namespace fabryki interpolacji: `MathNet.Numerics`
- Namespace klasy spline: `MathNet.Numerics.Interpolation`
- Interfejs wynikowy: `IInterpolation`

## API kluczowe dla M2-1

## 1) `Interpolate.Linear(IEnumerable<double> points, IEnumerable<double> values)`

Opis:
- Tworzy **piecewise linear interpolation** na podstawie punktów i wartości.
- Zwraca `IInterpolation`, którego można używać do odczytu wartości w dowolnym punkcie (`Interpolate(t)`).

Uwagi z dokumentacji:
- Dla danych już posortowanych wydajniejszy bywa wariant oparty o `LinearSpline.InterpolateSorted`.

## 2) `LinearSpline.InterpolateSorted(double[] x, double[] y)`

Opis:
- Tworzy interpolację liniową dla danych posortowanych rosnąco po `x`.
- Najbardziej naturalny wybór, gdy pipeline wcześniej porządkuje serię czasową.

Powiązane metody:
- `LinearSpline.Interpolate(...)` — dla danych nieposortowanych.
- `LinearSpline.InterpolateInplace(...)` — działa in-place i może przestawiać kolejność danych.
- `LinearSpline.Interpolate(t)` — oblicza wartość w punkcie `t`.

## Sugerowany kontrakt serwisu w M2-1

Wejście:
- dwie serie czasowe z punktami `(timestamp, value)`
- strategia wyrównania osi czasu

Wyjście:
- seria skorelowana z uzupełnionymi wartościami przez interpolację liniową
- metadane: liczba punktów interpolowanych, odrzuconych, poza zakresem

Błędy/edge cases:
- puste serie
- pojedynczy punkt (brak możliwości interpolacji odcinkowej)
- duplikaty timestampów
- zapytanie o punkt poza zakresem wejściowych `x`

## Wskazówki implementacyjne

- Najpierw sortowanie i deduplikacja `x`.
- Dla punktów poza zakresem rozważyć jawną politykę:
  - brak ekstrapolacji (np. `null`), albo
  - kontrolowana ekstrapolacja.
- Unikać `InterpolateInplace`, jeśli dane wejściowe mają pozostać nienaruszone.
