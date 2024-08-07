# Mortal Kombat Overlay

## Overview  

Mortal Kombat Overlay is a utility that provides an overlay for the game Mortal Kombat™ 1 (2023). It displays move lists while the game is running.

<img width="1164" alt="Screenshot 2023-10-17 121911" src="https://github.com/gettierproblem/MortalKombatOverlay/assets/2585255/13a2c311-4724-4a5e-b5f2-df144c77c8be">

## Features

- Show move lists for both players with the ability to toggle between different move list states.
- Automatically detect movelists from game screen, displayed as an overlay.

## Prerequisites

- Windows operating system.
- .NET Framework 7.
- Mortal Kombat™ 1 installed via Steam.

## Installation

1. Download the MortalKombatOverlay.zip file from [releases.](https://github.com/gettierproblem/MortalKombatOverlay/releases)
2. Extract the zip file to a folder.
3. Download .NET framework 7 runtime if it is not already present on your system. You can get it from: [microsoft dotnet 7 site](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).  

## Usage

1. Run "MortalKombatOverlay.exe"
<img width="600" alt="Screenshot 2023-10-17 122618" src="https://github.com/gettierproblem/MortalKombatOverlay/assets/2585255/f62f4c04-7a08-445f-b081-ffd43716a8a1">

2. Click the Launch Mortal Kombat" button to start the game if it's not already running.  Otherwise, alt-tab to the game.

3. The overlay will automatically update to display move lists when a game starts in any mode.
   
4. Toggle between different move list states for each player using the provided buttons.  The four states are "special moves only", "fatalities only", "hidden", "all moves."  These can be toggled individually per player.

5. Close the application by clicking the "Close" button in the bottom of the overlay.

## Known issues

1. App is ~500mb.  It uses OCR to scrape the text from the screen, and PaddleOCR/OpenCV dlls are huge.
2. Application is not signed using smartscreen.

## Anti-cheat notice

1. This application is not designed to cheat or exploit Mortal Kombat gameplay - it is a productivity tool, intended so that the user can view character movelists easily, and does not confer a gameplay advantage. It functions independently, running in a separate process, and uses OCR to display move lists. The program neither injects code into the Mortal Kombat game nor automates any in-game actions, and therefore should not trigger anti-cheat mechanisms. Use of this application is deemed to be safe under these conditions.
2. While the design and functionality of this application do not intentionally interact with Mortal Kombat in a way that should trigger anti-cheat systems or infringe the Mortal Kombat EULA, there can be no assurance or guarantee that using this overlay is entirely risk-free. Given the evolving nature of anti-cheat algorithms and [terms of service](https://legal.wbgames.com/eula/steam/eula_stm_en_US.html), users employ this overlay at their own risk and responsibility. The developer cannot be held liable for any repercussions, including but not limited to account suspension or banning, resulting from the use of this application.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- This project uses PaddleOCR to detect player names in the game, and OpenCV for image processing.  The idea behind the transparent overlay came from Hearthstone Decktracker.

## Authors

- veri
