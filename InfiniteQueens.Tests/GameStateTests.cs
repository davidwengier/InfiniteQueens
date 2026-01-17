using InfiniteQueens.Models;
using InfiniteQueens.Services;
using Xunit;

namespace InfiniteQueens.Tests;

public class GameStateTests
{
    [Fact]
    public void Constructor_ShouldInitializeEmptyBoard()
    {
        // Arrange & Act
        var gameState = new GameState(6);

        // Assert
        Assert.Equal(6, gameState.BoardSize);
        for (int r = 0; r < 6; r++)
        {
            for (int c = 0; c < 6; c++)
            {
                Assert.Equal(CellState.Empty, gameState.Board[r, c]);
                Assert.False(gameState.AutoMarks[r, c]);
            }
        }
    }

    [Fact]
    public void SetCell_ShouldUpdateCellState()
    {
        // Arrange
        var gameState = new GameState(6);

        // Act
        gameState.SetCell(2, 3, CellState.Queen);

        // Assert
        Assert.Equal(CellState.Queen, gameState.GetCell(2, 3));
    }

    [Fact]
    public void HasConflict_ShouldDetectSameRowConflict()
    {
        // Arrange
        var gameState = new GameState(6);
        var regions = CreateSimpleRegions(6);
        gameState.SetRegions(regions);
        
        gameState.SetCell(2, 1, CellState.Queen);
        gameState.SetCell(2, 4, CellState.Queen);

        // Act & Assert
        Assert.True(gameState.HasConflict(2, 1));
        Assert.True(gameState.HasConflict(2, 4));
    }

    [Fact]
    public void HasConflict_ShouldDetectSameColumnConflict()
    {
        // Arrange
        var gameState = new GameState(6);
        var regions = CreateSimpleRegions(6);
        gameState.SetRegions(regions);
        
        gameState.SetCell(1, 3, CellState.Queen);
        gameState.SetCell(4, 3, CellState.Queen);

        // Act & Assert
        Assert.True(gameState.HasConflict(1, 3));
        Assert.True(gameState.HasConflict(4, 3));
    }

    [Fact]
    public void HasConflict_ShouldDetectAdjacentDiagonalConflict()
    {
        // Arrange
        var gameState = new GameState(6);
        var regions = CreateSimpleRegions(6);
        gameState.SetRegions(regions);
        
        gameState.SetCell(2, 2, CellState.Queen);
        gameState.SetCell(3, 3, CellState.Queen); // Adjacent diagonal

        // Act & Assert
        Assert.True(gameState.HasConflict(2, 2));
        Assert.True(gameState.HasConflict(3, 3));
    }

    [Fact]
    public void HasConflict_ShouldNotDetectNonAdjacentDiagonal()
    {
        // Arrange
        var gameState = new GameState(6);
        var regions = CreateSimpleRegions(6);
        gameState.SetRegions(regions);
        
        gameState.SetCell(0, 0, CellState.Queen);
        gameState.SetCell(2, 2, CellState.Queen); // Not adjacent diagonal

        // Act & Assert
        Assert.False(gameState.HasConflict(0, 0));
        Assert.False(gameState.HasConflict(2, 2));
    }

    [Fact]
    public void HasConflict_ShouldDetectSameRegionConflict()
    {
        // Arrange
        var gameState = new GameState(6);
        var regions = new int[6, 6];
        // Put cells (0,0) and (1,2) in same region
        regions[0, 0] = 0;
        regions[1, 2] = 0;
        // Fill rest with different regions
        int regionId = 1;
        for (int r = 0; r < 6; r++)
        {
            for (int c = 0; c < 6; c++)
            {
                if ((r == 0 && c == 0) || (r == 1 && c == 2))
                    continue;
                regions[r, c] = regionId++;
            }
        }
        gameState.SetRegions(regions);
        
        gameState.SetCell(0, 0, CellState.Queen);
        gameState.SetCell(1, 2, CellState.Queen);

        // Act & Assert
        Assert.True(gameState.HasConflict(0, 0));
        Assert.True(gameState.HasConflict(1, 2));
    }

