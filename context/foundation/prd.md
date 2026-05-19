---
product_type: internal_tool
language_family: dotnet
target_scale: single_user
timeline_budget: 6_weeks_solo
---

# PRD — 10xFotowoltaika MVP

## 1. Przegląd produktu

10xFotowoltaika to wewnętrzna aplikacja webowa dla właściciela domu, służąca do długoterminowej analizy warunków klimatycznych pomieszczenia i pogody zewnętrznej. Na podstawie zgromadzonych danych (minimum 1 rok) aplikacja generuje raport AI z rekomendacją optymalnej technologii inwertera i magazynu energii.

Aplikacja hostowana w chmurze Azure, zbudowana w C# / .NET MVC, z jednym kontem użytkownika.

## 2. Problem użytkownika

Właściciel domu chce podjąć świadomą decyzję o doborze inwertera i magazynu energii do instalacji fotowoltaicznej. Potrzebuje narzędzia, które:
- zbiera i koreluje dane klimatyczne z pomieszczenia z danymi pogodowymi,
- identyfikuje ekstremalne warunki,
- generuje merytoryczną rekomendację technologiczną opartą na zebranych danych.

## 3. Wymagania funkcjonalne

### FR-1: Uwierzytelnianie
- Logowanie jednego użytkownika (jedno konto).
- Sesja z timeoutem.

### FR-2: Import danych pomiarowych (CSV)
- Upload pliku CSV z urządzenia pomiarowego.
- Predefiniowany format: `timestamp, temperature_c, humidity_pct` (interwał 30 min).
- Walidacja formatu i duplikatów.
- Przechowywanie zaimportowanych rekordów w bazie danych.

### FR-3: Import danych pogodowych (CSV)
- Upload pliku CSV z danymi pogodowymi.
- Predefiniowany format: `timestamp, temperature_c, humidity_pct, cloud_cover_pct`.
- Interwał czasowy może się różnić od danych pomiarowych.
- Walidacja formatu.

### FR-4: Korelacja danych
- Automatyczne dopasowanie danych pogodowych do pomiarowych (interpolacja liniowa).
- Wynik: każdy rekord pomiarowy ma przypisane skorelowane dane pogodowe.

### FR-5: Notatki do rekordów pomiarowych
- Użytkownik może dodać/edytować tekstową uwagę do dowolnego rekordu pomiarowego.
- Uwagi przechowywane w bazie danych.

### FR-6: Wizualizacja (wykres)
- Wykres zależności między danymi pomiarowymi a pogodowymi.
- Dostępny w dowolnym momencie po imporcie danych.
- Oś X: czas; Oś Y: temperatura/wilgotność (pomiarowa vs pogodowa).

### FR-7: Ekstrema
- Wyróżnienie rekordów pomiarowych przekraczających progi min/max.
- Progi konfigurowalne w ustawieniach aplikacji.
- Widoczne oznaczenie ekstremów na liście danych i na wykresie.

### FR-8: Raport AI (PDF)
- Generowanie raportu na żądanie użytkownika.
- Integracja z OpenAI API (GPT).
- Treść raportu: ocena nadaje się / nie nadaje się, uzasadnienie, generyczna rekomendacja technologii inwertera i magazynu energii.
- Eksport do PDF (tylko tekst, bez grafiki).

### FR-9: Ustawienia
- Konfiguracja progów ekstremalnych (min/max temperatura, min/max wilgotność).

## 4. Granice produktu (poza zakresem MVP)

- Zaawansowana edycja danych pomiarowych.
- Obsługa wielu lokalizacji pomiarowych.
- Obsługa wielu użytkowników / ról.
- Analiza zapotrzebowania na energię i liczby paneli PV.
- Automatyczne pobieranie danych pogodowych z API.
- Wielojęzyczność (tylko PL w MVP).

## 5. Historyjki użytkownika

