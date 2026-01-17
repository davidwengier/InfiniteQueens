using InfiniteQueens.Services;
using Xunit;

namespace InfiniteQueens.Tests;

public class GameHistoryTests
{
    [Fact]
    public void HasHistory_ShouldReturnFalseForNewHistory()
    {
        // Arrange
        var history = new GameHistory();

        // Act & Assert
        Assert.False(history.HasHistory(6));
    }

    [Fact]
    public void AddResult_ShouldAddToHistory()
    {
        // Arrange
        var history = new GameHistory();

        // Act
        history.AddResult(6, 1, TimeSpan.FromMinutes(5));

        // Assert
        Assert.True(history.HasHistory(6));
        var results = history.GetHistory(6);
        Assert.Single(results);
        Assert.Equal(1, results[0].GameNumber);
        Assert.Equal(TimeSpan.FromMinutes(5), results[0].Time);
    }

    [Fact]
    public void GetHistory_ShouldReturnEmptyForUnusedBoardSize()
    {
        // Arrange
        var history = new GameHistory();

        // Act
        var results = history.GetHistory(8);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void GetHistory_ShouldReturnSortedByTime()
    {
        // Arrange
        var history = new GameHistory();
        history.AddResult(6, 1, TimeSpan.FromMinutes(10));
        history.AddResult(6, 2, TimeSpan.FromMinutes(3));
        history.AddResult(6, 3, TimeSpan.FromMinutes(7));

        // Act
        var results = history.GetHistory(6);

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal(TimeSpan.FromMinutes(3), results[0].Time); // Fastest first
        Assert.Equal(TimeSpan.FromMinutes(7), results[1].Time);
        Assert.Equal(TimeSpan.FromMinutes(10), results[2].Time);
    }

    [Fact]
    public void GetNextGameNumber_ShouldStartAtOne()
    {
        // Arrange
        var history = new GameHistory();

        // Act
        var gameNumber = history.GetNextGameNumber(6);

        // Assert
        Assert.Equal(1, gameNumber);
    }

    [Fact]
    public void GetNextGameNumber_ShouldIncrementForSameBoardSize()
    {
        // Arrange
        var history = new GameHistory();

        // Act
        var game1 = history.GetNextGameNumber(6);
        var game2 = history.GetNextGameNumber(6);
        var game3 = history.GetNextGameNumber(6);

        // Assert
        Assert.Equal(1, game1);
        Assert.Equal(2, game2);
        Assert.Equal(3, game3);
    }

    [Fact]
    public void GetNextGameNumber_ShouldTrackDifferentBoardSizesSeparately()
    {
        // Arrange
        var history = new GameHistory();

        // Act
        var size6Game1 = history.GetNextGameNumber(6);
        var size8Game1 = history.GetNextGameNumber(8);
        var size6Game2 = history.GetNextGameNumber(6);
        var size8Game2 = history.GetNextGameNumber(8);

        // Assert
        Assert.Equal(1, size6Game1);
        Assert.Equal(1, size8Game1);
        Assert.Equal(2, size6Game2);
        Assert.Equal(2, size8Game2);
    }

    [Fact]
    public void GameHistory_ShouldMaintainSeparateHistoriesPerBoardSize()
    {
        // Arrange
        var history = new GameHistory();
        
        // Act
        history.AddResult(4, 1, TimeSpan.FromMinutes(2));
        history.AddResult(6, 1, TimeSpan.FromMinutes(5));
        history.AddResult(4, 2, TimeSpan.FromMinutes(3));

        // Assert
        var size4History = history.GetHistory(4);
        var size6History = history.GetHistory(6);
        
        Assert.Equal(2, size4History.Count);
        Assert.Single(size6History);
    }

    [Fact]
    public void AddResult_ShouldAllowMultipleResultsWithSameTime()
    {
        // Arrange
        var history = new GameHistory();
        var time = TimeSpan.FromMinutes(5);

        // Act
        history.AddResult(6, 1, time);
        history.AddResult(6, 2, time);
        history.AddResult(6, 3, time);

        // Assert
        var results = history.GetHistory(6);
        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.Equal(time, r.Time));
    }

    [Fact]
    public void GetHistory_ShouldPreserveGameNumbers()
    {
        // Arrange
        var history = new GameHistory();
        
        // Act - Add out of order
        history.AddResult(6, 5, TimeSpan.FromMinutes(10));
        history.AddResult(6, 2, TimeSpan.FromMinutes(3));
        history.AddResult(6, 8, TimeSpan.FromMinutes(7));

        // Assert - Should be sorted by time but preserve original game numbers
        var results = history.GetHistory(6);
        Assert.Equal(2, results[0].GameNumber); // Game 2 was fastest
        Assert.Equal(8, results[1].GameNumber); // Game 8 was second
        Assert.Equal(5, results[2].GameNumber); // Game 5 was slowest
    }
}
