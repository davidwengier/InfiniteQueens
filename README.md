# Infinite Queens

A minimalistic Blazor WebAssembly implementation of the Queens (also called Crowns) puzzle game.

## Game Rules

Place one queen in each coloured region such that:
- No two queens share the same row
- No two queens share the same column
- No two queens are diagonally adjacent (only immediate diagonal neighbors are blocked)
- Exactly one queen per coloured region

## Project Structure

```
InfiniteQueens/
├── InfiniteQueens.Web/             # Blazor WebAssembly UI project
│   ├── Components/
│   │   ├── Pages/
│   │   │   └── Home.razor          # Main game page (all game UI)
│   │   ├── App.razor
│   │   ├── Routes.razor
│   │   └── _Imports.razor
│   ├── wwwroot/
│   │   ├── index.html              # WASM entry point
│   │   └── favicon.png
│   ├── Program.cs                  # Service configuration
│   └── InfiniteQueens.csproj
├── InfiniteQueens.Core/            # Business logic and game services
│   ├── Models/
│   │   ├── CellState.cs
│   │   └── GameResult.cs
│   ├── Services/
│   │   ├── BoardGenerator.cs
│   │   ├── GameState.cs
│   │   └── GameHistory.cs
│   └── InfiniteQueens.Core.csproj
├── InfiniteQueens.Tests/           # xUnit test project (38 tests)
│   ├── BoardGeneratorTests.cs
│   ├── GameStateTests.cs
│   ├── GameHistoryTests.cs
│   └── InfiniteQueens.Tests.csproj
├── InfiniteQueens.slnx             # Solution file
└── README.md
```

## Features

- **Multiple board sizes**: 4x4 through 10x10
- **Timed mode**: Optional timer and game history leaderboard
- **Auto-marking**: Automatically mark invalid squares when placing queens
- **URL parameters**: Bookmark specific board sizes (`?size=8`)
- **Three-state cells**: Click to cycle through empty → mark (✕) → queen (♛)
- **Board validation**: Only generates solvable puzzles
- **Game history**: Track completion times with medals for top 3 per board size

## Running the Game

```bash
cd InfiniteQueens\InfiniteQueens.Web
dotnet run
```

Then navigate to http://localhost:5030 (or the port shown in console).

## Running Tests

```bash
cd InfiniteQueens\InfiniteQueens.Tests
dotnet test
```

All 38 tests validate:
- Board generation (region creation, connectivity, solvability)
- Game state management (conflicts, win detection)
- Game history tracking (sorting, game numbering)

## Building the Solution

```bash
cd InfiniteQueens
dotnet build
```

## Technical Details

- **Framework**: .NET 10.0
- **Platform**: Blazor WebAssembly (runs entirely client-side)
- **Testing**: xUnit with comprehensive coverage
- **Architecture**: Clean separation of concerns (UI, Core logic, Tests)
