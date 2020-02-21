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
}
