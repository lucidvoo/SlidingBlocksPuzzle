using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stores one int var for easy and decoupled sharing data between scripts

[CreateAssetMenu(fileName = "newInt", menuName = "SO/Int Variable", order = 20)]
public class IntVariableSO : ScriptableObject
{
    [SerializeField] private bool isPositiveOnly;
    [SerializeField] private int value;
    [SerializeField] private int defaultValue = 0;

    public int Value 
    { 
        get => value;
        set
        {
            if (isPositiveOnly && value < 0)
            {
                this.value = 0;
                Debug.Log("Smth tried to assign negative value to Int Var marked as Positive only. Var name: " + name);
            }
            else
            {
                this.value = value;
            }
        }
    }

    private void OnEnable()
    {
        if (isPositiveOnly && defaultValue < 0)
        {
            Debug.LogError("Positive only Int var can't have a negative default value!");
        }
        else
        {
            Reset();
        }
    }

    public void Reset() => Value = defaultValue;

    public bool IsZero() => Value == 0;

}
