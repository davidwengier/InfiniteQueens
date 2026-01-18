# Test Easy mode generation rate

$code = @"
using InfiniteQueens.Services;
using System;
using System.Diagnostics;

var generator = new BoardGenerator();

Console.WriteLine(""Testing Easy mode generation rates...\n"");

foreach (var boardSize in new[] { 4, 6, 8 })
{
    Console.WriteLine($""Board Size: {boardSize}x{boardSize}"");
    
    int attempts = 0;
    int successes = 0;
    var sw = Stopwatch.StartNew();
    int maxTests = 100;
    
    for (int test = 0; test < maxTests; test++)
    {
        attempts++;
        var regions = generator.GenerateRegions(boardSize);
        if (generator.IsSolvable(regions, boardSize))
        {
            successes++;
            if (successes >= 10) break;
        }
    }
    
    sw.Stop();
    double successRate = (double)successes / attempts * 100;
    double avgTime = successes > 0 ? sw.ElapsedMilliseconds / (double)successes : 0;
    
    Console.WriteLine($""  {successes}/{attempts} boards ({successRate:F1}% success rate, avg {avgTime:F0}ms per board)\n"");
}
"@

# Create a temp C# project
$tempDir = Join-Path $env:TEMP "EasyModeTest"
if (Test-Path $tempDir) { Remove-Item $tempDir -Recurse -Force }
New-Item -ItemType Directory -Path $tempDir | Out-Null

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
"@ | Out-File -FilePath (Join-Path $tempDir "Test.csproj")

$code | Out-File -FilePath (Join-Path $tempDir "Program.cs")

Push-Location $tempDir
try {
    dotnet run
} finally {
    Pop-Location
}
