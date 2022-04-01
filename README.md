# SlidingBlocksPuzzle
Project for study, demonstration and fun

Hi!
The project is in early development in the moment but already it can be used to judge my code style and programming skills.
It's a simple sliding puzzle game (tags game). 

Gameplay
Remember the picture, choose a tile to remove from board. It'll be shuffled. 
Try to restore initial picture by sliding the tiles onto empty space.

Project structure
A sole in-game scene is one of the easy levels of the game. By now it has only 3x3 board and single picture.

Scripts
"TileGrid" - contains array of references to tiles. Manage all actions with tiles and board
"Tile" - tile class, attached to tile prefab
"TileMover" - contains animations on board, encapsulates DoTween commands for tweening
"UVAdjuster" - adjusts UVs on tiles to be able to use single material, single texture and sigle prefab for tile
"Events" - generic event system for project.

Dependencies between scripts are based mainly on Events and pluggable variables made with ScriptableObjects.
No singletons used.
