<!-- BEGIN @przeprogramowani/10x-cli -->

## 10xDevs AI Toolkit — Moduł 1, Lekcja 1

Uruchom projekt greenfield od początku do końca za pomocą **łańcucha kształtowania**:

```
/10x-init → /10x-shape → /10x-prd → (10x-tech-stack-selector) → (bootstrapper)
```

Pierwsze trzy umiejętności są dostarczane w tej lekcji; ostatnie dwa to kolejne ogniwa w łańcuchu.

### Router zadań — Od czego zacząć

| Umiejętność | Użyj, gdy |
| --- | --- |
| **Konfiguracja projektu** | |
| `/10x-init` | Katalog projektu jest świeży. Tworzy szkielet `context/foundation/lessons.md` i `docs/reference/contract-surfaces.md`, aby reszta przepływu pracy miała gdzie pisać. Uruchom to raz na projekt. |
| **Odkrywanie** | |
| `/10x-shape` | Masz pomysł i musisz przekształcić go w ustrukturyzowane notatki kształtujące ZANIM napiszesz PRD. Tylko greenfield. Przechodzi przez wizję → persona/dostęp → MVP → FR (z sokratycznym wyzwaniem) → logika biznesowa i dane → szkic otwartości stosu. Wykrywa antywzorce empty-CRUD i MVP-too-big po nazwie. Wynik: `context/foundation/shape-notes.md` z blokiem `checkpoint:` umożliwiającym wznowienie. |
| **Generowanie dokumentów** | |
| `/10x-prd` | Masz notatki kształtujące (lub surowe notatki) i chcesz uzyskać zgodny ze schematem `context/foundation/prd.md`. Generuje zgodnie z zablokowanym schematem, kieruje każdą lukę dosłownie do `## Open Questions` i odmawia wymyślania decyzji domenowych. W przypadku kolizji, monituje o nadpisanie lub zapisanie wersji (`prd-vN.md`). |

### Jak łańcuch przekazuje dane

- `/10x-init` tworzy szkielet przepływu pracy v2 (`context/foundation/`, `lessons.md`, `contract-surfaces.md`). `/10x-shape` wymaga tego i zaoferuje delegowanie do `/10x-init`, jeśli brakuje.
- `/10x-shape` zapisuje `context/foundation/shape-notes.md` z frontmatter `checkpoint:` (current_phase, phases_completed, frs_drafted, quality_check_status). Po ponownym wejściu wznawia od następnej niedokończonej fazy.
- `/10x-prd` odczytuje `shape-notes.md` (domyślnie) lub dowolną ścieżkę, którą podasz, ocenia dane wejściowe na podstawie heurystyki 4 sygnałów, ostrzega o zbyt małej ilości danych wejściowych i zapisuje `context/foundation/prd.md` zgodnie ze schematem w `skills/10x-shape/references/prd-schema.md` (frontmatter dopasowany 1:1 do Q1–Q7 10x-tech-stack-selector).

### Co PRD zawiera (a czego NIE)

- **Zawiera**: wizję, personę, kryteria sukcesu, historie użytkowników (Given/When/Then), FR (FR-NNN), NFR, logikę biznesową (najpierw zasada jednego zdania), model danych, kontrolę dostępu, trwałe decyzje implementacyjne, strategię testowania, strategię wdrożenia i CI/CD, cele nieobjęte, otwarte pytania.
- **NIE zawiera (celowo)**: wybory frameworków, wybory baz danych, ścieżki plików, platforma wdrożeniowa. Otwartość stosu jest wiążąca — tylko `product_type` i `tech_preferences.language_family` odzwierciedlają intencje dotyczące kształtu stosu. Frameworki to zadanie 10x-tech-stack-selector.

### Antywzorce wykryte podczas kształtowania

- **Empty-CRUD**: logika biznesowa, która sprowadza się do „użytkownicy dodają i usuwają rekordy” bez reguły domenowej. `/10x-shape` nazywa to jawnie i monituje o prawdziwy kształt reguły (rekomendacja, priorytetyzacja, klasyfikacja, walidacja, punktacja, przepływ pracy, obliczenia).
- **MVP-too-big**: szacowany czas pierwszego przepływu przekracza ~1 tydzień pracy po godzinach, lub > 4 odrębne akcje użytkownika przed widoczną wartością dla użytkownika, lub wymaga wielu integracji przed uzyskaniem korzyści. Umiejętność nazywa kosztowne elementy i oferuje konkretne ruchy w celu zmniejszenia zakresu.

Oba są **miękkimi bramkami**: ostrzegają, ale pozwalają na nadpisanie. Nadpisania są rejestrowane w punkcie kontrolnym i wyświetlane w `## Open Questions` PRD.

### Ścieżki podstawowe używane w tej lekcji

- `context/foundation/shape-notes.md` — wynik `/10x-shape`
- `context/foundation/prd.md` (lub `prd-vN.md`) — wynik `/10x-prd`
- `context/foundation/lessons.md` — powtarzające się zasady i pułapki (szkielet tworzony przez `/10x-init`)
- `docs/reference/contract-surfaces.md` — rejestr nazw nośnych (szkielet tworzony przez `/10x-init`)

### Uniwersalny język

Dostarczone umiejętności nie zawierają odniesień do 10xDevs / kohorty / certyfikacji. Mechanika (sokratyczne wyzwanie, odkrywanie szarych stref, łagodzenie zmęczenia zalecanymi odpowiedziami, miękka brama jakości) to uniwersalne wskaźniki dobrze określonego projektu greenfield.

Umiejętności nie mogą zapisywać do `context/archive/`. Zarchiwizowane zmiany są niezmienne; jeśli rozwiązana ścieżka docelowa zaczyna się od `context/archive/`, przerwij z komunikatem: "Ta zmiana jest zarchiwizowana. Zamiast tego otwórz nową zmianę za pomocą `/10x-new`."

<!-- END @przeprogramowani/10x-cli -->
