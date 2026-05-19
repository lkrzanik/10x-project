---
starter_id: "dotnet"
package_manager: "dotnet"
project_name: "10x-fotowoltaika"
hints:
  language_family: "dotnet"
  team_size: "solo"
  deployment_target: "self-host"
  ci_provider: "github-actions"
  ci_default_flow: "auto-deploy-on-merge"
  bootstrapper_confidence: "verified"
  path_taken: "custom"
  quality_override: false
  self_check_answers:
    typed: true
    from_official_starter: true
    conventions: true
    docs_current: true
    can_judge_agent: true
  has_auth: false
  has_payments: false
  has_realtime: false
  has_ai: true
  has_background_jobs: false
---

## Why this stack

Wybrano `dotnet`, bo wskazałeś preferencję technologii Microsoft i języka C#, a karta startera ma status `verified`, pełne typowanie i dojrzałą dokumentację. Ponieważ dla komórki `desktop + dotnet` nie ma domyślnej rekomendacji, zapisano ścieżkę `custom`; przy MVP dla pojedynczego operatora ustawiono `team_size: solo`, cel wdrożenia `self-host` oraz domyślny pipeline GitHub Actions. Flagi funkcjonalne odzwierciedlają PRD: AI jest w zakresie, a auth, payments, realtime i background jobs pozostają poza MVP.
