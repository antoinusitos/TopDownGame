﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldGeneration : MonoBehaviour
{
    public bool             DEBUG = false;
    public float            STEPTIME = 0.5f;
    public DebugCanvas      DEBUGCANVAS = null;

    public int              myGivenSeed = 0;
    public bool             myUseSeed = false;
    public bool             myRandomSeed = false;
    public PlayerMovement   myPlayerPrefab = null;

    public InputField       myInputFieldSeed = null;

    public bool             myPlacePlayer = false;

    public bool             myGenerationFinished = false;

    public Biome            myBiomePrefab = null;
    private const int       myBiomesSideNumber = 1;
    private Biome[]         myBiomes = null;
    private const int       myBiomeSideSize = 10;
    private const int       myRoomSideSize = 20;

    private List<Room>      myRooms = new List<Room>();
    private Random.State    myCurrentSeedState; // *****To Serialize*****
    private int             myCurrentSeed = 0;// *****To Serialize*****
    private const int       myDefaultSeed = 0;
    private const float     myGenerationRoomPercentage = 0.4f;
    private Transform       myTransform = null;
    private PlayerMovement  myPlayerMovement = null;

    private Biome           myCurrentActiveBiome = null;

    private static WorldGeneration  _instance = null;

    private void Awake()
    {
        _instance = this;
        myTransform = transform;
        Data.ShuffleBiomesNames();
    }

    public static WorldGeneration GetInstance()
    {
        return _instance;
    }

    public void StartGenerating()
    {
        if (DEBUG)
        {
            DEBUGCANVAS.Init(myBiomesSideNumber, myBiomeSideSize);
        }
        else
        {
            DEBUGCANVAS.gameObject.SetActive(false);
        }

        GenerateWorld();
    }

    public void GenerateWorld()
    {
        Data.myCurrentSeedRoom = GameInstance.GetInstance().GetSeed();
        myCurrentSeed = Data.myCurrentSeedRoom;
        Random.InitState(myCurrentSeed);

        /*if (myUseSeed)
        {
            myGivenSeed = int.Parse(myInputFieldSeed.text);
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
        myCurrentSeedState = Random.state;*/

        if (DEBUG)
        {
            StartCoroutine("DelayedGenerateBiomes");
            return;
        }
        else
        {
            //Generate the biomes
            GenerateBiomes();

            //Instantiate rooms for each biome and link them
            GenerateBiomesRooms();

            StartCoroutine("ContinueGeneration");
        }
    }

    // Step Way 1
    private IEnumerator DelayedGenerateBiomes()
    {
        myBiomes = new Biome[myBiomesSideNumber * myBiomesSideNumber];
        int x = 0;
        int y = 0;
        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i] = Instantiate(myBiomePrefab, transform);
            myBiomes[i].transform.localPosition = new Vector3(x * myBiomeSideSize * myRoomSideSize, y * myBiomeSideSize * myRoomSideSize, 0);
            myBiomes[i].Init(x, y, 0);
            DEBUGCANVAS.HighlightMap(x, y);
            x++;
            if (x >= myBiomesSideNumber)
            {
                x = 0;
                y++;
            }
            yield return new WaitForSeconds(STEPTIME);
        }
        
        StartCoroutine("DelayedGenerateBiomesRooms");
    }

    // Step Way 2
    private IEnumerator DelayedGenerateBiomesRooms()
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            DEBUGCANVAS.SelectMap(myBiomes[i].myX, myBiomes[i].myY);
            myBiomes[i].GenerateRooms();
        }

        FindBiomesExtremeRooms();

        LinkBiomesWithRooms();

        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i].InstantiateRoom();
            while (!myBiomes[i].myGenerationDone)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(1);
        DEBUGCANVAS.gameObject.SetActive(false);

        SpawnPlayer();
    }

    // Normal Way 1
    private void GenerateBiomes()
    {
        myBiomes = new Biome[myBiomesSideNumber * myBiomesSideNumber];
        int x = 0;
        int y = 0;
        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i] = Instantiate(myBiomePrefab, transform);
            myBiomes[i].gameObject.name = "Biome" + i;
            myBiomes[i].transform.localPosition = new Vector3(x * myBiomeSideSize * myRoomSideSize, y * myBiomeSideSize * myRoomSideSize, 0);
            myBiomes[i].Init(x, y, 0);
            x++;
            if(x >= myBiomesSideNumber)
            {
                x = 0;
                y++;
            }
        }
    }

    // Normal Way 2
    private void GenerateBiomesRooms()
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i].GenerateRooms();
        }
    }

    // Normal Way 3
    private IEnumerator ContinueGeneration()
    {
        //Find farest North, South, West and East room for each biome
        FindBiomesExtremeRooms();

        //Link biomes with room
        LinkBiomesWithRooms();

        //Spawn tiles
        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i].InstantiateRoom();
            while (!myBiomes[i].myGenerationDone)
            {
                yield return null;
            }
        }

        //Create physic link between biomes
        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i].AffectNextRoomsTriggers();
        }

        //Spawn NPC
        //Create Quests

        ChangeTileRendering();

        SpawnResources();

        SpawnDecoration();

        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i].HideRooms();
        }

        myCurrentActiveBiome = myBiomes[0];

        myCurrentActiveBiome.gameObject.SetActive(true);

        myCurrentActiveBiome.GetStartingRoom().gameObject.SetActive(true);

        SpawnPlayer();
    }

    public Biome GetCurrentActiveBiome()
    {
        return myCurrentActiveBiome;
    }

    public void SetCurrentActiveBiome(Biome aBiome)
    {
        myCurrentActiveBiome = aBiome;
    }

    private void FindBiomesExtremeRooms()
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i].FindExtremeRooms();
        }
    }

    private void LinkBiomesWithRooms()
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            // Affect Left transition
            if (myBiomes[i].myX > 0)
            {
                RoomData roomData = myBiomes[i].GetRoomData(myBiomes[i].myWestRoom.myX, myBiomes[i].myWestRoom.myY);

                roomData.myTransitionDatas.Add(
                        new TransitionData() { myRoomX = myBiomes[i - 1].myEastRoom.myX, myRoomY = myBiomes[i - 1].myEastRoom.myY, myBiome = myBiomes[i - 1], myTransitionDirection = TransitionDirection.LEFT }
                );

                myBiomes[i].SetRoomData(myBiomes[i].myWestRoom.myX, myBiomes[i].myWestRoom.myY, roomData);
            }
            // Affect Right transition
            if (myBiomes[i].myX < myBiomesSideNumber - 1)
            {
                Debug.Log("Looking for X" + myBiomes[i].myEastRoom.myX + " : " + myBiomes[i].myEastRoom.myY);
                Debug.Log("in biome " + myBiomes[i].gameObject.name);
                RoomData roomData = myBiomes[i].GetRoomData(myBiomes[i].myEastRoom.myX, myBiomes[i].myEastRoom.myY);

                roomData.myTransitionDatas.Add(
                        new TransitionData() { myRoomX = myBiomes[i + 1].myWestRoom.myX, myRoomY = myBiomes[i + 1].myWestRoom.myY, myBiome = myBiomes[i + 1], myTransitionDirection = TransitionDirection.RIGHT }
                );

                myBiomes[i].SetRoomData(myBiomes[i].myEastRoom.myX, myBiomes[i].myEastRoom.myY, roomData);
            }
        }
    }

    public Biome[] GetBiomes()
    {
        return myBiomes;
    }

    public void SetPlacePlayer(bool aNewState)
    {
        myPlacePlayer = aNewState;
    }

    private void ChangeTileRendering()
    {
        for (int i = 0; i < myBiomes.Length; ++i)
        {
            myBiomes[i].ChangeTileRendering();
        }
    }

    private void SpawnResources()
    {
        for (int i = 0; i < myBiomes.Length; ++i)
        {
            myBiomes[i].SpawnResources();
        }
    }

    private void SpawnDecoration()
    {
        for (int i = 0; i < myBiomes.Length; ++i)
        {
            myBiomes[i].SpawnDecoration();
        }
    }

    public int GetSeed()
    {
        return myCurrentSeed;
    }

    private void DebugSeed()
    {
        Debug.Log("Seed used :" + myCurrentSeed.ToString());
    }

    public int GetWorldSideSize()
    {
        return myBiomeSideSize;
    }

    private void SpawnPlayer()
    {
        myPlayerMovement = Instantiate(myPlayerPrefab, Vector3.zero, Quaternion.identity);
        FindObjectOfType<CameraFollowPlayer>().myPlayer = myPlayerMovement.transform;

        if(myPlacePlayer)
        {
            PlacePlayer();
        }

        myGenerationFinished = true;
    }

    public void PlacePlayer()
    {
        Room myStartingRoom = myCurrentActiveBiome.GetStartingRoom();
        myPlayerMovement.transform.position = myStartingRoom.GetMidTile().transform.position + Vector3.forward * -0.05f;
        MapUI mapUI = myPlayerMovement.GetComponentInChildren<MapUI>();
        mapUI.Init();
        mapUI.SetRoomVisited(myStartingRoom.myRoomData.myX, myStartingRoom.myRoomData.myY);
        myPlayerMovement.SetCurrentRoom(myStartingRoom);
    }

    public void PlacePlayerInRoom(Room aRoom)
    {
        Room myStartingRoom = aRoom;
        myPlayerMovement.transform.position = myStartingRoom.GetMidTile().transform.position + Vector3.forward * -0.05f;
        MapUI mapUI = myPlayerMovement.GetComponentInChildren<MapUI>();
        mapUI.Init();
        mapUI.SetRoomVisited(myStartingRoom.myRoomData.myX, myStartingRoom.myRoomData.myY);
        myPlayerMovement.SetCurrentRoom(myStartingRoom);
    }

    public PlayerMovement GetPlayerMovement()
    {
        return myPlayerMovement;
    }

    public void ActivateBiome(int aX, int aY)
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            if (myBiomes[i].myX == aX && myBiomes[i].myY == aY)
            {
                myCurrentActiveBiome = myBiomes[i];
                myCurrentActiveBiome.gameObject.SetActive(true);
            }
            else
            {
                myCurrentActiveBiome.gameObject.SetActive(false);
            }
        }
    }

    public Biome GetBiome(int aX, int aY)
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            if (myBiomes[i].myX == aX && myBiomes[i].myY == aY)
            {
                return myBiomes[i];
            }
        }

        return null;
    }

    public Room ActivateRoom(int aX, int aY)
    {
        return myCurrentActiveBiome.ActivateRoom(aX, aY);
    }

    public void PlacePlayerAt(float aX, float aY)
    {
        myPlayerMovement.transform.position = new Vector3(aX, aY, 0);
    }
}
