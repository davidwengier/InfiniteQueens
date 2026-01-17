namespace InfiniteQueens.Services;

public class BoardGenerator
{
    private readonly Random _random = new();

    public int[,] GenerateRegions(int boardSize)
    {
        var regions = new int[boardSize, boardSize];
        var assigned = new bool[boardSize, boardSize];
        
        // Initialize all cells as unassigned
        for (int r = 0; r < boardSize; r++)
            for (int c = 0; c < boardSize; c++)
                regions[r, c] = -1;

        // Create N seed points for N regions
        var seeds = new List<(int row, int col)>();
        while (seeds.Count < boardSize)
        {
            int r = _random.Next(boardSize);
            int c = _random.Next(boardSize);
            if (!assigned[r, c])
            {
                seeds.Add((r, c));
                assigned[r, c] = true;
                regions[r, c] = seeds.Count - 1;
            }
        }

        // Expand regions using flood-fill from seeds
        var queue = new Queue<(int row, int col, int region)>();
        foreach (var (seed, index) in seeds.Select((s, i) => (s, i)))
        {
            queue.Enqueue((seed.row, seed.col, index));
        }

        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };

        while (queue.Count > 0)
        {
            var (row, col, region) = queue.Dequeue();

            // Try to expand to ALL adjacent unassigned cells
            for (int i = 0; i < 4; i++)
            {
                int newR = row + dr[i];
                int newC = col + dc[i];
                if (newR >= 0 && newR < boardSize && newC >= 0 && newC < boardSize && !assigned[newR, newC])
                {
                    regions[newR, newC] = region;
                    assigned[newR, newC] = true;
                    queue.Enqueue((newR, newC, region));
                }
            }
        }
        
        // Safety check: assign any remaining unassigned cells to a random adjacent region
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (regions[r, c] == -1)
                {
                    // Find an adjacent assigned cell
                    for (int i = 0; i < 4; i++)
                    {
                        int adjR = r + dr[i];
                        int adjC = c + dc[i];
                        if (adjR >= 0 && adjR < boardSize && adjC >= 0 && adjC < boardSize && regions[adjR, adjC] != -1)
                        {
                            regions[r, c] = regions[adjR, adjC];
                            break;
                        }
                    }
                    // If still unassigned (shouldn't happen), assign to region 0
                    if (regions[r, c] == -1)
                    {
                        regions[r, c] = 0;
                    }
                }
            }
        }

        return regions;
    }

    public bool IsSolvable(int[,] regions, int boardSize)
    {
        var testBoard = new bool[boardSize, boardSize];
        return SolveBacktrack(0, testBoard, regions, boardSize);
    }

    private bool SolveBacktrack(int region, bool[,] testBoard, int[,] regions, int boardSize)
    {
        if (region == boardSize)
            return true;

        // Find all cells in this region
        var cells = new List<(int row, int col)>();
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (regions[r, c] == region)
                    cells.Add((r, c));
            }
        }

        // Try placing a queen in each cell of this region
        foreach (var (row, col) in cells)
        {
            if (!CanPlaceQueen(row, col, testBoard, boardSize))
                continue;

            testBoard[row, col] = true;

            if (SolveBacktrack(region + 1, testBoard, regions, boardSize))
                return true;

            testBoard[row, col] = false;
        }

        return false;
    }

    private bool CanPlaceQueen(int row, int col, bool[,] testBoard, int boardSize)
    {
        // Check row
        for (int c = 0; c < boardSize; c++)
            if (testBoard[row, c])
                return false;

        // Check column
        for (int r = 0; r < boardSize; r++)
            if (testBoard[r, col])
                return false;

        // Check only immediately adjacent diagonal cells (distance = 1)
        int[] dr = { -1, -1, 1, 1 };
        int[] dc = { -1, 1, -1, 1 };
        for (int i = 0; i < 4; i++)
        {
            int newR = row + dr[i];
            int newC = col + dc[i];
            if (newR >= 0 && newR < boardSize && newC >= 0 && newC < boardSize)
            {
                if (testBoard[newR, newC])
                    return false;
            }
        }

        return true;
    }
}
