# Agent Guidelines

> **Game Jam Project:** This is a game jam project. Prioritize fast, working solutions over long-term maintainability. Skip over-engineering, extensive abstractions, and premature optimization. Get things working quickly.

Before starting any work, read the [PLAN.md](./PLAN.md) file to understand the game design and mechanics.

## Technical Stack

- **Engine:** Godot 4.6
- **Runtime:** .NET 10
- **Language:** C# only (no GDScript)
- **Target Platform:** Windows PC (low-end hardware, keep performance lightweight)

## Code Standards

### General Rules

- Write all game logic in C#
- Never use GDScript for any functionality
- Follow C# naming conventions (PascalCase for public members, camelCase for private)
- Use Godot's C# API and patterns
- **Avoid obvious comments** - Don't add XML summaries or comments that just restate what the code clearly shows. Only comment when explaining non-obvious behavior or important context. Example: `/// <summary>Movement speed of the player.</summary>` on `MoveSpeed` is redundant.

### File Organization

- Place scene files (`.tscn`) in `Scenes/` or `Prefabs/` directories
- Place C# scripts in `Scripts/` directory
- Keep related scripts and scenes organized by feature when the project grows

### Godot-Specific

- Use `[Export]` attribute for exposing variables to the editor
- Inherit from appropriate Godot node types (`Node`, `Node2D`, `Node3D`, `CharacterBody2D`, etc.)
- Use signals for decoupled communication between nodes
- **Never search for nodes or scripts by name** - Names can change and break code. Instead, use `[Export]` references set in the editor. Create wrapper scripts with exported node references when you need to access child nodes from external scripts.

### No Hardcoding

- **Never hardcode asset/file references** - Use `[Export]` and tell the developer to set references in the editor
- **Never hardcode configurable values** - Expose tunable values (speeds, timers, thresholds, etc.) via `[Export]` so they can be adjusted in the editor without code changes
- Example: Instead of `private float speed = 5.0f;` use `[Export] public float Speed { get; set; } = 5.0f;`

## Agent Behavior

**Important:** When something is unclear or information is missing, always ask the developer clarifying questions before proceeding. Do not make assumptions about:

- Game mechanics that aren't documented
- Art style or visual requirements
- Sound/audio requirements
- Specific implementation details that could go multiple ways
