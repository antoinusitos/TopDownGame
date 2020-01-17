using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData     myRoomData;
    public Tile         myTileFloorPrefab = null;
    public Tile         myTileWallPrefab = null;
    public bool         myStartingRoom = false;

    public Material[]   myMaterials = null;

    private Transform   myTransform = null;

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

                /*
                    GRASS = 1,
                    CITY,
                    DIRT,
                    DESERT,
                    MOUNTAIN
                */

                tile.GetComponent<Renderer>().material = myMaterials[myRoomData.myType - 1];
            }
        }
    }

    public void PlaceWalls(int aRoomSize)
    {
        for (int y = 0; y < aRoomSize; ++y)
        {
            for (int x = 0; x < aRoomSize; ++x)
            {
                bool construct = false;

                if(x == 0)
                {
                    if (y == aRoomSize / 2 && myRoomData.myHasLeftNeighbour)
                    {
                        construct = false;
                    }
                    else
                    {
                        construct = true;
                    }
                }
                else if (x == aRoomSize - 1)
                {
                    if (y == aRoomSize / 2 && myRoomData.myHasRightNeighbour)
                    {
                        construct = false;
                    }
                    else
                    {
                        construct = true;
                    }
                }
                else if (y == 0)
                {
                    if (x == aRoomSize / 2 && myRoomData.myHasTopNeighbour)
                    {
                        construct = false;
                    }
                    else
                    {
                        construct = true;
                    }
                }
                else if (y == aRoomSize - 1)
                {
                    if (x == aRoomSize / 2 && myRoomData.myHasBottomNeighbour)
                    {
                        construct = false;
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
            }
        }
    }
}
