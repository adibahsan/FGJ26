# Nukkumaski

Game jam prototype made with **Godot 4.6** + **C# (.NET)**.

You play as a **house elf** trying to keep the homeowner asleep (and alive) while a malfunctioning smart-home AI causes problems throughout the night.

## Gameplay

- **Goal**: Keep the homeowner asleep as long as possible.
- **How**: React to events and apply the correct mask to counter them.
- **Scoring**: Points accumulate while the person is asleep (deeper sleep = faster points).

## Events & masks (design)

The current design includes events like heater/lights/vacuum merchant/teams call/water leak/gas leak/CPAP failure/electrical malfunction, each countered by a specific mask.

Full design doc: `PLAN.md`.

## Controls

- **Move**: WASD (bound via Input Map actions: `move_forward`, `move_back`, `move_left`, `move_right`)

## Run from the editor

1. Install **Godot 4.6 (.NET / C# build)**.
2. Open `project.godot` in Godot.
3. Press **Play**.

## Project structure

- `Scenes/`: main scenes (e.g. `MainMenu.tscn`, `game_level.tscn`)
- `Prefabs/`: reusable scenes (e.g. `player.tscn`, `house.tscn`)
- `Scripts/`: C# gameplay code

## License

See `LICENSE`.

