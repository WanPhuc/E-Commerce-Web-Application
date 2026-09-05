# ==============================================================================
# rename_to_auramart.ps1
# Doi ten project tu "Shoppy" -> "AuraMart" toan bo codebase
# Chay tu thu muc: d:\VSC\WEB\Shoppy\SourceCode\
# ==============================================================================

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$ErrorActionPreference = "Continue"

# ---- Mapping [from, to] - dung array de tranh duplicate key ----
# Thu tu: cu the -> chung
$Replacements = @(
    # Docker / Infra (lowercase specific)
    @("name: shoppy",                  "name: auramart"),
    @("container_name: shoppy-",       "container_name: auramart-"),
    @("shoppy_events",                 "auramart_events"),
    @("shoppy_default",                "auramart_default"),
    @("shoppy-dev-jwt-secret-key",     "auramart-dev-jwt-secret-key"),
    # Env vars - mixed case
    @("JWT_ISSUER=Shoppy",             "JWT_ISSUER=AuraMart"),
    @("JWT_AUDIENCE=Shoppy",           "JWT_AUDIENCE=AuraMart"),
    @("JwtSettings__Issuer: Shoppy",   "JwtSettings__Issuer: AuraMart"),
    @("JwtSettings__Audience: Shoppy", "JwtSettings__Audience: AuraMart"),
    @("JwtSettings__SecretKey: shoppy","JwtSettings__SecretKey: auramart"),
    # General - Pascal case (C# namespaces, class names, strings)
    @("Shoppy",                        "AuraMart"),
    # General - lowercase (docker, env, exchange names, urls)
    @("shoppy",                        "auramart")
)

# ---- Extensions can xu ly ----
$Extensions = @(
    "*.cs", "*.csproj", "*.sln",
    "*.yml", "*.yaml",
    "*.json",
    ".env", "*.example",
    "*.md", "*.txt",
    ".gitignore", ".dockerignore",
    "Dockerfile"
)

# ---- Thu muc bo qua ----
$ExcludeDirs = @("\\bin\\", "\\obj\\", "\\.vs\\", "\\.git\\", "\\node_modules\\", "\\.angular\\")

Write-Host "=== AuraMart Rename Script ===" -ForegroundColor Cyan
Write-Host "Root: $Root" -ForegroundColor Gray
Write-Host ""

# ---- Thu thap tat ca files ----
$AllFiles = @()
foreach ($ext in $Extensions) {
    if ($ext.StartsWith(".")) {
        # Handle dotfiles (.gitignore, .env, etc)
        $files = Get-ChildItem -Path $Root -Recurse -File -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -eq $ext }
    } else {
        $files = Get-ChildItem -Path $Root -Filter $ext -Recurse -File -ErrorAction SilentlyContinue
    }
    $AllFiles += $files
}

# Loc bo thu muc exclude va chinh script nay
$ScriptPath = $MyInvocation.MyCommand.Path
$AllFiles = $AllFiles | Where-Object {
    $path = $_.FullName
    if ($path -eq $ScriptPath) { return $false }
    foreach ($ex in $ExcludeDirs) {
        if ($path -match [regex]::Escape($ex)) { return $false }
    }
    return $true
} | Sort-Object FullName -Unique

Write-Host "Files to process: $($AllFiles.Count)" -ForegroundColor Yellow

# ---- Thay the noi dung file ----
$ChangedFiles = 0

foreach ($file in $AllFiles) {
    try {
        $content = Get-Content $file.FullName -Raw -Encoding UTF8 -ErrorAction Stop
        if ($null -eq $content) { continue }

        $newContent = $content

        foreach ($pair in $Replacements) {
            $from = $pair[0]
            $to   = $pair[1]
            # Su dung StringComparison.Ordinal (case-sensitive)
            $newContent = $newContent.Replace($from, $to)
        }

        if ($newContent -ne $content) {
            # Giu nguyen line endings
            Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
            $ChangedFiles++
            Write-Host "  [CHANGED] $($file.FullName.Replace($Root, '.'))" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "  [ERROR] $($file.FullName): $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Content changes: $ChangedFiles files" -ForegroundColor Green

# ---- Rename files/folders co "Shoppy"/"shoppy" trong ten ----
Write-Host ""
Write-Host "--- Renaming files/folders with 'Shoppy'/'shoppy' in name ---" -ForegroundColor Yellow

$ItemsToRename = Get-ChildItem -Path $Root -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -match "shoppy" -or $_.Name -match "Shoppy" } |
    Where-Object {
        foreach ($ex in $ExcludeDirs) {
            if ($_.FullName -match [regex]::Escape($ex)) { return $false }
        }
        return $true
    } |
    Sort-Object { $_.FullName.Length } -Descending  # rename sau nhat truoc

$RenamedCount = 0
foreach ($item in $ItemsToRename) {
    $newName = $item.Name `
        -replace "Shoppy", "AuraMart" `
        -replace "shoppy", "auramart"

    if ($newName -ne $item.Name) {
        try {
            Rename-Item -Path $item.FullName -NewName $newName -ErrorAction Stop
            Write-Host "  [RENAMED] $($item.Name) -> $newName" -ForegroundColor Magenta
            $RenamedCount++
        }
        catch {
            Write-Host "  [RENAME SKIP] $($item.Name): $($_.Exception.Message)" -ForegroundColor DarkYellow
        }
    }
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host " RENAME COMPLETE: Shoppy -> AuraMart         " -ForegroundColor Green
Write-Host " Content changes : $ChangedFiles files       " -ForegroundColor Green
Write-Host " Items renamed   : $RenamedCount             " -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Yellow
Write-Host "  1. Build check:  cd web_banhang_be && dotnet build WebBanHang.sln"
Write-Host "  2. Rebuild Docker (tu web_banhang/):"
Write-Host "     docker compose down"
Write-Host "     docker compose build --no-cache"
Write-Host "     docker compose up -d"
