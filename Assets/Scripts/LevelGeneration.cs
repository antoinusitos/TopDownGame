using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    private enum GridSpace { EMPTY, FLOOR, WALL };

    private GridSpace[,]    myGrid;
    private int             myRoomHeight;
    private int             myRoomWidth;
    private Vector2         myRoomSizeWorldUnits = Vector2.one * Data.myRoomSideSize;
    private float           myWorldUnitsInOneGridCell = 1;
    
    private struct Walker
    {
        public Vector2 myDir;
        public Vector2 myPos;
    }

    private List<Walker>    myWalkers;
    private float           myChanceWalkerChangeDir = 0.5f;
    private float           myChangeWalkerSpawn = 0.05f;
    private float           myChanceWalkerDestroy = 0.05f;
    private int             myMaxWalkers = 10;
    private float           myPercentToFill = 0.2f;

    private List<GameObject> mySpawned;

    private int             myWestX = -1;
    private int             myWestY = -1;
    private int             myEastX = -1;
    private int             myEastY = -1;
    private int             myNorthX = -1;
    private int             myNorthY = -1;
    private int             mySouthX = -1;
    private int             mySouthY = -1;

    public GameObject       myWallObj;
    public GameObject       myFloorObj;

    public int              mySeed = 0;

    private void Start()
    {
        Invoke("GetSeedAndGenerate", 1.0f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (mySpawned != null)
            {
                for (int i = 0; i < mySpawned.Count; i++)
                {
                    Destroy(mySpawned[i]);
                }
            }

            Generate();
        }
    }

    private void GetSeedAndGenerate()
    {
        GameInstance gi = FindObjectOfType<GameInstance>();
        if (gi != null)
            mySeed = gi.GetSeed();
        else
            mySeed = 0;
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
        myGrid = new GridSpace[myRoomWidth, myRoomHeight];

        //set grid's default state
        for (int x = 0; x < myRoomWidth; x++)
        {
            for (int y = 0; y < myRoomHeight; y++)
            {
                myGrid[x, y] = GridSpace.EMPTY;
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
        switch(choice)
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
            foreach(Walker aWalker in myWalkers)
            {
                myGrid[(int)aWalker.myPos.x, (int)aWalker.myPos.y] = GridSpace.FLOOR;
            }

            //chance: destroy walker
            int numberChecks = myWalkers.Count;
            for(int i = 0; i < numberChecks; i++)
            {
                //only if it's not the only one and at a low chance
                if(Random.value < myChanceWalkerDestroy && myWalkers.Count > 1)
                {
                    myWalkers.RemoveAt(i);
                    break;
                }
            }

            //chance: walker pick new direction
            for(int i = 0; i <myWalkers.Count; i++)
            {
                if(Random.value < myChanceWalkerChangeDir)
                {
                    Walker aWalker = myWalkers[i];
                    aWalker.myDir = RandomDirection();
                    myWalkers[i] = aWalker;
                }
            }

            //chance: spawn new walker
            numberChecks = myWalkers.Count;
            for(int i = 0; i < numberChecks; i++)
            {
                //only if # of walkers < max and at a low chance
                if(Random.value < myChangeWalkerSpawn && myWalkers.Count < myMaxWalkers)
                {
                    Walker newWalker = new Walker();
                    newWalker.myDir = RandomDirection();
                    newWalker.myPos = myWalkers[i].myPos;
                    myWalkers.Add(newWalker);
                }
            }

            //move walkers
            for(int i = 0; i < myWalkers.Count; i++)
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
            if(((float)NumberOfFloors() / (float)myGrid.Length) > myPercentToFill)
            {
                break;
            }

            iterations++;

        } while (iterations < 100000);
    }

    private int NumberOfFloors()
    {
        int count = 0; 
        foreach(GridSpace space in myGrid)
        {
            if(space == GridSpace.FLOOR)
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
               switch(myGrid[x, y])
                {
                    case GridSpace.EMPTY:
                        break;
                    case GridSpace.FLOOR:
                        Spawn(x, y, myFloorObj);
                        break;
                    case GridSpace.WALL:
                        Spawn(x, y, myWallObj);
                        break;
                }
            }
        }
    }

    private void Spawn(float aX, float aY, GameObject aToSpawn)
    {
        //find the position to spawn
        Vector2 offset = myRoomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(aX, aY) * myWorldUnitsInOneGridCell - offset;

        //spawn object
        GameObject spawned = Instantiate(aToSpawn, spawnPos, Quaternion.identity);
        spawned.name = spawned.name + (aX * myRoomWidth + aY).ToString();
        TempTile tile = spawned.AddComponent<TempTile>();
        tile.myX = (int)aX;
        tile.myY = (int)aY;
        mySpawned.Add(spawned);
    }

    private void CreateWalls()
    {
        //loop through every grid space
        for (int x = 0; x < myRoomWidth - 1; x++)
        {
            for (int y = 0; y < myRoomHeight - 1; y++)
            {
                //if there is a floor, check the space around it
                if(myGrid[x, y] == GridSpace.FLOOR)
                {
                    //if any surrounding spaces are empty, place a wall
                    if(myGrid[x, y + 1] == GridSpace.EMPTY)
                    {
                        myGrid[x, y + 1] = GridSpace.WALL;
                    }
                    if (myGrid[x, y - 1] == GridSpace.EMPTY)
                    {
                        myGrid[x, y - 1] = GridSpace.WALL;
                    }
                    if (myGrid[x + 1, y] == GridSpace.EMPTY)
                    {
                        myGrid[x + 1, y] = GridSpace.WALL;
                    }
                    if (myGrid[x - 1, y] == GridSpace.EMPTY)
                    {
                        myGrid[x - 1, y] = GridSpace.WALL;
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
                if (myGrid[x, y] == GridSpace.WALL)
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
                            if (myGrid[x + checkX, y + checkY] != GridSpace.FLOOR)
                            {
                                allFloor = false;
                            }
                        }
                    }
                    if(allFloor)
                    {
                        myGrid[x, y] = GridSpace.FLOOR;
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
                if(myGrid[x, y] == GridSpace.FLOOR)
                {
                    if(x <= myWestX)
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

        for(int i = 0; i < mySpawned.Count; i++)
        {
            TempTile tile = mySpawned[i].GetComponent<TempTile>();
            if (tile.myX == myWestX && tile.myY == myWestY)
            {
                mySpawned[i].GetComponentInChildren<SpriteRenderer>().color = Color.green;
                continue;
            }
            else if (tile.myX == myEastX && tile.myY == myEastY)
            {
                mySpawned[i].GetComponentInChildren<SpriteRenderer>().color = Color.green;
                continue;
            }
            else if (tile.myX == myNorthX && tile.myY == myNorthY)
            {
                mySpawned[i].GetComponentInChildren<SpriteRenderer>().color = Color.green;
                continue;
            }
            else if (tile.myX == mySouthX && tile.myY == mySouthY)
            {
                mySpawned[i].GetComponentInChildren<SpriteRenderer>().color = Color.green;
                continue;
            }
        }

    }
}
