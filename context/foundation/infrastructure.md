---
project: 10xPV
researched_at: 2026-05-22
recommended_platform: azure_app_service
runner_up: railway
context_type: infrastructure
tech_stack: context/foundation/tech-stack.md
---

# Infrastructure — 10xPV

## Rekomendacja

**Azure App Service** (region: polandcentral) z wbudowanym Deployment Center (Oryx build) połączonym z GitHub repo — branch `main`.

### Dlaczego Azure App Service

- Natywne wsparcie .NET 10
- Free tier (F1) wystarczający na MVP
- Azure SQL w tej samej lokalizacji (polandcentral)
- CLI-first (`az webapp`)
- Wbudowany CI/CD przez Deployment Center (Oryx) — bez GitHub Actions

## Deployment model

**Azure Deployment Center (GitHub integration via App Service Build Service)** — po push na `main` Azure sam pobiera kod, buduje go silnikiem Oryx i deployuje. Zero plików workflow w repo.

Konfiguracja:
- Source: GitHub
- Branch: `main`
- Build provider: **App Service Build Service (Oryx)**
- Runtime stack: .NET 10
- App setting `PROJECT`: `app/10xPV.csproj` (Oryx musi wiedzieć, gdzie jest .csproj w subfolderze)

## Porównanie platform (skrót)

| Kryterium | Azure App Service | Railway | Render |
|-----------|:-:|:-:|:-:|
| CLI-first | Pass | Pass | Partial |
| Managed/serverless | Pass | Pass | Pass |
| Agent-readable docs | Partial | Pass | Partial |
| Stable deploy API | Pass | Pass | Pass |
| MCP / agent integration | Partial | Fail | Fail |

## Anty-uprzedzenia — wyniki

### Adwokat diabła
1. F1 tier: brak always-on, cold start ~20s
2. Oryx build na F1 może być wolny (ograniczone zasoby CPU)
3. Azure SQL free tier: limit 32 GB, 12 mies.

### Pre-mortem (150 słów)
Sześć miesięcy później aplikacja przestała odpowiadać po 20 min bezczynności — F1 nie ma always-on. Build przez Oryx trwa 4 minuty, bo free tier ma ograniczone CPU. Przy próbie upgrade na B1 okazuje się, że koszty SQL rosną jednocześnie. Deployment Center cicho failuje bo `.csproj` path się zmienił po reorganizacji i nikt nie zauważył przez tydzień.

### Nieznane niewiadome
1. Oryx auto-detection może nie rozpoznać subfolderu `app/` — wymaga ustawienia `PROJECT` app setting
2. Free tier wymusza shared infrastructure — sporadyczne 503 pod obciążeniem
3. Deployment Center + GitHub wymaga autoryzacji OAuth — token może wygasnąć

## Historia operacyjna

- **Podgląd**: `az webapp browse` lub https://app-10xProject.azurewebsites.net
- **Sekrety**: `az webapp config appsettings set` / `connection-string set`
- **Wycofywanie**: Deployment Center → poprzedni commit → Redeploy
- **Zatwierdzanie**: `git push origin main` = auto-deploy
- **Logi**: `az webapp log tail`

## Rejestr ryzyka

| # | Ryzyko | Soczewka | Mitygacja |
|---|--------|--------|-----------|
| 1 | Cold start na F1 | Devil's advocate | Upgrade do B1 gdy potrzebne |
| 2 | Oryx nie znajdzie projektu w subfolderze | Unknown unknowns | Ustawić `PROJECT=app/10xPV.csproj` |
| 3 | OAuth token GitHub wygasa | Unknown unknowns | Monitorować Deployment Center status |
| 4 | SQL free tier wygasa po 12 mies. | Pre-mortem | Zaplanować budżet ~$5/mies. |

## Decyzja użytkownika

✅ Kontynuuj z liderem (Azure App Service) — ryzyka włączone do rejestru.
