using InfiniteQueens.Models;

namespace InfiniteQueens.Services;

public class GameHistory
{
    private readonly Dictionary<int, List<GameResult>> _history = new();
    private readonly Dictionary<int, int> _gameCounters = new();

    public bool HasHistory(int boardSize) => _history.ContainsKey(boardSize) && _history[boardSize].Count > 0;

    public List<GameResult> GetHistory(int boardSize)
    {
        if (!_history.ContainsKey(boardSize))
            return new List<GameResult>();
        
        return _history[boardSize].OrderBy(g => g.Time).ToList();
    }

    public int GetNextGameNumber(int boardSize)
    {
        if (!_gameCounters.ContainsKey(boardSize))
        {
            _gameCounters[boardSize] = 1;
        }
        else
        {
            _gameCounters[boardSize]++;
        }
        return _gameCounters[boardSize];
    }

    public void AddResult(int boardSize, int gameNumber, TimeSpan time, string boardHash)
    {
        if (!_history.ContainsKey(boardSize))
        {
            _history[boardSize] = new List<GameResult>();
        }

        _history[boardSize].Add(new GameResult
        {
            GameNumber = gameNumber,
            Time = time,
            BoardHash = boardHash
        });
    }
    
    public GameResult? FindPreviousGameByHash(int boardSize, string boardHash)
    {
        if (!_history.ContainsKey(boardSize))
            return null;
            
        return _history[boardSize].FirstOrDefault(g => g.BoardHash == boardHash);
    }
}