    [Fact]
    public void AutoMarkInvalidSquares_ShouldMarkRowAndColumn()
    {
        // Arrange
        var gameState = new GameState(6);
        var regions = CreateSimpleRegions(6);
        gameState.SetRegions(regions);
        gameState.SetCell(2, 3, CellState.Queen);

        // Act
        gameState.AutoMarkInvalidSquares(2, 3);

        // Assert
        // Check row 2
        for (int c = 0; c < 6; c++)
        {
            if (c != 3)
                Assert.True(gameState.AutoMarks[2, c], $"Cell (2,{c}) should be auto-marked");
        }
        
        // Check column 3
        for (int r = 0; r < 6; r++)
        {
            if (r != 2)
                Assert.True(gameState.AutoMarks[r, 3], $"Cell ({r},3) should be auto-marked");
        }
    }

    [Fact]
    public void AutoMarkInvalidSquares_ShouldMarkAdjacentDiagonals()
    {
        // Arrange
        var gameState = new GameState(6);
        var regions = CreateSimpleRegions(6);
        gameState.SetRegions(regions);
        gameState.SetCell(2, 2, CellState.Queen);

        // Act
        gameState.AutoMarkInvalidSquares(2, 2);

        // Assert - 4 adjacent diagonal cells
        Assert.True(gameState.AutoMarks[1, 1]);
        Assert.True(gameState.AutoMarks[1, 3]);
        Assert.True(gameState.AutoMarks[3, 1]);
        Assert.True(gameState.AutoMarks[3, 3]);
    }

    [Fact]
    public void CheckWin_ShouldReturnFalseForEmptyBoard()
    {
        // Arrange
        var gameState = new GameState(4);
        var regions = CreateSimpleRegions(4);
        gameState.SetRegions(regions);

        // Act & Assert
        Assert.False(gameState.CheckWin());
    }

    [Fact]
    public void CheckWin_ShouldReturnFalseWhenNotAllRegionsHaveQueen()
    {
        // Arrange
        var gameState = new GameState(4);
        var regions = CreateSimpleRegions(4);
        gameState.SetRegions(regions);
        
        // Only place 3 queens instead of 4
        gameState.SetCell(0, 0, CellState.Queen);
        gameState.SetCell(1, 2, CellState.Queen);
        gameState.SetCell(2, 1, CellState.Queen);

        // Act & Assert
        Assert.False(gameState.CheckWin());
    }

    [Fact]
    public void CheckWin_ShouldReturnFalseWhenQueensHaveConflicts()
    {
        // Arrange
        var gameState = new GameState(4);
        var regions = CreateSimpleRegions(4);
        gameState.SetRegions(regions);
        
        // Place queens with conflicts (same row)
        gameState.SetCell(0, 0, CellState.Queen);
        gameState.SetCell(0, 1, CellState.Queen); // Conflict!
        gameState.SetCell(2, 2, CellState.Queen);
        gameState.SetCell(3, 3, CellState.Queen);

        // Act & Assert
        Assert.False(gameState.CheckWin());
    }

    [Fact]
    public void ResetBoard_ShouldClearAllCells()
    {
        // Arrange
        var gameState = new GameState(6);
        gameState.SetCell(0, 0, CellState.Queen);
        gameState.SetCell(1, 1, CellState.ManualCross);
        gameState.SetAutoMark(2, 2, true);

        // Act
        gameState.ResetBoard();

        // Assert
        for (int r = 0; r < 6; r++)
        {
            for (int c = 0; c < 6; c++)
            {
                Assert.Equal(CellState.Empty, gameState.Board[r, c]);
                Assert.False(gameState.AutoMarks[r, c]);
            }
        }
    }

    private int[,] CreateSimpleRegions(int boardSize)
    {
        // Each row is a separate region
        var regions = new int[boardSize, boardSize];
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                regions[r, c] = r;
            }
        }
        return regions;
    }
}
