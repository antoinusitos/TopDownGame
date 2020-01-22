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

    private Transform               myTransform = null;
    private Tile                    myMidTile = null;
    private List<Tile>              myTiles = new List<Tile>();

    private List<TriggerNextRoom>   myTriggerNextRooms = new List<TriggerNextRoom>();

    private const float             mySpriteSpace = 1;//2.5f;

    private WorldGeneration         myWorldGeneration = null;

    private int                     myRoomSize = 0;

    public void ConstuctRoom(int aRoomSize, WorldGeneration aWorldGeneration)
    {
        myTransform = transform;
        myWorldGeneration = aWorldGeneration;
        myRoomSize = myWorldGeneration.GetRoomSize();

        SelectTileType(aRoomSize);
    }

    private void SelectTileType(int aRoomSize)
    {
        for (int y = 0; y < aRoomSize; ++y)
        {
            for (int x = 0; x < aRoomSize; ++x)
            {
                int tileType = -1; //0 = floor, 1 = wall, 2 = transition
                int transitionType = 0;

                TriggerPlace triggerPlace = TriggerPlace.CENTER;

                if (x == 0)
                {
                    if (myRoomData.myHasLeftNeighbour)
                    {
                        if (y == aRoomSize / 2 || y == aRoomSize / 2 + 1 || y == aRoomSize / 2 - 1)
                        {
                            tileType = 2;
                            transitionType = 3;
                            if (y == aRoomSize / 2 + 1)
                                triggerPlace = TriggerPlace.RIGHT;
                            else if (y == aRoomSize / 2 - 1)
                                triggerPlace = TriggerPlace.LEFT;
                            else
                                triggerPlace = TriggerPlace.CENTER;

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
                            tileType = 2;
                            transitionType = 1;
                            if (y == aRoomSize / 2 + 1)
                                triggerPlace = TriggerPlace.RIGHT;
                            else if (y == aRoomSize / 2 - 1)
                                triggerPlace = TriggerPlace.LEFT;
                            else
                                triggerPlace = TriggerPlace.CENTER;

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
                            tileType = 2;
                            transitionType = 2;
                            if (x == aRoomSize / 2 + 1)
                                triggerPlace = TriggerPlace.RIGHT;
                            else if (x == aRoomSize / 2 - 1)
                                triggerPlace = TriggerPlace.LEFT;
                            else
                                triggerPlace = TriggerPlace.CENTER;

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
                            tileType = 2;
                            transitionType = 0;
                            if (x == aRoomSize / 2 + 1)
                                triggerPlace = TriggerPlace.RIGHT;
                            else if (x == aRoomSize / 2 - 1)
                                triggerPlace = TriggerPlace.LEFT;
                            else
                                triggerPlace = TriggerPlace.CENTER;

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
                else if (tileType == 2)
                {
                    TriggerNextRoom triggerNextRoom = Instantiate(myTriggerNextRoomPrefab, myTransform);
                    tileSpawned = triggerNextRoom.GetComponent<Tile>();
                    triggerNextRoom.SetActualRoom(this);

                    tileSpawned.myTileType = TileType.FLOOR;
                    tileSpawned.myTileData.myType = myRoomData.myType;

                    triggerNextRoom.myTransitionType = transitionType;
                    triggerNextRoom.myTriggerPlace = triggerPlace;

                    myTriggerNextRooms.Add(triggerNextRoom);
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
                    Tile tile = myTriggerNextRooms[i].GetComponent<Tile>();
                    myTopSpawningTile = myTiles[(tile.myTileData.myY - 1) * myWorldGeneration.GetRoomSize() + tile.myTileData.myX];
                }
            }
            else if (myTriggerNextRooms[i].myTransitionType == 1)
            {
                neighbourX += 1;
                if (myTriggerNextRooms[i].myTriggerPlace == TriggerPlace.CENTER)
                {
                    Tile tile = myTriggerNextRooms[i].GetComponent<Tile>();
                    myRightSpawningTile = myTiles[tile.myTileData.myY * myWorldGeneration.GetRoomSize() + tile.myTileData.myX - 1];
                }
            }
            else if (myTriggerNextRooms[i].myTransitionType == 2)
            {
                neighbourY -= 1;
                if (myTriggerNextRooms[i].myTriggerPlace == TriggerPlace.CENTER)
                {
                    Tile tile = myTriggerNextRooms[i].GetComponent<Tile>();
                    myBottomSpawningTile = myTiles[(tile.myTileData.myY + 1) * myWorldGeneration.GetRoomSize() + tile.myTileData.myX];
                }
            }
            else if (myTriggerNextRooms[i].myTransitionType == 3)
            {
                neighbourX -= 1;
                if (myTriggerNextRooms[i].myTriggerPlace == TriggerPlace.CENTER)
                {
                    Tile tile = myTriggerNextRooms[i].GetComponent<Tile>();
                    myLeftSpawningTile = myTiles[tile.myTileData.myY * myWorldGeneration.GetRoomSize() + tile.myTileData.myX + 1];
                }
            }

            Room room = myWorldGeneration.GetRoom(neighbourX, neighbourY);
            Debug.Log("for room :(" + myRoomData.myX + "," + myRoomData.myY + ")");
            Debug.Log("trying to get :(" + neighbourX + "," + neighbourY + ")");
            myTriggerNextRooms[i].SetNextRoom(room);
        }
    }

    public void ChangeTileRendering()
    {
        for(int i = 0; i < myTiles.Count; i++)
        {
            myTiles[i].GetComponent<TileRendererChanger>().ChangeRendering();
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
        Debug.Log("want:" + aX + "," + aY);
        return myTiles[aY * myRoomSize + aX];
    }
}
