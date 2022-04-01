using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main tile class

public class Tile : MonoBehaviour
{
    [Tooltip("X and Y indexes of the correct tile position (zero indexing from bottom-left)")]
    [SerializeField] private int correctIndX, correctIndY;

    public int IndX { get; set; }
    public int IndY { get; set; }

    public int CorrectIndX => correctIndX;
    public int CorrectIndY => correctIndY;

    public bool IsHoleTile { get; set; }


    private void OnMouseDown()
    {
        Events.onTileClicked.Invoke(this);
    }
}
