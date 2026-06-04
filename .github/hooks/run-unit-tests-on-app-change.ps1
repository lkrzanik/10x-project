$ErrorActionPreference = "Stop"

$rawPayload = ($input | Out-String)
if ([string]::IsNullOrWhiteSpace($rawPayload)) {
    exit 0
}

try {
    $payload = $rawPayload | ConvertFrom-Json
}
catch {
    # Ignore malformed payloads to avoid blocking agent flow.
    exit 0
}

$toolName = ""
if ($payload.PSObject.Properties.Name -contains "tool_name") {
    $toolName = [string]$payload.tool_name
}
elseif ($payload.PSObject.Properties.Name -contains "toolName") {
    $toolName = [string]$payload.toolName
}

$toolName = $toolName.ToLowerInvariant()
$writeTools = @("edit", "write", "create", "multiedit", "multi_edit")
if ($writeTools -notcontains $toolName) {
    exit 0
}

$toolInput = $null
if ($payload.PSObject.Properties.Name -contains "tool_input") {
    $toolInput = $payload.tool_input
}
elseif ($payload.PSObject.Properties.Name -contains "toolArgs") {
    $toolInput = $payload.toolArgs
}

if ($null -eq $toolInput) {
    exit 0
}

$pathValues = New-Object System.Collections.Generic.List[string]

function Collect-PathValues {
    param(
        [Parameter(Mandatory = $false)]
        [object]$Node
    )

    if ($null -eq $Node) {
        return
    }

    if ($Node -is [string]) {
        return
    }

    if ($Node -is [System.Collections.IDictionary]) {
        foreach ($key in $Node.Keys) {
            $value = $Node[$key]
            if ($key -match "(?i)(file|path)") {
                if ($value -is [string] -and -not [string]::IsNullOrWhiteSpace($value)) {
                    [void]$pathValues.Add([string]$value)
                }
            }

            Collect-PathValues -Node $value
        }
        return
    }

    if ($Node -is [System.Array] -or $Node -is [System.Collections.IList]) {
        foreach ($item in $Node) {
            Collect-PathValues -Node $item
        }
        return
    }

    foreach ($prop in $Node.PSObject.Properties) {
        if ($prop.Name -match "(?i)(file|path)") {
            if ($prop.Value -is [string] -and -not [string]::IsNullOrWhiteSpace($prop.Value)) {
                [void]$pathValues.Add([string]$prop.Value)
            }
        }

        Collect-PathValues -Node $prop.Value
    }
}

Collect-PathValues -Node $toolInput

$touchesApp = $false
foreach ($candidate in $pathValues) {
    $normalized = $candidate.Replace("\\", "/").Trim()
    if ($normalized -match "(^|/)app(/|$)") {
        $touchesApp = $true
        break
    }
}

if (-not $touchesApp) {
    exit 0
}

Push-Location "app"
try {
    dotnet test ".\\Tests\\10xPV.Tests\\10xPV.Tests.csproj" --nologo --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        exit 2
    }
}
finally {
    Pop-Location
}

exit 0