# MathNet.Numerics — instalacja i kompatybilność

## Źródła

- https://github.com/mathnet/mathnet-numerics
- https://numerics.mathdotnet.com/Packages
- https://www.nuget.org/packages/mathnet.numerics/

## Kluczowe informacje

- `MathNet.Numerics` to open-source'owa biblioteka obliczeń numerycznych dla .NET.
- Licencja: **MIT**.
- Zakres funkcjonalny obejmuje m.in. statystykę, interpolację, regresję i algebrę liniową.

## Instalacja (NuGet)

Rekomendowany pakiet bazowy:
- `MathNet.Numerics`

Opcjonalne (niepotrzebne na start M2-1):
- `MathNet.Numerics.MKL.Win-x64` — provider wydajnościowy.

## Kompatybilność

Według dokumentacji/README:
- .NET 5.0+
- .NET Framework 4.6.1+
- .NET Standard 2.0+

W kontekście projektu `10xPV` (`net10.0`) pakiet jest zgodny koncepcyjnie ze stackiem aplikacji ASP.NET Core MVC.

## Minimalna decyzja dla M2-1

Do implementacji M2-1 użyć wyłącznie:
- `MathNet.Numerics`

Dodatkowe providery (MKL/OpenBLAS) dodawać tylko po pomiarach wydajności i potwierdzeniu bottlenecku.
