using UnityEngine;

public class TileRendererChanger : MonoBehaviour
{
    /*GRASS = 1,
    CITY,
    ROCK,
    DESERT,
    MOUNTAIN*/

    public Sprite[] mySprites = null;

    public void ChangeRendering()
    {
        Tile tile = GetComponent<Tile>();
        if(tile != null)
        {
            switch(tile.myTileType)
            {
                case TileType.FLOOR:
                    {
                        Debug.Log("lol" + tile.myTileData.myType);
                        GetComponent<SpriteRenderer>().sprite = mySprites[tile.myTileData.myType - 1];
                        break;
                    }
                case TileType.WALL:
                    {
                        break;
                    }
            }
        }
    }
}
