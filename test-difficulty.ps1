# Test script to check difficulty generation rates

$code = @"
using InfiniteQueens.Services;
using System;
using System.Diagnostics;

var generator = new BoardGenerator();

Console.WriteLine("Testing board generation rates with unique solution constraint...\n");

foreach (var boardSize in new[] { 4, 6, 8 })
{
    Console.WriteLine($"=== Board Size: {boardSize}x{boardSize} ===");
    
    foreach (var difficulty in new[] { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard })
    {
        int attempts = 0;
        int successes = 0;
        var sw = Stopwatch.StartNew();
        int maxTests = 100;
        
        for (int test = 0; test < maxTests; test++)
        {
            attempts++;
            var regions = generator.GenerateRegions(boardSize);
            if (generator.IsSolvableWithDifficulty(regions, boardSize, difficulty))
            {
                successes++;
                if (successes >= 10) break; // Found 10 boards, that's enough
            }
        }
        
        sw.Stop();
        double successRate = (double)successes / attempts * 100;
        double avgTime = successes > 0 ? sw.ElapsedMilliseconds / (double)successes : 0;
        
        Console.WriteLine($"  {difficulty,-6}: {successes}/100 boards ({successRate:F1}% success rate, avg {avgTime:F0}ms per board)");
    }
    
    // Test any unique solution (no difficulty filter)
    Console.WriteLine($"  Any (unique solution only):");
    int anyAttempts = 0;
    int anySuccesses = 0;
    var anySw = Stopwatch.StartNew();
    
    for (int test = 0; test < 100; test++)
    {
        anyAttempts++;
        var regions = generator.GenerateRegions(boardSize);
        if (generator.IsSolvable(regions, boardSize))
        {
            anySuccesses++;
            if (anySuccesses >= 10) break;
        }
    }
    
    anySw.Stop();
    double anySuccessRate = (double)anySuccesses / anyAttempts * 100;
    double anyAvgTime = anySuccesses > 0 ? anySw.ElapsedMilliseconds / (double)anySuccesses : 0;
    Console.WriteLine($"    {anySuccesses}/{anyAttempts} boards ({anySuccessRate:F1}% success rate, avg {anyAvgTime:F0}ms per board)");
    Console.WriteLine();
}
"@

# Create a temp C# project to run this
$tempDir = Join-Path $env:TEMP "InfiniteQueensTest"
if (Test-Path $tempDir) { Remove-Item $tempDir -Recurse -Force }
New-Item -ItemType Directory -Path $tempDir | Out-Null

# Create project file
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="$PWD\InfiniteQueens.Core\InfiniteQueens.Core.csproj" />
  </ItemGroup>
</Project>
"@ | Out-File -FilePath (Join-Path $tempDir "TestDifficulty.csproj")

# Create Program.cs
$code | Out-File -FilePath (Join-Path $tempDir "Program.cs")

# Run it
Push-Location $tempDir
try {
    dotnet run
} finally {
    Pop-Location
}
