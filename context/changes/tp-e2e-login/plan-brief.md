# tp-e2e-login - Krotki plan

> Pełny plan: `context/changes/tp-e2e-login/plan.md`

## Co i dlaczego

Budujemy minimalny, wiarygodny sygnał E2E dla ryzyka R7: brak nieautoryzowanego dostępu do widoków chronionych i poprawne wejście do aplikacji po logowaniu. Celem jest stabilny test smoke, który przechodzi przez realny formularz loginu i potwierdza zachowanie redirectów.

## Punkt wyjscia

Projekt ma już aktywne middleware auth, seed konta admin i działający test Playwright logowania. Obecny test kończy się na `/ClimateImport`, a w tej zmianie uzgodniono docelowe lądowanie na Home/Index przy zachowaniu wąskiego zakresu.

## Pozadany stan koncowy

Po realizacji planu mamy spójny kontrakt dokumentacji i testu: anonimowy user trafia na logowanie przy próbie wejścia na trasę chronioną, poprawny login daje dostęp do Home/Index, a asercje są oparte o semantyczne lokatory. Artefakty statusowe i checklisty są gotowe do kolejnych faz rolloutu testów.

## Kluczowe podjete decyzje

| Decyzja | Wybór | Dlaczego (1 zdanie) |
| --- | --- | --- |
| Zakres fazy | Tylko pozytywny login + redirect | Najszybszy i najtańszy sygnał dla R7 bez rozszerzania scope o timeout i negatywne ścieżki. |
| Trasa docelowa po loginie | Home/Index | Upraszcza scenariusz i redukuje zależność od danych domenowych widoków importu. |
| Strategia locatorów | `getByRole` / `getByLabel` | Zwiększa stabilność testu i zgodność z zasadami dostępności. |
| Przygotowanie sesji | Login przez UI w każdym teście | Utrzymuje niezależność testu i sprawdza faktyczny przepływ użytkownika. |

## Zakres

**W zakresie:**
- Ujednolicenie cookbooka i kontraktu fazy 8 dla tp-e2e-login.
- Dostosowanie testu E2E logowania do Home/Index.
- Stabilizacja locatorów i kryteriów akceptacyjnych dla R7.

**Poza zakresem:**
- Timeout sesji i polityki wygaszania.
- Rozbudowa o scenariusze negatywnego loginu i pełny cykl auth.
- Zmiany architektury Identity, migracje, CI pipeline.

## Architektura / Podejscie

Jeden test browser-flow uruchamiany przez Playwright `webServer` wymusza wejście anonimowe na trasę chronioną, przechodzi przez formularz loginu i kończy asercjami stanu zalogowanego. Plan dzieli pracę na trzy krótkie fazy: kontrakt, implementacja testu, domknięcie artefaktów.

## Fazy w skrocie

| Faza | Co dostarcza | Kluczowe ryzyko |
| --- | --- | --- |
| 1. Ujednolicenie kontraktu | Spójny opis scenariusza i asercji w artefaktach | Rozjazd dokumentacji i implementacji |
| 2. Stabilna implementacja E2E | Zielony test login smoke dla Home/Index | Flaky lokatory lub kruchy redirect |
| 3. Domknięcie fazy | Spójne statusy/checklisty i gotowość do kolejnych etapów | Niedomknięte artefakty po wdrożeniu |

**Wymagania wstępne:** działające lokalnie `dotnet run`, seed admin (`AdminEmail`, `AdminPassword`), Playwright dependencies.
**Szacowany wysiłek:** ~1 sesja, 3 krótkie fazy.

## Otwarte ryzyka i zalozenia

- Założenie: Home/Index pozostaje trasą chronioną przez globalny filtr auth.
- Ryzyko: zmiana tekstów UI może wymagać aktualizacji locatorów opartych o role/label.
- Ryzyko: lokalne środowisko bez poprawnych zmiennych admin może dać fałszywie negatywny wynik testu.

## Kryteria sukcesu (podsumowanie)

- Test E2E logowania przechodzi lokalnie i pokazuje poprawny redirect + dostęp po autoryzacji.
- Dokumentacja fazy 8 i plan wdrożenia opisują ten sam kontrakt scenariusza.
- Zakres pozostaje celowo wąski: tylko sygnał R7 dla pozytywnego loginu.
