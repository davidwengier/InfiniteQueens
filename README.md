# Infinite Queens

A minimalistic puzzle game where you place queens on a board following classic chess rules with a colorful twist.

## Game Rules

Place one queen in each colored region such that:
- No two queens share the same row
- No two queens share the same column
- No two queens are diagonally adjacent
- Exactly one queen per colored region

## Features

- **Multiple board sizes**: Play on boards from 4×4 to 10×10
- **Timed challenges**: Track your completion times and compete with yourself
- **Smart assistance**: Automatically marks invalid squares as you place queens
- **Game history**: Review your past games with medals for your best times
- **Progressive difficulty**: Larger boards create more complex puzzles

## How to Play

1. Choose your board size (4×4 to 10×10)
2. Click a cell to place a queen (♛)
3. Click again to mark a cell as blocked (✕)
4. Click once more to clear the cell
5. Complete the board by satisfying all placement rules

## Development

Built with .NET 10.0 and Blazor WebAssembly.

### Running Locally

```bash
cd InfiniteQueens\InfiniteQueens.Web
dotnet run
```

### Running Tests

```bash
cd InfiniteQueens\InfiniteQueens.Tests
dotnet test
```
