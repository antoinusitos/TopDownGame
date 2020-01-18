using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData         myRoomData;
    public Tile             myTileFloorPrefab = null;
    public Tile             myTileWallPrefab = null;
    public TriggerNextRoom  myTriggerNextRoomPrefab = null;
    public bool             myStartingRoom = false;

    public Material[]       myMaterials = null;

    public Tile             myBottomSpawningTile = null;
    public Tile             myTopSpawningTile = null;
    public Tile             myLeftSpawningTile = null;
    public Tile             myRightSpawningTile = null;

    private Transform       myTransform = null;
    private Tile            myMidTile = null;
    private List<Tile>      myTiles = new List<Tile>();

    public void ConstuctRoom(int aRoomSize)
    {
        myTransform = transform;

        for (int y = 0; y < aRoomSize; ++y)
        {
            for (int x = 0; x < aRoomSize; ++x)
            {
                Tile tile = Instantiate(myTileFloorPrefab, myTransform);
                Transform tileTransform = tile.transform;
                tileTransform.parent = myTransform;
                tileTransform.localPosition = new Vector3(x, 0, y);
                tile.myTileData.myX = x;
                tile.myTileData.myY = y;
                tile.myParentRoom = this;

                if(x == aRoomSize / 2 && y == aRoomSize / 2)
                {
                    myMidTile = tile;
                }

                /*
                    GRASS = 1,
                    CITY,
                    DIRT,
                    DESERT,
                    MOUNTAIN
                */

                tile.GetComponent<Renderer>().material = myMaterials[myRoomData.myType - 1];
                myTiles.Add(tile);
            }
        }
    }

    public void PlaceWalls(int aRoomSize, WorldGeneration aWorldGeneration)
    {
        for (int y = 0; y < aRoomSize; ++y)
        {
            for (int x = 0; x < aRoomSize; ++x)
            {
                bool construct = false;
                bool transition = false;

                int neighbourX = myRoomData.myX;
                int neighbourY = myRoomData.myY;
                int transitionType = 0;

                if (x == 0)
                {
                    if (myRoomData.myHasLeftNeighbour)
                    {
                        if (y == aRoomSize / 2 || y == aRoomSize / 2 + 1 || y == aRoomSize / 2 - 1)
                        {
                            construct = false;
                            neighbourX -= 1;
                            transition = true;
                            transitionType = 3;
                            if (y == aRoomSize / 2)
                            {
                                myLeftSpawningTile = myTiles[y * aRoomSize + (x+1)];
                            }
                        }
                        else
                        {
                            construct = true;
                        }
                    }
                    else
                    {
                        construct = true;
                    }
                }
                else if (x == aRoomSize - 1)
                {
                    if (myRoomData.myHasRightNeighbour)
                    {
                        if (y == aRoomSize / 2 || y == aRoomSize / 2 - 1 || y == aRoomSize / 2 + 1)
                        {
                            construct = false;
                            neighbourX += 1;
                            transition = true;
                            transitionType = 1;
                            if (y == aRoomSize / 2)
                            {
                                myRightSpawningTile = myTiles[y * aRoomSize + (x-1)];
                            }
                        }
                        else
                        {
                            construct = true;
                        }
                    }
                    else
                    {
                        construct = true;
                    }
                }
                else if (y == 0)
                {
                    if(myRoomData.myHasTopNeighbour)
                    {
                        if (x == aRoomSize / 2 || x == aRoomSize / 2 - 1|| x == aRoomSize / 2 + 1)
                        {
                            construct = false;
                            neighbourY -= 1;
                            transition = true;
                            transitionType = 2;
                            if (x == aRoomSize / 2)
                            {
                                myBottomSpawningTile = myTiles[(y+1) * aRoomSize + x];
                            }
                        }
                        else
                        {
                            construct = true;
                        }
                    }
                    else
                    {
                        construct = true;
                    }
                }
                else if (y == aRoomSize - 1)
                {
                    if(myRoomData.myHasBottomNeighbour)
                    {
                        if (x == aRoomSize / 2 || x == aRoomSize / 2 - 1 || x == aRoomSize / 2 + 1)
                        {
                            construct = false;
                            neighbourY += 1;
                            transition = true;
                            transitionType = 0;
                            if (x == aRoomSize / 2)
                            {
                                myTopSpawningTile = myTiles[(y-1) * aRoomSize + x];
                            }
                        }
                        else
                        {
                            construct = true;
                        }
                    }
                    else
                    {
                        construct = true;
                    }
                }

                if (construct)
                {
                    Tile tileUp = Instantiate(myTileWallPrefab, myTransform);
                    Transform tileUpTransform = tileUp.transform;
                    tileUpTransform.parent = myTransform;
                    tileUpTransform.localPosition = new Vector3(x, 0.5f, y);
                    tileUp.myTileData.myX = x;
                    tileUp.myTileData.myY = y;
                    tileUp.myParentRoom = this;

                    tileUp.GetComponent<Renderer>().material = myMaterials[myRoomData.myType - 1];
                }
                else if(transition)
                {
                    TriggerNextRoom triggerNextRoom = Instantiate(myTriggerNextRoomPrefab, myTransform);
                    Transform tileUpTransform = triggerNextRoom.transform;
                    tileUpTransform.parent = myTransform;
                    tileUpTransform.localPosition = new Vector3(x, 0.5f, y);

                    triggerNextRoom.SetActualRoom(this);

                    Room room = aWorldGeneration.GetRoom(neighbourX, neighbourY);
                    triggerNextRoom.SetNextRoom(room);

                    triggerNextRoom.myTransitionType = transitionType;
                }
            }
        }
    }

    public Tile GetMidTile()
    {
        return myMidTile;
    }
}
