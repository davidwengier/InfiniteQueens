#!/usr/bin/env pwsh
# Generate PNG icons using .NET

Add-Type -AssemblyName System.Drawing

function Create-Icon {
    param(
        [int]$Size,
        [string]$OutputPath
    )
    
    $bitmap = New-Object System.Drawing.Bitmap($Size, $Size)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    
    # Background gradient
    $rect = New-Object System.Drawing.Rectangle(0, 0, $Size, $Size)
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush($rect, 
        [System.Drawing.Color]::FromArgb(33, 150, 243),  # #2196F3
        [System.Drawing.Color]::FromArgb(25, 118, 210),  # #1976D2
        45)
    $graphics.FillRectangle($brush, $rect)
    
    # Draw queen symbol
    $scale = $Size / 512.0
    $centerX = $Size / 2
    $centerY = $Size * 0.47
    $queenSize = $Size * 0.27
    
    # Queen shape (simplified)
    $whiteBrush = [System.Drawing.Brushes]::White
    $pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, $Size * 0.01)
    
    # Crown top
    $crownTop = $centerY - $queenSize * 0.7
    $crownBottom = $centerY - $queenSize * 0.3
    $graphics.FillEllipse($whiteBrush, $centerX - $queenSize * 0.15, $crownTop, $queenSize * 0.3, $queenSize * 0.3)
    
    # Crown body
    $crownRect = New-Object System.Drawing.Rectangle(
        $centerX - $queenSize * 0.4,
        $crownBottom,
        $queenSize * 0.8,
        $queenSize * 0.6
    )
    $graphics.FillRectangle($whiteBrush, $crownRect)
    
    # Base
    $baseRect = New-Object System.Drawing.Rectangle(
        $centerX - $queenSize * 0.5,
        $centerY + $queenSize * 0.3,
        $queenSize,
        $queenSize * 0.2
    )
    $graphics.FillRectangle($whiteBrush, $baseRect)
    
    # Add gold star at top
    $goldBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 215, 0))
    $graphics.FillEllipse($goldBrush, $centerX - $Size * 0.02, $crownTop + $Size * 0.02, $Size * 0.04, $Size * 0.04)
    
    # Draw infinity symbol (chess piece)
    $font = New-Object System.Drawing.Font("Arial", $Size * 0.25, [System.Drawing.FontStyle]::Bold)
    $text = "♛"
    $textSize = $graphics.MeasureString($text, $font)
    $textX = ($Size - $textSize.Width) / 2
    $textY = ($Size - $textSize.Height) / 2
    $graphics.DrawString($text, $font, $whiteBrush, $textX, $textY)
    
    # Save
    $bitmap.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    
    # Cleanup
    $graphics.Dispose()
    $bitmap.Dispose()
    $brush.Dispose()
    $pen.Dispose()
    $goldBrush.Dispose()
    $font.Dispose()
}

Write-Host "Generating icons..." -ForegroundColor Green

$wwwroot = "InfiniteQueens.Web/wwwroot"
$sizes = @(
    @{ Size = 512; Name = "icon-512.png" },
    @{ Size = 192; Name = "icon-192.png" },
    @{ Size = 180; Name = "apple-touch-icon.png" },
    @{ Size = 32; Name = "favicon-32x32.png" },
    @{ Size = 16; Name = "favicon.png" }
)

foreach ($icon in $sizes) {
    $outputPath = Join-Path $wwwroot $icon.Name
    Create-Icon -Size $icon.Size -OutputPath $outputPath
    Write-Host "✓ Generated $($icon.Name)" -ForegroundColor Cyan
}

Write-Host "`nAll icons generated successfully!" -ForegroundColor Green
