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

    // main array of all tiles
    // indexation goes along X and Y axises. x,y = 1,2 means second column and third row from bottom-left corner
    private Tile[,] tiles;
    // The tile, that'll be hidden to meke hole in the puzzle
    private Tile holeTile;
    // Game begins as soon as player choses tile to remove
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
            tiles[tileRefs[i].CorrectIndX, tileRefs[i].CorrectIndY] = tileRefs[i];
            tileRefs[i].IndX = tileRefs[i].CorrectIndX;
            tileRefs[i].IndY = tileRefs[i].CorrectIndY;
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
        if (tileClicked.IndX != holeTile.IndX &&
            tileClicked.IndY != holeTile.IndY)
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
        int previousHoleIndX = holeTile.IndX;
        int previousHoleIndY = holeTile.IndY;
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
            for (int i = 0; i < levelSettings.PuzzleSize; i++)
            {
                Tile tile = tiles[holeTile.IndX, i];
                if (tile != holeTile && tile != tiles[previousHoleIndX, previousHoleIndY])
                {
                    tilesAvilable.Add(tile);
                }
                tile = tiles[i, holeTile.IndY];
                if (tile != holeTile && tile != tiles[previousHoleIndX, previousHoleIndY])
                {
                    tilesAvilable.Add(tile);
                }
            }
            // Randomly choose one tile from available ones
            tileToMove = tilesAvilable[UnityEngine.Random.Range(0, tilesAvilable.Count)];
            previousHoleIndX = holeTile.IndX;
            previousHoleIndY = holeTile.IndY;

            MoveTiles(tileToMove);
            yield return null;
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
        int tilesToMoveCount = Mathf.Abs(holeTile.IndX - tileClicked.IndX + holeTile.IndY - tileClicked.IndY);

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
        tiles[neighbor.IndX, neighbor.IndY] = tile;
        tiles[tile.IndX, tile.IndY] = neighbor;
        // swap CurrentIndexes of tiles
        int varForSwapX = tile.IndX;
        int varForSwapY = tile.IndY;
        tile.IndX = neighbor.IndX;
        tile.IndY = neighbor.IndY;
        neighbor.IndX = varForSwapX;
        neighbor.IndY = varForSwapY;
    }

    private Tile GetTileInDirectionFromTarget(Tile targetTile, Direction dir)
    {
        // Get direction unit vector
        Vector3 dirVector3 = dir.GetUnitVector3();
        // Convert it to "array vector"
        Vector2Int dirIndVector = new Vector2Int(Mathf.RoundToInt(dirVector3.x), Mathf.RoundToInt(dirVector3.y));

        int neededTileIndX = targetTile.IndX + dirIndVector.x;
        int neededTileIndY = targetTile.IndY + dirIndVector.y;

        if (!AreIndexesInArrayRange(neededTileIndX, neededTileIndY))
        {
            Debug.Log("Attempt to get tile from out of array bounds.");
            return null;
        }
        
        return tiles[neededTileIndX, neededTileIndY];
    }

    private bool AreIndexesInArrayRange(int indX, int indY)
    {
        if (indX < 0 || indY < 0 )
        {
            return false;
        }
        if (indX >= tiles.GetLength(0) || indY >= tiles.GetLength(1))
        {
            return false;
        }
        return true;
    }
}
