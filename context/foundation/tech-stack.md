---
starter_id: dotnet-mvc
package_manager: dotnet
project_name: 10xPV
hints:
  path_taken: standard
  deployment_target: azure_app_service
  database: azure_sql
  ci_cd: azure_devops_yaml_github
  auth: aspnet_identity
  ai_provider: openai
  pdf_generation: true
  feature_flags:
    authentication: true
    payments: false
    real_time: false
    ai: true
    background_jobs: false
  bootstrapper_confidence: first-class
  agent_friendly_gates:
    typed: true
    convention_based: true
    popular_in_training_data: true
    well_documented: true
---

## Why this stack

ASP.NET Core MVC na Azure App Service to naturalny wybór dla single-user internal tool w ekosystemie .NET. Silne typowanie C#, konwencyjny układ MVC i doskonała dokumentacja Microsoft zapewniają wysoką produktywność agenta AI i solo-developera. Azure SQL Database pokrywa potrzeby persystencji bez nadmiarowej złożoności. Azure DevOps Pipelines (YAML) z repozytorium GitHub daje CI/CD natywnie zintegrowane z targetem deploymentu. OpenAI SDK dla .NET i biblioteki do generowania PDF (np. QuestPDF) są dobrze udokumentowane i typowane.
