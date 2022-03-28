using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Storing tile array and manipulating tiles on grid level

public class TileGrid : MonoBehaviour
{
    [SerializeField] private LevelSettingsSO levelSettings;
    [Tooltip("Add all tiles in any order")]
    [SerializeField] private Tile[] tileRefs;

    private Tile[,] tiles;
    // The tile, that'll be hidden to meke hole in the puzzle
    private Tile holeTile;

    private void Awake()
    {
        InitializeTileArray();
        // TODO: This must be invoked from another place. After player shows he ready to play
        MakeHoleTileFromLastTile();
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
        }
    }

    private void MakeHoleTileFromLastTile()
    {
        holeTile = tiles[tiles.GetLength(0) - 1, tiles.GetLength(1) - 1];
        // TODO: This must be tweening
        holeTile.gameObject.SetActive(false);
    }


}
