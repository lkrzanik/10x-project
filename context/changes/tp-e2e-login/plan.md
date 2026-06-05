# Plan wdrozenia tp-e2e-login

## Przeglad

Wdrażamy minimalny i stabilny plan testu E2E logowania dla ryzyka R7. Zakres obejmuje tylko pozytywny scenariusz: anonimowy użytkownik trafia na logowanie przy próbie wejścia na chronioną trasę, po poprawnym logowaniu uzyskuje dostęp do widoku docelowego Home/Index.

## Analiza stanu obecnego

Globalna autoryzacja jest już aktywna, więc kontrolery wymagają uwierzytelnienia przez filtr i middleware. Istnieje działający kontroler logowania oraz seed konta administratora z konfiguracji. Infrastruktura Playwright jest skonfigurowana i zawiera jeden test E2E, ale bieżący scenariusz używa docelowej trasy /ClimateImport, podczas gdy dla tej zmiany uzgodniono Home/Index.

## Pozadany stan koncowy

Po zakończeniu planu projekt ma jednoznaczny, udokumentowany i testowalny kontrakt E2E dla fazy tp-e2e-login: test przechodzi dla przepływu login przez UI, używa semantycznych locatorów dostępności i potwierdza ochronę trasy oraz dostęp po autoryzacji dla Home/Index.

### Kluczowe odkrycia:

- Globalny wymóg uwierzytelnienia jest dodany przez filtr w app startup: `app/Program.cs:43`.
- Kolejność middleware auth jest poprawna (`UseAuthentication` przed `UseAuthorization`): `app/Program.cs:77`.
- Seed konta admin zależy od `AdminEmail` i `AdminPassword`: `app/Data/SeedData.cs:9`.
- Istniejący test E2E używa scenariusza redirect -> `/ClimateImport`: `app/Tests/e2e/login.spec.ts:6`.
- Konfiguracja Playwright ma aktywny `webServer` dla `dotnet run`: `playwright.config.ts:14`.

## Czego NIE robimy

- Nie implementujemy timeoutu sesji ani polityk wygasania cookies.
- Nie dodajemy nowych scenariuszy auth poza uzgodnionym scenariuszem pozytywnym.
- Nie migrujemy testów na model preauth `auth.json`.
- Nie zmieniamy architektury ASP.NET Identity ani modelu użytkownika.

## Podejscie do implementacji

Najpierw ujednolicamy kontrakt testu i dokumentacji fazy 8 (docelowa trasa Home/Index). Następnie aktualizujemy test Playwright tak, aby był niezależny, oparty o logowanie przez UI i semantyczne locatory (`getByRole` / `getByLabel`). Na końcu uruchamiamy weryfikacje automatyczne i przygotowujemy checklistę ręczną.

## Krytyczne szczegoly implementacji

### Sekwencjonowanie stanu

Asercje po zalogowaniu muszą być wykonywane po nawigacji wynikającej z `LocalRedirect(returnUrl ?? "/")`, bo wejście na chronioną trasę ustawia kontekst przekierowania przed logowaniem (`app/Controllers/AccountController.cs:30`). W praktyce test najpierw wymusza redirect anonimowego użytkownika na `/Account/Login`, a dopiero potem weryfikuje finalny dostęp po submit.

## Faza 1: Ujednolicenie kontraktu scenariusza E2E

### Przeglad

Faza ustala jeden spójny kontrakt funkcjonalny między cookbookiem test-planu i planowaną implementacją testu.

### Wymagane zmiany:

#### 1. Kontrakt testowy w dokumentacji

**Plik**: `context/foundation/test-plan.md`

**Cel**: Uzgodnić opis fazy 8 z decyzją planistyczną o trasie docelowej Home/Index po poprawnym logowaniu.

**Kontrakt**: W sekcji 6.8 scenariusz i asercje końcowe wskazują Home/Index jako docelowy widok po autoryzacji, a zakres pozostaje ograniczony do pozytywnego loginu i ochrony trasy.

#### 2. Definicja asercji sygnałowych

**Plik**: `context/changes/tp-e2e-login/plan.md`

**Cel**: Zamrozić minimalny zestaw sygnałów akceptacyjnych, aby implementacja nie rozszerzała zakresu.

