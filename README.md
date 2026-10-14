# Keyboard Follow

A BepInEx mod for [Sephiria](https://store.steampowered.com/), designed to change weapon and magic aiming behavior using keyboard movement directions.

## Features

- Attack follows the direction of last keyboard input. So, You always attack front.

> This mod is a work in progress. Some features may require further testing.

## Requirements

- Sephiria (PC version)
- BepInEx 5 (Insdie Sephiria folder)
- A compatible version of the game and its assemblies

## Installation

1. Install BepInEx 5 for Sephiria and move folder like 'Sephiria/BepInEx/'
2. Download the compiled `KeyboardFollow.dll` from the Releases page.
3. Copy `KeyboardFollow.dll` into the game's `BepInEx/plugins/` directory.
4. Launch the game.

## Building from Source

1. Clone this repository.
2. Open the solution in Visual Studio 2022.
3. Configure references to the required BepInEx, Harmony, Unity, and game assemblies.
4. Build the project.
5. Copy the resulting `KeyboardFollow.dll` into `BepInEx/plugins/`.

Game assemblies are not included in this repository.

## Dependencies

This project references the following assemblies:

| Assembly | Purpose |
|---|---|
| `BepInEx.dll` | BepInEx plugin framework |
| `0Harmony.dll` | Harmony runtime patching |
| `Assembly-CSharp.dll` | Game-specific classes and logic |
| `Mirror.dll` | Networking framework |
| `Mirror.Components.dll` | Mirror networking components |
| `UnityEngine.dll` | Unity engine API |
| `UnityEngine.CoreModule.dll` | Core Unity engine functionality |
| `UnityEngine.InputSystem.dll` | Unity Input System |
| `UnityEngine.InputLegacyModule.dll` | Legacy Unity input support |



### Reference Setup

These assemblies are referenced during development and are not included in this repository.

- **BepInEx, 0Harmonyvassemblies** should come from the installed BepInEx environment(\BepInEx\core).
- **Other assemblies** should come from the game's installation directory(\Sephiria\Sephiria_Data\Managed).

Exact file locations may vary depending on the game and BepInEx installation.

## Compatibility

Compatibility may vary depending on the game version, BepInEx version, and multiplayer or network behavior.

## Disclaimer

This is an unofficial, community-made mod and is not affiliated with or endorsed by the developers of Sephiria.

Use at your own risk.

## License
