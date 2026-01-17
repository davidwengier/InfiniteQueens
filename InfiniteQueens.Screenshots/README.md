# Screenshot Tool

This tool captures screenshots of the Blazor WASM app at different viewport sizes to help visualize mobile/desktop layouts.

## Usage

```powershell
# Basic usage with deployed site
dotnet run --project InfiniteQueens.Screenshots\InfiniteQueens.Screenshots.csproj -- "https://wengier.com/InfiniteQueens/" "screenshot.png"

# With custom viewport size
dotnet run --project InfiniteQueens.Screenshots\InfiniteQueens.Screenshots.csproj -- "https://wengier.com/InfiniteQueens/" "screenshot.png" 375 667

# Local development server
dotnet run --project InfiniteQueens.Screenshots\InfiniteQueens.Screenshots.csproj -- "http://localhost:5000" "local-screenshot.png"
```

## Common Viewport Sizes

- **iPhone SE**: 375 x 667
- **iPhone 12/13/14**: 390 x 844
- **iPhone 14 Pro Max**: 430 x 932
- **iPad**: 768 x 1024
- **Desktop**: 1920 x 1080

## Examples

```powershell
# iPhone SE
dotnet run --project InfiniteQueens.Screenshots\InfiniteQueens.Screenshots.csproj -- "https://wengier.com/InfiniteQueens/" "iphone-se.png" 375 667

# iPad
dotnet run --project InfiniteQueens.Screenshots\InfiniteQueens.Screenshots.csproj -- "https://wengier.com/InfiniteQueens/" "ipad.png" 768 1024

# Desktop
dotnet run --project InfiniteQueens.Screenshots\InfiniteQueens.Screenshots.csproj -- "https://wengier.com/InfiniteQueens/" "desktop.png" 1920 1080
```

## Notes

- Screenshots are saved in the root project directory
- The tool simulates a mobile device with touch support
- Uses Chromium browser via Playwright
- Screenshots are taken after network idle to ensure full page load
