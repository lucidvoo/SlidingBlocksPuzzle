using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Storing tile array and manipulating tiles on grid level

public class TileGrid : MonoBehaviour
{
    [Tooltip("Add all tiles in any order")]
    [SerializeField] private Tile[] tileRefs;

    private Tile[,] tiles;

    private void Awake()
    {
        InitializeTileArray();
    }

    // fill 2-dim tiles array with refs from editor
    private void InitializeTileArray()
    {
        if (tileRefs.Length < 4)
        {
            Debug.LogError("Fill tileRefs array in tile grid!");
        }
        
        int arraySide = Mathf.RoundToInt(Mathf.Sqrt(tileRefs.Length));
        tiles = new Tile[arraySide, arraySide];

        for (int i = 0; i < tileRefs.Length; i++)
        {
            tiles[tileRefs[i].CorrectTileIndexes.x, tileRefs[i].CorrectTileIndexes.y] = tileRefs[i];
        }

        //Test
        Debug.Log("tiles in order:");
        foreach (var item in tiles)
        {
            Debug.Log(item.CorrectTileIndexes.x + ", " + item.CorrectTileIndexes.y);
        }
    }
}
