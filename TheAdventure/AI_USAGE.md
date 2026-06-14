# AI Usage Disclosure

## Tools used

- **Claude Opus 4.8 (Anthropic)**: chat-based code suggestions and rubber-ducking.

## How each tool was used

- **Claude Opus 4.8**: Used for chat-based assistance generating specific blocks of game
  logic (entity classes, collision math, boss projectile spread, save/load
  serialization, the rendering routine), and for rubber-ducking design questions
  about the game loop and level layout. Generated blocks were reviewed, integrated,
  and in places adjusted by hand. No agentic/autonomous editing was used.

No AI-generated images, sounds, or fonts were used. The game renders with solid
colored rectangles only, so there are no external assets.

## Fully AI-generated files / regions

Fully AI-generated blocks are marked inline in the source with a `// AI-generated`
comment at the top and a matching `// end AI-generated` comment at the bottom.

The following regions are fully AI-generated:

- **Entities.cs** - entire file (`Box`, `IEntity`, `Player`, `Enemy`, `Boss`,
  `Projectile`).
- **SaveSystem.cs** - entire file (`SaveData`, `SaveDataException`, `SaveSystem`).
- **Game.cs** - the following regions:
  - `UpdatePlayer(...)`
  - `SpawnBossShots(...)` through `LoadLevel(...)`
  - `Render(GameRenderer r)`

