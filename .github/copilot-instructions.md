<!-- BEGIN @przeprogramowani/10x-cli -->

## 10xDevs AI Toolkit — Moduł 1, Lekcja 2

Wybierz starter i stos dla PRD, który napisałeś w Lekcji 1, z **łańcuchem stosu**:

```
(/10x-init → /10x-shape → /10x-prd) → /10x-tech-stack-selector → (bootstrapper)
```

Łańcuch PRD pochodzi z Lekcji 1 (ponownie uwzględniony w tej lekcji, abyś mógł poprawić PRD w trakcie pracy). `/10x-tech-stack-selector` to główny temat lekcji; `/10x-bootstrapper` to następne ogniwo, nauczane w Lekcji 3.

### Router zadań — Od czego zacząć

| Umiejętność | Kiedy jej używać |
| --- | --- |
| **Wybór stosu (główny temat lekcji)** | |
| `/10x-tech-stack-selector` | Masz PRD w `context/foundation/prd.md` i musisz wybrać starter. Rozpoczyna się od wyraźnego wyboru (przyjmij zalecaną domyślną opcję dla swojej komórki `(product_type, language_family)` lub zaprojektuj własną), przeprowadza przez zestaw pytań uzupełniających, gdy projektujesz własną, stosuje cztery bramki jakości przyjazne dla agentów, analizuje rejestr starterów uwzględniający język i zapisuje `context/foundation/tech-stack.md`. Opcjonalny argument `[path-to-prd]` pozwala wskazać niestandardową lokalizację PRD (np. `/10x-tech-stack-selector @context/foundation/prd-v2.md`); bez niego umiejętność domyślnie używa `context/foundation/prd.md`. Użyj PO `/10x-prd`, PRZED `/10x-bootstrapper`. |
| **Ponowne uruchomienie upstream w razie potrzeby** | |
| `/10x-init` / `/10x-shape` / `/10x-prd` | Zestawione, abyś mógł poprawić PRD w trakcie pracy. Jeśli `/10x-tech-stack-selector` ujawni lukę (np. Wymaganie Funkcjonalne, które wymusza funkcję, której nie ma Twój zalecany starter), uruchom ponownie `/10x-prd`, aby poprawić PRD przed wyborem stosu. |

### Jak działa przekazywanie

- `/10x-tech-stack-selector` odczytuje frontmatter `context/foundation/prd.md` (`product_type`, `target_scale`, `timeline_budget`) jako priorytet. Jeśli PRD jest nieobecne, odmawia z jednosentencyjnym przekierowaniem do `/10x-shape` — brak wbudowanego awaryjnego mini-PRD.
- Umiejętność zapisuje `context/foundation/tech-stack.md` z 4-kluczowym frontmatterem (`starter_id`, `package_manager`, `project_name`, `hints`) plus jednoparagraphowym ciałem `## Why this stack`. Przekazanie jest celowo minimalne — bootstrapper nie analizuje uzasadnienia, tylko pola.
- `/10x-bootstrapper` (Lekcja 3) odczytuje `tech-stack.md` i rejestr, aby stworzyć szkielet projektu.

### Co przechwytuje tech-stack-selector (a czego NIE)

- **Przechwycone**: wybór startera (w kształcie rejestru), rodzina języków, menedżer pakietów (otwarty ciąg znaków dla każdego ekosystemu — `pnpm`, `uv`, `bundle`, `cargo` itp.), rozmiar zespołu, cel wdrożenia (pobrany z `deployment_defaults` wybranego startera), dostawca CI/CD + przepływ, pewność bootstrappera (`verified | first-class | best-effort`), wybrana ścieżka (standardowa | niestandardowa), odpowiedzi na autokontrolę (ścieżka niestandardowa), nadpisanie jakości (ustawiane, gdy użytkownik kontynuuje ze starterem, który nie przeszedł ≥1 bramki przyjaznej dla agenta), flagi funkcji (uwierzytelnianie/płatności/real-time/AI/zadania w tle).
- **NIE przechwycone (celowo)**: strategiczny plan testów, strategiczny plan wdrożenia, strategiczne decyzje implementacyjne. Są one dalszym etapem wyboru stosu — przyszłym problemem mapy drogowej technicznej, jeszcze nie zaplanowanym. Tech-stack-selector odpowiada za wybory testów/wdrożenia/CI w *kształcie frameworka*, ponieważ są one nierozerwalnie związane z wyborem stosu; to, co jest odroczone, to warstwa *strategiczna* ("testujemy TDD na powierzchni X", "środowisko podglądu dla każdego PR").

### Wybór początkowy (nośny)

Pierwsze pytanie to wyraźny wybór — nigdy nie cichy. Umiejętność od razu podaje zalecany starter dla Twojej komórki `(product_type, language_family)` i prosi o wyraźne potwierdzenie:

