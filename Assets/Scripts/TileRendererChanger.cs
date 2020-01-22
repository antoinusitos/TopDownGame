using UnityEngine;

public class TileRendererChanger : MonoBehaviour
{
    /*GRASS = 1,
    CITY,
    ROCK,
    DESERT,
    MOUNTAIN*/

    public Sprite[] mySprites = null;
    public Sprite[] myWallSprites = null;

    public bool top = false;
    public bool bottom = false;
    public bool left = false;
    public bool right = false;

    public void ChangeRendering()
    {
        Tile tile = GetComponent<Tile>();
        if(tile != null)
        {
            switch(tile.myTileType)
            {
                case TileType.FLOOR:
                    {
                        GetComponent<SpriteRenderer>().sprite = mySprites[tile.myTileData.myType - 1];
                        break;
                    }
                case TileType.WALL:
                    {
                        int x = tile.myTileData.myX;
                        int y = tile.myTileData.myY;

                        Room parentRoom = tile.myParentRoom;
                        int roomSize = parentRoom.GetRoomSize();

                        if (x == 0 && (y == 0 || y == roomSize - 1))
                        {
                            // top corner
                            break;
                        }
                        else if (x == roomSize - 1 && (y == 0 || y == roomSize - 1))
                        {
                            // bottom corner
                            break;
                        }

                        top = false;
                        bottom = false;
                        left = false;
                        right = false;

                        if (y > 0 && parentRoom.GetTile(x, y - 1).myTileType == TileType.WALL)
                        {
                            bottom = true;
                        }
                        if (y < roomSize - 1 && parentRoom.GetTile(x, y + 1).myTileType == TileType.WALL)
                        {
                            top = true;
                        }
                        if (x < roomSize - 1 && parentRoom.GetTile(x + 1, y).myTileType == TileType.WALL)
                        {
                            right = true;
                        }
                        if (x > 0 && parentRoom.GetTile(x - 1, y).myTileType == TileType.WALL)
                        {
                            left = true;
                        }

                        if(top && bottom)
                        {
                            if (x == 0)
                            {
                                GetComponent<SpriteRenderer>().sprite = myWallSprites[8];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = myWallSprites[9];
                            }
                            //GetComponent<SpriteRenderer>().sprite = myWallSprites[0];
                        }
                        else if (top)
                        {
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[1];
                        }
                        else if (bottom)
                        {
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[2];
                        }
                        if (right && left)
                        {
                            if(y == 0)
                            {
                                GetComponent<SpriteRenderer>().sprite = myWallSprites[6];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = myWallSprites[7];
                            }
                            //GetComponent<SpriteRenderer>().sprite = myWallSprites[3];
                        }
                        else if (right)
                        {
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[4];
                        }
                        else if (left)
                        {
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[5];
                        }

                        break;
                    }
            }
        }
    }
}
