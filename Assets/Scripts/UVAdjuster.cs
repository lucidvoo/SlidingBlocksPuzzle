using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scale down and offset mesh UVs to represent proper part of the texture
public class UVAdjuster : MonoBehaviour
{
    [Tooltip("How many rows of tiles the level has")]
    [SerializeField] private int puzzleSize;
    [Tooltip("Row and Column of the tile, indexed from 0")]
    [SerializeField] private Vector2Int tileIndexes;
    [SerializeField] MeshFilter meshFilter;

    private float scaleFactorUVs;
    private Vector2 offsetUVs;

    private void Awake()
    {
        scaleFactorUVs = 1 / (puzzleSize + 2f);

        float xOffset = (float)(1 + tileIndexes.y) / (puzzleSize + 2);
        float yOffset = (float)(puzzleSize - tileIndexes.x) / (puzzleSize + 2);
        offsetUVs = new Vector2(xOffset, yOffset);

        AdjustUVs();
    }

    private void AdjustUVs()
    {
        Vector2[] uvs = meshFilter.mesh.uv;

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] *= scaleFactorUVs;
            uvs[i] += offsetUVs;
        }

        meshFilter.mesh.uv = uvs;
    }
}
