using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Storing tile array and manipulating tiles on grid level

public class TileGrid : MonoBehaviour
{
    [SerializeField] private LevelSettingsSO levelSettings;
    [Tooltip("Add all tiles in any order")]
    [SerializeField] private Tile[] tileRefs;
    [SerializeField] private IntVariableSO numOfGameBlockersInt;
    [SerializeField] private TileMover tileMover;
    [SerializeField] private float pauseBtwHoleRemoveAndShuffle = 0.6f;
    [SerializeField] private float shuffleTime = 4f;
    [SerializeField] private float speedUpFactor = 6f;

    private Tile[,] tiles;
    // The tile, that'll be hidden to meke hole in the puzzle
    private Tile holeTile;
    private bool isGameBegun = false;


    private void OnEnable()
    {
        Events.onTileClicked.AddListener(OnTileClicker_Handler);
    }

    private void OnDisable()
    {
        Events.onTileClicked.RemoveListener(OnTileClicker_Handler);
    }


    private void Awake()
    {
        InitializeTileArray();
    }

    // fill 2-dim tiles array with refs from editor
    private void InitializeTileArray()
    {
        if (tileRefs.Length != levelSettings.PuzzleSize * levelSettings.PuzzleSize)
        {
            Debug.LogError("Something wrong with initialization. Check Level settings SO and Tile Refs in TileGrid");
        }

        tiles = new Tile[levelSettings.PuzzleSize, levelSettings.PuzzleSize];

        for (int i = 0; i < tileRefs.Length; i++)
        {
            tiles[tileRefs[i].CorrectTileIndexes.x, tileRefs[i].CorrectTileIndexes.y] = tileRefs[i];
            tileRefs[i].Indexes = tileRefs[i].CorrectTileIndexes;
        }
    }


    // Invokes every time when tile clicked
    private void OnTileClicker_Handler(Tile tileClicked)
    {
        if (!numOfGameBlockersInt.IsZero())
        {
            return;
        }

        // Choosing a tile to remove at the very beginning
        if (!isGameBegun)
        {
            isGameBegun = true;

            holeTile = tileClicked;
            holeTile.IsHoleTile = true;

            tileMover.RemoveHoleTile(tileClicked);

            StartCoroutine(ShuffleBoard());

            return;
        }

        // Check if holeTile is in the same row or column as clicked tile
        if (tileClicked.Indexes.x != holeTile.Indexes.x &&
            tileClicked.Indexes.y != holeTile.Indexes.y)
        {
            tileMover.WrongTileClick(tileClicked);
            return;
        }

        MoveTiles(tileClicked);
    }

    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(0.1f);
        // wait for tile removal tween completed
        yield return WaitUntilGameUnblocks();

        // as soon as gameplay unblocked, pause
        numOfGameBlockersInt.Value++;
        yield return new WaitForSeconds(pauseBtwHoleRemoveAndShuffle);
        numOfGameBlockersInt.Value--;

        // check if nothing blocks us
        yield return WaitUntilGameUnblocks();

        tileMover.SpeedUpTime(shuffleTime, speedUpFactor);

