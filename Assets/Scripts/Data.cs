using UnityEngine;

[System.Serializable]
public struct RoomData
{
    public int myX;
    public int myY;
    public int myType;

    public bool myHasTopNeighbour;
    public bool myHasRightNeighbour;
    public bool myHasBottomNeighbour;
    public bool myHasLeftNeighbour;


    public RoomData(int aX, int aY, int aType)
    {
        myX = aX;
        myY = aY;
        myType = aType;
        myHasTopNeighbour = false;
        myHasRightNeighbour = false;
        myHasBottomNeighbour = false;
        myHasLeftNeighbour = false;
    }
}

public enum RoomType
{
    GRASS = 1,
    CITY,
    ROCK,
    DESERT,
    MOUNTAIN
}

[System.Serializable]
public struct TileData
{
    public int myX;
    public int myY;
    public int myType;

    public TileData(int aX, int aY, int aType)
    {
        myX = aX;
        myY = aY;
        myType = aType;
    }
}

[System.Serializable]
public struct Item
{
    public int myID;
    public int myQuantity;

    public Item(int anID)
    {
        myID = anID;
        myQuantity = 0;
    }
}

public class Data : MonoBehaviour
{

}
