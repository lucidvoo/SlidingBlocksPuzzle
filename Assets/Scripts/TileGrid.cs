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
    private bool isBoardShuffled = false;


    private void OnEnable()
    {
        Events.onTileClicked.AddListener(OnTileClicker_Handler);
        Events.onTileCompleteAnyMovement.AddListener(CheckForWin);
    }

    private void OnDisable()
    {
        Events.onTileClicked.RemoveListener(OnTileClicker_Handler);
        Events.onTileCompleteAnyMovement.RemoveListener(CheckForWin);
    }

    private void Awake()
    {
        InitializeTileArray();
    }


    // fill 2-dim tiles array with refs from editor. Init tiles.
    private void InitializeTileArray()
    {
        if (tileRefs.Length != levelSettings.PuzzleSize * levelSettings.PuzzleSize)
        {
            Debug.LogError("Something wrong with initialization. Check Level settings SO and Tile Refs in TileGrid");
        }

        tiles = new Tile[levelSettings.PuzzleSize, levelSettings.PuzzleSize];

        // find bottom-left tile position
        Vector3 zeroTileOffset = tileRefs[0].transform.position;
        foreach (Tile tile in tileRefs)
        {
            if (tile.transform.position.x < zeroTileOffset.x || tile.transform.position.y < zeroTileOffset.y)
            {
                zeroTileOffset = tile.transform.position;
            }
        }
        // Initialize Tiles and fill tiles[] array
        for (int i = 0; i < tileRefs.Length; i++)
        {
            tileRefs[i].Init(zeroTileOffset, levelSettings.TileStepDistance);
            int indX = tileRefs[i].IndX();
            int indY = tileRefs[i].IndY();
            tiles[indX, indY] = tileRefs[i];
        }
    }


    // Invokes every time when tile clicked
    private void OnTileClicker_Handler(Tile tileClicked)
    {
        if (!numOfGameBlockersInt.IsZero())
        {
            return;
        }

        // Choosing a tile to remove at the very beginning and shuffle
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
        if (tileClicked.IndX() != holeTile.IndX() &&
            tileClicked.IndY() != holeTile.IndY())
        {
            tileMover.WrongTileClick(tileClicked);
            return;
        }

        StartTilesMovement(tileClicked);
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

        Events.onBoardStartShuffling.Invoke();

        tileMover.SpeedUpTime(shuffleTime, speedUpFactor);

        float timeToStopShuffling = Time.realtimeSinceStartup + shuffleTime;
        Tile tileToMove;
        bool moveHorizontally = true;
        List<Tile> tilesAvilable = new List<Tile>(levelSettings.PuzzleSize - 1);
        while (true)
        {
            yield return WaitUntilGameUnblocks();

            // quit shuffling by timer
            if (Time.realtimeSinceStartup > timeToStopShuffling)
            {
                break;
            }

            tilesAvilable.Clear();

            // Make list of tiles available to move
            for (int i = 0; i < levelSettings.PuzzleSize; i++)
            {
                Tile tile = moveHorizontally ? tiles[i, holeTile.IndY()] : tiles[holeTile.IndX(), i];
                
                if (tile != holeTile)
                {
                    tilesAvilable.Add(tile);
                }
            }
            moveHorizontally = !moveHorizontally;

            // Randomly choose one tile from available ones
            tileToMove = tilesAvilable[UnityEngine.Random.Range(0, tilesAvilable.Count)];

            StartTilesMovement(tileToMove);
            yield return null;
        }
        isBoardShuffled = true;
        Time.timeScale = 1f; // just to be sure
        Events.onBoardShuffled.Invoke();
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
    private void StartTilesMovement(Tile tileClicked)
    {
        Vector2Int indexVector = new Vector2Int(holeTile.IndX() - tileClicked.IndX(), holeTile.IndY() - tileClicked.IndY());
        // Direction for tile moving
        Direction dir = indexVector.ConvertToDirection();
        // How many tiles to move (only one index delta is non-zero)
        int tilesToMoveCount = Mathf.Abs(indexVector.x + indexVector.y); 

        // tiles that have to be moved are gathering in array. First tile - clicked one, last tile - closest to hole
        Tile[] tilesToMove = new Tile[tilesToMoveCount];
        tilesToMove[0] = tileClicked;
        for (int i = 1; i < tilesToMoveCount; i++)
        {
            tilesToMove[i] = GetTileInDirectionFromTarget(tilesToMove[i - 1], dir);
        }

        SlideTileArray(tilesToMove, dir);
    }


    // Shift group of tiles in array tiles[] and move gameobjects
    private void SlideTileArray(Tile[] tilesToMove, Direction dir)
    {
        int writeIndX = holeTile.IndX();
        int writeIndY = holeTile.IndY();
        // "gradual shift" tiles in direction of hole
        for (int i = tilesToMove.Length - 1; i >= 0; i--)
        {
            tiles[writeIndX, writeIndY] = tilesToMove[i];
            writeIndX = tilesToMove[i].IndX();
            writeIndY = tilesToMove[i].IndY();
        }
        // move hole in place of last moved tile
        tiles[writeIndX, writeIndY] = holeTile;

        // move actual gameobjects
        tileMover.SlideTiles(tilesToMove, dir);
    }


    private Tile GetTileInDirectionFromTarget(Tile targetTile, Direction dir)
    {
        // Get direction unit vector
        Vector2Int dirIndVector = dir.GetUnitVector2Int();

        int neededTileIndX = targetTile.IndX() + dirIndVector.x;
        int neededTileIndY = targetTile.IndY() + dirIndVector.y;

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


    // Check win conditions when onTileCompleteAnyMovement event is fired
    private void CheckForWin()
    {
        if (!isBoardShuffled)
        {
            return;
        }

        // Check if all tiles in their correct places
        int tilesChecked = 0;
        foreach (Tile tile in tiles)
        {
            if (tile.IndX() != tile.CorrectIndX || tile.IndY() != tile.CorrectIndY)
            {
                break;
            }
            tilesChecked++;
        }

        if (tilesChecked == tiles.Length)
        {
            tileMover.WinSequence();
        }
    }
}
