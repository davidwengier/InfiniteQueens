#!/usr/bin/env pwsh
# Generate PNG icons from SVG logo

$svgPath = "InfiniteQueens.Web/wwwroot/logo.svg"
$wwwroot = "InfiniteQueens.Web/wwwroot"

# Check if we have magick (ImageMagick) available
$hasMagick = Get-Command magick -ErrorAction SilentlyContinue

if ($hasMagick) {
    Write-Host "Using ImageMagick to generate icons..." -ForegroundColor Green
    
    # Generate various sizes
    $sizes = @(
        @{ Size = 512; Name = "icon-512.png" },
        @{ Size = 192; Name = "icon-192.png" },
        @{ Size = 180; Name = "apple-touch-icon.png" },
        @{ Size = 32; Name = "favicon-32x32.png" },
        @{ Size = 16; Name = "favicon.png" }
    )
    
    foreach ($icon in $sizes) {
        $outputPath = Join-Path $wwwroot $icon.Name
        magick -background none -density 300 $svgPath -resize "$($icon.Size)x$($icon.Size)" $outputPath
        Write-Host "✓ Generated $($icon.Name)" -ForegroundColor Cyan
    }
    
    Write-Host "`nAll icons generated successfully!" -ForegroundColor Green
} else {
    Write-Host "ImageMagick not found. Please install it or use an online SVG to PNG converter." -ForegroundColor Yellow
    Write-Host "Install ImageMagick: winget install ImageMagick.ImageMagick" -ForegroundColor Yellow
    Write-Host "`nOr use https://convertio.co/svg-png/ to convert logo.svg to PNGs" -ForegroundColor Yellow
}
