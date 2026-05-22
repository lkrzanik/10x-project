---
project: 10xPV
researched_at: 2026-05-22
recommended_platform: azure_app_service
runner_up: azure_container_apps
context_type: infrastructure
tech_stack: dotnet-mvc
decision: accepted_leader
---

# Infrastructure — Azure App Service

## Rekomendacja

**Azure App Service (Windows)** jako platforma wdrożeniowa MVP.

Plan wdrożenia:
- Start na **F1 (free tier)** dla developmentu i wczesnych testów
- Migracja na **B1 (~$13/mies.)** gdy CPU quota (60 min/dzień) stanie się ograniczeniem
- Azure SQL Database **Free tier** (12 miesięcy) → potem Basic DTU (~$5/mies.)
- Deployment mode: **self-contained** (eliminuje zależność od runtime .NET 10 na platformie)
- OS: **Windows** (zapewnia kompatybilność QuestPDF bez dodatkowych zależności systemowych)
- Region: **Poland Central** (kolokacja app + database)

## Porównanie platform — macierz punktacji

| Kryterium | Azure App Service | Azure Container Apps | Fly.io |
|-----------|:-:|:-:|:-:|
| CLI-first | ✅ Pass | ✅ Pass | ✅ Pass |
| Managed/serverless | ✅ Pass | ✅ Pass | ⚠️ Partial |
| Agent-readable docs | ✅ Pass | ✅ Pass | ⚠️ Partial |
| Stable deploy API | ✅ Pass | ✅ Pass | ✅ Pass |
| MCP/agent integration | ⚠️ Partial | ⚠️ Partial | ❌ Fail |
| **Kolokacja z Azure SQL** | ✅ | ✅ | ❌ |
| **Natywny .NET (bez Docker)** | ✅ | ❌ | ❌ |
| **Free/low-cost tier** | ✅ F1 free | ⚠️ consumption billing | ⚠️ $0 hobby (ograniczenia) |

### Platformy wyeliminowane (twarde filtry)

- **Cloudflare Workers/Pages** — brak .NET runtime
- **Vercel** — brak .NET server-side
- **Netlify** — brak .NET server-side
- **Render** — brak kolokacji z Azure SQL, wymaga Docker

## Wyniki anty-uprzedzeniowe

### Adwokat diabła — słabości Azure App Service

1. Free tier (F1) ma limit 60 min CPU/dzień — intensywny import CSV + raport AI może go wyczerpać
2. Brak always-on na F1 — cold start 5-15s po 20 min nieaktywności
3. B1 ($13/mies.) + Azure SQL Basic ($5/mies.) + OpenAI API = szybkie zużycie $35 budżetu
4. Deployment slots niedostępne poniżej Standard (S1, ~$55/mies.)
5. Azure SQL Free tier wygasa po 12 miesiącach od utworzenia subskrypcji

### Pre-mortem — narracja awarii (150-200 słów)

Projekt ruszył na F1 free tier. Przez pierwsze tygodnie było świetnie — jeden użytkownik, rzadkie requesty. Potem zacząłem importować duże pliki CSV (50k+ rekordów) i generować raporty AI w jednej sesji. CPU quota się wyczerpała w 40 minut. Aplikacja padła na resztę dnia. Przeszedłem na B1 ($13/mies.), ale wtedy okazało się, że Azure SQL free tier wygasł (był tylko na 12 miesięcy od utworzenia subskrypcji, nie od deploymentu). Nagle hosting to $13 + $5 + OpenAI usage = $20+/mies., a z $35 łącznego budżetu zostało na niecałe 2 miesiące. Gdybym od razu policzył TCO z zapasem na API calls, wybrałbym B1 od startu i zoptymalizował zapytania AI.

### Nieznane niewiadome

1. .NET 10 na App Service — może nie mieć Day-1 runtime support (workaround: self-contained deploy)
2. QuestPDF na Linux App Service wymaga libgdiplus/SkiaSharp — Windows eliminuje ryzyko
3. Azure SQL serverless auto-pause = 30-60s cold-start bazy przy sporadycznym użyciu
4. $35 credits mogą mieć datę wygaśnięcia i ograniczenia usługowe
5. OpenAI API calls wychodzą przez internet — brak Private Link na tym tier

## Historia operacyjna

| Oś | Odpowiedź |
|----|-----------|
| **Preview/deploy** | `az webapp deploy --src-path ./publish.zip --type zip` |
| **Sekrety** | `az webapp config appsettings set --settings KEY=VALUE` (App Settings → env vars) |
| **Wycofywanie** | Redeployment poprzedniego ZIP lub użycie deployment center history |
| **Zatwierdzanie** | `az webapp browse` + health check endpoint |
| **Logi** | `az webapp log tail` (streaming) lub Kudu SCM console |

## Rejestr ryzyka

| # | Ryzyko | Prawdopodobieństwo | Wpływ | Mitygacja | Soczewka |
|---|--------|-------------------|-------|-----------|----------|
| 1 | CPU quota F1 wyczerpana przy heavy workload | Średnie | Średni (app niedostępna do jutra) | Monitoruj użycie; migruj na B1 gdy osiągniesz 50% quota regularnie | Devil's advocate |
| 2 | .NET 10 runtime niedostępny na App Service w dniu GA | Niskie | Niski (workaround istnieje) | Self-contained deployment | Unknown unknowns |
| 3 | Azure SQL free tier wygasa nieoczekiwanie | Średnie | Średni (koszt rośnie) | Zaplanuj budżet $5/mies. od miesiąca 12 | Pre-mortem |
| 4 | Cold start bazy po auto-pause | Średnie | Niski (UX: 30-60s wait) | Użyj Basic DTU (always-on) zamiast serverless | Unknown unknowns |
| 5 | Budżet $35 niewystarczający >3 miesięcy na B1 | Średnie | Wysoki (wymusza shutdown) | Zostań na F1 jak najdłużej; optymalizuj OpenAI calls (cache promptów) | Pre-mortem |
| 6 | QuestPDF incompatibility na Linux | Niskie | Średni (wymaga zmiany OS) | Użyj Windows App Service Plan | Research finding |
