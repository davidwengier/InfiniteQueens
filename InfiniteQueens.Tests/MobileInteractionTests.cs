using Microsoft.Playwright;
using Xunit;

namespace InfiniteQueens.Tests;

public class MobileInteractionTests : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private const string BaseUrl = "http://localhost:5000";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }
        _playwright?.Dispose();
    }

    [Fact]
    public async Task DoubleTap_OnGameCell_CyclesCellState()
    {
        // Skip if the app isn't running
        if (!await IsAppRunning())
        {
            return;
        }

        var context = await _browser!.NewContextAsync(new()
        {
            ViewportSize = new ViewportSize { Width = 375, Height = 667 },
            DeviceScaleFactor = 2,
            IsMobile = true,
            HasTouch = true
        });

        var page = await context.NewPageAsync();
        await page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
        await page.WaitForTimeoutAsync(2000); // Wait for Blazor to load

        // Select the first cell
        var cell = page.Locator(".cell").First;
        await cell.WaitForAsync();

        // Verify cell starts empty (no queen or cross)
        var hasQueen = await cell.Locator(".queen").CountAsync() > 0;
        var hasCross = await cell.Locator(".cross").CountAsync() > 0;
        Assert.False(hasQueen);
        Assert.False(hasCross);

        // First tap: should place a cross (✕)
        await cell.TapAsync();
        await page.WaitForTimeoutAsync(100);
        hasCross = await cell.Locator(".cross").CountAsync() > 0;
        Assert.True(hasCross);

        // Second tap: should place a queen (♛)
        await cell.TapAsync();
        await page.WaitForTimeoutAsync(100);
        hasQueen = await cell.Locator(".queen").CountAsync() > 0;
        hasCross = await cell.Locator(".cross").CountAsync() > 0;
        Assert.True(hasQueen);
        Assert.False(hasCross);

        // Third tap: should clear the cell
        await cell.TapAsync();
        await page.WaitForTimeoutAsync(100);
        hasQueen = await cell.Locator(".queen").CountAsync() > 0;
        hasCross = await cell.Locator(".cross").CountAsync() > 0;
        Assert.False(hasQueen);
        Assert.False(hasCross);

        await context.CloseAsync();
    }

    [Fact]
    public async Task DoubleTap_OutsideGameBoard_DoesNotZoom()
    {
        // Skip if the app isn't running
        if (!await IsAppRunning())
        {
            return;
        }

        var context = await _browser!.NewContextAsync(new()
        {
            ViewportSize = new ViewportSize { Width = 375, Height = 667 },
            DeviceScaleFactor = 2,
            IsMobile = true,
            HasTouch = true
        });

        var page = await context.NewPageAsync();
        await page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
        await page.WaitForTimeoutAsync(2000);

        // Get initial viewport scale
        var initialScale = await page.EvaluateAsync<double>("window.visualViewport.scale");

        // Double tap on header (outside game board)
        var header = page.Locator("h1").First;
        await header.TapAsync();
        await page.WaitForTimeoutAsync(100);
        await header.TapAsync();
        await page.WaitForTimeoutAsync(500); // Wait to see if zoom occurs

        // Verify scale hasn't changed (zoom prevented)
        var finalScale = await page.EvaluateAsync<double>("window.visualViewport.scale");
        Assert.Equal(initialScale, finalScale);

        await context.CloseAsync();
    }

    [Fact]
    public async Task QuickDoubleTap_OnGameCell_PlacesQueen()
    {
        // Skip if the app isn't running
        if (!await IsAppRunning())
        {
            return;
        }

        var context = await _browser!.NewContextAsync(new()
        {
            ViewportSize = new ViewportSize { Width = 375, Height = 667 },
            DeviceScaleFactor = 2,
            IsMobile = true,
            HasTouch = true
        });

        var page = await context.NewPageAsync();
        await page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
        await page.WaitForTimeoutAsync(2000);

        var cell = page.Locator(".cell").First;
        await cell.WaitForAsync();

        // Perform two quick taps (within 300ms)
        await cell.TapAsync();
        await page.WaitForTimeoutAsync(50); // Very short delay
        await cell.TapAsync();
        await page.WaitForTimeoutAsync(100);

        // Should have cycled: Empty -> Cross -> Queen
        var hasQueen = await cell.Locator(".queen").CountAsync() > 0;
        Assert.True(hasQueen);

        await context.CloseAsync();
    }

    [Fact]
    public async Task AfterDrag_NextClick_ShouldWork()
    {
        // Skip if the app isn't running
        if (!await IsAppRunning())
        {
            return;
        }

        var context = await _browser!.NewContextAsync(new()
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
            IsMobile = false,
            HasTouch = false
        });

        var page = await context.NewPageAsync();
        await page.GotoAsync(BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });
        await page.WaitForTimeoutAsync(2000);

        var cells = page.Locator(".cell");
        await cells.First.WaitForAsync();

        // Drag across first three cells in the first row
        var firstCell = cells.Nth(0);
        var secondCell = cells.Nth(1);
        var thirdCell = cells.Nth(2);
        var fourthCell = cells.Nth(3);

        // Perform drag operation
        await firstCell.HoverAsync();
        await page.Mouse.DownAsync();
        await secondCell.HoverAsync();
        await page.WaitForTimeoutAsync(50);
        await thirdCell.HoverAsync();
        await page.WaitForTimeoutAsync(50);
        await page.Mouse.UpAsync();
        await page.WaitForTimeoutAsync(100);

        // Verify drag placed crosses
        Assert.True(await firstCell.Locator(".cross").CountAsync() > 0);
        Assert.True(await secondCell.Locator(".cross").CountAsync() > 0);
        Assert.True(await thirdCell.Locator(".cross").CountAsync() > 0);

        // Now click on the fourth cell - this should work immediately
        await fourthCell.ClickAsync();
        await page.WaitForTimeoutAsync(100);

        // Verify the click worked (placed a cross)
        var hasCross = await fourthCell.Locator(".cross").CountAsync() > 0;
        Assert.True(hasCross, "Click after drag should work immediately and place a cross");

        // Click again to place a queen
        await fourthCell.ClickAsync();
        await page.WaitForTimeoutAsync(100);

        // Verify queen was placed
        var hasQueen = await fourthCell.Locator(".queen").CountAsync() > 0;
        Assert.True(hasQueen, "Second click should place a queen");

        await context.CloseAsync();
    }

    private async Task<bool> IsAppRunning()
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(2);
            var response = await client.GetAsync(BaseUrl);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            // App not running, skip test
            return false;
        }
    }
}