- **Ścieżka standardowa** — zaakceptuj zalecaną domyślną opcję. Umiejętność pomija audyt funkcji, profil zespołu, preferencje techniczne i pytania dotyczące wariantów frameworka; pyta tylko o wdrożenie, CI/CD i nazwę projektu. Przekazanie rejestruje `path_taken: standard` w `hints`.
- **Ścieżka niestandardowa** — zaprojektuj własną. Umiejętność przeprowadza przez pełny zestaw pytań uzupełniających (audyt funkcji, profil zespołu, preferencje techniczne, wdrożenie, CI/CD, wariant frameworka), zagłębia się w pytanie o runnera testów tylko wtedy, gdy wybrany starter pozostawia to niejednoznaczne, i kończy 5-punktową autokontrolą gotowości (z lekcji przygotowawczej 4.1) przed zablokowaniem. Przekazanie rejestruje `path_taken: custom` i wypełnia `self_check_answers`.

Mapa zalecanych domyślnych opcji dla każdej komórki jest wielojęzyczna: web/JS i saas/JS oba → 10x-astro-starter (starter marki 10x prowadzi, gdy konkuruje w komórce JS); api/JS → hono; api/Python → fastapi; web/Python → django; web/Ruby → rails; api/Go → go; api/Rust → axum; mobile/Dart → flutter; desktop/Rust → tauri; itd. Komórki bez zweryfikowanej domyślnej opcji mają `<none>` i wymuszają ścieżkę niestandardową.

### Bramki jakości (kryteria przyjazne dla agentów)

Każda karta startera zawiera cztery wartości logiczne, które LLM filtruje:

1. **Typed** — jawne typy/schematy, z których agent może wnioskować bez uruchamiania programu.
2. **Convention-based** — silne opinie na temat układu, routingu, konfiguracji.
3. **Popular in training data** — oceniane *dla każdej rodziny języków*, a nie globalnie (Django jest popularne w danych treningowych Pythona; Spring w Javie; itd.).
4. **Well-documented** — aktualna, przypięta do wersji, linkowalna dokumentacja.

Kandydaci, którzy nie przejdą żadnej bramki, są wykluczani z zestawu niezapowiedzianych rekomendacji. Jeśli jawnie nazwiesz starter, który nie przeszedł testu, jako swoje preferencje, umiejętność zakwestionuje ten wybór — wskazując najsilniejszą alternatywę o wyższych kryteriach ORAZ ścieżkę kompensacji (instrukcje w pliku konfiguracyjnym AI projektu (AGENTS.md), które łatają luki) — i poprosi o potwierdzenie lub zmianę. Potwierdzenie wyboru znanego z tarć rejestruje nadpisanie w przekazaniu, aby bootstrapper mógł się dostosować.

### Pewność bootstrappera

Każda rekomendacja wyświetla `bootstrapper_confidence` dosłownie — nigdy nie jest pomijana w milczeniu:

- **`verified`** — bootstrapper został uruchomiony od początku do końca na tym stosie; tworzenie szkieletu będzie płynne.
- **`first-class`** — zarejestrowany z prawidłowym CLI, oczekuje się, że będzie działać, ale nie jest przetestowany w boju; spodziewaj się w większości płynnego tworzenia szkieletu z okazjonalnymi ręcznymi krokami.
- **`best-effort`** — ograniczone wsparcie; prawdopodobne ręczne kroki; spodziewaj się tarć (a generowanie pliku konfiguracyjnego AI projektu (AGENTS.md) przez bootstrappera kompensuje to dodatkowym kontekstem specyficznym dla ekosystemu).

To jest ostrzeżenie przed uruchomieniem `/10x-bootstrapper`, abyś wiedział, czego się spodziewać.

### Ścieżki bazowe używane w tej lekcji

- `context/foundation/prd.md` — dane wejściowe (z Lekcji 1)
- `context/foundation/tech-stack.md` — dane wyjściowe (przekazanie łańcucha)
- `context/foundation/lessons.md` — powtarzające się zasady i pułapki
- `docs/reference/contract-surfaces.md` — rejestr nazw nośnych

### Język uniwersalny

Dostarczona umiejętność nie zawiera odniesień do 10xDevs / kohorty / certyfikacji. Rejestr zalecanych domyślnych opcji jest wielojęzyczny (JS, Python, Ruby, Java, Go, Rust, PHP, .NET, Dart), a `10x-astro-starter` kohorty to jedna karta w komórce JS+web — nie "jedyna" zalecana ścieżka dla wszystkich.

Umiejętności nie mogą zapisywać do `context/archive/`. Zarchiwizowane zmiany są niezmienne; jeśli docelowa ścieżka rozwiązania zaczyna się od `context/archive/`, przerwij z komunikatem: "Ta zmiana jest zarchiwizowana. Zamiast tego otwórz nową zmianę za pomocą `/10x-new`."

<!-- END @przeprogramowani/10x-cli -->
