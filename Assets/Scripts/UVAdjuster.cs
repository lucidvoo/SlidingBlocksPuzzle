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
        scaleFactorUVs = 1 / (levelSettings.PuzzleSize + 2f);

        float uOffset = (float)(1 + tileScriptRef.CorrectTileIndexes.y) / (levelSettings.PuzzleSize + 2);
        float vOffset = (float)(levelSettings.PuzzleSize - tileScriptRef.CorrectTileIndexes.x) / (levelSettings.PuzzleSize + 2);
        offsetUVs = new Vector2(uOffset, vOffset);

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