        float timeToStopShuffling = Time.realtimeSinceStartup + shuffleTime;
        Tile tileToMove;
        Vector2Int previousHoleCoords = holeTile.Indexes;
        List<Tile> tilesAvilable = new List<Tile>(levelSettings.PuzzleSize * 2 - 2);
        while (true)
        {
            yield return WaitUntilGameUnblocks();

            if (Time.realtimeSinceStartup > timeToStopShuffling)
            {
                break;
            }

            tilesAvilable.Clear();

            // find all tiles avilable to move, except one where the hole was in previous movement

            // TODO: CORRECT MISTAKE SOMEWHERE HERE!

            for (int i = 0; i < levelSettings.PuzzleSize; i++)
            {
                Tile tile = tiles[holeTile.Indexes.x, i];
                if (tile != holeTile && tile != tiles[previousHoleCoords.x, previousHoleCoords.y])
                {
                    tilesAvilable.Add(tile);
                }
                tile = tiles[i, holeTile.Indexes.y];
                if (tile != holeTile && tile != tiles[previousHoleCoords.x, previousHoleCoords.y])
                {
                    tilesAvilable.Add(tile);
                }
            }
            // Randomly choose one tile from available ones
            tileToMove = tilesAvilable[UnityEngine.Random.Range(0, tilesAvilable.Count)];
            previousHoleCoords = holeTile.Indexes;

            // CHANGE WHEN MISTAKE WILL BE CORRECTED !
            //MoveTiles(tileToMove);
            OnTileClicker_Handler(tileToMove);
        }
    }

    private IEnumerator WaitUntilGameUnblocks()
    {
        while (true)
        {
            if (numOfGameBlockersInt.IsZero())
            {
                break;
            }
            yield return null;
        }
    }

    // Successful tile click - one or more tiles will be moved.
    private void MoveTiles(Tile tileClicked)
    {
        // Direction for tile moving
        Direction dir = (holeTile.transform.position - tileClicked.transform.position).ComputeDirectionFromVector3();
        // How many tiles to move (only one index delta is non-zero)
        int tilesToMoveCount = Mathf.Abs(holeTile.Indexes.x - tileClicked.Indexes.x) +
                               Mathf.Abs(holeTile.Indexes.y - tileClicked.Indexes.y);

        // tiles that have to be moved are gathering in array
        Tile[] tilesToMove = new Tile[tilesToMoveCount];
        tilesToMove[0] = tileClicked;
        for (int i = 1; i < tilesToMoveCount; i++)
        {
            tilesToMove[i] = GetTileInDirectionFromTarget(tilesToMove[i - 1], dir);
        }

        // swap tiles in tiles array and refresh tile indexes 
        for (int i = tilesToMove.Length - 1; i >= 0; i--)
        {
            SwapTileWithHole(tilesToMove[i]);
        }

        tileMover.SlideTiles(tilesToMove, dir);
    }

    // Swap tile with hole near by in tiles array. Refresh Indexes. It doesn't move actual gameobjects
    private void SwapTileWithHole(Tile tile)
    {
        Tile neighbor = null;
        Direction d;
        for (d = Direction.UP; d < Direction.Count; d++)
        {
            neighbor = GetTileInDirectionFromTarget(tile, d);
            if (neighbor == null)
            {
                continue;
            }
            else if (neighbor.IsHoleTile)
            {
                break;
            }
        }
        if (d == Direction.Count)
        {
            Debug.LogWarning("Attempt to swap tile with no hole near by");
            return;
        }
        // swap tiles in array
        tiles[neighbor.Indexes.x, neighbor.Indexes.y] = tile;
        tiles[tile.Indexes.x, tile.Indexes.y] = neighbor;
        // swap CurrentIndexes of tiles
        Vector2Int varForSwap = tile.Indexes;
        tile.Indexes = neighbor.Indexes;
        neighbor.Indexes = varForSwap;
    }

    private Tile GetTileInDirectionFromTarget(Tile targetTile, Direction dir)
    {
        // Get direction unit vector
        Vector3 dirVector3 = dir.GetUnitVector3();
        // Convert it to "array vector"
        Vector2Int dirIndVector = new Vector2Int(Mathf.RoundToInt(-dirVector3.y), Mathf.RoundToInt(dirVector3.x));

        Vector2Int neededTileIndexes = targetTile.Indexes + dirIndVector;

        if (!AreIndexesInArrayRange(neededTileIndexes))
        {
            Debug.Log("Attempt to get tile from out of array bounds.");
            return null;
        }
        
        return tiles[neededTileIndexes.x, neededTileIndexes.y];
    }

    private bool AreIndexesInArrayRange(Vector2Int indexes)
    {
        if (indexes.x < 0 || indexes.y < 0 )
        {
            return false;
        }
        if (indexes.x >= tiles.GetLength(0) || indexes.y >= tiles.GetLength(1))
        {
            return false;
        }
        return true;
    }
}