### US-1: Logowanie
Jako użytkownik chcę się zalogować do aplikacji, aby uzyskać dostęp do swoich danych.
**Kryteria akceptacji:**
- Formularz logowania (login + hasło).
- Po poprawnym logowaniu przekierowanie na dashboard.
- Błędne dane → komunikat o błędzie.

### US-2: Import danych pomiarowych
Jako użytkownik chcę zaimportować plik CSV z urządzenia pomiarowego, aby aplikacja przechowywała moje odczyty.
**Kryteria akceptacji:**
- Upload pliku CSV przez formularz.
- Walidacja formatu (błąd jeśli niezgodny).
- Dane zapisane w bazie po pomyślnym imporcie.
- Informacja o liczbie zaimportowanych rekordów.

### US-3: Import danych pogodowych
Jako użytkownik chcę zaimportować plik CSV z danymi pogodowymi, aby aplikacja mogła je skorelować z moimi pomiarami.
**Kryteria akceptacji:**
- Upload pliku CSV przez formularz.
- Walidacja formatu.
- Dane zapisane w bazie.

### US-4: Korelacja danych
Jako użytkownik chcę, aby aplikacja automatycznie skorelowała dane pomiarowe z pogodowymi, abym widział pełny kontekst klimatyczny.
**Kryteria akceptacji:**
- Po imporcie danych pogodowych następuje automatyczna korelacja.
- Interpolacja liniowa dla niedopasowanych interwałów.
- Każdy rekord pomiarowy ma przypisane wartości pogodowe.

### US-5: Dodawanie notatek
Jako użytkownik chcę dodać uwagę tekstową do rekordu pomiarowego, aby opisać anomalie.
**Kryteria akceptacji:**
- Pole tekstowe przy rekordzie pomiarowym.
- Zapis i edycja notatki.

### US-6: Wyświetlenie wykresu
Jako użytkownik chcę wyświetlić wykres porównawczy danych pomiarowych i pogodowych, aby wizualnie ocenić zależności.
**Kryteria akceptacji:**
- Wykres liniowy (czas vs wartości).
- Możliwość wyboru zakresu dat.
- Widoczne serie: temperatura pomiarowa, temperatura pogodowa, wilgotność pomiarowa, wilgotność pogodowa.

### US-7: Ekstrema
Jako użytkownik chcę widzieć wyróżnione ekstremalne odczyty, abym mógł szybko zidentyfikować problematyczne warunki.
**Kryteria akceptacji:**
- Rekordy przekraczające progi wyróżnione kolorem/ikoną.
- Oddzielna sekcja/widok z listą ekstremów.

### US-8: Konfiguracja progów
Jako użytkownik chcę ustawić progi min/max dla temperatury i wilgotności, aby system wiedział, co jest ekstremum.
**Kryteria akceptacji:**
- Formularz ustawień z polami min/max.
- Zmiana progów natychmiast wpływa na oznaczenie ekstremów.

### US-9: Generowanie raportu AI
Jako użytkownik chcę wygenerować raport AI na podstawie zgromadzonych danych, aby otrzymać rekomendację technologiczną.
**Kryteria akceptacji:**
- Przycisk „Generuj raport".
- Wywołanie OpenAI API z kontekstem danych.
- Wyświetlenie treści raportu w aplikacji.
- Pobranie raportu w formacie PDF.

## 6. Metryki sukcesu

1. 100% zaimportowanych rekordów pomiarowych jest skorelowanych z danymi pogodowymi.
2. Raport AI zawiera ocenę nadaje się / nie nadaje się + rekomendację technologii inwertera i magazynu energii.
3. Czas generowania raportu < 60 sekund.

## 7. Wymagania niefunkcjonalne

- **Hosting:** Azure (App Service + Azure SQL Database).
- **Język interfejsu:** polski.
- **Bezpieczeństwo:** HTTPS, uwierzytelnianie ASP.NET Identity.
- **Wydajność:** obsługa zbioru danych do 20 000 rekordów pomiarowych (1 rok × 48 odczytów/dzień × 365 dni ≈ 17 520).
- **CI/CD:** Azure DevOps + GitHub.
