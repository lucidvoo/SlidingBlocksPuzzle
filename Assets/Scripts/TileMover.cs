using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Moving tiles, tweening via DoTween
public class TileMover: MonoBehaviour
{
    [SerializeField] private LevelSettingsSO levelSettings;
    [SerializeField] private IntVariableSO numOfGameBlockersInt;

    [Space]
    [Tooltip("Overall animation of tiles speed")]
    [SerializeField] private float animSpeed = 1f;

    [Header("Wrong Shake params")]
    [SerializeField] private float shakeDuration = 0.6f;
    [SerializeField] private Vector3 shakeStrength;
    [SerializeField] private int shakeVibrato = 20;

    [Header("Hole tile removing params")]

    private Tile holeTile;


    // In the beginning of any movement
    private void OnTweenStart()
    {
        numOfGameBlockersInt.Value++;
        Events.onTilesStartMoving.Invoke();
    }

    // In the end of any movement
    private void OnTweenComplete()
    {
        numOfGameBlockersInt.Value--;
        Events.onTilesCompleteMoving.Invoke();
    }


    // TODO: tween reaction to wrong click
    internal void WrongTileClick(Tile tileClicked)
    {
        Tweener tweener = tileClicked.transform.DOShakePosition(shakeDuration * animSpeed, shakeStrength, shakeVibrato);
        tweener.OnStart(OnTweenStart).OnKill(OnTweenComplete);
    }

    internal void RemoveHoleTile(Tile tileToRemove)
    {
        holeTile = tileToRemove;

        // TODO: tweening

        holeTile.gameObject.SetActive(false);
    }
}
