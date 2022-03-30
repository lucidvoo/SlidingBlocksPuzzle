using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scale down and offset mesh UVs to represent proper part of the texture
public class UVAdjuster : MonoBehaviour
{
    [Tooltip("Level settings scriptable object file")]
    [SerializeField] private LevelSettingsSO levelSettings;
    [Tooltip("Tile script reference")]
    [SerializeField] private Tile tileScriptRef;
    [SerializeField] MeshFilter meshFilter;

    private float scaleFactorUVs;
    private Vector2 offsetUVs;

    private void Awake()
    {
        CalculateScaleAndOffset();
        AdjustUVs();
    }

    private void CalculateScaleAndOffset()
    {
        scaleFactorUVs = 1 / (levelSettings.PuzzleSize + 2f);

        float indRow = tileScriptRef.CorrectTileIndexes.x;
        float indCol = tileScriptRef.CorrectTileIndexes.y;
        float uOffset = (1f + indCol) / (levelSettings.PuzzleSize + 2f);
        float vOffset = (levelSettings.PuzzleSize - indRow) / (levelSettings.PuzzleSize + 2f);
        offsetUVs = new Vector2(uOffset, vOffset);
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
