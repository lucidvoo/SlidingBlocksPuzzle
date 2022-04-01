using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SO with general level settings for referencing in scripts

[CreateAssetMenu(fileName = "xxxLevelSettings", menuName = "SO/Level Settings", order = 10)]
public class LevelSettingsSO : ScriptableObject
{
    [Tooltip("Number of tiles in each row or column")]
    [SerializeField] private int puzzleSize;
    [Tooltip("Distance between centers of two adjacent tiles")]
    [SerializeField] private float tileStepDistance = 1f;

    public int PuzzleSize => puzzleSize;
    public float TileStepDistance => tileStepDistance;
}
