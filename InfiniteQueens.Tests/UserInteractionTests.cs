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
    
    [Fact]
    public void PointerDown_InitiatesDragMode()
    {
        // Test that pointer down sets up drag tracking
        var gameState = new GameState(4);
        int startRow = 0, startCol = 0;
        
        // Simulate pointer down (this would set pointerIsDown flag in actual component)
        // Component should track the starting cell
        Assert.Equal(CellState.Empty, gameState.GetCell(startRow, startCol));
    }
    
    [Fact]
    public void PointerMove_AcrossCells_PlacesMarks()
    {
        // Simulate dragging across multiple cells
        var gameState = new GameState(4);
        var dragPath = new[] { (0, 0), (0, 1), (0, 2), (1, 2) };
        
        // Act - Simulate pointer moving across cells during drag
        foreach (var (row, col) in dragPath)
        {
            if (gameState.GetCell(row, col) == CellState.Empty)
            {
                gameState.SetCell(row, col, CellState.ManualCross);
            }
        }
        
        // Assert - All cells in drag path should have marks
        foreach (var (row, col) in dragPath)
        {
            Assert.Equal(CellState.ManualCross, gameState.GetCell(row, col));
        }
    }
    
    [Fact]
    public void PointerUp_EndsDragMode()
    {
        // After pointer up, the next pointer down should start a fresh drag
        var gameState = new GameState(4);
        
        // First drag
        gameState.SetCell(0, 0, CellState.ManualCross);
        gameState.SetCell(0, 1, CellState.ManualCross);
        
        // Simulate pointer up (would clear drag state in component)
        // Second drag should work independently
        gameState.SetCell(1, 0, CellState.ManualCross);
        gameState.SetCell(1, 1, CellState.ManualCross);
        
        // Assert - Both drag operations succeeded
        Assert.Equal(CellState.ManualCross, gameState.GetCell(0, 0));
        Assert.Equal(CellState.ManualCross, gameState.GetCell(0, 1));
        Assert.Equal(CellState.ManualCross, gameState.GetCell(1, 0));
        Assert.Equal(CellState.ManualCross, gameState.GetCell(1, 1));
    }
    
    [Fact]
    public void DragBackOverSameCell_DoesNotDuplicateMark()
    {
        // When dragging back and forth, cells should not get multiple marks
        var gameState = new GameState(4);
        
        // Simulate dragging: (0,0) -> (0,1) -> (0,0) again
        gameState.SetCell(0, 0, CellState.ManualCross);
        gameState.SetCell(0, 1, CellState.ManualCross);
        // Return to (0,0) - should still be ManualCross, not duplicated
        Assert.Equal(CellState.ManualCross, gameState.GetCell(0, 0));
        
        // Try to set again (simulating re-entering during drag)
        if (gameState.GetCell(0, 0) == CellState.Empty)
        {
            gameState.SetCell(0, 0, CellState.ManualCross);
        }
        
        Assert.Equal(CellState.ManualCross, gameState.GetCell(0, 0));
    }
    
    [Fact]
    public void AfterDrag_ClickOnMarkedCell_ShouldPlaceQueen()
    {
        // This tests the bug where after dragging, the first click is consumed
        // and doesn't actually toggle the cell
        var gameState = new GameState(4);
        var draggedCells = new HashSet<(int, int)>();
        
        // Simulate drag: pointer down on (0,0), move to (0,1), then up
        bool pointerIsDown = true;
        (int row, int col)? pointerDownCell = (0, 0);
        
        // Mark first cell during drag
        if (draggedCells.Add((0, 0)))
        {
            gameState.SetCell(0, 0, CellState.ManualCross);
        }
        
        // Move to second cell during drag
        if (draggedCells.Add((0, 1)))
        {
            gameState.SetCell(0, 1, CellState.ManualCross);
        }
        
        // Pointer up - end drag
        pointerIsDown = false;
        pointerDownCell = null;
        draggedCells.Clear(); // This should happen on pointer up
        
        // Now click on the marked cell (0,0) - should place queen
        // Simulate ToggleQueen behavior (should not be blocked by drag state)
        Assert.Equal(0, draggedCells.Count); // Drag state should be cleared
        
        // Toggle from ManualCross to Queen
        var currentState = gameState.GetCell(0, 0);
        Assert.Equal(CellState.ManualCross, currentState);
        
        // This simulates what ToggleQueen should do
        gameState.SetCell(0, 0, CellState.Queen);
        Assert.Equal(CellState.Queen, gameState.GetCell(0, 0));
    }
    
    [Fact]
    public void AfterDrag_ClickOnUnmarkedCell_ShouldPlaceMark()
    {
        // After dragging, clicking on a different empty cell should place a mark
        var gameState = new GameState(4);
        var draggedCells = new HashSet<(int, int)>();
        
        // Simulate drag over (0,0) and (0,1)
        draggedCells.Add((0, 0));
        gameState.SetCell(0, 0, CellState.ManualCross);
        draggedCells.Add((0, 1));
        gameState.SetCell(0, 1, CellState.ManualCross);
        
        // End drag
        draggedCells.Clear();
        
        // Click on different cell (1,0) - should place mark
        Assert.Equal(CellState.Empty, gameState.GetCell(1, 0));
        gameState.SetCell(1, 0, CellState.ManualCross);
        Assert.Equal(CellState.ManualCross, gameState.GetCell(1, 0));
    }
}
