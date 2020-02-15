using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public bool             DEBUG = false;
    public float            STEPTIME = 0.5f;
    public DebugCanvas      DEBUGCANVAS = null;

    public int              myGivenSeed = 0;
    public bool             myUseSeed = false;
    public bool             myRandomSeed = false;
    public PlayerMovement   myPlayerPrefab = null;

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
            myBiomes[i].Init(x, y, 0, myBiomeSideSize, myRoomSideSize);
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
            myBiomes[i].transform.localPosition = new Vector3(x * myBiomeSideSize * myRoomSideSize, y * myBiomeSideSize * myRoomSideSize, 0);
            myBiomes[i].Init(x, y, 0, myBiomeSideSize, myRoomSideSize);
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

        Invoke("SpawnPlayer", 0.5f);
    }

    public Biome GetCurrentActiveBiome()
    {
        return myCurrentActiveBiome;
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
        /*for (int i = 0; i < myBiomes.Length; i++)
        {
            if(myBiomes[i].myX > 0)
            {
                myBiomes[i].myWestRoom.myHasLeftNeighbour = true;
                myBiomes[i - 1].myEastRoom.myHasRightNeighbour = true;
            }
        }*/
    }

    public Biome[] GetBiomes()
    {
        return myBiomes;
    }

    private void ChangeTileRendering()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            myRooms[i].ChangeTileRendering();
        }
    }

    private void SpawnResources()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            myRooms[i].SpawnResources();
        }
    }

    private void SpawnDecoration()
    {
        for (int i = 0; i < myRooms.Count; ++i)
        {
            myRooms[i].SpawnDecoration();
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
        Room myStartingRoom = myCurrentActiveBiome.GetStartingRoom();
        myPlayerMovement = Instantiate(myPlayerPrefab, myStartingRoom.GetMidTile().transform.position + Vector3.forward * -0.05f, Quaternion.identity);
        FindObjectOfType<CameraFollowPlayer>().myPlayer = myPlayerMovement.transform;
        MapUI mapUI = myPlayerMovement.GetComponentInChildren<MapUI>();
        mapUI.Init();
        mapUI.SetRoomVisited(myStartingRoom.myRoomData.myX, myStartingRoom.myRoomData.myY);
        myPlayerMovement.SetCurrentRoom(myStartingRoom);
    }

    public PlayerMovement GetPlayerMovement()
    {
        return myPlayerMovement;
    }
}
