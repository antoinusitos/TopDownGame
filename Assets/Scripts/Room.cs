using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData                 myRoomData;
    public Tile                     myTileFloorPrefab = null;
    public Tile                     myTileWallPrefab = null;
    public bool                     myStartingRoom = false;

    public Tile                     myBottomSpawningTile = null;
    public Tile                     myTopSpawningTile = null;
    public Tile                     myLeftSpawningTile = null;
    public Tile                     myRightSpawningTile = null;

    public Transition               myBottomTransitionTile = null;
    public Transition               myTopTransitionTile = null;
    public Transition               myLeftTransitionTile = null;
    public Transition               myRightTransitionTile = null;

    public Enemy                    myEnemyPrefab = null;

    public List<TransitionData>     myTransitions = new List<TransitionData>();

    public ResourceUsable           myResourcePrefab = null;
    public Transform                myDecorationPrefab = null;
    private List<ResourceUsable>    myResourceUsables = new List<ResourceUsable>();

    private List<Enemy>             myEnemies = new List<Enemy>();

    private Tile                    myMidTile = null;
    private List<Tile>              myFloorTiles = new List<Tile>();
    private List<Tile>              myWallTiles = new List<Tile>();

    private const float             mySpriteSpace = 1;//2.5f;

    private Biome                   myBiome = null;

    private int                     myRoomSize = 0;

    private float                   myLastTime = 0;

    private const int               myDecorationNumber = 15;
    private const int               myResourceNumber = 4;
    private const int               myEnemiesNumber = 3;

    private TileType[,]             myGrid;
    private Tile[]                  myGridBisBis;
    private int                     myRoomHeight;
    private int                     myRoomWidth;
    private Vector2                 myRoomSizeWorldUnits = Vector2.one * Data.myRoomSideSize;
    private float                   myWorldUnitsInOneGridCell = 1;

    private List<Walker>            myWalkers;
    private float                   myChanceWalkerChangeDir = 0.75f;//0.5f;
    private float                   myChangeWalkerSpawn = 0.1f;//0.05f;
    private float                   myChanceWalkerDestroy = 0.03f;//0.05f;
    private int                     myMaxWalkers = 10;
    private float                   myPercentToFill = 0.2f;

    private List<GameObject>        mySpawned;

    private int                     myWestX = -1;
    private int                     myWestY = -1;
    private int                     myEastX = -1;
    private int                     myEastY = -1;
    private int                     myNorthX = -1;
    private int                     myNorthY = -1;
    private int                     mySouthX = -1;
    private int                     mySouthY = -1;

    public GameObject               myWallObj;
    public GameObject               myFloorObj;

    public int                      mySeed = 0;

    public void GetSeedAndGenerate(Biome aBiome)
    {
        myBiome = aBiome;
        mySeed = Data.myCurrentSeedRoom;
        Data.myCurrentSeedRoom++;
        Generate();
    }

    private void Generate()
    {
        Random.InitState(mySeed);

        Setup();
        CreateFloors();
        CreateWalls();
        RemoveSingleWalls();
        SpawnLevel();
        FindExtremeFloors();
    }

    private void Setup()
    {
        //find grid size
        myRoomHeight = Mathf.RoundToInt(myRoomSizeWorldUnits.x / myWorldUnitsInOneGridCell);
        myRoomWidth = Mathf.RoundToInt(myRoomSizeWorldUnits.y / myWorldUnitsInOneGridCell);

        //create grid
        myGrid = new TileType[myRoomWidth, myRoomHeight];
        myGridBisBis = new Tile[myRoomWidth * myRoomHeight];

        //set grid's default state
        for (int x = 0; x < myRoomWidth; x++)
        {
            for (int y = 0; y < myRoomHeight; y++)
            {
                myGrid[x, y] = TileType.EMPTY;
            }
        }

        //set first walker
        //init list
        myWalkers = new List<Walker>();

        //create a walker
        Walker newWalker = new Walker();
        newWalker.myDir = RandomDirection();

        //find center of grid
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(myRoomWidth / 2), Mathf.RoundToInt(myRoomHeight / 2));
        newWalker.myPos = spawnPos;

        //add walker to list
        myWalkers.Add(newWalker);

        mySpawned = new List<GameObject>();
    }

    private Vector2 RandomDirection()
    {
        //pick random int between 0 and 3
        int choice = Mathf.FloorToInt(Random.value * 3.99f);

        //use that int to chose a direction
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            default:
                return Vector2.right;
        }
    }

    private void CreateFloors()
    {
        //loop will not run forever
        int iterations = 0;
        do
        {
            //create floor at position of every walker
            foreach (Walker aWalker in myWalkers)
            {
                myGrid[(int)aWalker.myPos.x, (int)aWalker.myPos.y] = TileType.FLOOR;
            }

            //chance: destroy walker
            int numberChecks = myWalkers.Count;
            for (int i = 0; i < numberChecks; i++)
            {
                //only if it's not the only one and at a low chance
                if (Random.value < myChanceWalkerDestroy && myWalkers.Count > 1)
                {
                    myWalkers.RemoveAt(i);
                    break;
                }
            }

            //chance: walker pick new direction
            for (int i = 0; i < myWalkers.Count; i++)
            {
                if (Random.value < myChanceWalkerChangeDir)
                {
                    Walker aWalker = myWalkers[i];
                    aWalker.myDir = RandomDirection();
                    myWalkers[i] = aWalker;
                }
            }

            //chance: spawn new walker
            numberChecks = myWalkers.Count;
            for (int i = 0; i < numberChecks; i++)
            {
                //only if # of walkers < max and at a low chance
                if (Random.value < myChangeWalkerSpawn && myWalkers.Count < myMaxWalkers)
                {
                    Walker newWalker = new Walker();
                    newWalker.myDir = RandomDirection();
                    newWalker.myPos = myWalkers[i].myPos;
                    myWalkers.Add(newWalker);
                }
            }

            //move walkers
            for (int i = 0; i < myWalkers.Count; i++)
            {
                Walker aWalker = myWalkers[i];
                aWalker.myPos += aWalker.myDir;
                myWalkers[i] = aWalker;
            }

            //avoid border of grid
            for (int i = 0; i < myWalkers.Count; i++)
            {
                Walker aWalker = myWalkers[i];

                //clamp x,y to leave a 1 space border for the walls
                aWalker.myPos.x = Mathf.Clamp(aWalker.myPos.x, 1, myRoomWidth - 2);
                aWalker.myPos.y = Mathf.Clamp(aWalker.myPos.y, 1, myRoomHeight - 2);
                myWalkers[i] = aWalker;
            }

            //check to exit loop
            if (((float)NumberOfFloors() / (float)myGrid.Length) > myPercentToFill)
            {
                break;
            }

            iterations++;

        } while (iterations < 100000);
    }

    private int NumberOfFloors()
    {
        int count = 0;
        foreach (TileType space in myGrid)
        {
            if (space == TileType.FLOOR)
            {
                count++;
            }
        }
        return count;
    }

    private void SpawnLevel()
    {
        for (int x = 0; x < myRoomWidth; x++)
        {
            for (int y = 0; y < myRoomHeight; y++)
            {
                switch (myGrid[x, y])
                {
                    case TileType.EMPTY:
                        break;
                    case TileType.FLOOR:
                        Spawn(x, y, myFloorObj, myGrid[x, y]);
                        break;
                    case TileType.WALL:
                        Spawn(x, y, myWallObj, myGrid[x, y]);
                        break;
                }
            }
        }
    }

    private void Spawn(float aX, float aY, GameObject aToSpawn, TileType aTileType)
    {
        //find the position to spawn
        Vector2 offset = myRoomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(aX, aY) * myWorldUnitsInOneGridCell - offset;

        //spawn object
        GameObject spawned = Instantiate(aToSpawn, transform);
        spawned.transform.localPosition = spawnPos;
        spawned.name = spawned.name + (aX * myRoomWidth + aY).ToString();

        mySpawned.Add(spawned);
        spawned.transform.parent = transform;

        Tile tile = spawned.GetComponent<Tile>();
        tile.myTileData.myX = (int)aX;
        tile.myTileData.myY = (int)aY;
        tile.myIndex = (int)aX * myRoomWidth + (int)aY;
        tile.myTileData.myType = myBiome.myBiomeType;
        tile.myTileType = aTileType;
        tile.myParentRoom = this;

        myGridBisBis[(int)aX * myRoomWidth + (int)aY] = tile;

        if (aX == Data.myRoomSideSize / 2 && aY == Data.myRoomSideSize / 2)
        {
            myMidTile = tile;
        }

        if(aTileType == TileType.FLOOR)
        {
            myFloorTiles.Add(tile);
        }
        else if( aTileType == TileType.WALL)
        {
            myWallTiles.Add(tile);
        }
    }

    private void CreateWalls()
    {
        //loop through every grid space
        for (int x = 0; x < myRoomWidth - 1; x++)
        {
            for (int y = 0; y < myRoomHeight - 1; y++)
            {
                //if there is a floor, check the space around it
                if (myGrid[x, y] == TileType.FLOOR)
                {
                    //if any surrounding spaces are empty, place a wall
                    if (myGrid[x, y + 1] == TileType.EMPTY)
                    {
                        myGrid[x, y + 1] = TileType.WALL;
                    }
                    if (myGrid[x, y - 1] == TileType.EMPTY)
                    {
                        myGrid[x, y - 1] = TileType.WALL;
                    }
                    if (myGrid[x + 1, y] == TileType.EMPTY)
                    {
                        myGrid[x + 1, y] = TileType.WALL;
                    }
                    if (myGrid[x - 1, y] == TileType.EMPTY)
                    {
                        myGrid[x - 1, y] = TileType.WALL;
                    }
                }
            }
        }
    }

    public void CheckWallsSurrounding()
    {
        for (int x = 0; x < myRoomWidth - 1; x++)
        {
            for (int y = 0; y < myRoomHeight - 1; y++)
            {
                if (myGrid[x, y] == TileType.WALL)
                {
                    Tile t = null;
                    for (int i = 0; i < myWallTiles.Count; i++)
                    {
                        if (myWallTiles[i].myTileData.myX == x && myWallTiles[i].myTileData.myY == y)
                        {
                            t = myWallTiles[i];
                            break;
                        }
                    }

                    if (y < myRoomHeight - 2 && myGrid[x, y + 1] == TileType.WALL)
                    {
                        if(t != null)
                        {
                            t.myHasTopNeighbour = true;
                        }
                    }
                    if (y > 0 && myGrid[x, y - 1] == TileType.WALL)
                    {
                        if (t != null)
                        {
                            t.myHasDownNeighbour = true;
                        }
                    }
                    if (x < myRoomWidth - 2 && myGrid[x + 1, y] == TileType.WALL)
                    {
                        if (t != null)
                        {
                            t.myHasRightNeighbour = true;
                        }
                    }
                    if (x > 0 && myGrid[x - 1, y] == TileType.WALL)
                    {
                        if (t != null)
                        {
                            t.myHasLeftNeighbour = true;
                        }
                    }
                }
            }
        }
    }

    private void RemoveSingleWalls()
    {
        //loop through every grid space
        for (int x = 0; x < myRoomWidth - 1; x++)
        {
            for (int y = 0; y < myRoomHeight - 1; y++)
            {
                //if there is a wall, check the space around it
                if (myGrid[x, y] == TileType.WALL)
                {
                    //assume all space aroud wall are floors
                    bool allFloor = true;
                    //check each side to see if they are all floors
                    for (int checkX = -1; checkX <= 1; checkX++)
                    {
                        for (int checkY = -1; checkY <= 1; checkY++)
                        {
                            if (x + checkX < 0 || x + checkX > myRoomWidth - 1 ||
                                y + checkY < 0 || y + checkY > myRoomHeight - 1)
                            {
                                //skip check that are out of range
                                continue;
                            }
                            if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
                            {
                                //skip corners and center
                                continue;
                            }
                            if (myGrid[x + checkX, y + checkY] != TileType.FLOOR)
                            {
                                allFloor = false;
                            }
                        }
                    }
                    if (allFloor)
                    {
                        myGrid[x, y] = TileType.FLOOR;
                    }
                }
            }
        }
    }

    private void FindExtremeFloors()
    {
        myWestX = myRoomWidth;
        myWestY = myRoomHeight;
        myEastX = 0;
        myEastY = 0;
        mySouthX = myRoomHeight;
        mySouthY = myRoomHeight;
        myNorthX = 0;
        myNorthY = 0;
        //loop through every grid space
        for (int x = 0; x < myRoomWidth - 1; x++)
        {
            for (int y = 0; y < myRoomHeight - 1; y++)
            {
                if (myGrid[x, y] == TileType.FLOOR)
                {
                    if (x <= myWestX)
                    {
                        myWestX = x;
                        myWestY = y;
                    }
                    if (x >= myEastX)
                    {
                        myEastX = x;
                        myEastY = y;
                    }
                    if (y >= myNorthY)
                    {
                        myNorthX = x;
                        myNorthY = y;
                    }
                    if (y <= mySouthY)
                    {
                        mySouthX = x;
                        mySouthY = y;
                    }
                }
            }
        }

        for (int i = 0; i < mySpawned.Count; i++)
        {
            Tile tile = mySpawned[i].GetComponent<Tile>();

            if (tile.myTileData.myX == myWestX && tile.myTileData.myY == myWestY)
            {
                myLeftSpawningTile = tile;
            }
            if (tile.myTileData.myX == myEastX && tile.myTileData.myY == myEastY)
            {
                myRightSpawningTile = tile;
            }
            if (tile.myTileData.myX == myNorthX && tile.myTileData.myY == myNorthY)
            {
                myTopSpawningTile = tile;
            }
            if (tile.myTileData.myX == mySouthX && tile.myTileData.myY == mySouthY)
            {
                myBottomSpawningTile = tile;
            }

            if (tile.myTileData.myX == myWestX - 1 && tile.myTileData.myY == myWestY)
            {
                myLeftTransitionTile = SetTransitionTile(mySpawned[i]);
                continue;
            }
            else if (tile.myTileData.myX == myEastX + 1 && tile.myTileData.myY == myEastY)
            {
                myRightTransitionTile = SetTransitionTile(mySpawned[i]);
                continue;
            }
            else if (tile.myTileData.myX == myNorthX && tile.myTileData.myY == myNorthY + 1)
            {
                myTopTransitionTile = SetTransitionTile(mySpawned[i]);
                continue;
            }
            else if (tile.myTileData.myX == mySouthX && tile.myTileData.myY == mySouthY - 1)
            {
                myBottomTransitionTile = SetTransitionTile(mySpawned[i]);
                continue;
            }
        }

    }

    private Transition SetTransitionTile(GameObject aTile)
    {
        aTile.GetComponentInChildren<SpriteRenderer>().color = Color.green;
        Transition toReturn = aTile.AddComponent<Transition>();
        aTile.GetComponent<Collider2D>().isTrigger = true;
        aTile.layer = 8;

        return toReturn;
    }

    public void AffectTransitions()
    {
        //Affect Left transition
        if(myRoomData.myX > 0)
        {
            Room room = myBiome.GetRoom(myRoomData.myX - 1, myRoomData.myY);
            if(room != null)
            {
                myLeftTransitionTile.myNextTransition = room.myRightTransitionTile;
                myLeftTransitionTile.myNextRoomTile = room.myRightSpawningTile;
            }
        }
        //Affect Right Transition
        if (myRoomData.myX < Data.myBiomeSideSize)
        {
            Room room = myBiome.GetRoom(myRoomData.myX + 1, myRoomData.myY);
            if (room != null)
            {
                myRightTransitionTile.myNextTransition = room.myLeftTransitionTile;
                myRightTransitionTile.myNextRoomTile = room.myLeftSpawningTile;
            }
        }
        //Affect Bottom Transition
        if (myRoomData.myY > 0)
        {
            Room room = myBiome.GetRoom(myRoomData.myX, myRoomData.myY - 1);
            if (room != null)
            {
                myBottomTransitionTile.myNextTransition = room.myTopTransitionTile;
                myBottomTransitionTile.myNextRoomTile = room.myTopSpawningTile;
            }
        }
        //Affect Top Transition
        if (myRoomData.myY < Data.myBiomeSideSize)
        {
            Room room = myBiome.GetRoom(myRoomData.myX, myRoomData.myY + 1);
            if (room != null)
            {
                myTopTransitionTile.myNextTransition = room.myBottomTransitionTile;
                myTopTransitionTile.myNextRoomTile = room.myBottomSpawningTile;
            }
        }
    }

    public void RemoveAllEmptyTransitions()
    {
        if(myLeftTransitionTile.myNextTransition == null)
        {
            myLeftTransitionTile.GetComponent<SpriteRenderer>().color = Color.white;
            Destroy(myLeftTransitionTile);
        }
        if (myRightTransitionTile.myNextTransition == null)
        {
            myRightTransitionTile.GetComponent<SpriteRenderer>().color = Color.white;
            Destroy(myRightTransitionTile);
        }
        if (myTopTransitionTile.myNextTransition == null)
        {
            myTopTransitionTile.GetComponent<SpriteRenderer>().color = Color.white;
            Destroy(myTopTransitionTile);
        }
        if (myBottomTransitionTile.myNextTransition == null)
        {
            myBottomTransitionTile.GetComponent<SpriteRenderer>().color = Color.white;
            Destroy(myBottomTransitionTile);
        }
    }

    public void AffectTransitionsTo(Room aRoom)
    {

    }

    public void ChangeTileRendering()
    {
        for(int i = 0; i < myFloorTiles.Count; i++)
        {
            myFloorTiles[i].GetComponent<TileRendererChanger>().ChangeRendering();
        }
        for (int i = 0; i < myWallTiles.Count; i++)
        {
            myWallTiles[i].GetComponent<TileRendererChanger>().ChangeRendering();
        }
    }

    public void SpawnResources()
    {
        for(int i = 0; i < myResourceNumber; ++i)
        {
            bool occupied = true;

            int tries = 10;
            int currentTries = 0;

            Tile tile = null;

            while (occupied)
            {
                tile = myFloorTiles[Random.Range(0, myFloorTiles.Count)];

                currentTries++;

                occupied = tile.myOccupied;

                if (currentTries >= tries)
                    break;
            }

            if (tile != null)
            {
                ResourceUsable resource = Instantiate(myResourcePrefab, transform);
                resource.transform.localPosition = tile.transform.localPosition;
                myResourceUsables.Add(resource);
                tile.myOccupied = true;
                resource.SetTile(tile);
            }

        }
    }

    public void SpawnDecoration()
    {
        for (int i = 0; i < myDecorationNumber; ++i)
        {
            bool occupied = true;

            int tries = 10;
            int currentTries = 0;

            Tile tile = null;

            while (occupied)
            {
                tile = myFloorTiles[Random.Range(0, myFloorTiles.Count)];

                currentTries++;

                occupied = tile.myOccupied;

                if (currentTries >= tries)
                    break;
            }

            if (tile != null)
            {
                Transform resource = Instantiate(myDecorationPrefab, transform);
                resource.localPosition = tile.transform.localPosition;

                tile.myOccupied = true;
            }
        }
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < myEnemiesNumber; ++i)
        {
            bool occupied = true;

            int tries = 10;
            int currentTries = 0;

            Tile tile = null;

            while (occupied)
            {
                tile = myFloorTiles[Random.Range(0, myFloorTiles.Count)];

                currentTries++;

                occupied = tile.myOccupied;

                if (currentTries >= tries)
                    break;
            }

            if (tile != null)
            {
                Enemy enemy = Instantiate(myEnemyPrefab, transform);
                enemy.transform.localPosition = tile.transform.localPosition;

                myEnemies.Add(enemy);
            }
        }
    }

    public Tile GetMidTile()
    {
        return myMidTile;
    }

    public int GetRoomSize()
    {
        return myRoomSize;
    }

    public TileType GetTileType(int aX, int aY)
    {
        if(aX < 0 || aX > myRoomWidth || aY < 0 || aY > myRoomHeight)
        {
            return TileType.EMPTY;
        }

        return myGrid[aX, aY];
    }

    public Tile GetRealType(int aX, int aY)
    {
        if (aX < 0 || aY < 0 || aX > myRoomWidth - 1 || aY > myRoomHeight - 1)
            return null;

        return myGridBisBis[aX * myRoomWidth + aY];
    }

    public Tile GetFloorTile(int aX, int aY)
    {
        return myFloorTiles[aY * myRoomSize + aX];
    }

    public void OnLeavingRoom()
    {
        myLastTime = Time.time;
    }

    public void OnEnteringRoom()
    {
        float diffTime = Time.time - myLastTime;

        ResourceUsable[] resourceUsables = GetComponentsInChildren<ResourceUsable>();
        for(int i = 0; i < resourceUsables.Length; i++)
        {
            resourceUsables[i].OnEnteringRoom(diffTime);
        }
    }

    public List<ResourceUsable> GetResourceToSave()
    {
        List<ResourceUsable> toSave = new List<ResourceUsable>();

        for(int i = 0; i < myResourceUsables.Count; ++i)
        {
            if(myResourceUsables[i].GetMarkAsToSave())
            {
                toSave.Add(myResourceUsables[i]);
            }
        }

        return toSave;
    }

    public Biome GetBiome()
    {
        return myBiome;
    }
}
