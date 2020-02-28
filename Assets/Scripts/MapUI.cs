using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public Image            myRoomImagePrefab = null;

    private GridLayoutGroup myGridLayoutGroup = null;
    private Image[]         myRoomImages = null;
    private WorldGeneration myWorldGeneration = null;

    private Image           myCurrentRoom = null;
    private float           myVisibility = 1;
    private const float     myBlinkSpeed = 2.0f;

    private List<Room>      myVisitedRooms = new List<Room>();

    public void Init()
    {
        myGridLayoutGroup = GetComponent<GridLayoutGroup>();

        myGridLayoutGroup.cellSize = Vector2.one * (myGridLayoutGroup.GetComponent<RectTransform>().rect.width / Data.myBiomeSideSize);

        if (myWorldGeneration == null)
            myWorldGeneration = FindObjectOfType<WorldGeneration>();

        UpdateBiomeOnMap();
    }

    public void UpdateBiomeOnMap()
    {
        if(myRoomImages != null)
        {
            for(int i = 0; i < myRoomImages.Length; ++i)
            {
                Destroy(myRoomImages[i].gameObject);
            }
        }

        int biomeSideSize = Data.myBiomeSideSize;

        myRoomImages = new Image[biomeSideSize * biomeSideSize];

        Color transparent = Color.white;
        transparent.a = 0;

        for (int y = 0; y < biomeSideSize; ++y)
        {
            for (int x = 0; x < biomeSideSize; ++x)
            {
                Image img = Instantiate(myRoomImagePrefab, myGridLayoutGroup.transform);
                myRoomImages[y * biomeSideSize + x] = img;

                if (myWorldGeneration.GetCurrentActiveBiome().GetRoom(x, y) != null)
                {
                    Color col = Color.grey;
                    col.a = 0.25f;
                    img.color = col;
                }
                else
                {
                    img.color = transparent;
                }
            }
        }

        for(int i = 0; i < myVisitedRooms.Count; i++)
        {
            if(myVisitedRooms[i].GetBiome() == myWorldGeneration.GetCurrentActiveBiome())
            {
                SetRoomVisited(myVisitedRooms[i]);
            }
        }

    }

    private void Update()
    {
        myVisibility = (Mathf.Cos(Time.time * myBlinkSpeed) + 1) / 4 + 0.5f;
        if(myCurrentRoom != null)
        {
            Color col = myCurrentRoom.color;
            col.a = myVisibility;
            myCurrentRoom.color = col; 
        }
    }

    public void SetRoomVisited(Room aRoom)
    {
        int x = aRoom.myRoomData.myX;
        int y = aRoom.myRoomData.myY;

        if (aRoom.myRoomData.myType == RoomType.FOREST)
        {
            myRoomImages[y * myWorldGeneration.GetWorldSideSize() + x].color = Color.green;
        }
        else if (aRoom.myRoomData.myType == RoomType.MOUNTAIN)
        {
            myRoomImages[y * myWorldGeneration.GetWorldSideSize() + x].color = new Color(0.62f, 0.32f, 0.17f);
        }
        else if (aRoom.myRoomData.myType == RoomType.DESERT)
        {
            myRoomImages[y * myWorldGeneration.GetWorldSideSize() + x].color = Color.yellow;
        }
        else if (aRoom.myRoomData.myType == RoomType.ICE)
        {
            myRoomImages[y * myWorldGeneration.GetWorldSideSize() + x].color = Color.white;
        }
        else if (aRoom.myRoomData.myType == RoomType.TRANSITION)
        {
            myRoomImages[y * myWorldGeneration.GetWorldSideSize() + x].color = Color.gray;
        }
    }

    public void SetRoomVisited(int aX, int aY, Biome aBiome = null)
    {
        Room room = null;
        if (aBiome == null)
            room = myWorldGeneration.GetCurrentActiveBiome().GetRoom(aX, aY);
        else
            room = aBiome.GetRoom(aX, aY);
        myVisitedRooms.Add(room);

        if (room.myRoomData.myType == RoomType.FOREST)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = Color.green;
        }
        else if (room.myRoomData.myType == RoomType.MOUNTAIN)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = new Color(0.62f, 0.32f, 0.17f);
        }
        else if (room.myRoomData.myType == RoomType.DESERT)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = Color.yellow;
        }
        else if (room.myRoomData.myType == RoomType.ICE)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = Color.white;
        }
        else if (room.myRoomData.myType == RoomType.TRANSITION)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = Color.gray;
        }

        if (myCurrentRoom != null)
        {
            Color col = myCurrentRoom.color;
            col.a = 1;
            myCurrentRoom.color = col;
        }
        myCurrentRoom = myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX];
        myVisibility = 1;
    }

    public List<Room> GetVisitedRooms()
    {
        return myVisitedRooms;
    }
}
