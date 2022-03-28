using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SO with general level settings for referencing in scripts

[CreateAssetMenu(fileName = "xxxLevelSettings", menuName = "SO/Level Settings", order = 10)]
public class LevelSettingsSO : ScriptableObject
{
    [Tooltip("Number of tiles in each row")]
    [SerializeField] private int puzzleSize;

    public int PuzzleSize => puzzleSize;
}
