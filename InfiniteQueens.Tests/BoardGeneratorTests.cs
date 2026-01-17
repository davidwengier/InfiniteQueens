using InfiniteQueens.Services;
using Xunit;

namespace InfiniteQueens.Tests;

public class BoardGeneratorTests
{
    private readonly BoardGenerator _generator;

    public BoardGeneratorTests()
    {
        _generator = new BoardGenerator();
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(10)]
    public void GenerateRegions_ShouldCreateCorrectNumberOfRegions(int boardSize)
    {
        // Arrange & Act
        var regions = _generator.GenerateRegions(boardSize);

        // Assert
        var uniqueRegions = new HashSet<int>();
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                uniqueRegions.Add(regions[r, c]);
            }
        }

        Assert.Equal(boardSize, uniqueRegions.Count);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public void GenerateRegions_ShouldAssignAllCells(int boardSize)
    {
        // Arrange & Act
        var regions = _generator.GenerateRegions(boardSize);

        // Assert - no cell should be -1 (unassigned)
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                Assert.InRange(regions[r, c], 0, boardSize - 1);
            }
        }
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public void GenerateRegions_ShouldCreateVariedRegions(int boardSize)
    {
        // Arrange & Act - Generate multiple boards
        var regionSets = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var regions = _generator.GenerateRegions(boardSize);
            var regionString = RegionsToString(regions, boardSize);
            regionSets.Add(regionString);
        }

        // Assert - At least some boards should be different (randomness check)
        var uniqueBoards = new HashSet<string>(regionSets);
        Assert.True(uniqueBoards.Count > 1, "Generator should create varied region layouts");
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    public void IsSolvable_ShouldReturnTrueForValidBoard(int boardSize)
    {
        // Arrange
        int maxAttempts = 50;
        bool foundSolvable = false;

        // Act - Try generating boards until we find a solvable one
        for (int i = 0; i < maxAttempts; i++)
        {
            var regions = _generator.GenerateRegions(boardSize);
            if (_generator.IsSolvable(regions, boardSize))
            {
                foundSolvable = true;
                break;
            }
        }

        // Assert
        Assert.True(foundSolvable, $"Should generate at least one solvable board within {maxAttempts} attempts");
    }

    [Fact]
    public void IsSolvable_ShouldReturnFalseForImpossibleBoard()
    {
        // Arrange - Create an impossible board where region 0 has all cells in same row
        int boardSize = 4;
        var regions = new int[boardSize, boardSize];
        
        // Region 0 takes entire first row (impossible to place queen)
        for (int c = 0; c < boardSize; c++)
            regions[0, c] = 0;
        
        // Other regions take remaining cells
        int regionId = 1;
        for (int r = 1; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                regions[r, c] = regionId++;
            }
        }

        // Act
        bool isSolvable = _generator.IsSolvable(regions, boardSize);

        // Assert
        Assert.False(isSolvable, "Board with entire region in one row should be unsolvable");
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    public void GenerateRegions_ShouldCreateConnectedRegions(int boardSize)
    {
        // Arrange & Act
        var regions = _generator.GenerateRegions(boardSize);

        // Assert - Each region should be contiguous (flood-fill test)
        for (int regionId = 0; regionId < boardSize; regionId++)
        {
            Assert.True(IsRegionConnected(regions, boardSize, regionId), 
                $"Region {regionId} should be contiguous");
        }
    }

    private bool IsRegionConnected(int[,] regions, int boardSize, int regionId)
    {
        // Find all cells in this region
        var regionCells = new List<(int r, int c)>();
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (regions[r, c] == regionId)
                    regionCells.Add((r, c));
            }
        }

        if (regionCells.Count == 0)
            return false;

        // Flood fill from first cell
        var visited = new HashSet<(int, int)>();
        var queue = new Queue<(int, int)>();
        queue.Enqueue(regionCells[0]);
        visited.Add(regionCells[0]);

        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                int newR = r + dr[i];
                int newC = c + dc[i];
                if (newR >= 0 && newR < boardSize && newC >= 0 && newC < boardSize &&
                    regions[newR, newC] == regionId && !visited.Contains((newR, newC)))
                {
                    visited.Add((newR, newC));
                    queue.Enqueue((newR, newC));
                }
            }
        }

        // All region cells should be visited if region is connected
        return visited.Count == regionCells.Count;
    }

    private string RegionsToString(int[,] regions, int boardSize)
    {
        var result = "";
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                result += regions[r, c].ToString();
            }
        }
        return result;
    }
}
