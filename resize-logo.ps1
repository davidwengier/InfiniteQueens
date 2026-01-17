#!/usr/bin/env pwsh
# Resize logo to all needed sizes

Add-Type -AssemblyName System.Drawing

$sourcePath = "InfiniteQueens.Web/wwwroot/logo-source.png"
$wwwroot = "InfiniteQueens.Web/wwwroot"

function Resize-Image {
    param(
        [string]$SourcePath,
        [int]$TargetSize,
        [string]$OutputPath
    )
    
    $sourceImage = [System.Drawing.Image]::FromFile($SourcePath)
    $targetImage = New-Object System.Drawing.Bitmap($TargetSize, $TargetSize)
    $graphics = [System.Drawing.Graphics]::FromImage($targetImage)
    
    # High quality resize
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    
    $graphics.DrawImage($sourceImage, 0, 0, $TargetSize, $TargetSize)
    
    $targetImage.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    
    $graphics.Dispose()
    $targetImage.Dispose()
    $sourceImage.Dispose()
}

Write-Host "Resizing logo to all required sizes..." -ForegroundColor Green

$sizes = @(
    @{ Size = 512; Name = "icon-512.png" },
    @{ Size = 192; Name = "icon-192.png" },
    @{ Size = 180; Name = "apple-touch-icon.png" },
    @{ Size = 32; Name = "favicon-32x32.png" },
    @{ Size = 16; Name = "favicon.png" }
)

foreach ($icon in $sizes) {
    $outputPath = Join-Path $wwwroot $icon.Name
    Resize-Image -SourcePath $sourcePath -TargetSize $icon.Size -OutputPath $outputPath
    Write-Host "✓ Generated $($icon.Name) ($($icon.Size)x$($icon.Size))" -ForegroundColor Cyan
}

Write-Host "`nAll icon sizes generated successfully!" -ForegroundColor Green
