using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    public int myX;
    public int myY;
    public int myType;
    public List<Room> myRooms;

    public void Init(int aX, int aY, int aType)
    {
        myX = aX;
        myY = aY;
        myType = aType;
        myRooms = new List<Room>();
    }

    public void GenerateRooms()
    {

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

        DebugSeed();

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
                        myWorld[y * myWorldSideSize + x].myHasLeftNeighbour = true;
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
                if (myWorld[y * myWorldSideSize + x].myType > 0)
                {
                    Room room = Instantiate(myRoomPrefab, new Vector3(x * myRoomSize * mySpriteSpace, y * myRoomSize * mySpriteSpace, 0), Quaternion.identity);
                    room.myRoomData = myWorld[y * myWorldSideSize + x];
                    myRooms.Add(room);
                    room.ConstuctRoom(myRoomSize, this);

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
}
