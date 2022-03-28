using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main tile class

public class Tile : MonoBehaviour
{
    [Tooltip("Row and column of the right tile position (zero indexing)")]
    [SerializeField] private Vector2Int correctTileIndexes;

    public Vector2Int CurrentTileIndexes { get; set; }

    public Vector2Int CorrectTileIndexes => correctTileIndexes;


    private void OnMouseDown()
    {
        Events.onTileClicked.Invoke(this);
    }
}
