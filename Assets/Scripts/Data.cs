using System.Collections.Generic;
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
    public int      myID;
    public int      myQuantity;
    public Sprite   mySprite;

    public Item(int anID)
    {
        myID = anID;
        myQuantity = 0;
        mySprite = null;
    }
}

public enum TransitionDirection
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}

public class Data : MonoBehaviour
{
    public static string[] myBiomesNames =
    {
        "Groenavatn",
        "Mork",
        "Mjola",
        "Hafnarlond",
        "Garpsdalr",
        "Kopanes",
        "Miohus",
        "Haugr",
        "Esjuberg",
        "Eiriksvagr",
    };

    public static int transitionTriggerIndex = 0;

    private static int myBiomeNameIndex = 0;

    public static void ShuffleBiomesNames()
    {
        for(int i = 0; i < myBiomesNames.Length; i++)
        {
            int rand = Random.Range(0, myBiomesNames.Length);
            string temp = myBiomesNames[rand];
            myBiomesNames[rand] = myBiomesNames[i];
            myBiomesNames[i] = temp;
        }
    }

    public static string GetBiomeName()
    {
        string toReturn = myBiomesNames[myBiomeNameIndex];
        myBiomeNameIndex++;
        return toReturn;
    }
}
