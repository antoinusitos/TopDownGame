﻿using UnityEngine;

public class TileRendererChanger : MonoBehaviour
{
    public Sprite[] mySprites = null;
    public Sprite[] myWallSprites = null;

    public void ChangeRendering()
    {
        Tile tile = GetComponent<Tile>();
        if(tile != null)
        {
            switch(tile.myTileType)
            {
                case TileType.FLOOR:
                    {
                        switch(tile.myTileData.myType)
                        {
                            case RoomType.FOREST:
                                GetComponent<SpriteRenderer>().sprite = mySprites[0];
                                break;
                            case RoomType.DESERT:
                                GetComponent<SpriteRenderer>().sprite = mySprites[3];
                                break;
                            case RoomType.MOUNTAIN:
                                GetComponent<SpriteRenderer>().sprite = mySprites[2];
                                break;
                            case RoomType.ICE:
                                GetComponent<SpriteRenderer>().sprite = mySprites[4];
                                break;
                            case RoomType.TRANSITION:
                                GetComponent<SpriteRenderer>().sprite = mySprites[1];
                                break;
                        }
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

                        if(tile.myHasTopNeighbour && tile.myHasDownNeighbour)
                        {
                            /* if (x == 0)
                             {
                                 GetComponent<SpriteRenderer>().sprite = myWallSprites[8];
                             }
                             else
                             {
                                 GetComponent<SpriteRenderer>().sprite = myWallSprites[9];
                             }*/

                            GetComponent<SpriteRenderer>().sprite = myWallSprites[8];
                        }
                        else if (tile.myHasTopNeighbour)
                        {
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[1];
                        }
                        else if (tile.myHasDownNeighbour)
                        {
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[2];
                        }

                        if (tile.myHasRightNeighbour && tile.myHasLeftNeighbour)
                        {
                           /* if(y == 0)
                            {
                                GetComponent<SpriteRenderer>().sprite = myWallSprites[6];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = myWallSprites[7];
                            }*/
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[6];
                        }
                        else if (tile.myHasRightNeighbour)
                        {
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[4];
                        }
                        else if (tile.myHasLeftNeighbour)
                        {
                            GetComponent<SpriteRenderer>().sprite = myWallSprites[5];
                        }

                        break;
                    }
            }
        }
    }
}
