# Mortal Kombat Overlay

## Overview

Mortal Kombat Overlay is a utility that provides an overlay for the game Mortal Kombat™ 1 (2023). It displays move lists while the game is running.

## Features

- Show move lists for both players with the ability to toggle between different move list states.
- Automatically detect when Mortal Kombat is running and update the overlay accordingly.
- Launch Mortal Kombat from within the application.

## Prerequisites

- Windows operating system.
- .NET Framework 7.
- Mortal Kombat™ 1 installed via Steam.

## Installation

1. Clone this repository to your local machine.

2. Open the project in Visual Studio.

3. Build the project to generate the executable.

## Usage

1. Run the application.

2. Click the "Launch Mortal Kombat" button to start the game if it's not already running.

3. The overlay will automatically update to display move lists when a game starts in any mode.  Here is a picture of the overlay in action:
   ![image](https://github.com/taferro/MortalKombatOverlay/assets/2585255/eb712be4-71e1-44b8-a9fb-7e1e57c94e05)

4. Toggle between different move list states for each player using the provided buttons.  The four states are "special moves only", "fatalities only", "hidden", "all moves."  These can be toggled individually per player.

5. Close the application by clicking the "Close" button in the bottom of the overlay.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- This project uses PaddleOCR to detect player names in the game, and OpenCV for image processing.

## Authors

- veri
