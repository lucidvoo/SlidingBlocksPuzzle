using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main tile class

public class Tile : MonoBehaviour
{
    [Tooltip("Row and column of the right tile position (zero indexing)")]
    [SerializeField] Vector2Int correctTileIndexes;

    public Vector2Int CorrectTileIndexes => correctTileIndexes;
}
