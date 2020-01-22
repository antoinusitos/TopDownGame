using UnityEngine;

public enum TileType
{
    FLOOR,
    WALL,
}

public class Tile : MonoBehaviour
{
    public Room         myParentRoom = null;
    public RoomData     myTileData;
    public TileType     myTileType;
}
