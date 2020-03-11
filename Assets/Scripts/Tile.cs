using UnityEngine;

public enum TileType
{
    FLOOR,
    WALL,
    EMPTY
}

public class Tile : MonoBehaviour
{
    public Room         myParentRoom = null;
    public RoomData     myTileData;
    public TileType     myTileType;
    public bool         myOccupied = false;

    public bool         myHasLeftNeighbour = false;
    public bool         myHasRightNeighbour = false;
    public bool         myHasTopNeighbour = false;
    public bool         myHasDownNeighbour = false;

    public int          myIndex = -1;
}
