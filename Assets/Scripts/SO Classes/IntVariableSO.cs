using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stores one persistent int var for easy sharing data between scripts

[CreateAssetMenu(fileName = "newInt", menuName = "SO/Int Variable", order = 20)]
public class IntVariableSO : ScriptableObject
{
    public int value = 0;

    public void Increment() => value++;

    public void Decrement() => value--;

    public void Reset() => value = 0;

    public bool IsZero() => value == 0;

    public bool IsPositive() => value >= 0; // ?

    public bool IsNegative() => value < 0;
}
