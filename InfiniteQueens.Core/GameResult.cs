namespace InfiniteQueens.Models;

public class GameResult
{
    public int GameNumber { get; set; }
    public TimeSpan Time { get; set; }
    public string? BoardHash { get; set; }
    public int? Seed { get; set; }
}
