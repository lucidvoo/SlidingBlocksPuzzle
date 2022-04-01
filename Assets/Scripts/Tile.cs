using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main tile class

public class Tile : MonoBehaviour
{
    /*[Tooltip("X and Y indexes of the correct tile position (zero indexing from bottom-left)")]
    [SerializeField] private int correctIndX, correctIndY;*/

    private Vector3 zeroTileOffset;
    private float tileStepDistance;

    public int CorrectIndX { get; set; }
    public int CorrectIndY { get; set; }

    public bool IsHoleTile { get; set; }

    public void Init(Vector3 zeroTileOffset, float tileStepDistance)
    {
        this.zeroTileOffset = zeroTileOffset;
        this.tileStepDistance = tileStepDistance;
        CorrectIndX = IndX();
        CorrectIndY = IndY();
    }

    private void OnMouseDown()
    {
        Events.onTileClicked.Invoke(this);
    }

    public int IndX()
    {
        return Mathf.RoundToInt((transform.position.x - zeroTileOffset.x) / tileStepDistance);
    }

    public int IndY()
    {
        return Mathf.RoundToInt((transform.position.y - zeroTileOffset.y) / tileStepDistance);
    }
}
