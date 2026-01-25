namespace InfiniteQueens.Services;

public class ShapeTemplateGenerator
{
    // Common puzzle piece shapes defined as relative coordinates from origin (0,0)
    private static readonly List<List<(int row, int col)>> Templates = new()
    {
        // Small shapes (3-4 cells)
        new() { (0,0), (0,1), (1,0) },                           // L-shape (3 cells)
        new() { (0,0), (0,1), (0,2) },                           // Horizontal line (3 cells)
        new() { (0,0), (1,0), (2,0) },                           // Vertical line (3 cells)
        new() { (0,0), (0,1), (1,0), (1,1) },                    // Square (4 cells)
        new() { (0,0), (0,1), (0,2), (1,1) },                    // T-shape (4 cells)
        new() { (0,0), (0,1), (1,1), (1,2) },                    // Z-shape (4 cells)
        new() { (0,0), (1,0), (1,1), (2,0) },                    // Vertical T (4 cells)
        
        // Medium shapes (5-6 cells)
        new() { (0,0), (0,1), (0,2), (1,0) },                    // L-shape (4 cells)
        new() { (0,0), (0,1), (1,0), (2,0), (2,1) },             // Large L (5 cells)
        new() { (0,0), (0,1), (0,2), (0,3) },                    // Long horizontal (4 cells)
        new() { (0,0), (1,0), (2,0), (3,0) },                    // Long vertical (4 cells)
        new() { (0,0), (0,1), (0,2), (1,0), (1,1) },             // Large L variant (5 cells)
        new() { (0,0), (0,1), (0,2), (1,1), (2,1) },             // T with stem (5 cells)
        new() { (0,1), (1,0), (1,1), (1,2), (2,1) },             // Plus (5 cells)
        new() { (0,0), (0,1), (1,1), (1,2), (2,2) },             // Diagonal stairs (5 cells)
        new() { (0,0), (1,0), (1,1), (2,1), (2,2) },             // S-shape (5 cells)
        
        // Larger shapes (6-8 cells)
        new() { (0,0), (0,1), (1,0), (1,1), (2,0), (2,1) },      // Rectangle 3x2 (6 cells)
        new() { (0,0), (0,1), (0,2), (1,0), (1,1), (1,2) },      // Rectangle 2x3 (6 cells)
        new() { (0,0), (0,1), (0,2), (1,0), (1,2) },             // U-shape (5 cells)
        new() { (0,0), (0,1), (0,2), (1,1), (2,0), (2,1), (2,2) },  // H-shape (7 cells)
    };

    public int[,] GenerateRegions(int boardSize, int? seed = null)
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        
        var regions = new int[boardSize, boardSize];
        var assigned = new bool[boardSize, boardSize];
        
        // Initialize all cells as unassigned
        for (int r = 0; r < boardSize; r++)
            for (int c = 0; c < boardSize; c++)
                regions[r, c] = -1;

        int currentRegion = 0;
        int maxAttempts = 1000;
        int attempts = 0;

        // Place shapes until we have N regions or can't place more
        while (currentRegion < boardSize && attempts < maxAttempts)
        {
            attempts++;
            
            // Pick a random template
            var template = Templates[random.Next(Templates.Count)];
            
            // Try random rotations (0, 90, 180, 270 degrees)
            var rotation = random.Next(4);
            var rotatedTemplate = RotateShape(template, rotation);
            
            // Try random position
            int startRow = random.Next(boardSize);
            int startCol = random.Next(boardSize);
            
            // Check if shape fits
            if (CanPlaceShape(rotatedTemplate, startRow, startCol, assigned, boardSize))
            {
                // Place the shape
                PlaceShape(rotatedTemplate, startRow, startCol, currentRegion, regions, assigned);
                currentRegion++;
                attempts = 0; // Reset attempts on successful placement
            }
        }

        // Fill remaining cells with smaller shapes or single cells
        if (currentRegion < boardSize)
        {
            FillRemainingCells(regions, assigned, boardSize, ref currentRegion, random);
        }

