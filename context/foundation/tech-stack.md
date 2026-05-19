---
starter_id: dotnet-wpf
package_manager: nuget
project_name: 10x-fotowoltaika
hints: "path_taken: custom; language_family: csharp; deployment_target: windows-desktop-installer(local); ci_cd_provider: github-actions; ci_cd_flow: dotnet-build-test-on-pr + signed-msix-release-on-tag; bootstrapper_confidence: best-effort; team_size: solo; quality_override: none; self_check_answers: stack_fit=high|team_fit=high|delivery_fit=medium|risk=vendor_lockin_low|confidence=high; feature_flags: auth=false,payments=false,realtime=false,ai=optional,background_jobs=false"
---

## Why this stack

Dla MVP desktop działającego lokalnie wybór `dotnet-wpf` w C# lepiej odpowiada preferencjom Microsoft i realiom projektu na Windows: zapewnia dojrzały ekosystem .NET, natywne UI, prostą obsługę plików CSV i lokalnej persystencji, a także wygodne generowanie raportów bez warstwy serwerowej. Taki stos skraca czas dowozu dla pojedynczego zespołu i pozostawia czytelną ścieżkę rozwoju pod integracje pogodowe oraz dalsze reguły rekomendacji technologii OZE.