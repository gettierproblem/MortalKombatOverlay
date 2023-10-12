# Mortal Kombat Overlay

## Overview

Mortal Kombat Overlay is a utility that provides an overlay for the game Mortal Kombat™ 1. It displays move lists while the game is running.

## Features

- Display player names for Player 1 and Player 2.
- Show move lists for both players with the ability to toggle between different move list states.
- Automatically detect when Mortal Kombat is running and update the overlay accordingly.
- Launch Mortal Kombat from within the application.

## Prerequisites

- Windows operating system.
- .NET Framework.
- Mortal Kombat™ 1 installed via Steam.

## Installation

1. Clone this repository to your local machine.

2. Open the project in Visual Studio.

3. Build the project to generate the executable.

## Usage

1. Run the application.

2. Click the "Launch Mortal Kombat" button to start the game if it's not already running.

3. The overlay will automatically update to display move lists when Mortal Kombat is detected.

4. Toggle between different move list states for each player using the provided buttons.

5. Close the application by clicking the "X" button in the corner of the overlay.

## Configuration

- The character move list data is loaded from a JSON file located in the "Resources" folder. Replace this file with your own move list data if needed.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- This project uses PaddleOCR to detect player names in the game, and OpenCV for image processing.

## Authors

- veri
