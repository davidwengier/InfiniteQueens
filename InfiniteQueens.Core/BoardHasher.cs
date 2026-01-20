using System.Security.Cryptography;
using System.Text;

namespace InfiniteQueens.Services;

public static class BoardHasher
{
    public static string GetRotationInvariantHash(int[,] regions)
    {
        int size = regions.GetLength(0);
        
        // Generate all 4 rotations
        var rotation0 = regions;
        var rotation90 = Rotate90(regions, size);
        var rotation180 = Rotate180(regions, size);
        var rotation270 = Rotate270(regions, size);
        
        // Normalize each rotation
        var normalized0 = NormalizeRegionIds(rotation0, size);
        var normalized90 = NormalizeRegionIds(rotation90, size);
        var normalized180 = NormalizeRegionIds(rotation180, size);
        var normalized270 = NormalizeRegionIds(rotation270, size);
        
        // Pick the canonical (lexicographically smallest) rotation
        var canonical = normalized0;
        if (IsLexicographicallySmaller(normalized90, canonical, size))
            canonical = normalized90;
        if (IsLexicographicallySmaller(normalized180, canonical, size))
            canonical = normalized180;
        if (IsLexicographicallySmaller(normalized270, canonical, size))
            canonical = normalized270;
        
        // Hash the canonical rotation
        return HashRegionMap(canonical, size);
    }
    
    private static int[,] Rotate90(int[,] regions, int size)
    {
        var rotated = new int[size, size];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                rotated[col, size - 1 - row] = regions[row, col];
            }
        }
        return rotated;
    }
    
    private static int[,] Rotate180(int[,] regions, int size)
    {
        var rotated = new int[size, size];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                rotated[size - 1 - row, size - 1 - col] = regions[row, col];
            }
        }
        return rotated;
    }
    
    private static int[,] Rotate270(int[,] regions, int size)
    {
        var rotated = new int[size, size];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                rotated[size - 1 - col, row] = regions[row, col];
            }
        }
        return rotated;
    }
    
    private static int[,] NormalizeRegionIds(int[,] regions, int size)
    {
        var normalized = new int[size, size];
        var mapping = new Dictionary<int, int>();
        int nextId = 0;
        
        // Scan left-to-right, top-to-bottom and assign new IDs in order of first appearance
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                int originalId = regions[row, col];
                if (!mapping.ContainsKey(originalId))
                {
                    mapping[originalId] = nextId++;
                }
                normalized[row, col] = mapping[originalId];
            }
        }
        
        return normalized;
    }
    
    private static bool IsLexicographicallySmaller(int[,] a, int[,] b, int size)
    {
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                if (a[row, col] < b[row, col]) return true;
                if (a[row, col] > b[row, col]) return false;
            }
        }
        return false;
    }
    
    private static string HashRegionMap(int[,] regions, int size)
    {
        var sb = new StringBuilder();
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                sb.Append(regions[row, col]);
                sb.Append(',');
            }
        }
        
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var hashBytes = SHA256.HashData(bytes);
        
        // Return first 12 characters of hex string (48 bits) for a shorter, readable hash
        return Convert.ToHexString(hashBytes)[..12];
    }
}
