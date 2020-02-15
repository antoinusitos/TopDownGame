using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData                 myRoomData;
    public Tile                     myTileFloorPrefab = null;
    public Tile                     myTileWallPrefab = null;
    public TriggerNextRoom          myTriggerNextRoomPrefab = null;
    public bool                     myStartingRoom = false;

    public Tile                     myBottomSpawningTile = null;
    public Tile                     myTopSpawningTile = null;
    public Tile                     myLeftSpawningTile = null;
    public Tile                     myRightSpawningTile = null;

    public Enemy                    myEnemyPrefab = null;

    public ResourceUsable           myResourcePrefab = null;
    public Transform                myDecorationPrefab = null;
    private List<ResourceUsable>    myResourceUsables = new List<ResourceUsable>();

    private List<Enemy>             myEnemies = new List<Enemy>();

    private Transform               myTransform = null;
    private Tile                    myMidTile = null;
    private List<Tile>              myTiles = new List<Tile>();

    private List<TriggerNextRoom>   myTriggerNextRooms = new List<TriggerNextRoom>();

    private const float             mySpriteSpace = 1;//2.5f;

    private Biome                   myBiome = null;

    private int                     myRoomSize = 0;

    private float                   myLastTime = 0;

    public void ConstuctRoom(int aRoomSize, Biome aBiome)
    {
        myTransform = transform;
        myBiome = aBiome;
        myRoomSize = aBiome.GetRoomSize();

        SelectTileType(aRoomSize);

        for(int i = 0; i < 3; ++i)
        {
            int x = Random.Range(1, myRoomSize - 1);
            int y = Random.Range(1, myRoomSize - 1);
            Enemy e = Instantiate(myEnemyPrefab, transform);
            e.transform.localPosition = new Vector3(x * mySpriteSpace, y * mySpriteSpace, 0);
            myEnemies.Add(e);
        }
    }

    private void SelectTileType(int aRoomSize)
    {
        for (int y = 0; y < aRoomSize; ++y)
        {
            for (int x = 0; x < aRoomSize; ++x)
            {
                int tileType = -1; //0 = floor, 1 = wall
                int transitionType = -1;
                bool haveTransition = false;

                if (x == 0)
                {
                    if (myRoomData.myHasLeftNeighbour)
                    {
                        if (y == aRoomSize / 2 || y == aRoomSize / 2 + 1 || y == aRoomSize / 2 - 1)
                        {
                            tileType = 0;
                            //tileType = 2;
                            transitionType = 3;
                            if (y == aRoomSize / 2)
                                haveTransition = true;
                        }
                        else
                        {
                            tileType = 1;
                        }
                    }
                    else
                    {
                        tileType = 1;
                    }
                }
                else if (x == aRoomSize - 1)
                {
                    if (myRoomData.myHasRightNeighbour)
                    {
                        if (y == aRoomSize / 2 || y == aRoomSize / 2 - 1 || y == aRoomSize / 2 + 1)
                        {
                            tileType = 0;
                            //tileType = 2;
                            transitionType = 1;
                            if (y == aRoomSize / 2)
                                haveTransition = true;
                        }
                        else
                        {
                            tileType = 1;
                        }
                    }
                    else
                    {
                        tileType = 1;
                    }
                }
                else if (y == 0)
                {
                    if (myRoomData.myHasTopNeighbour)
                    {
                        if (x == aRoomSize / 2 || x == aRoomSize / 2 - 1 || x == aRoomSize / 2 + 1)
                        {
                            tileType = 0;
                            //tileType = 2;
                            transitionType = 2;
                            if (x == aRoomSize / 2)
                                haveTransition = true;
                        }
                        else
                        {
                            tileType = 1;
                        }
                    }
                    else
                    {
                        tileType = 1;
                    }
                }
                else if (y == aRoomSize - 1)
                {
                    if (myRoomData.myHasBottomNeighbour)
                    {
                        if (x == aRoomSize / 2 || x == aRoomSize / 2 - 1 || x == aRoomSize / 2 + 1)
                        {
                            tileType = 0;
                            //tileType = 2;
                            transitionType = 0;
                            if (x == aRoomSize / 2)
                                haveTransition = true;
                        }
                        else
                        {
                            tileType = 1;
                        }
                    }
                    else
                    {
                        tileType = 1;
                    }
                }
                else
                {
                    tileType = 0;
                }

                Tile tileSpawned = null;
                if (tileType == 0)
                {
                    tileSpawned = Instantiate(myTileFloorPrefab, myTransform);
                    tileSpawned.myTileType = TileType.FLOOR;
                    tileSpawned.myTileData.myType = myRoomData.myType;
                }
                else if (tileType == 1)
                {
                    tileSpawned = Instantiate(myTileWallPrefab, myTransform);
                    tileSpawned.myTileType = TileType.WALL;
                    tileSpawned.myTileData.myType = myRoomData.myType;
                }

                if (tileSpawned != null)
                {
                    Transform tileTransform = tileSpawned.transform;
                    tileTransform.parent = myTransform;
                    tileTransform.localPosition = new Vector3(x * mySpriteSpace, y * mySpriteSpace, 0);
                    tileSpawned.myTileData.myX = x;
                    tileSpawned.myTileData.myY = y;
                    tileSpawned.myParentRoom = this;

                    if (x == aRoomSize / 2 && y == aRoomSize / 2)
                    {
                        myMidTile = tileSpawned;
                    }

                    myTiles.Add(tileSpawned);
                }

                if (haveTransition && tileSpawned != null)
                {
                    TriggerNextRoom triggerNextRoom = Instantiate(myTriggerNextRoomPrefab, myTransform);
                    triggerNextRoom.SetActualRoom(this);
                    triggerNextRoom.myTile = tileSpawned;
                    triggerNextRoom.transform.localPosition = tileSpawned.transform.localPosition;

                    triggerNextRoom.myTriggerPlace = TriggerPlace.CENTER;

                    triggerNextRoom.myID = Data.transitionTriggerIndex;
                    Data.transitionTriggerIndex++;

                    triggerNextRoom.myTransitionType = transitionType;

                    if (transitionType == 1 || transitionType == 3)
                    {
                        triggerNextRoom.transform.localScale = new Vector3(1, 3, 1);
                    }

                    myTriggerNextRooms.Add(triggerNextRoom);
                }
            }
        }
    }

    public void AffectTransitions()
    {
        for(int i = 0; i < myTriggerNextRooms.Count; i++)
        {
            int neighbourX = myRoomData.myX;
            int neighbourY = myRoomData.myY;

            if(myTriggerNextRooms[i].myTransitionType == 0)
            {
                neighbourY += 1;
                if(myTriggerNextRooms[i].myTriggerPlace == TriggerPlace.CENTER)
                {
                    Tile tile = myTriggerNextRooms[i].myTile;
                    myTopSpawningTile = myTiles[(tile.myTileData.myY - 1) * myBiome.GetRoomSize() + tile.myTileData.myX];
                }
            }
            else if (myTriggerNextRooms[i].myTransitionType == 1)
            {
                neighbourX += 1;
                if (myTriggerNextRooms[i].myTriggerPlace == TriggerPlace.CENTER)
                {
                    Tile tile = myTriggerNextRooms[i].myTile;
                    myRightSpawningTile = myTiles[tile.myTileData.myY * myBiome.GetRoomSize() + tile.myTileData.myX - 1];
                }
            }
            else if (myTriggerNextRooms[i].myTransitionType == 2)
            {
                neighbourY -= 1;
                if (myTriggerNextRooms[i].myTriggerPlace == TriggerPlace.CENTER)
                {
                    Tile tile = myTriggerNextRooms[i].myTile;
                    myBottomSpawningTile = myTiles[(tile.myTileData.myY + 1) * myBiome.GetRoomSize() + tile.myTileData.myX];
                }
            }
            else if (myTriggerNextRooms[i].myTransitionType == 3)
            {
                neighbourX -= 1;
                if (myTriggerNextRooms[i].myTriggerPlace == TriggerPlace.CENTER)
                {
                    Tile tile = myTriggerNextRooms[i].myTile;
                    myLeftSpawningTile = myTiles[tile.myTileData.myY * myBiome.GetRoomSize() + tile.myTileData.myX + 1];
                }
            }

            Room room = myBiome.GetRoom(neighbourX, neighbourY);
            myTriggerNextRooms[i].SetNextRoom(room);
        }
    }

    public void AffectTransitionsTo(Room aRoom)
    {

    }

    public void ChangeTileRendering()
    {
        for(int i = 0; i < myTiles.Count; i++)
        {
            myTiles[i].GetComponent<TileRendererChanger>().ChangeRendering();
        }
    }

    public void SpawnResources()
    {
        for(int i = 0; i < 3; ++i)
        {
            bool occupied = true;

            int x = 0;
            int y = 0;

            while (occupied)
            {
                x = Random.Range(2, myRoomSize - 1);
                y = Random.Range(2, myRoomSize - 1);

                occupied = myTiles[y * myRoomSize + x].myOccupied;
            }

            ResourceUsable resource = Instantiate(myResourcePrefab, transform);
            resource.transform.localPosition = new Vector3(x, y, 0);
            myResourceUsables.Add(resource);

            myTiles[y * myRoomSize + x].myOccupied = true;
        }
    }

    public void SpawnDecoration()
    {
        for (int i = 0; i < 15; ++i)
        {
            bool occupied = true;

            int x = 0;
            int y = 0;

            while (occupied)
            {
                x = Random.Range(1, myRoomSize);
                y = Random.Range(2, myRoomSize - 1);

                occupied = myTiles[y * myRoomSize + x].myOccupied;
            }

            Transform resource = Instantiate(myDecorationPrefab, transform);
            resource.localPosition = new Vector3(x, y, 0);

            myTiles[y * myRoomSize + x].myOccupied = true;
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

    public Tile GetTile(int aX, int aY)
    {
        return myTiles[aY * myRoomSize + aX];
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
