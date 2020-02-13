using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    public int              myX;
    public int              myY;
    public int              myType;
    public List<Room>       myRooms;
    public Room             myRoomPrefab = null;

    private Transform       myTransform = null;   
    private int             myRoomSideSize = 20;
    private int             myRoomsNumber = 0;
    private const int       myTryNumber = 20;
    private const int       myMinRoomNumber = 20;
    private RoomData[]      myWorld = null;
    private RoomData        myStartingRoomData;
    private Room            myStartingRoom;
    private const float     myGenerationRoomPercentage = 0.4f;
    private const float     mySpriteSpace = 1;//2.5f;
    private int             myBiomeSideSize = 10;

    public RoomData        myWestRoom;
    public RoomData        myEastRoom;
    public RoomData        myNorthRoom;
    public RoomData        mySouthRoom;

    public void Init(int aX, int aY, int aType, int aBiomeSideSize, int aRoomSideSize)
    {
        myX = aX;
        myY = aY;
        myType = aType;
        myRooms = new List<Room>();
        myTransform = transform;
        myBiomeSideSize = aBiomeSideSize;
        myRoomSideSize = aRoomSideSize;
    }

    public void GenerateRooms()
    {
        myWorld = new RoomData[myBiomeSideSize * myBiomeSideSize];

        // Generate all the data for the rooms
        for (int y = 0; y < myBiomeSideSize; ++y)
        {
            for (int x = 0; x < myBiomeSideSize; ++x)
            {
                RoomData theRoom = myWorld[y * myBiomeSideSize + x];
                theRoom.myX = x;
                theRoom.myY = y;
                theRoom.myType = -1;
                myWorld[y * myBiomeSideSize + x] = theRoom;
            }
        }

        // Calculate the room to start with
        int midWorld = myWorld.Length / 2 + myBiomeSideSize / 2;
        int topMidWorld = midWorld - myBiomeSideSize;
        int bottomMidWorld = midWorld + myBiomeSideSize;

        int[] startingPointsIndex = {
            topMidWorld - 1, topMidWorld, topMidWorld + 1,
            midWorld - 1, midWorld, midWorld + 1,
            bottomMidWorld - 1, bottomMidWorld, bottomMidWorld + 1
        };

        if (!TryFillRoom(startingPointsIndex))
        {
            Debug.LogError("Unable to create enough rooms");
            return;
        }

        CheckNeighbours();

        InstantiateRooms();

        AffectNextRoomsTriggers();
    }

    private bool TryFillRoom(int[] aStartingPointsIndex)
    {
        myRoomsNumber = 0;
        int currentTryNumber = 0;

        while (myRoomsNumber < myMinRoomNumber && currentTryNumber < myTryNumber)
        {
            for (int i = 0; i < myWorld.Length; ++i)
            {
                myWorld[i].myType = -1;
            }

            Debug.Log("Try number to generate rooms : " + currentTryNumber);
            myRoomsNumber = 0;
            currentTryNumber++;

            int startingPointIndex = aStartingPointsIndex[Random.Range(0, aStartingPointsIndex.Length)];

            myStartingRoomData = myWorld[startingPointIndex];

            FillRoom(myStartingRoomData.myX, myStartingRoomData.myY, true);
        }

        //DebugRooms();

        if (myRoomsNumber >= myMinRoomNumber)
        {
            return true;
        }

        return false;
    }

    private bool FillRoom(int aX, int aY, bool aStartingRoom = false)
    {
        bool generate = Random.Range(0f, 1f) >= myGenerationRoomPercentage ? true : false;

        if (!generate && !aStartingRoom)
        {
            myWorld[aY * myBiomeSideSize + aX].myType = 0;
            return false;
        }

        myWorld[aY * myBiomeSideSize + aX].myType = Random.Range(1, 6);
        myRoomsNumber++;

        if (aX - 1 > 0 && myWorld[aY * myBiomeSideSize + aX - 1].myType == -1)
        {
            FillRoom(aX - 1, aY);
        }
        if (aX + 1 < myBiomeSideSize && myWorld[aY * myBiomeSideSize + aX + 1].myType == -1)
        {
            FillRoom(aX + 1, aY);
        }
        if (aY - 1 > 0 && myWorld[(aY - 1) * myBiomeSideSize + aX].myType == -1)
        {
            FillRoom(aX, aY - 1);
        }
        if (aY + 1 < myBiomeSideSize && myWorld[(aY + 1) * myBiomeSideSize + aX].myType == -1)
        {
            FillRoom(aX, aY + 1);
        }

        return true;
    }

    private void CheckNeighbours()
    {
        for (int y = 0; y < myBiomeSideSize; ++y)
        {
            for (int x = 0; x < myBiomeSideSize; ++x)
            {
                if (myWorld[y * myBiomeSideSize + x].myType > 0)
                {
                    // Check left neighbour
                    if (x > 0 && myWorld[y * myBiomeSideSize + x - 1].myType > 0)
                    {
                        myWorld[y * myBiomeSideSize + x].myHasLeftNeighbour = true;
                        myWorld[y * myBiomeSideSize + x - 1].myHasRightNeighbour = true;
                    }
                    // Check right neighbour
                    if (x < myBiomeSideSize - 1 && myWorld[y * myBiomeSideSize + x + 1].myType > 0)
                    {
                        myWorld[y * myBiomeSideSize + x].myHasRightNeighbour = true;
                        myWorld[y * myBiomeSideSize + x + 1].myHasLeftNeighbour = true;
                    }
                    // Check bottom neighbour
                    if (y > 0 && myWorld[(y - 1) * myBiomeSideSize + x].myType > 0)
                    {
                        myWorld[y * myBiomeSideSize + x].myHasTopNeighbour = true;
                        myWorld[(y - 1) * myBiomeSideSize + x].myHasBottomNeighbour = true;
                    }
                    // Check top neighbour
                    if (y < myBiomeSideSize - 1 && myWorld[(y + 1) * myBiomeSideSize + x].myType > 0)
                    {
                        myWorld[y * myBiomeSideSize + x].myHasBottomNeighbour = true;
                        myWorld[(y + 1) * myBiomeSideSize + x].myHasTopNeighbour = true;
                    }
                }
            }
        }
    }

    private void InstantiateRooms()
    {
        for (int y = 0; y < myBiomeSideSize; ++y)
        {
            for (int x = 0; x < myBiomeSideSize; ++x)
            {
                if (myWorld[y * myBiomeSideSize + x].myType > 0)
                {
                    Room room = Instantiate(myRoomPrefab, myTransform);
                    room.transform.localPosition = new Vector3(x * myRoomSideSize * mySpriteSpace, y * myRoomSideSize * mySpriteSpace, 0);
                    room.myRoomData = myWorld[y * myBiomeSideSize + x];
                    myRooms.Add(room);
                    room.ConstuctRoom(myRoomSideSize, this);

                    if (myStartingRoomData.myX == x && myStartingRoomData.myY == y)
                    {
                        room.myStartingRoom = true;
                        myStartingRoom = room;
                    }

                    room.transform.parent = myTransform;
                }
            }
        }
    }

    public void AffectNextRoomsTriggers()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            myRooms[i].AffectTransitions();
        }
    }

    public void HideRooms()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            if (!myRooms[i].myStartingRoom)
            {
                myRooms[i].gameObject.SetActive(false);
            }
            else
            {
                Camera.main.transform.position = myRooms[i].transform.position + Vector3.up * 10 + Vector3.right * (myRoomSideSize / 2);
            }
        }
    }

    public void FindExtremeRooms()
    {
        myWestRoom = myRooms[0].myRoomData;
        myEastRoom = myRooms[0].myRoomData;
        myNorthRoom = myRooms[0].myRoomData;
        mySouthRoom = myRooms[0].myRoomData;

        for (int i = 0; i < myRooms.Count; ++i)
        {
            if(myRooms[i].myRoomData.myY > myNorthRoom.myY)
            {
                myNorthRoom = myRooms[i].myRoomData;
            }
            if (myRooms[i].myRoomData.myY < mySouthRoom.myY)
            {
                mySouthRoom = myRooms[i].myRoomData;
            }
            if (myRooms[i].myRoomData.myX < myWestRoom.myX)
            {
                myWestRoom = myRooms[i].myRoomData;
            }
            if (myRooms[i].myRoomData.myX > myEastRoom.myX)
            {
                myEastRoom = myRooms[i].myRoomData;
            }
        }
    }

    public int GetWorldSideSize()
    {
        return myBiomeSideSize;
    }

    public int GetRoomSize()
    {
        return myRoomSideSize;
    }

    public void DebugRooms()
    {
        for (int y = 0; y < myBiomeSideSize; ++y)
        {
            string line = "";
            for (int x = 0; x < myBiomeSideSize; ++x)
            {
                line += myWorld[y * myBiomeSideSize + x].myType.ToString();
            }
            Debug.Log(line);
        }
    }

    public Room GetRoom(int aX, int aY)
    {
        if (aX < 0 || aY < 0 || aX == myBiomeSideSize || aY == myBiomeSideSize)
            return null;

        for (int i = 0; i < myRooms.Count; ++i)
        {
            if (myRooms[i].myRoomData.myX == aX & myRooms[i].myRoomData.myY == aY)
                return myRooms[i];
        }

        return null;
    }
}
