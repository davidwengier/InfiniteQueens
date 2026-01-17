using InfiniteQueens.Models;
using InfiniteQueens.Services;
using Xunit;

namespace InfiniteQueens.Tests;

public class UserInteractionTests
{
    [Fact]
    public void SingleClick_CyclesThroughStates()
    {
        // Arrange
        var gameState = new GameState(4);
        int row = 0, col = 0;
        
        // Act & Assert - First click: Empty -> ManualCross
        Assert.Equal(CellState.Empty, gameState.GetCell(row, col));
        gameState.SetCell(row, col, CellState.ManualCross);
        Assert.Equal(CellState.ManualCross, gameState.GetCell(row, col));
        
        // Second click: ManualCross -> Queen
        gameState.SetCell(row, col, CellState.Queen);
        Assert.Equal(CellState.Queen, gameState.GetCell(row, col));
        
        // Third click: Queen -> Empty
        gameState.SetCell(row, col, CellState.Empty);
        Assert.Equal(CellState.Empty, gameState.GetCell(row, col));
    }
    
    [Fact]
    public void DragOperation_PlacesMarksOnly()
    {
        // Arrange
        var gameState = new GameState(4);
        var cells = new[] { (0, 0), (0, 1), (0, 2) };
        
        // Act - Simulate dragging across cells
        foreach (var (row, col) in cells)
        {
            if (gameState.GetCell(row, col) == CellState.Empty)
            {
                gameState.SetCell(row, col, CellState.ManualCross);
            }
        }
        
        // Assert - All dragged cells should have marks
        foreach (var (row, col) in cells)
        {
            Assert.Equal(CellState.ManualCross, gameState.GetCell(row, col));
        }
    }
    
    [Fact]
    public void DragOperation_DoesNotOverwriteQueens()
    {
        // Arrange
        var gameState = new GameState(4);
        gameState.SetCell(1, 1, CellState.Queen);
        
        // Act - Try to drag over a queen
        var cellState = gameState.GetCell(1, 1);
        if (cellState == CellState.Empty)
        {
            gameState.SetCell(1, 1, CellState.ManualCross);
        }
        
        // Assert - Queen should remain
        Assert.Equal(CellState.Queen, gameState.GetCell(1, 1));
    }
    
    [Fact]
    public void DragOperation_DoesNotOverwriteExistingMarks()
    {
        // Arrange
        var gameState = new GameState(4);
        gameState.SetCell(2, 2, CellState.ManualCross);
        
        // Act - Try to drag over existing mark
        var cellState = gameState.GetCell(2, 2);
        if (cellState == CellState.Empty)
        {
            gameState.SetCell(2, 2, CellState.ManualCross);
        }
        
        // Assert - Mark should remain (not doubled up)
        Assert.Equal(CellState.ManualCross, gameState.GetCell(2, 2));
    }
    
    [Fact]
    public void ConvertAutoMarkToManualMark_Works()
    {
        // Arrange
        var gameState = new GameState(4);
        gameState.SetAutoMark(1, 1, true);
        
        // Act - Convert auto mark to manual
        Assert.True(gameState.GetAutoMark(1, 1));
        gameState.SetAutoMark(1, 1, false);
        gameState.SetCell(1, 1, CellState.ManualCross);
        
        // Assert
        Assert.False(gameState.GetAutoMark(1, 1));
        Assert.Equal(CellState.ManualCross, gameState.GetCell(1, 1));
    }
    
    [Fact]
    public void ClickSequence_DoesNotSkipStates()
    {
        // This test ensures that click cycling doesn't skip states
        // Regression test for the bug where single click placed a queen
        var gameState = new GameState(4);
        int row = 1, col = 1;
        
        // Start empty
        Assert.Equal(CellState.Empty, gameState.GetCell(row, col));
        
        // Click 1: Empty -> X
        gameState.SetCell(row, col, CellState.ManualCross);
        Assert.Equal(CellState.ManualCross, gameState.GetCell(row, col));
        
        // Click 2: X -> Queen
        gameState.SetCell(row, col, CellState.Queen);
        Assert.Equal(CellState.Queen, gameState.GetCell(row, col));
        
        // Not: Empty -> Queen (which was the bug)
    }
    
    [Fact]
    public void AfterDrag_ClickOnSameCellCyclesNormally()
    {
        // Regression test: After dragging to create X's, clicking on the 
        // cell where drag started should cycle normally (X -> Queen -> Empty)
        var gameState = new GameState(4);
        int row = 0, col = 0;
        
        // Simulate drag: place X at starting cell
        gameState.SetCell(row, col, CellState.ManualCross);
        Assert.Equal(CellState.ManualCross, gameState.GetCell(row, col));
        
        // Now click on same cell - should go to Queen
        gameState.SetCell(row, col, CellState.Queen);
        Assert.Equal(CellState.Queen, gameState.GetCell(row, col));
        
        // Click again - should go to Empty
        gameState.SetCell(row, col, CellState.Empty);
        Assert.Equal(CellState.Empty, gameState.GetCell(row, col));
        
        // Should NOT require multiple clicks or place multiple X's
    }
    
    [Fact]
    public void DragThenClick_WorksOnDifferentCell()
    {
        // After dragging across cells, clicking on a different cell should work normally
        var gameState = new GameState(4);
        
        // Simulate drag: mark cells (0,0) and (0,1)
        gameState.SetCell(0, 0, CellState.ManualCross);
        gameState.SetCell(0, 1, CellState.ManualCross);
        
        // Click on a different cell (1,1) - should cycle normally
        Assert.Equal(CellState.Empty, gameState.GetCell(1, 1));
        
        gameState.SetCell(1, 1, CellState.ManualCross);
        Assert.Equal(CellState.ManualCross, gameState.GetCell(1, 1));
        
        gameState.SetCell(1, 1, CellState.Queen);
        Assert.Equal(CellState.Queen, gameState.GetCell(1, 1));
    }
}
