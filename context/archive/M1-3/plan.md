# Plan implementacji — M1-3 UI importu CSV

## 1) Cel zmiany

Dostarczyć ekran importu CSV, który pozwala zalogowanemu użytkownikowi:
- przesłać plik CSV,
- wskazać typ schematu (`Sensor` lub `Weather`),
- uruchomić istniejący parser (`ICsvImportService`),
- zobaczyć podsumowanie importu i listę błędów per wiersz.

Wynik ma być gotowy do rozszerzenia o zapis do bazy i podgląd tabelaryczny w M1-4.

## 2) Zakres / poza zakresem

### W zakresie
- Nowy endpoint MVC do obsługi uploadu (GET + POST, anty-forgery).
- ViewModel-e dla formularza i wyniku importu.
- Widok Razor z formularzem uploadu, wyborem schematu, sekcją podsumowania i tabelą błędów.
- Obsługa podstawowego stanu „w trakcie” po stronie UI (blokada submit + komunikat).
- Integracja z `ICsvImportService` i mapowanie wyniku parsera na model widoku.

### Poza zakresem
- Upload chunkowany / realtime progress z backendu (SignalR, polling).
- Asynchroniczne kolejki importu i job processing.
- Rozbudowane zarządzanie historią importów.
- Finalna persistencja rekordów do DB, jeśli wymaga zmiany kontraktu parser→encje (ryzyko opisane niżej).

## 3) Zależności i bramki

- **Brama wejścia:**
  - E0-2: użytkownik jest uwierzytelniany (globalny `AuthorizeFilter` już aktywny).
  - M1-2: parser CSV i kontrakty błędów są dostępne.
- **Brama wyjścia:**
  - Użytkownik może wykonać import CSV z pełnym feedbackiem.
  - Kontrakt UI jest stabilny i gotowy pod domknięcie persistence + M1-4.

## 4) Kontrakty plików (planowane)

- `app/Controllers/ClimateImportController.cs`  
  Nowy kontroler MVC dla ekranu importu (`Index` GET/POST).
- `app/Models/ClimateImport/ClimateImportFormViewModel.cs`  
  Dane wejściowe formularza uploadu (plik + typ schematu).
- `app/Models/ClimateImport/ClimateImportSummaryViewModel.cs`  
  Podsumowanie importu (total/valid/invalid + status).
- `app/Models/ClimateImport/ClimateImportErrorViewModel.cs`  
  Reprezentacja błędu wiersza do renderowania w tabeli.
- `app/Models/ClimateImport/ClimateImportPageViewModel.cs`  
  Model strony łączący formularz i wynik.
- `app/Views/ClimateImport/Index.cshtml`  
  UI importu: formularz, komunikaty, podsumowanie, lista błędów.
- `app/Views/Shared/_Layout.cshtml`  
  Dodanie linku nawigacyjnego do ekranu importu.
- `app/Tests/10xPV.Tests/Controllers/ClimateImportControllerTests.cs`  
  Testy kontrolera: happy path + błędy wejścia.
- `app/Services/ClimateImport/IClimateImportOrchestrator.cs` + `ClimateImportOrchestrator.cs`  
  Orkiestracja importu (`parser -> persistence adapter`) bez zmiany kontraktu kontrolera/UI.
- `app/Services/ClimateImport/IClimateImportPersistenceAdapter.cs` + `NoOpClimateImportPersistenceAdapter.cs`  
  Punkt rozszerzenia pod zapis poprawnych rekordów do DB w M1-4.

## 4.1) Kontrakt wejścia/wyjścia (MVP)

### Input (POST)
- `IFormFile File` — wymagany, rozszerzenie `.csv`.
- `CsvSchemaType SchemaType` — wymagany (`Sensor` / `Weather`).

### Walidacja wejścia (MVP)
- Akceptowane rozszerzenie: `.csv` (case-insensitive).
- `Content-Type` traktowany pomocniczo (niezaufany), główna walidacja po rozszerzeniu i treści parsowanej.
- Plik pusty (`Length == 0`) zwraca walidację użytkownika bez wywołania parsera.
- Limit rozmiaru pliku: 10 MB (MVP, pojedynczy upload synchroniczny).
- Jeden plik na żądanie.

### Output (UI)
- Podsumowanie:
  - `TotalRows`
  - `ValidRows`
  - `InvalidRows`
- Lista błędów:
  - `LineNumber`
  - `Field`
  - `Code`
  - `Message`

### Error modes kontrolera
- Błąd walidacji formularza: zwracany ten sam widok z `ModelState`.
- `OperationCanceledException`: neutralny komunikat „Import anulowany”, bez stack trace.
- Nieoczekiwany wyjątek parsera/IO: bezpieczny komunikat użytkownika + log po stronie serwera.

## 5) Fazy realizacji

## Faza 1 — Endpoint i model strony importu
- Utworzyć kontroler `ClimateImportController` z akcją `Index` (GET/POST).
- Dodać ViewModel-e strony i walidację formularza.
- Dodać routing przez konwencję MVC (bez attribute routing).

### Kryteria akceptacji F1
- Strona importu renderuje się dla zalogowanego użytkownika.
- Submit bez pliku lub bez schematu zwraca walidację.
- Kompilacja przechodzi.

