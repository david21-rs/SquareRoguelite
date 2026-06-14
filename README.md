# The Adventure

A small top-down rogue-lite built on the course SDL2 skeleton. Fight through two rooms
of enemies, grab upgrades, then take down the boss in room three.

## Run

    dotnet run

Requires the .NET 10 SDK. SDL2 natives ship via the Silk.NET packages, no extra install needed.

## Controls

- WASD / arrow keys: move
- Space: melee attack (short swing in the direction you are facing)
- R: restart after a game over or victory
- Close the window to quit

## Rules

- Clear all enemies in a room to open the green exit tile.
- Items: red = health potion (+2 HP), yellow = power gem (+1 damage), cyan = speed boots.
- Level 3 is the boss arena. The boss charges slowly and fires spreads of projectiles.
- Win by killing the boss, lose when your hearts run out.
- Score: 10 per enemy, 100 for the boss, 5 per item, 50 per cleared room.



## What this has

- Game loop: input - update - render in `Program.Main`, fixed-clamped delta time.
- Input handling: `Input.cs`, fed from SDL key events.
- Win/lose: boss kill = victory, 0 HP = game over.
- State beyond a single variable: wall list, enemy list, item list, projectile list, player stats.
- Persistence: high score in `save.json` (`SaveSystem.cs`).
- Language features used: LINQ (collision/cleanup queries in `Game.cs`), interfaces
  (`IEntity`, `IItem`), inheritance (`Boss : Enemy`), pattern matching (switch expressions
  and `is ... or` in `Game.cs`/`Program.cs`), `IDisposable` (`GameRenderer`), custom
  exception (`SaveDataException`).

## AI disclosure

Parts of this project were developed with AI assistance (Claude).