        // If we still don't have enough regions, expand some cells from existing regions
        // into new single-cell regions
        if (currentRegion < boardSize)
        {
            CreateSingleCellRegions(regions, assigned, boardSize, ref currentRegion, random);
        }

        // Safety: assign any remaining unassigned cells to adjacent regions
        AssignUnassignedCells(regions, assigned, boardSize);

        return regions;
    }

    private List<(int row, int col)> RotateShape(List<(int row, int col)> shape, int rotation)
    {
        var rotated = new List<(int row, int col)>(shape);
        
        for (int i = 0; i < rotation; i++)
        {
            // Rotate 90 degrees clockwise: (row, col) -> (col, -row)
            rotated = rotated.Select(p => (p.col, -p.row)).ToList();
        }
        
        // Normalize so minimum coordinates are at (0,0)
        int minRow = rotated.Min(p => p.row);
        int minCol = rotated.Min(p => p.col);
        return rotated.Select(p => (p.row - minRow, p.col - minCol)).ToList();
    }

    private bool CanPlaceShape(List<(int row, int col)> shape, int startRow, int startCol, 
        bool[,] assigned, int boardSize)
    {
        foreach (var (dr, dc) in shape)
        {
            int r = startRow + dr;
            int c = startCol + dc;
            
            if (r < 0 || r >= boardSize || c < 0 || c >= boardSize)
                return false;
            
            if (assigned[r, c])
                return false;
        }
        return true;
    }

    private void PlaceShape(List<(int row, int col)> shape, int startRow, int startCol, 
        int regionId, int[,] regions, bool[,] assigned)
    {
        foreach (var (dr, dc) in shape)
        {
            int r = startRow + dr;
            int c = startCol + dc;
            regions[r, c] = regionId;
            assigned[r, c] = true;
        }
    }

    private void FillRemainingCells(int[,] regions, bool[,] assigned, int boardSize, 
        ref int currentRegion, Random random)
    {
        // Try to place small shapes in remaining spaces
        for (int attempt = 0; attempt < 500 && currentRegion < boardSize; attempt++)
        {
            // Use only small templates (first 7)
            var template = Templates[random.Next(Math.Min(7, Templates.Count))];
            var rotation = random.Next(4);
            var rotatedTemplate = RotateShape(template, rotation);
            
            int startRow = random.Next(boardSize);
            int startCol = random.Next(boardSize);
            
            if (CanPlaceShape(rotatedTemplate, startRow, startCol, assigned, boardSize))
            {
                PlaceShape(rotatedTemplate, startRow, startCol, currentRegion, regions, assigned);
                currentRegion++;
            }
        }
    }

    private void CreateSingleCellRegions(int[,] regions, bool[,] assigned, int boardSize, 
        ref int currentRegion, Random random)
    {
        // Create single-cell regions from unassigned cells
        var unassignedCells = new List<(int row, int col)>();
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (!assigned[r, c])
                    unassignedCells.Add((r, c));
            }
        }

        // Shuffle and assign
        unassignedCells = unassignedCells.OrderBy(_ => random.Next()).ToList();
        
        foreach (var (r, c) in unassignedCells)
        {
            if (currentRegion >= boardSize)
                break;
            
            regions[r, c] = currentRegion;
            assigned[r, c] = true;
            currentRegion++;
        }
    }

    private void AssignUnassignedCells(int[,] regions, bool[,] assigned, int boardSize)
    {
        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };
        
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (!assigned[r, c] || regions[r, c] == -1)
                {
                    // Find an adjacent assigned cell
                    for (int i = 0; i < 4; i++)
                    {
                        int adjR = r + dr[i];
                        int adjC = c + dc[i];
                        if (adjR >= 0 && adjR < boardSize && adjC >= 0 && adjC < boardSize && 
                            assigned[adjR, adjC] && regions[adjR, adjC] != -1)
                        {
                            regions[r, c] = regions[adjR, adjC];
                            assigned[r, c] = true;
                            break;
                        }
                    }
                    
                    // If still unassigned, assign to region 0
                    if (!assigned[r, c] || regions[r, c] == -1)
                    {
                        regions[r, c] = 0;
                        assigned[r, c] = true;
                    }
                }
            }
        }
    }
}
