using Microsoft.Playwright;

// Parse command line arguments
string url = args.Length > 0 ? args[0] : "http://localhost:5000";
string outputPath = args.Length > 1 ? args[1] : "screenshot.png";
int width = args.Length > 2 ? int.Parse(args[2]) : 375;  // iPhone SE width
int height = args.Length > 3 ? int.Parse(args[3]) : 667; // iPhone SE height
bool simulateWin = args.Length > 4 && args[4] == "win";

Console.WriteLine($"Taking screenshot of {url}");
Console.WriteLine($"Viewport: {width}x{height}");
Console.WriteLine($"Output: {outputPath}");
if (simulateWin) Console.WriteLine("Simulating win state...");

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new()
{
    Headless = true
});

var context = await browser.NewContextAsync(new()
{
    ViewportSize = new ViewportSize { Width = width, Height = height },
    DeviceScaleFactor = 2, // Retina display
    IsMobile = true,
    HasTouch = true
});

var page = await context.NewPageAsync();
await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.NetworkIdle });

// Wait for the board to be generated
await page.WaitForTimeoutAsync(3000);

// If simulating win, place queens to win the game
if (simulateWin)
{
    // Inject JavaScript to simulate a win by placing queens in valid positions
    await page.EvaluateAsync(@"
        // Find all cells and click to place queens
        const cells = document.querySelectorAll('.cell');
        const boardSize = Math.sqrt(cells.length);
        
        // Click queens in a simple diagonal pattern (adjust as needed)
        for (let i = 0; i < boardSize; i++) {
            const cell = cells[i * boardSize + i];
            // Click twice: once for X, twice for queen
            cell.click();
            cell.click();
        }
    ");
    
    await page.WaitForTimeoutAsync(500);
}

await page.ScreenshotAsync(new()
{
    Path = outputPath,
    FullPage = false
});

Console.WriteLine("Screenshot saved!");
await browser.CloseAsync();