## Faza 2 — Integracja parsera i prezentacja wyników
- Podpiąć `ICsvImportService.ImportAsync` w akcji POST.
- Zmapować `CsvImportResult` do ViewModel-u strony.
- Wyświetlać podsumowanie importu oraz listę błędów per wiersz.
- Dodać podstawowy UX „w trakcie importu” (disable button + komunikat).

### Kryteria akceptacji F2
- Poprawny plik wyświetla poprawne metryki (total/valid/invalid).
- Błędy parsera są widoczne i czytelne dla użytkownika.
- Jeden błędny wiersz nie blokuje podsumowania pozostałych.

## Faza 3 — Gotowość integracyjna pod persistence i M1-4
- Ustalić punkt rozszerzenia dla zapisu rekordów do bazy (adapter/orchestrator).
- Udokumentować decyzję dot. rozbieżności kontraktów parser↔encje.
- Dodać testy kontrolera (happy path + walidacja + błąd parsera).

### Kryteria akceptacji F3
- Testy kontrolera przechodzą.
- Build i testy regresji przechodzą.
- Planowany interfejs pod M1-4 nie wymaga łamania kontraktu UI.

## 6) Ryzyka i decyzje robocze

- **Rozbieżność modelu parsera i encji DB:**
  - Obecne kontrakty parsera są zbliżone do encji (`Temperature`, `Humidity`, `CloudCover`), ale nadal istnieje warstwa translacji domenowej (m.in. `DateTime` -> `DateTimeOffset`, strategia identyfikatorów, reguły deduplikacji i decyzje transakcyjne).
  - Decyzja dla M1-3: wprowadzamy warstwę `IClimateImportOrchestrator` oraz `IClimateImportPersistenceAdapter` (na razie `NoOp`), dzięki czemu `ClimateImportController` i kontrakt UI pozostają stabilne, a mapowanie/persistencja mogą zostać domknięte w M1-4 bez łamania endpointu.
- **Duże pliki:**
  - W MVP brak dedykowanego stronicowania błędów; możliwe ograniczenie liczby renderowanych błędów (np. top 200) jeśli UX tego wymaga.

## 6.1) Edge cases do pokrycia w implementacji/testach

- Brak pliku w formularzu.
- Plik 0 B.
- Niepoprawne rozszerzenie (np. `.txt`).
- Poprawny nagłówek + same puste wiersze.
- Mieszane wiersze poprawne/błędne (częściowy sukces).
- Bardzo duża liczba błędów (czytelność UI, ewentualny limit renderu).
- Anulowanie requestu przez użytkownika.
- Wyjątek I/O przy odczycie strumienia.

## 7) Weryfikacja

Uruchomienia lokalne (Windows/PowerShell):
- `cd app`
- `dotnet build`
- `dotnet test`

## 8) Definition of Done

- Dostępny ekran importu CSV z formularzem i podsumowaniem.
- Import uruchamia parser i pokazuje pełny feedback walidacyjny.
- Obsłużone stany walidacji formularza oraz komunikat „w trakcie”.
- Testy kontrolera i regresja build/test są zielone.

## Progress

- [x] F1 — Endpoint i model strony importu
  - ✅ Zrealizowano: kontroler `ClimateImportController` (GET/POST), walidacja uploadu (brak pliku, 0 B, rozszerzenie `.csv`, limit 10 MB, brak schematu), modele strony importu, link nawigacyjny i testy kontrolera.
  - ✅ Weryfikacja: `dotnet build` oraz `dotnet test` (9/9) przechodzą.
  - 🔖 Commit: `198cac3`
- [x] F2 — Integracja parsera i prezentacja wyników
  - ✅ Zrealizowano: integrację `ICsvImportService.ImportAsync` w `POST Index`, mapowanie `CsvImportResult` → `ClimateImportPageViewModel`, obsługę `OperationCanceledException` i bezpieczny komunikat dla wyjątków nieoczekiwanych oraz status importu (`sukces` / `częściowy sukces` / `błędy`).
  - ✅ UI: `Views/ClimateImport/Index.cshtml` pokazuje metryki + alert statusu i listę błędów per wiersz; UX „w trakcie importu” (disable submit + komunikat) aktywny.
  - ✅ Weryfikacja: `dotnet build` (PASS, ostrzeżenia NU1903 istniejące), `dotnet test` (11/11 PASS), brak błędów diagnostycznych w workspace.
  - 🔖 Commit: _(pending)_
- [x] F3 — Gotowość integracyjna pod persistence i M1-4
  - ✅ Zrealizowano: dodano `IClimateImportOrchestrator` + `ClimateImportOrchestrator` oraz punkt rozszerzenia `IClimateImportPersistenceAdapter` (implementacja `NoOp`), co stabilizuje kontrakt kontrolera/UI pod M1-4.
  - ✅ Decyzja integracyjna: mapowanie parser→encje oraz zapis do DB będą domknięte przez adapter persistence; obecna warstwa orchestratora izoluje UI od zmian modelu trwałości.
  - ✅ Testy: rozszerzono `ClimateImportControllerTests` o scenariusz nieoczekiwanego wyjątku (błąd parsera/IO) obok happy path i walidacji.
  - 🔖 Commit: `e70d979`

<!-- Updated by /10x-implement: phase status, commit SHA -->