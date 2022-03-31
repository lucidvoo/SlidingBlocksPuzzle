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
            tileRefs[i].Indexes = tileRefs[i].CorrectTileIndexes;
        }
    }


    // Invokes every time when tile clicked
    private void OnTileClicker_Handler(Tile tileClicked)
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
        if (tileClicked.Indexes.x != holeTile.Indexes.x &&
            tileClicked.Indexes.y != holeTile.Indexes.y)
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
        int tilesToMoveCount = Mathf.Abs(holeTile.Indexes.x - tileClicked.Indexes.x +
                                         holeTile.Indexes.y - tileClicked.Indexes.y);

        // tiles that have to be moved are gathering in array
        Tile[] tilesToMove = new Tile[tilesToMoveCount];
        tilesToMove[0] = tileClicked;
        for (int i = 1; i < tilesToMoveCount; i++)
        {
            tilesToMove[i] = GetTileInDirectionFromTarget(tilesToMove[i - 1], dir);
        }

        // swap tiles in tiles array and refresh tile indexes 
        for (int i = tilesToMove.Length - 1; i >= 0; i--)
        {
            SwapTileWithHole(tilesToMove[i]);
        }

        tileMover.SlideTiles(tilesToMove, dir);
    }

    // Swap tile with hole near by in tiles array. Refresh Indexes
    private void SwapTileWithHole(Tile tile)
    {
        Tile neighbor = null;
        Direction d;
        for (d = Direction.UP; d < Direction.Count; d++)
        {
            neighbor = GetTileInDirectionFromTarget(tile, d);
            if (neighbor == null)
            {
                continue;
            }
            else if (neighbor.IsHoleTile)
            {
                break;
            }
        }
        if (d == Direction.Count)
        {
            Debug.LogWarning("Attempt to swap tile with no hole near by");
            return;
        }
        // swap tiles in array
        tiles[neighbor.Indexes.x, neighbor.Indexes.y] = tile;
        tiles[tile.Indexes.x, tile.Indexes.y] = neighbor;
        // swap CurrentIndexes of tiles
        Vector2Int varForSwap = tile.Indexes;
        tile.Indexes = neighbor.Indexes;
        neighbor.Indexes = varForSwap;
    }

    private Tile GetTileInDirectionFromTarget(Tile targetTile, Direction dir)
    {
        // Get direction unit vector
        Vector3 dirVector3 = dir.GetUnitVector3();
        // Convert it to "array vector"
        Vector2Int dirIndVector = new Vector2Int(Mathf.RoundToInt(-dirVector3.y), Mathf.RoundToInt(dirVector3.x));

        Vector2Int neededTileIndexes = targetTile.Indexes + dirIndVector;

        if (!IsIndexesInArrayRange(neededTileIndexes))
        {
            Debug.LogWarning("Attempt to get tile from out of array bounds.");
            return null;
        }
        
        return tiles[neededTileIndexes.x, neededTileIndexes.y];
    }

    private bool IsIndexesInArrayRange(Vector2Int indexes)
    {
        if (indexes.x < 0 || indexes.y < 0 )
        {
            return false;
        }
        if (indexes.x >= tiles.GetLength(0) || indexes.y >= tiles.GetLength(1))
        {
            return false;
        }
        return true;
    }
}
