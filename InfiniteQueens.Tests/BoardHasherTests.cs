using InfiniteQueens.Services;
using Xunit;

namespace InfiniteQueens.Tests;

public class BoardHasherTests
{
    [Fact]
    public void GetRotationInvariantHash_ShouldReturnSameHashForAllRotations()
    {
        // Arrange - Create a simple asymmetric board
        int[,] board = new int[4, 4]
        {
            { 0, 0, 1, 1 },
            { 0, 2, 2, 1 },
            { 3, 2, 2, 1 },
            { 3, 3, 3, 1 }
        };
        
        // Create actual rotations using the same logic as the hasher
        int[,] board90 = Rotate90Clockwise(board);
        int[,] board180 = Rotate90Clockwise(board90);
        int[,] board270 = Rotate90Clockwise(board180);

        // Act
        string hash0 = BoardHasher.GetRotationInvariantHash(board);
        string hash90 = BoardHasher.GetRotationInvariantHash(board90);
        string hash180 = BoardHasher.GetRotationInvariantHash(board180);
        string hash270 = BoardHasher.GetRotationInvariantHash(board270);

        // Assert
        Assert.Equal(hash0, hash90);
        Assert.Equal(hash0, hash180);
        Assert.Equal(hash0, hash270);
    }
    
    private static int[,] Rotate90Clockwise(int[,] board)
    {
        int size = board.GetLength(0);
        var rotated = new int[size, size];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                rotated[col, size - 1 - row] = board[row, col];
            }
        }
        return rotated;
    }
    
    [Fact]
    public void GetRotationInvariantHash_ShouldReturnDifferentHashesForDifferentBoards()
    {
        // Arrange
        int[,] board1 = new int[4, 4]
        {
            { 0, 0, 1, 1 },
            { 0, 2, 2, 1 },
            { 3, 2, 2, 1 },
            { 3, 3, 3, 1 }
        };
        
        int[,] board2 = new int[4, 4]
        {
            { 0, 0, 0, 1 },
            { 0, 2, 2, 1 },
            { 3, 2, 2, 1 },
            { 3, 3, 3, 1 }
        };

        // Act
        string hash1 = BoardHasher.GetRotationInvariantHash(board1);
        string hash2 = BoardHasher.GetRotationInvariantHash(board2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }
    
    [Fact]
    public void GetRotationInvariantHash_ShouldReturnConsistentHash()
    {
        // Arrange
        int[,] board = new int[3, 3]
        {
            { 0, 1, 2 },
            { 0, 1, 2 },
            { 0, 1, 2 }
        };

        // Act
        string hash1 = BoardHasher.GetRotationInvariantHash(board);
        string hash2 = BoardHasher.GetRotationInvariantHash(board);

        // Assert
        Assert.Equal(hash1, hash2);
    }
    
    [Fact]
    public void GetRotationInvariantHash_ShouldReturnFixedLengthHash()
    {
        // Arrange
        int[,] board = new int[5, 5]
        {
            { 0, 0, 1, 2, 2 },
            { 0, 1, 1, 2, 3 },
            { 4, 1, 3, 3, 3 },
            { 4, 4, 4, 3, 3 },
            { 4, 4, 4, 3, 3 }
        };

        // Act
        string hash = BoardHasher.GetRotationInvariantHash(board);

        // Assert
        Assert.Equal(12, hash.Length); // 48 bits = 12 hex characters
    }
}
