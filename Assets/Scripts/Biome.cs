using System.Collections;
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

    public bool             myGenerationDone = false;

    public string           myName = "";

    public void Init(int aX, int aY, int aType)
    {
        myX = aX;
        myY = aY;
        myType = aType;
        myRooms = new List<Room>();
        myTransform = transform;
        myBiomeSideSize = Data.myBiomeSideSize;
        myRoomSideSize = Data.myRoomSideSize;
        myName = Data.GetBiomeName();
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
                theRoom.Init();
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
            Debug.LogError("Unable to create enough rooms with " + myRoomsNumber + " on " + myMinRoomNumber);
            return;
        }

        CheckNeighbours();
    }

    public void InstantiateRoom()
    {
        StartCoroutine(InstantiateRooms());
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

            //Debug.Log("Try number to generate rooms : " + currentTryNumber);
            myRoomsNumber = 0;
            currentTryNumber++;

            int startingPointIndex = aStartingPointsIndex[Random.Range(0, aStartingPointsIndex.Length)];

            if(startingPointIndex >= aStartingPointsIndex.Length)
            {
                startingPointIndex--;
            }

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
        LoadingManager.GetInstance().AddToSpawn();

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
                        myWorld[y * myBiomeSideSize + x].myTransitionDatas.Add(
                            new TransitionData() { myRoomX = x - 1, myRoomY = y, myBiome = this, myTransitionDirection = TransitionDirection.LEFT }
                        );
                    }
                    // Check right neighbour
                    if (x < myBiomeSideSize - 1 && myWorld[y * myBiomeSideSize + x + 1].myType > 0)
                    {
                        myWorld[y * myBiomeSideSize + x].myTransitionDatas.Add(
                            new TransitionData() { myRoomX = x + 1, myRoomY = y, myBiome = this, myTransitionDirection = TransitionDirection.RIGHT }
                        );
                    }
                    // Check bottom neighbour
                    if (y > 0 && myWorld[(y - 1) * myBiomeSideSize + x].myType > 0)
                    {
                        myWorld[y * myBiomeSideSize + x].myTransitionDatas.Add(
                            new TransitionData() { myRoomX = x, myRoomY = y - 1, myBiome = this, myTransitionDirection = TransitionDirection.DOWN }
                        );
                    }
                    // Check top neighbour
                    if (y < myBiomeSideSize - 1 && myWorld[(y + 1) * myBiomeSideSize + x].myType > 0)
                    {
                        myWorld[y * myBiomeSideSize + x].myTransitionDatas.Add(
                            new TransitionData() { myRoomX = x, myRoomY = y + 1, myBiome = this, myTransitionDirection = TransitionDirection.UP }
                        );
                    }
                }
            }
        }
    }

    private IEnumerator InstantiateRooms()
    {
        if (WorldGeneration.GetInstance().DEBUG)
        {
            WorldGeneration.GetInstance().DEBUGCANVAS.ClearHighlightBiome();
        }

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
                    //room.ConstuctRoom(myRoomSideSize, this);

                    room.GetSeedAndGenerate(this);

                    if (myStartingRoomData.myX == x && myStartingRoomData.myY == y)
                    {
                        room.myStartingRoom = true;
                        myStartingRoom = room;
                    }

                    room.transform.parent = myTransform;

                    LoadingManager.GetInstance().AddSpawned();

                    if (WorldGeneration.GetInstance().DEBUG)
                    {
                        WorldGeneration.GetInstance().DEBUGCANVAS.HighlightBiome(x, y);

                        yield return new WaitForSeconds(WorldGeneration.GetInstance().STEPTIME);
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }

        myGenerationDone = true;

        gameObject.SetActive(false);
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
            myRooms[i].gameObject.SetActive(false);
        }
    }

    public void SpawnResources()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            myRooms[i].SpawnResources();
        }
    }

    public void SpawnDecoration()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            myRooms[i].SpawnDecoration();
        }
    }

    public void FindExtremeRooms()
    {
        myWestRoom = myWorld[0];
        myEastRoom = myWorld[0];
        myNorthRoom = myWorld[0];
        mySouthRoom = myWorld[0];

        for (int i = 0; i < myWorld.Length; ++i)
        {
            if(myWestRoom.myType == -1 && myWorld[i].myType > 0)
            {
                myWestRoom = myWorld[i];
            }
            if (myNorthRoom.myType == -1 && myWorld[i].myType > 0)
            {
                myNorthRoom = myWorld[i];
            }
            if (mySouthRoom.myType == -1 && myWorld[i].myType > 0)
            {
                mySouthRoom = myWorld[i];
            }
            if (myEastRoom.myType == -1 && myWorld[i].myType > 0)
            {
                myEastRoom = myWorld[i];
            }

            if (myWorld[i].myY >= myNorthRoom.myY && myWorld[i].myType > 0)
            {
                myNorthRoom = myWorld[i];
            }
            if (myWorld[i].myY <= mySouthRoom.myY && myWorld[i].myType > 0)
            {
                mySouthRoom = myWorld[i];
            }
            if (myWorld[i].myX <= myWestRoom.myX && myWorld[i].myType > 0)
            {
                myWestRoom = myWorld[i];
            }
            if (myWorld[i].myX >= myEastRoom.myX && myWorld[i].myType > 0)
            {
                myEastRoom = myWorld[i];
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

    public Room GetStartingRoom()
    {
        return myStartingRoom;
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

    public RoomData GetRoomData(int aX, int aY)
    {
        if (aX < 0 || aY < 0 || aX == myBiomeSideSize || aY == myBiomeSideSize)
        {
            Debug.LogError("GetRoomData cannot return a correct value");
            return new RoomData();
        }

        for (int i = 0; i < myWorld.Length; ++i)
        {
            if (myWorld[i].myX == aX & myWorld[i].myY == aY)
            {
                //Debug.Log("GetRoomData returning a correct value");
                return myWorld[i];
            }
        }

        Debug.LogError("GetRoomData cannot return a correct value");
        return new RoomData();
    }

    public void SetRoomData(int aX, int aY, RoomData aRoomData)
    {
        for (int i = 0; i < myWorld.Length; ++i)
        {
            if (myWorld[i].myX == aX & myWorld[i].myY == aY)
            {
                myWorld[i] = aRoomData;
            }
        }
    }

    public List<ResourceUsable> GetResourceUsablesToSave()
    {
        List<ResourceUsable> toSave = new List<ResourceUsable>();

        for (int i = 0; i < myRooms.Count; ++i)
        {
            List<ResourceUsable> resourceToSave = myRooms[i].GetResourceToSave();
            for (int j = 0; j < resourceToSave.Count; ++j)
            {
                toSave.Add(resourceToSave[j]);
            }
        }

        return toSave;
    }

    public Room ActivateRoom(int aX, int aY)
    {
        Room theRoom = null;
        for (int i = 0; i < myRooms.Count; i++)
        {
            if (myRooms[i].myRoomData.myX == aX && myRooms[i].myRoomData.myY == aY)
            {
                theRoom = myRooms[i];
                myRooms[i].gameObject.SetActive(true);
            }
            else
            {
                myRooms[i].gameObject.SetActive(false);
            }
        }

        return theRoom;
    }

    public void ChangeTileRendering()
    {
        for (int i = 0; i < myRooms.Count; i++)
        {
            myRooms[i].ChangeTileRendering();
        }
    }
}
