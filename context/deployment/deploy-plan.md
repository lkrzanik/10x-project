---
project: 10xPV
approved_at: 2026-05-22
status: approved
platform: azure_app_service
region: westeurope
---

# Deploy Plan — 10xPV MVP

## Zatwierdzony plan wdrożenia

### Krok 1: Utworzenie Resource Group
```
az group create --name rg-10xpv --location westeurope
```
- Typ: zautomatyzowany

### Krok 2: Utworzenie App Service Plan (F1 free, Windows)
```
az appservice plan create --name plan-10xpv --resource-group rg-10xpv --location westeurope --sku F1 --is-linux false
```
- Typ: zautomatyzowany

### Krok 3: Utworzenie Web App
```
az webapp create --name app-10xpv --resource-group rg-10xpv --plan plan-10xpv --runtime "dotnet:10"
```
- Typ: zautomatyzowany
- Fallback: self-contained deploy jeśli runtime niedostępny

### Krok 4: Utworzenie Azure SQL Server
```
az sql server create --name sql-10xpv --resource-group rg-10xpv --location westeurope --admin-user 10xpvadmin --admin-password <GENERATE_STRONG_PASSWORD>
```
- Typ: ⚠️ RĘCZNA BRAMKA — użytkownik generuje hasło

### Krok 5: Utworzenie Azure SQL Database (Free tier)
```
az sql db create --name db-10xpv --resource-group rg-10xpv --server sql-10xpv --free-limit --free-limit-exhaustion-behavior AutoPause --capacity 5 --edition GeneralPurpose --compute-model Serverless --family Gen5
```
- Typ: zautomatyzowany

### Krok 6: Firewall — zezwolenie Azure services
```
az sql server firewall-rule create --resource-group rg-10xpv --server sql-10xpv --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
```
- Typ: zautomatyzowany

### Krok 7: Publish & Deploy (self-contained ZIP)
```powershell
cd app
dotnet publish -c Release -r win-x64 --self-contained -o ./publish
Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip -Force
az webapp deploy --resource-group rg-10xpv --name app-10xpv --src-path ./publish.zip --type zip
```
- Typ: zautomatyzowany

### Krok 8: Konfiguracja Connection String
```
az webapp config connection-string set --resource-group rg-10xpv --name app-10xpv --connection-string-type SQLAzure --settings DefaultConnection="Server=tcp:sql-10xpv.database.windows.net,1433;Database=db-10xpv;User ID=10xpvadmin;Password=<PASSWORD>;Encrypt=true;TrustServerCertificate=false;"
```
- Typ: ⚠️ RĘCZNA BRAMKA — wymaga hasła

### Krok 9: Konfiguracja App Settings (OpenAI — przyszłościowo)
```
az webapp config appsettings set --resource-group rg-10xpv --name app-10xpv --settings OpenAI__ApiKey="<KEY>"
```
- Typ: ⚠️ RĘCZNA BRAMKA — gdy klucz będzie potrzebny

### Krok 10: Weryfikacja
```
az webapp browse --resource-group rg-10xpv --name app-10xpv
az webapp log tail --resource-group rg-10xpv --name app-10xpv
```
- Oczekiwany wynik: strona Home/Index ładuje się poprawnie

## Koszty

| Usługa | Tier | Koszt/mies. |
|--------|------|-------------|
| App Service Plan | F1 | $0 |
| Azure SQL Database | Free (12 mies.) | $0 |
| **Razem (start)** | | **$0/mies.** |

Po free tier: B1 ($13) + SQL Basic ($5) = ~$18/mies.

## Ręczne bramki

1. Krok 4 — wygenerowanie hasła SQL admin
2. Krok 8 — wklejenie hasła do connection string
3. Krok 9 — klucz OpenAI (gdy potrzebny)

## Nazwy zasobów

| Zasób | Nazwa |
|-------|-------|
| Resource Group | rg-10xpv |
| App Service Plan | plan-10xpv |
| Web App | app-10xpv |
| SQL Server | sql-10xpv |
| SQL Database | db-10xpv |
| URL | https://app-10xpv.azurewebsites.net |