**Kontrakt**: Asercje obejmują: redirect anonimowego użytkownika do `/Account/Login`, poprawny submit formularza loginu i widoczność elementów zalogowanego stanu na stronie docelowej.

### Kryteria sukcesu:

#### Weryfikacja automatyczna:

- Dokument `test-plan.md` zawiera spójny opis fazy 8 z trasą Home/Index.
- `dotnet build app/10xPV.sln` przechodzi bez błędów.

#### Weryfikacja ręczna:

- Odczyt sekcji 6.8 potwierdza, że zakres nie obejmuje negatywnego loginu ani timeoutu sesji.

**Uwaga implementacyjna**: Po zakończeniu tej fazy i pomyślnym przejściu wszystkich automatycznych weryfikacji, zatrzymaj się tutaj, aby uzyskać ręczne potwierdzenie od człowieka, że testy ręczne zakończyły się sukcesem, zanim przejdziesz do następnej fazy.

---

## Faza 2: Stabilna implementacja testu Playwright

### Przeglad

Faza wdraża test zgodny z kontraktem fazy 1, zachowując niezależność i standard locatorów.

### Wymagane zmiany:

#### 1. Aktualizacja scenariusza logowania

**Plik**: `app/Tests/e2e/login.spec.ts`

**Cel**: Dostosować scenariusz E2E do uzgodnionej trasy docelowej Home/Index i stabilnych asercji stanu po logowaniu.

**Kontrakt**: Test wykonuje ścieżkę: wejście bez sesji na chroniony endpoint, redirect do `/Account/Login`, logowanie poprawnymi danymi i asercja finalnego URL/elementów po zalogowaniu dla Home/Index.

#### 2. Ustandaryzowanie locatorów

**Plik**: `app/Tests/e2e/login.spec.ts`

**Cel**: Wyeliminować kruche wybory pól formularza oparte o indeksy.

**Kontrakt**: Preferowane lokatory to `getByRole` i `getByLabel`; `getByTestId` tylko gdy semantyczne lokatory byłyby niejednoznaczne.

#### 3. Spójność uruchamiania lokalnego

**Plik**: `package.json`

**Cel**: Zachować czytelne i jednoznaczne polecenie uruchomienia pojedynczego testu loginu.

**Kontrakt**: Skrypt `test:e2e:login` wskazuje właściwy plik testowy i pozostaje zgodny z konfiguracją `testDir` w `playwright.config.ts`.

### Kryteria sukcesu:

#### Weryfikacja automatyczna:

- `npm run test:e2e:login` przechodzi lokalnie.
- `npx playwright test app/Tests/e2e/login.spec.ts` przechodzi lokalnie.
- Test nie zawiera `waitForTimeout` ani selektorów CSS/XPath.

#### Weryfikacja ręczna:

- Raport testu potwierdza redirect na logowanie bez sesji i poprawny dostęp po loginie.
- Treść asercji jest czytelna i jednoznacznie mapuje się do ryzyka R7.

**Uwaga implementacyjna**: Po zakończeniu tej fazy i pomyślnym przejściu wszystkich automatycznych weryfikacji, zatrzymaj się tutaj, aby uzyskać ręczne potwierdzenie od człowieka, że testy ręczne zakończyły się sukcesem, zanim przejdziesz do następnej fazy.

---

## Faza 3: Domkniecie fazy i gotowosc do implementacji kolejnych ryzyk

### Przeglad

Faza porządkuje artefakty po wdrożeniu testu, tak aby wynik był gotowy do dalszych etapów rolloutu test-planu.

### Wymagane zmiany:

#### 1. Aktualizacja statusu i checklist

**Plik**: `context/foundation/test-plan.md`

**Cel**: Utrzymać zgodność statusu fazy 8 ze stanem implementacji.

**Kontrakt**: Wiersz fazy `tp-e2e-login` odzwierciedla rzeczywisty postęp po wdrożeniu; cookbook 6.8 zawiera finalną komendę i zakres sygnału.

#### 2. Utrwalenie postępu zmiany

**Plik**: `context/changes/tp-e2e-login/change.md`

