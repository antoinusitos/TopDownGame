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

    private void Start()
    {
        /*myGridLayoutGroup = GetComponent<GridLayoutGroup>();

        if(myWorldGeneration == null)
            myWorldGeneration = FindObjectOfType<WorldGeneration>();
        int worldSideSize = myWorldGeneration.GetWorldSideSize();

        myRoomImages = new Image[worldSideSize * worldSideSize];

        Color transparent = Color.white;
        transparent.a = 0; 

        for(int y = 0; y < worldSideSize; ++y)
        {
            for (int x = 0; x < worldSideSize; ++x)
            {
                Image img = Instantiate(myRoomImagePrefab, myGridLayoutGroup.transform);
                myRoomImages[y * worldSideSize + x] = img;

                if (myWorldGeneration.GetRoom(x, y) != null)
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
        }*/
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

    public void SetRoomVisited(int aX, int aY)
    {
        /*Room room = myWorldGeneration.GetRoom(aX, aY);
        if(room.myRoomData.myType == 1)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = Color.green;
        }
        else if (room.myRoomData.myType == 2)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = Color.grey;
        }
        else if (room.myRoomData.myType == 3)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = new Color(0.62f, 0.32f, 0.17f);
        }
        else if (room.myRoomData.myType == 4)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = Color.yellow;
        }
        else if (room.myRoomData.myType == 5)
        {
            myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX].color = Color.white;
        }

        if (myCurrentRoom != null)
        {
            Color col = myCurrentRoom.color;
            col.a = 1;
            myCurrentRoom.color = col;
        }
        myCurrentRoom = myRoomImages[aY * myWorldGeneration.GetWorldSideSize() + aX];
        myVisibility = 1;*/
    }
}
