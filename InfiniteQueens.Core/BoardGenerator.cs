namespace InfiniteQueens.Services;

public enum RegionGeneratorType
{
    Organic,
    ShapeTemplate
}

public class BoardGenerator
{
    private readonly ShapeTemplateGenerator _shapeGenerator = new();

    public int[,] GenerateRegions(int boardSize, int? seed = null, RegionGeneratorType type = RegionGeneratorType.Organic)
    {
        return type switch
        {
            RegionGeneratorType.ShapeTemplate => _shapeGenerator.GenerateRegions(boardSize, seed),
            _ => GenerateOrganicRegions(boardSize, seed)
        };
    }

    private int[,] GenerateOrganicRegions(int boardSize, int? seed = null)
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        
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
            int r = random.Next(boardSize);
            int c = random.Next(boardSize);
            if (!assigned[r, c])
            {
                seeds.Add((r, c));
                assigned[r, c] = true;
                regions[r, c] = seeds.Count - 1;
            }
        }

        // Random frontier with directional bias for winding, organic shapes
        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };
        
        // Track frontier with direction history for momentum
        var frontier = new List<(int row, int col, int region, int lastDir)>();
        
        // Initialize frontier with neighbors of all seeds
        for (int i = 0; i < seeds.Count; i++)
        {
            var (r, c) = seeds[i];
            for (int d = 0; d < 4; d++)
            {
                int newR = r + dr[d];
                int newC = c + dc[d];
                if (newR >= 0 && newR < boardSize && newC >= 0 && newC < boardSize && !assigned[newR, newC])
                {
                    frontier.Add((newR, newC, i, d));
                }
            }
        }
        
        // Process frontier with directional preference
        while (frontier.Count > 0)
        {
            // Heavily bias toward cells that continue in same direction (80% of time)
            int idx;
            if (random.NextDouble() < 0.8 && frontier.Count > 1)
            {
                // Find cells that would continue straight - create winding paths
                var continuingStraight = new List<int>();
                for (int i = 0; i < Math.Min(frontier.Count, 10); i++)
                {
                    int testIdx = random.Next(frontier.Count);
                    var (testR, testC, testReg, testDir) = frontier[testIdx];
                    // Check if continuing in same direction from parent
                    int checkR = testR + dr[testDir];
                    int checkC = testC + dc[testDir];
                    if (checkR >= 0 && checkR < boardSize && checkC >= 0 && checkC < boardSize && !assigned[checkR, checkC])
                    {
                        continuingStraight.Add(testIdx);
                    }
                }
                idx = continuingStraight.Count > 0 ? continuingStraight[random.Next(continuingStraight.Count)] : random.Next(frontier.Count);
            }
            else
            {
                idx = random.Next(frontier.Count);
            }
            
            var (row, col, region, lastDir) = frontier[idx];
            frontier.RemoveAt(idx);
            
            // Skip if already assigned
            if (assigned[row, col]) continue;
            
            // Assign this cell
            regions[row, col] = region;
            assigned[row, col] = true;
            
            // Add neighbors with direction info
            for (int i = 0; i < 4; i++)
            {
                int newR = row + dr[i];
                int newC = col + dc[i];
                if (newR >= 0 && newR < boardSize && newC >= 0 && newC < boardSize && !assigned[newR, newC])
                {
                    frontier.Add((newR, newC, region, i));
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
        if (!SolveBacktrack(0, testBoard, regions, boardSize))
            return false;

        // Check constraint density - much lower thresholds for harder puzzles
        double constraintDensity = CalculateConstraintDensity(regions, boardSize);
        double threshold = boardSize switch
        {
            5 => 0.38,     // 5x5 boards - significantly harder
            6 => 0.34,     // 6x6 much harder
            _ => 0.30      // 8x8 and larger - very challenging
        };
        
        return constraintDensity < threshold;
    }

    private double CalculateConstraintDensity(int[,] regions, int boardSize)
    {
        // Calculate how constrained each region is by other regions
        // Higher value = more intersecting constraints = harder puzzle
        
        int totalConstraints = 0;
        int totalPossibleConstraints = 0;

        for (int regionId = 0; regionId < boardSize; regionId++)
        {
            // Get all cells in this region
            var regionCells = new List<(int row, int col)>();
            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    if (regions[r, c] == regionId)
                        regionCells.Add((r, c));
                }
            }

            // For each cell in this region, count conflicts with other regions
            foreach (var (row, col) in regionCells)
            {
                int conflictCount = 0;
                
                // Count cells in OTHER regions that would conflict with this placement
                for (int r = 0; r < boardSize; r++)
                {
                    for (int c = 0; c < boardSize; c++)
                    {
                        if (regions[r, c] == regionId)
                            continue;

                        // Check if this cell conflicts (same row, col, or adjacent diagonal)
                        if (r == row || c == col || 
                            (Math.Abs(r - row) == 1 && Math.Abs(c - col) == 1))
                        {
                            conflictCount++;
                        }
                    }
                }

                totalConstraints += conflictCount;
                totalPossibleConstraints += boardSize * boardSize - regionCells.Count;
            }
        }

        // Return normalized constraint density (0.0 to 1.0)
        return totalPossibleConstraints > 0 
            ? (double)totalConstraints / totalPossibleConstraints 
            : 0.0;
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
