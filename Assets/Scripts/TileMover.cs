using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Moving tiles, encapsulates tweening via DoTween
public class TileMover: MonoBehaviour
{
    [SerializeField] private LevelSettingsSO levelSettings;
    [SerializeField] private IntVariableSO numOfGameBlockersInt;
    [SerializeField] private GameObject vfxForHoleDisappear;

    [Space]
    [Tooltip("Overall animation of tiles speed")]
    [SerializeField] private float animSpeed = 1f;

    [Header("Tile sliding params")]
    [Tooltip("Time between starting tiles to slide when multiple tiles moving")]
    [SerializeField] private float slideStepTime = 0.2f;
    [SerializeField] private float slideDuration = 0.5f;

    [Header("Wrong Shake params")]
    [SerializeField] private float shakeDuration = 0.6f;
    [SerializeField] private Vector3 shakeStrength;
    [SerializeField] private int shakeVibrato = 20;

    [Header("Hole tile removing params")]
    [Tooltip("Relative vector to move hole tile")]
    [SerializeField] private Vector3 extractionVector;
    [SerializeField] private float extractionDuration = 0.4f;
    [Tooltip("Relative vector to rotate hole tile")]
    [SerializeField] private Vector3 rotationVector;
    [SerializeField] private float rotationDuration = 0.5f;
    [SerializeField] private float vfxDelay;

    private Tile holeTile;


    private void Awake()
    {
        // DoTween will reuse tweens using it's object pool, be careful with tween references.
        DOTween.Init(true, true, LogBehaviour.Default);
    }


    // In the beginning of any movement
    private void OnTweenStart()
    {
        numOfGameBlockersInt.Value++;
        Events.onTileStartAnyMovement.Invoke();
    }


    // In the end of any movement
    private void OnTweenComplete()
    {
        numOfGameBlockersInt.Value--;
        Events.onTileCompleteAnyMovement.Invoke();
    }


    // tween shake reaction for unmovable tile click
    internal void WrongTileClick(Tile tileClicked)
    {
        Tweener tweener = tileClicked.transform.DOShakePosition(shakeDuration / animSpeed, shakeStrength, shakeVibrato);
        tweener.OnStart(OnTweenStart).OnKill(OnTweenComplete);
        Events.onWrongTileClicked.Invoke();
    }


    // animation and vfx of disappearance of hole tile selected by user 
    internal void RemoveHoleTile(Tile tileToRemove)
    {
        holeTile = tileToRemove;

        // Save initial position, rotation and scale of the tile
        Vector3 initPos = holeTile.transform.position;
        Quaternion initRot = holeTile.transform.rotation;
        Vector3 initScale = holeTile.transform.localScale;

        StartCoroutine(HoleDisappearVFX());
        // Make sequence of tweens
        Sequence sequence = DOTween.Sequence();
        sequence.Append(holeTile.transform.DOLocalMove(extractionVector, extractionDuration / animSpeed).SetRelative().SetEase(Ease.OutCubic))
            .Append(holeTile.transform.DOLocalRotate(rotationVector, rotationDuration / animSpeed, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.InBack))
            .Join(holeTile.transform.DOScale(0f, rotationDuration / animSpeed).SetEase(Ease.InBack))
            .PrependCallback(OnTweenStart)
            .AppendCallback(() => RemoveHoleTileFinalizer(initPos, initRot, initScale))
            .AppendCallback(OnTweenComplete);

        Events.onHoleTileRemovingStarted.Invoke();
    }


    // Speeds up and slows down unity time scale for cool shuffling
    internal void SpeedUpTime(float speedUpTime, float speedUpFactor)
    {
        Sequence sequence = DOTween.Sequence();
        Time.timeScale = 0.6f;
        sequence.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, speedUpFactor, speedUpTime * 0.4f).SetEase(Ease.InOutQuad))
                .AppendInterval(speedUpTime * 0.2f)
                .Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.6f, speedUpTime * 0.4f).SetEase(Ease.InOutQuad))
                .AppendCallback(() => Time.timeScale = 1f)
                .SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
    }


    private IEnumerator HoleDisappearVFX()
    {
        yield return new WaitForSeconds(vfxDelay / animSpeed);
        GameObject effect = Instantiate(vfxForHoleDisappear, holeTile.transform.position, Quaternion.identity);
        Destroy(effect, 3f);
        Events.onHoleTileVfxAppear.Invoke();
    }


    // deactivate hole tile and restore its transform to initial
    private void RemoveHoleTileFinalizer(Vector3 initPos, Quaternion initRot, Vector3 initScale)
    {
        holeTile.gameObject.SetActive(false);
        holeTile.transform.position = initPos;
        holeTile.transform.rotation = initRot;
        holeTile.transform.localScale = initScale;
    }


    // Slide move group of tiles in place of the hole
    internal void SlideTiles(Tile[] tilesToMove, Direction dir)
    {
        // Relative moving vector
        Vector3 moveByVector = dir.GetUnitVector3() * levelSettings.TileStepDistance;

        // Tweening sequence
        Sequence sequence = DOTween.Sequence();
        for (int i = tilesToMove.Length - 1; i >= 0; i--)
        {
            // pause befor tile start sliding
            float interval = (tilesToMove.Length - 1 - i) * slideStepTime / animSpeed;
            sequence.Insert(interval, tilesToMove[i].transform.DOMove(moveByVector, slideDuration / animSpeed).SetRelative().SetEase(Ease.InQuad))
                .OnStart(() => Events.onTileStartSliding.Invoke())
                .OnKill(() => Events.onTileStopSliding.Invoke());
        }
        sequence.PrependCallback(OnTweenStart).AppendCallback(OnTweenComplete);

        // Hole tile goes to new place
        holeTile.transform.position = tilesToMove[0].transform.position;
    }
}
