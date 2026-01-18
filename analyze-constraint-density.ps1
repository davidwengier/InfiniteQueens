# Analyze constraint density distribution

$code = @"
using InfiniteQueens.Services;
using System;
using System.Linq;
using System.Collections.Generic;

var generator = new BoardGenerator();

Console.WriteLine("Analyzing constraint density distribution...\n");

foreach (var boardSize in new[] { 4, 6, 8 })
{
    Console.WriteLine($"=== Board Size: {boardSize}x{boardSize} ===");
    
    var densities = new List<double>();
    var solvableCount = 0;
    var uniqueSolutionCount = 0;
    
    // Generate 100 random boards
    for (int i = 0; i < 100; i++)
    {
        var regions = generator.GenerateRegions(boardSize);
        
        // Use reflection to call private CalculateConstraintDensity method
        var method = typeof(BoardGenerator).GetMethod(
            ""CalculateConstraintDensity"", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var density = (double)method.Invoke(generator, new object[] { regions, boardSize });
        densities.Add(density);
        
        // Check if solvable (has at least one solution)
        var countMethod = typeof(BoardGenerator).GetMethod(
            ""CountSolutions"", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var testBoard = new bool[boardSize, boardSize];
        int solutionCount = 0;
        countMethod.Invoke(generator, new object[] { 0, testBoard, regions, boardSize, solutionCount });
        
        // Actually call IsSolvable properly
        if (generator.IsSolvable(regions, boardSize))
        {
            uniqueSolutionCount++;
        }
        
        // Check if it has at least one solution (relaxed check)
        // We need to modify this to count all solutions
    }
    
    Console.WriteLine($""  Constraint Density Stats:"");
    Console.WriteLine($""    Min:    {densities.Min():F3}"");
    Console.WriteLine($""    Max:    {densities.Max():F3}"");
    Console.WriteLine($""    Avg:    {densities.Average():F3}"");
    Console.WriteLine($""    Median: {densities.OrderBy(x => x).ElementAt(densities.Count / 2):F3}"");
    Console.WriteLine($""    Unique solution boards: {uniqueSolutionCount}/100"");
    Console.WriteLine($""    Distribution:"");
    Console.WriteLine($""      < 0.4 (Easy):   {densities.Count(d => d < 0.4)}"");
    Console.WriteLine($""      0.4-0.7 (Med):  {densities.Count(d => d >= 0.4 && d < 0.7)}"");
    Console.WriteLine($""      >= 0.7 (Hard):  {densities.Count(d => d >= 0.7)}"");
    Console.WriteLine();
}
"@

# Create a temp C# project to run this
$tempDir = Join-Path $env:TEMP "InfiniteQueensAnalyze"
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
"@ | Out-File -FilePath (Join-Path $tempDir "Analyze.csproj")

# Create Program.cs
$code | Out-File -FilePath (Join-Path $tempDir "Program.cs")

# Run it
Push-Location $tempDir
try {
    dotnet run
} finally {
    Pop-Location
}
