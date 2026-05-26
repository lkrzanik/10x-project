# M1-2 — plan-brief

## Cel
Parser CSV dla `Sensor` i `Weather` z walidacją i błędami per wiersz, gotowy do podpięcia pod UI importu.

## Fazy
1. **Kontrakty**: `ICsvImportService`, DTO wyniku, DTO błędów, enum schematu.
2. **Implementacja**: parse + walidacja nagłówków i pól dla 2 formatów.
3. **Testy**: ścieżki pozytywne/negatywne i podsumowanie metryk importu.

## Kryteria sukcesu
- Obsługa 2 predefiniowanych formatów.
- Błędy per wiersz bez zatrzymania całego importu.
- `dotnet build` i `dotnet test` przechodzą.
- Kontrakt stabilny dla M1-3/M1-1.

## Komendy weryfikacyjne
- `cd app`
- `dotnet build`
- `dotnet test`

## Progress
- [ ] F1
- [ ] F2
- [ ] F3