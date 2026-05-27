# /10x-impl-review — M1-2 (2026-05-27)

## Zakres przeglądu

Źródła:
- `context/changes/M1-2/plan.md`
- `context/changes/M1-2/change.md`
- `context/changes/M1-2/integration-note.md`
- Implementacja w `app/Services/Csv/*`
- Testy w `app/Tests/10xPV.Tests/Services/Csv/CsvImportServiceTests.cs`

Przegląd obejmuje: zgodność z planem, dyscyplinę zakresu, jakość, bezpieczeństwo i gotowość integracyjną.

## Podsumowanie

Wynik: **REVIEW PASS WITH TRIAGE**

- Implementacja parsera CSV działa i jest przetestowana (happy path + błędy per wiersz).
- Bramy jakości lokalnie zielone (`build`, `test`, brak błędów w Problems).
- Zidentyfikowano 1 istotną rozbieżność kontraktową względem zamrożonego planu M1-2.

## Findings

### 1) Rozbieżność kontraktu nagłówków/DTO względem planu (zakres funkcjonalny)

- **Severity:** Medium
- **Impact now:** High
- **Kategoria:** Zgodność z planem / stabilność kontraktu
- **Status triage:** **fix (teraz)**

**Plan M1-2 (zamrożone nagłówki MVP):**
- `Sensor`: `Timestamp, SensorId, Value, Unit`
- `Weather`: `Timestamp, TemperatureC, WindSpeedMs, IrradianceWm2`

**Stan w kodzie:**
- `Sensor`: `Timestamp, Temperature, Humidity`
- `Weather`: `Timestamp, Temperature, Humidity, CloudCover`
- Potwierdzenie: `app/Services/Csv/CsvImportService.cs`, `SensorCsvRow.cs`, `WeatherCsvRow.cs`, testy parsera.

**Ryzyko:**
- Integracja M1-3/M1-1 może oprzeć się na innym modelu danych niż uzgodniony kontrakt planu.
- Wysokie prawdopodobieństwo kosztownej korekty w kolejnych krokach (UI/mapowania/testy).

**Rekomendacja naprawy:**
- Albo dostosować parser+DTO+testy do nagłówków zamrożonych w `plan.md`,
- albo formalnie zaktualizować plan/kontrakt i notatki integracyjne, jeśli nowy model jest świadomą decyzją domenową.

---

## Zgodność z planem (pozostałe kryteria)

✅ Spełnione:
- Interfejs parsera niezależny od MVC/EF (`Stream + schemaType`).
- Stabilny katalog kodów błędów istnieje i jest używany (`CsvErrorCodes`).
- Błędy zbierane per wiersz bez przerywania przetwarzania.
- Obsługa pustych wierszy i metryk (`TotalRows`, `ValidRows`, `InvalidRows`).
- Testy pokrywają ścieżki pozytywne i negatywne.

⚠️ Warunkowo spełnione:
- „Stabilny kontrakt dla M1-3/M1-1” — tylko po domknięciu rozbieżności nagłówków/DTO opisanej powyżej.

## Dyscyplina zakresu

- Brak wyjścia poza zakres (brak UI i DB w implementacji parsera).
- Brak naruszeń granic architektonicznych (service + contracts + tests).

## Bezpieczeństwo i jakość techniczna

- Brak sygnałów wycieku sekretów lub niebezpiecznych operacji.
- Parsowanie oparte o `InvariantCulture` i jawne walidacje typu.
- Potencjalny drobny follow-up jakościowy (niski wpływ): doprecyzować obsługę delimitera/kodowania i formatów dat dokładnie wg planu (w planie tylko ISO; w kodzie dodatkowo `yyyy/MM/dd  HH:mm:ss`).

## Bramki jakości (delta)

- **Build:** PASS
- **Lint/Typecheck:** PASS
- **Tests:** PASS

## Decyzje triage

1. **fix** — rozjazd kontraktu nagłówków/DTO (Medium severity, High impact).
2. **skip (na teraz)** — rozszerzone formaty dat (`yyyy/MM/dd  HH:mm:ss`) jako obserwacja niskiego ryzyka, jeśli zespół akceptuje szerszą kompatybilność wejścia.

## Wniosek

Implementacja jest technicznie poprawna i stabilna runtime/testowo, ale wymaga świadomego domknięcia rozbieżności kontraktu danych względem planu M1-2 przed scaleniem kolejnych etapów integracyjnych.
