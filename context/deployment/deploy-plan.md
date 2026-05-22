---
project: 10xPV
approved_at: 2026-05-23
status: approved
platform: azure_app_service
region: polandcentral
deployment_method: deployment_center_oryx
---

# Deploy Plan — 10xPV MVP

## Zatwierdzony plan wdrożenia

### Model CI/CD

```
git push origin main → Azure Deployment Center wykrywa push → Oryx buduje (dotnet publish) → deploy na App Service
```

- **Brak GitHub Actions** — zero plików `.github/workflows/`
- **Build na Azure** — Oryx (wbudowany build engine App Service) wykonuje build
- **Trigger**: każdy push na branch `main`
- **Rollback**: Deployment Center → wybierz poprzedni commit → Redeploy

---

### Krok 1: Utworzenie Resource Group
```
az group create --name rg-10xProject --location polandcentral
```
- Typ: zautomatyzowany

### Krok 2: Utworzenie App Service Plan (F1 free)
```
az appservice plan create --name plan-10xProject --resource-group rg-10xProject --location polandcentral --sku F1
```
- Typ: zautomatyzowany

### Krok 3: Utworzenie Web App
```
az webapp create --name app-10xProject --resource-group rg-10xProject --plan plan-10xProject --runtime "dotnet:10"
```
- Typ: zautomatyzowany

### Krok 4: Ustawienie ścieżki projektu dla Oryx
```
az webapp config appsettings set --resource-group rg-10xProject --name app-10xProject --settings PROJECT="app/10xPV.csproj"
```
- Typ: zautomatyzowany
- Uwaga: Oryx potrzebuje znać lokalizację .csproj w subfolderze `app/`

### Krok 5: Konfiguracja Deployment Center (GitHub + Oryx)
```
az webapp deployment source config --name app-10xProject --resource-group rg-10xProject --repo-url https://github.com/lkrzanik/10x-project --branch main --git-token <GITHUB_PAT>
```
- Typ: ⚠️ RĘCZNA BRAMKA — wymaga GitHub Personal Access Token z uprawnieniem `repo`
- Build provider: **App Service Build Service (Oryx)** — NIE GitHub Actions
- Po konfiguracji: każdy push na `main` = automatyczny build + deploy

### Krok 6: Utworzenie Azure SQL Server
```
az sql server create --name sql-10xProject --resource-group rg-10xProject --location polandcentral --admin-user 10xpvadmin --admin-password <GENERATE_STRONG_PASSWORD>
```
- Typ: ⚠️ RĘCZNA BRAMKA — użytkownik generuje hasło

### Krok 7: Utworzenie Azure SQL Database (Free tier)
```
az sql db create --name db-10xProject --resource-group rg-10xProject --server sql-10xProject --capacity 6 --edition GeneralPurpose --compute-model Serverless --family Gen5
```
- Typ: zautomatyzowany

### Krok 8: Firewall — zezwolenie Azure services
```
az sql server firewall-rule create --resource-group rg-10xProject --server sql-10xProject --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
```
- Typ: zautomatyzowany

### Krok 9: Konfiguracja Connection String
```
az webapp config connection-string set --resource-group rg-10xProject --name app-10xProject --connection-string-type SQLAzure --settings DefaultConnection="Server=tcp:sql-10xProject.database.windows.net,1433;Database=db-10xProject;User ID=10xpvadmin;Password=<PASSWORD>;Encrypt=true;TrustServerCertificate=false;"
```
- Typ: ⚠️ RĘCZNA BRAMKA — wymaga hasła

### Krok 10: Konfiguracja App Settings (OpenAI — przyszłościowo)
```
az webapp config appsettings set --resource-group rg-10xProject --name app-10xProject --settings OpenAI__ApiKey="<KEY>"
```
- Typ: ⚠️ RĘCZNA BRAMKA — gdy klucz będzie potrzebny

### Krok 11: Weryfikacja
```
az webapp browse --resource-group rg-10xProject --name app-10xProject
az webapp deployment source show --resource-group rg-10xProject --name app-10xProject
az webapp log tail --resource-group rg-10xProject --name app-10xProject
```
- Oczekiwany wynik: strona Home/Index ładuje się, deployment source wskazuje GitHub `main`

### Krok 12: Test auto-deploy
```
git commit --allow-empty -m "test: verify auto-deploy"
git push origin main
```
- Oczekiwany wynik: Deployment Center pokazuje nowy build w ciągu ~2-5 min

## Koszty

| Usługa | Tier | Koszt/mies. |
|--------|------|-------------|
| App Service Plan | F1 | $0 |
| Azure SQL Database | Free (12 mies.) | $0 |
| Deployment Center | wbudowane | $0 |
| **Razem (start)** | | **$0/mies.** |

Po free tier: B1 ($13) + SQL Basic ($5) = ~$18/mies.

## Ręczne bramki

1. Krok 5 — GitHub PAT z uprawnieniem `repo`
2. Krok 6 — wygenerowanie hasła SQL admin
3. Krok 9 — wklejenie hasła do connection string
4. Krok 10 — klucz OpenAI (gdy potrzebny)

## Nazwy zasobów

| Zasób | Nazwa |
|-------|-------|
| Resource Group | rg-10xProject |
| App Service Plan | plan-10xProject |
| Web App | app-10xProject |
| SQL Server | sql-10xProject |
| SQL Database | db-10xProject |
| URL | https://app-10xProject.azurewebsites.net |
| Deploy trigger | push to `main` → Deployment Center (Oryx) |
