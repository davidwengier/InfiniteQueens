using InfiniteQueens.Models;

namespace InfiniteQueens.Services;

public class GameState
{
    private int _boardSize;
    private CellState[,] _board;
    private bool[,] _autoMarks;
    private int[,] _regions;

    public int BoardSize => _boardSize;
    public CellState[,] Board => _board;
    public bool[,] AutoMarks => _autoMarks;
    public int[,] Regions => _regions;

    public GameState(int boardSize)
    {
        _boardSize = boardSize;
        _board = new CellState[boardSize, boardSize];
        _autoMarks = new bool[boardSize, boardSize];
        _regions = new int[boardSize, boardSize];
    }

    public void SetRegions(int[,] regions)
    {
        _regions = regions;
    }

    public void ResetBoard()
    {
        _board = new CellState[_boardSize, _boardSize];
        _autoMarks = new bool[_boardSize, _boardSize];
    }

    public void SetCell(int row, int col, CellState state)
    {
        _board[row, col] = state;
    }

    public CellState GetCell(int row, int col)
    {
        return _board[row, col];
    }

    public void SetAutoMark(int row, int col, bool value)
    {
        _autoMarks[row, col] = value;
    }

    public bool GetAutoMark(int row, int col)
    {
        return _autoMarks[row, col];
    }

    public bool HasConflict(int row, int col)
    {
        if (_board[row, col] != CellState.Queen)
            return false;

        // Check for conflicts with other queens
        for (int r = 0; r < _boardSize; r++)
        {
            for (int c = 0; c < _boardSize; c++)
            {
                if (r == row && c == col)
                    continue;

                if (_board[r, c] != CellState.Queen)
                    continue;

                // Same row or column
                if (r == row || c == col)
                    return true;

                // Same region
                if (_regions[r, c] == _regions[row, col])
                    return true;

                // Adjacent diagonal
                if (Math.Abs(r - row) == 1 && Math.Abs(c - col) == 1)
                    return true;
            }
        }

        return false;
    }

    public bool CheckWin()
    {
        // Count queens in each region
        int[] queensPerRegion = new int[_boardSize];

        for (int r = 0; r < _boardSize; r++)
        {
            for (int c = 0; c < _boardSize; c++)
            {
                if (_board[r, c] == CellState.Queen)
                {
                    queensPerRegion[_regions[r, c]]++;

                    // If any queen has a conflict, not solved
                    if (HasConflict(r, c))
                        return false;
                }
            }
        }

        // Check if each region has exactly one queen
        for (int i = 0; i < _boardSize; i++)
        {
            if (queensPerRegion[i] != 1)
                return false;
        }

        return true;
    }

    public void AutoMarkInvalidSquares(int queenRow, int queenCol)
    {
        for (int r = 0; r < _boardSize; r++)
        {
            for (int c = 0; c < _boardSize; c++)
            {
                // Skip if already marked or has a queen
                if (_board[r, c] != CellState.Empty)
                    continue;

                bool shouldMark = false;

                // Same row or column
                if (r == queenRow || c == queenCol)
                    shouldMark = true;

                // Same region
                if (_regions[r, c] == _regions[queenRow, queenCol])
                    shouldMark = true;

                // Adjacent diagonal
                if (Math.Abs(r - queenRow) == 1 && Math.Abs(c - queenCol) == 1)
                    shouldMark = true;

                if (shouldMark)
                {
                    _autoMarks[r, c] = true;
                }
            }
        }
    }

    public void RemoveAutoMarksFromQueen(int queenRow, int queenCol)
    {
        for (int r = 0; r < _boardSize; r++)
        {
            for (int c = 0; c < _boardSize; c++)
            {
                // Skip if not an auto-mark or if it's a manually placed mark/queen
                if (!_autoMarks[r, c] || _board[r, c] != CellState.Empty)
                    continue;

                // Check if this auto-mark was created by this queen
                bool wasFromThisQueen = false;

                // Same row or column
                if (r == queenRow || c == queenCol)
                    wasFromThisQueen = true;

                // Same region
                if (_regions[r, c] == _regions[queenRow, queenCol])
                    wasFromThisQueen = true;

                // Adjacent diagonal
                if (Math.Abs(r - queenRow) == 1 && Math.Abs(c - queenCol) == 1)
                    wasFromThisQueen = true;

                if (!wasFromThisQueen)
                    continue;

                // Check if any other queen still invalidates this square
                bool stillInvalid = false;
                for (int qr = 0; qr < _boardSize; qr++)
                {
                    for (int qc = 0; qc < _boardSize; qc++)
                    {
                        if (qr == queenRow && qc == queenCol)
                            continue;

                        if (_board[qr, qc] != CellState.Queen)
                            continue;

                        // Check if this other queen invalidates the cell
                        if (qr == r || qc == c)
                            stillInvalid = true;
                        if (_regions[qr, qc] == _regions[r, c])
                            stillInvalid = true;
                        if (Math.Abs(qr - r) == 1 && Math.Abs(qc - c) == 1)
                            stillInvalid = true;

                        if (stillInvalid)
                            break;
                    }
                    if (stillInvalid)
                        break;
                }

                // If no other queen invalidates this cell, remove the auto-mark
                if (!stillInvalid)
                {
                    _autoMarks[r, c] = false;
                }
            }
        }
    }
}
