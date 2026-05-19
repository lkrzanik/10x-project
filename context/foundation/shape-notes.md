# Shape Notes — 10xFotowoltaika

## Profil projektu

| Wymiar | Wartość |
|--------|---------|
| Typ produktu | Narzędzie wewnętrzne (internal tool) |
| Użytkownik | Właściciel domu (1 osoba, 1 konto) |
| Skala | Single-user, single-location |
| Hosting | Azure (chmura) |
| Język UI | Polski |
| Stack | C# / .NET MVC / Azure DevOps / GitHub |
| Budżet | 6 tygodni, solo, po godzinach |
| Dane pomiarowe | CSV z urządzenia (format TBD), minimum 1 rok zbierania |
| Dane pogodowe | CSV z zewnętrznego serwisu (format TBD) |
| Korelacja | Interpolacja liniowa |
| AI | OpenAI API → generyczna rekomendacja technologiczna (tekst) |
| Raport | PDF, tylko tekst, bez grafiki |

## Odpowiedzi na pytania uzupełniające

### 1. Użytkownik i kontekst biznesowy

- Docelowy użytkownik: właściciel domu
- Typ aplikacji: narzędzie wewnętrzne (nie SaaS)
- Okres gromadzenia danych przed raportem: minimum 1 rok

### 2. Dane i integracje

- Format CSV urządzenia pomiarowego: nieznany (do ustalenia przed implementacją)
- Źródło danych pogodowych: nieznane (do ustalenia)
- Korelacja danych: interpolacja liniowa wystarczy

### 3. AI i raport PDF

- Model AI: OpenAI (GPT)
- Źródło wiedzy o produktach: generyczna rekomendacja technologiczna (bez bazy produktów)
- Format raportu: PDF, tylko tekst, bez grafiki/wykresów

### 4. Wymagania niefunkcjonalne

- Jednoczesnych użytkowników: 1 (jedno konto)
- Hosting: Azure (chmura)
- Język interfejsu: polski

### 5. Ograniczenia / preferencje techniczne

- Język/framework: C# / .NET MVC
- CI/CD: Azure DevOps + GitHub
- Budżet czasowy: 6 tygodni
- Zespół: 1 osoba (solo, po godzinach)

## Zidentyfikowane ryzyka / otwarte kwestie

1. **Format CSV nieznany** — PRD zdefiniuje kontrakt na „zdefiniowany format", ale konkretny schemat kolumn trzeba ustalić przed implementacją (lub pozwolić użytkownikowi mapować kolumny).
2. **Źródło danych pogodowych nieznane** — PRD założy generyczny CSV z kolumnami `timestamp, temp, humidity, cloud_cover`; konkretny serwis do ustalenia.
3. **Solo + 6 tygodni + .NET MVC** — realistyczne dla MVP, ale PDF generation i OpenAI integration to dodatkowa złożoność.
