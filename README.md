# Overview
This Unity project is a top-down maze game and environment with a lobby area, procedural maze generation, validation utilities, UI/interaction scripts, and several integration scripts (MetaMask, MaskEditor, environment managers). The maze generator guarantees a connected, solvable maze that starts from the right edge centered vertically and connects into the lobby through a doorway. The repository includes tools for generation, validation, and runtime behaviors for enemies, pickups, and emergency exit features.
<br> 
# Features
  - Procedural maze generation using recursive backtracking with guaranteed connectivity and solvability.
  - Right-edge entry: maze starts from the right edge at vertical center and connects to the lobby via a doorway.
  - Maze validation utilities including BFS pathfinding used by EnsureExitReachable to guarantee an exit.
  - Runtime features: enemy spawners, audio manager, spikes, masked UI, and emergency exit creation/hide logic.
  - Integration placeholders: MetaMask authentication and other systems represented as components to be wired in the Inspector.
  - Build output: simple visualizer and exportable scenes for testing.

<br>
# Folder structure
  - Assets/Scripts
  - MazeGenerator.cs
  - MazeValidator.cs
  - MaskedTextboxScript.cs
  - MetaMask (MetaMask.cs and related scripts)
  - MaskEditor (editor utilities)
  - CompleteGame (level scripts)
  - UI and managers (AudioManager.cs, PerformanceManager.cs)
  - Assets/Prefabs
  - lobby prefabs, emergency exit prefabs, enemy prefabs
  - Scenes
  - MainScene.unity
  - Packages
  - manifest.json
