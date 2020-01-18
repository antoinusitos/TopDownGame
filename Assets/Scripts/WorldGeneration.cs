using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public int              myGivenSeed = 0;
    public bool             myUseSeed = false;
    public bool             myRandomSeed = false;
    public Room             myRoomPrefab = null;
    public PlayerMovement   myPlayerPrefab = null;

    private const int       myWorldSideSize = 10;
    private RoomData[]      myWorld = null;
    private List<Room>      myRooms = new List<Room>();
    private Random.State    myCurrentSeedState; // *****To Serialize*****
    private int             myCurrentSeed = 0;// *****To Serialize*****
    private const int       myDefaultSeed = 0;
    private const float     myGenerationRoomPercentage = 0.4f;
    private int             myRoomsNumber = 0;
    private const int       myTryNumber = 20;
    private const int       myMinRoomNumber = 20;
    private const int       myRoomSize = 20;
    private Transform       myTransform = null;
    private RoomData        myStartingRoomData;
    private Room            myStartingRoom;
    private PlayerMovement  myPlayerMovement = null;

    private void Awake()
    {
        myTransform = transform;
    }

    private void Start()
    {
        GenerateWorld();
    }

    public int GetWorldSideSize()
    {
        return myWorldSideSize;
    }

    public void GenerateWorld()
    {
        if (myUseSeed)
        {
            myCurrentSeed = myGivenSeed;
        }
        else if(myRandomSeed)
        {
            myCurrentSeed = Random.Range(0, 9999999);
        }
        else
        {
            myCurrentSeed = myDefaultSeed;
        }
        Random.InitState(myCurrentSeed);
        myCurrentSeedState = Random.state;

        myWorld = new RoomData[myWorldSideSize * myWorldSideSize];

        for (int y = 0; y < myWorldSideSize; ++y)
        {
            for (int x = 0; x < myWorldSideSize; ++x)
            {
                RoomData theRoom = myWorld[y * myWorldSideSize + x];
                theRoom.myX = x;
                theRoom.myY = y;
                theRoom.myType = -1;
                myWorld[y * myWorldSideSize + x] = theRoom;
            }
        }

        int midWorld = myWorld.Length / 2 + myWorldSideSize / 2;
        int topMidWorld = midWorld - myWorldSideSize;
        int bottomMidWorld = midWorld + myWorldSideSize;

        int[] startingPointsIndex = {
            topMidWorld - 1, topMidWorld, topMidWorld + 1,
            midWorld - 1, midWorld, midWorld + 1,
            bottomMidWorld - 1, bottomMidWorld, bottomMidWorld + 1
        };

        if(!TryFillRoom(startingPointsIndex))
        {
            Debug.LogError("Unable to create enough rooms");
            return;
        }

        CheckNeighbours();

        InstantiateRooms();

        InstantiateWalls();

        Invoke("HideRooms", 2);

        SpawnPlayer();
    }

    private bool TryFillRoom(int[] aStartingPointsIndex)
    {
        myRoomsNumber = 0;
        int currentTryNumber = 0;

        while(myRoomsNumber < myMinRoomNumber && currentTryNumber < myTryNumber)
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

        DebugSeed();

        if(myRoomsNumber >= myMinRoomNumber)
        {
            return true;
        }

        return false;
    }

    private bool FillRoom(int aX, int aY, bool aStartingRoom = false)
    {
        bool generate = Random.Range(0f, 1f) >= myGenerationRoomPercentage ? true : false;
        
        if(!generate && !aStartingRoom)
        {
            myWorld[aY * myWorldSideSize + aX].myType = 0;
            return false;
        }

        myWorld[aY * myWorldSideSize + aX].myType = Random.Range(1, 6);
        myRoomsNumber++;

        if (aX - 1 > 0 && myWorld[aY * myWorldSideSize + aX - 1].myType == -1)
        {
            FillRoom(aX - 1, aY);
        }
        if (aX + 1 < myWorldSideSize && myWorld[aY * myWorldSideSize + aX + 1].myType == -1)
        {
            FillRoom(aX + 1, aY);
        }
        if (aY - 1 > 0 && myWorld[(aY - 1) * myWorldSideSize + aX].myType == -1)
        {
            FillRoom(aX, aY - 1);
        }
        if (aY + 1 < myWorldSideSize && myWorld[(aY + 1) * myWorldSideSize + aX].myType == -1)
        {
            FillRoom(aX, aY + 1);
        }

        return true;
    }

    private void CheckNeighbours()
    {
        for (int y = 0; y < myWorldSideSize; ++y)
        {
            for (int x = 0; x < myWorldSideSize; ++x)
            {
                if (myWorld[y * myWorldSideSize + x].myType > 0)
                {
                    // Check left neighbour
                    if (x > 0 && myWorld[y * myWorldSideSize + x - 1].myType > 0)
                    {
                        myWorld[y * myWorldSideSize + x ].myHasLeftNeighbour = true;
                        myWorld[y * myWorldSideSize + x - 1].myHasRightNeighbour = true;
                    }
                    // Check right neighbour
                    if (x < myWorldSideSize - 1 && myWorld[y * myWorldSideSize + x + 1].myType > 0)
                    {
                        myWorld[y * myWorldSideSize + x].myHasRightNeighbour = true;
                        myWorld[y * myWorldSideSize + x + 1].myHasLeftNeighbour = true;
                    }
                    // Check bottom neighbour
                    if (y > 0 && myWorld[(y - 1) * myWorldSideSize + x].myType > 0)
                    {
                        myWorld[y * myWorldSideSize + x].myHasTopNeighbour = true;
                        myWorld[(y - 1) * myWorldSideSize + x].myHasBottomNeighbour = true;
                    }
                    // Check top neighbour
                    if (y < myWorldSideSize - 1 && myWorld[(y + 1) * myWorldSideSize + x].myType > 0)
                    {
                        myWorld[y * myWorldSideSize + x].myHasBottomNeighbour = true;
                        myWorld[(y + 1) * myWorldSideSize + x].myHasTopNeighbour = true;
                    }
                }
            }
        }
    }

    private void InstantiateRooms()
    {
        for (int y = 0; y < myWorldSideSize; ++y)
        {
            for (int x = 0; x < myWorldSideSize; ++x)
            {
                if(myWorld[y * myWorldSideSize + x].myType > 0)
                {
                    Room room = Instantiate(myRoomPrefab, new Vector3(x * myRoomSize, 0, y * myRoomSize ), Quaternion.identity);
                    room.myRoomData = myWorld[y * myWorldSideSize + x];
                    myRooms.Add(room);
                    room.ConstuctRoom(myRoomSize);

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

    private void InstantiateWalls()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            myRooms[i].PlaceWalls(myRoomSize, this);
        }
    }

    private void HideRooms()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            if(!myRooms[i].myStartingRoom)
            {
                myRooms[i].gameObject.SetActive(false);
            }
            else
            {
                Camera.main.transform.position = myRooms[i].transform.position + Vector3.up * 10 + Vector3.right * (myRoomSize / 2);
            }
        }

        myPlayerMovement.GetComponentInChildren<MapUI>().SetRoomVisited(myStartingRoomData.myX, myStartingRoomData.myY);
    }

    private void DebugRooms()
    {
        for (int y = 0; y < myWorldSideSize; ++y)
        {
            string line = "";
            for (int x = 0; x < myWorldSideSize; ++x)
            {
                line += myWorld[y * myWorldSideSize + x].myType.ToString();
            }
            Debug.Log(line);
        }
    }

    private void DebugSeed()
    {
        Debug.Log("Seed used :" + myCurrentSeed.ToString());
    }

    private void SpawnPlayer()
    {
        myPlayerMovement = Instantiate(myPlayerPrefab, myStartingRoom.GetMidTile().transform.position + Vector3.up * 0.5f, Quaternion.identity);
        FindObjectOfType<CameraFollowPlayer>().SetPlayerMovement(myPlayerMovement);
    }

    public Room GetRoom(int aX, int aY)
    {
        if (aX < 0 || aY < 0 || aX == myWorldSideSize || aY == myWorldSideSize)
            return null;

        for (int i = 0; i < myRooms.Count; ++i)
        {
            if (myRooms[i].myRoomData.myX == aX & myRooms[i].myRoomData.myY == aY)
                return myRooms[i];
        }

        return null;
    }
}
