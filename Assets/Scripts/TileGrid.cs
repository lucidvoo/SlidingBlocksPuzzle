using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Storing tile array and manipulating tiles on grid level

public class TileGrid : MonoBehaviour
{
    [SerializeField] private LevelSettingsSO levelSettings;
    [Tooltip("Add all tiles in any order")]
    [SerializeField] private Tile[] tileRefs;
    [SerializeField] private IntVariableSO numOfGameBlockersInt;
    [SerializeField] private TileMover tileMover;

    private Tile[,] tiles;
    // The tile, that'll be hidden to meke hole in the puzzle
    private Tile holeTile;
    private bool isGameBegun = false;


    private void OnEnable()
    {
        Events.onTileClicked.AddListener(OnTileClicker_Handler);
    }

    private void OnDisable()
    {
        Events.onTileClicked.RemoveListener(OnTileClicker_Handler);
    }


    private void Awake()
    {
        InitializeTileArray();
    }

    // fill 2-dim tiles array with refs from editor
    private void InitializeTileArray()
    {
        if (tileRefs.Length != levelSettings.PuzzleSize * levelSettings.PuzzleSize)
        {
            Debug.LogError("Something wrong with initialization. Check Level settings SO and Tile Refs in TileGrid");
        }

        tiles = new Tile[levelSettings.PuzzleSize, levelSettings.PuzzleSize];

        for (int i = 0; i < tileRefs.Length; i++)
        {
            tiles[tileRefs[i].CorrectTileIndexes.x, tileRefs[i].CorrectTileIndexes.y] = tileRefs[i];
            tileRefs[i].CurrentTileIndexes = tileRefs[i].CorrectTileIndexes;
        }
    }


    // Invokes every time when tile clicked
    private void OnTileClicker_Handler (Tile tileClicked)
    {
        if (!numOfGameBlockersInt.IsZero())
        {
            return;
        }

        // Choosing a tile to remove at the very beginning
        if (!isGameBegun)
        {
            isGameBegun = true;

            holeTile = tileClicked;
            holeTile.IsHoleTile = true;

            tileMover.RemoveHoleTile(tileClicked);

            return;
        }

        // Check if holeTile is in the same row or column as clicked tile
        if (tileClicked.CurrentTileIndexes.x != holeTile.CurrentTileIndexes.x && 
            tileClicked.CurrentTileIndexes.y != holeTile.CurrentTileIndexes.y)
        {
            tileMover.WrongTileClick(tileClicked);
            return;
        }

        MoveTiles(tileClicked);
    }

    // Successful tile click - one or more tiles will be moved.
    private void MoveTiles(Tile tileClicked)
    {
        // Direction for tile moving
        Direction dir = (holeTile.transform.position - tileClicked.transform.position).ComputeDirectionFromVector3();
        // How many tiles to move (only one index delta is non-zero)
        int tilesToMoveCount = holeTile.CurrentTileIndexes.x - tileClicked.CurrentTileIndexes.x +
                               holeTile.CurrentTileIndexes.y - tileClicked.CurrentTileIndexes.y;

        // TEST
        Debug.Log("Have to move " + tilesToMoveCount + " tiles in direction " + dir);

        // what  tiles to move?
        Tile[] tilesToMove = new Tile[tilesToMoveCount];
        tilesToMove[0] = tileClicked;
        for (int i = 1; i < tilesToMoveCount; i++)
        {
            tilesToMove[i] = GetTileInDirectionFromTarget(tilesToMove[i - 1], dir);
        }

        // TODO: what next?
    }

    private Tile GetTileInDirectionFromTarget(Tile targetTile, Direction dir)
    {
        // Get direction unit vector
        Vector3 dirVector3 = dir.GetUnitVector3();
        // Convert it to "array vector"
        Vector2Int dirIndVector = new Vector2Int(Mathf.RoundToInt(dirVector3.x), Mathf.RoundToInt(- dirVector3.y));

        Vector2Int neededTileIndexes = targetTile.CurrentTileIndexes + dirIndVector;

        return tiles[neededTileIndexes.x, neededTileIndexes.y];
    }

}