**Cel**: Zachować poprawny status i datę aktualizacji po realizacji planu.

**Kontrakt**: `status` i `updated` odzwierciedlają postęp implementacji zgodnie z workflow zmian.

### Kryteria sukcesu:

#### Weryfikacja automatyczna:

- `dotnet test app/Tests/10xPV.Tests/10xPV.Tests.csproj` przechodzi i nie pokazuje regresji backendowej.
- `npm run test:e2e:login` pozostaje zielone po aktualizacji artefaktów.

#### Weryfikacja ręczna:

- Checklisty w artefaktach testowych i zmiany są spójne z faktycznym wynikiem implementacji.

**Uwaga implementacyjna**: Po zakończeniu tej fazy i pomyślnym przejściu wszystkich automatycznych weryfikacji, zatrzymaj się tutaj, aby uzyskać ręczne potwierdzenie od człowieka, że testy ręczne zakończyły się sukcesem, zanim przejdziesz do następnej fazy.

## Strategia testowania

### Testy jednostkowe:

- Brak nowych testów jednostkowych w tej zmianie.
- Wykonanie istniejących testów backendowych jako osłona przed regresją.

### Testy integracyjne:

- Brak nowych testów integracyjnych HTTP w tej zmianie.
- Sygnał auth end-to-end realizowany przez Playwright browser flow.

### Kroki testowania ręcznego:

1. Uruchom test loginu i sprawdź, że przy wejściu bez sesji następuje przekierowanie do `/Account/Login`.
2. Zweryfikuj, że po submit poprawnych danych użytkownik ląduje na Home/Index.
3. Potwierdź, że elementy stanu zalogowanego (np. przycisk wylogowania) są widoczne.

## Uwagi dotyczace wydajnosci

Zakres to pojedynczy test smoke, więc koszt wykonania jest niski. Największy wpływ na czas ma start `dotnet run` przez `webServer`; nie wprowadzamy dodatkowych narzutów typu globalne fixture z resetem bazy.

## Uwagi dotyczace migracji

Brak migracji danych i brak zmian schematu. Zmiana dotyczy kontraktu testowego i artefaktów planu testów.

## Referencje

- Powiązany plan testów: `context/foundation/test-plan.md`
- Konfiguracja auth middleware: `app/Program.cs:43`
- Logowanie użytkownika: `app/Controllers/AccountController.cs:24`
- Seed administratora: `app/Data/SeedData.cs:9`
- Konfiguracja Playwright: `playwright.config.ts:5`
- Istniejący test E2E: `app/Tests/e2e/login.spec.ts:6`

## Progress

> Konwencja: `- [ ]` oczekujące, `- [x]` wykonane. Dodaj ` — <commit sha>`, gdy krok zostanie zrealizowany. Nie zmieniaj nazw tytułów kroków.

### Faza 1: Ujednolicenie kontraktu scenariusza E2E

#### Automatyczne

- [x] 1.1 Dokument `test-plan.md` spójny z trasą Home/Index
- [x] 1.2 `dotnet build app/10xPV.sln` przechodzi

#### Ręczne

- [x] 1.3 Zakres fazy 8 potwierdzony jako tylko pozytywny login

### Faza 2: Stabilna implementacja testu Playwright

#### Automatyczne

- [x] 2.1 `npm run test:e2e:login` przechodzi
- [x] 2.2 `npx playwright test app/Tests/e2e/login.spec.ts` przechodzi
- [x] 2.3 Brak `waitForTimeout` i brak CSS/XPath w teście

#### Ręczne

- [x] 2.4 Redirect do logowania bez sesji i dostęp po loginie potwierdzone
- [x] 2.5 Asercje czytelnie mapują się do ryzyka R7

### Faza 3: Domkniecie fazy i gotowosc do implementacji kolejnych ryzyk

#### Automatyczne

- [ ] 3.1 `dotnet test app/Tests/10xPV.Tests/10xPV.Tests.csproj` przechodzi
- [ ] 3.2 `npm run test:e2e:login` pozostaje zielone po domknięciu artefaktów

#### Ręczne

- [ ] 3.3 Statusy i checklisty artefaktów są spójne z wynikiem implementacji
