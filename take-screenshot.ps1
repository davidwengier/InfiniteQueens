#!/usr/bin/env pwsh
# Helper script to take screenshots of the Infinite Queens game

param(
    [string]$Url = "https://wengier.com/InfiniteQueens/",
    [string]$OutputFile = "screenshot.png",
    [int]$Width = 375,
    [int]$Height = 667,
    [string]$Device = ""
)

# Common device presets
$devices = @{
    "iphone-se" = @{ Width = 375; Height = 667 }
    "iphone-14" = @{ Width = 390; Height = 844 }
    "iphone-14-pro-max" = @{ Width = 430; Height = 932 }
    "ipad" = @{ Width = 768; Height = 1024 }
    "desktop" = @{ Width = 1920; Height = 1080 }
}

# Use device preset if specified
if ($Device -and $devices.ContainsKey($Device)) {
    $Width = $devices[$Device].Width
    $Height = $devices[$Device].Height
    Write-Host "Using $Device preset: ${Width}x${Height}"
}

# Ensure screenshots directory exists
$screenshotsDir = "screenshots"
if (-not (Test-Path $screenshotsDir)) {
    New-Item -ItemType Directory -Path $screenshotsDir | Out-Null
}

# Prepend screenshots directory to output path if not already included
if (-not $OutputFile.StartsWith($screenshotsDir)) {
    $OutputFile = Join-Path $screenshotsDir $OutputFile
}

# Run the screenshot tool
dotnet run --project InfiniteQueens.Screenshots\InfiniteQueens.Screenshots.csproj -- $Url $OutputFile $Width $Height

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nScreenshot saved to: $OutputFile" -ForegroundColor Green
} else {
    Write-Host "`nFailed to capture screenshot" -ForegroundColor Red
}
