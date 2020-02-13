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
    private const int       myBiomesSideNumber = 2;
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

    private void Start()
    {
        if(DEBUG)
        {
            DEBUGCANVAS.Init(myBiomesSideNumber, myBiomeSideSize);
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

            //Instantiate rooms for each biome
            GenerateBiomesRooms();

            //Find farest North, South, West and East room for each biome
            FindBiomesExtremeRooms();

            //Link rooms
            //Link biomes with room
            //Spawn tiles
            //Add Environment
            //Spawn NPC
            //Create Quests

            ChangeTileRendering();

            SpawnResources();

            SpawnDecoration();

            //Invoke("HideRooms", 2);

            //SpawnPlayer();
        }
    }

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

    private IEnumerator DelayedGenerateBiomes()
    {
        myBiomes = new Biome[myBiomesSideNumber * myBiomesSideNumber];
        int x = 0;
        int y = 0;
        for (int i = 0; i < myBiomes.Length; i++)
        {
            yield return new WaitForSeconds(STEPTIME);
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
        }

        StartCoroutine("DelayedGenerateBiomesRooms");
    }

    private void GenerateBiomesRooms()
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i].GenerateRooms();
        }
    }

    private IEnumerator DelayedGenerateBiomesRooms()
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            DEBUGCANVAS.SelectMap(myBiomes[i].myX, myBiomes[i].myY);
            myBiomes[i].GenerateRooms();
            while (!myBiomes[i].myGenerationDone)
            {
                yield return null;
            }
        }
    }

    private void FindBiomesExtremeRooms()
    {
        for (int i = 0; i < myBiomes.Length; i++)
        {
            myBiomes[i].FindExtremeRooms();
        }
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

    private void DebugSeed()
    {
        Debug.Log("Seed used :" + myCurrentSeed.ToString());
    }

    private void SpawnPlayer()
    {
        /*myPlayerMovement = Instantiate(myPlayerPrefab, myStartingRoom.GetMidTile().transform.position + Vector3.forward * -0.05f, Quaternion.identity);
        FindObjectOfType<CameraFollowPlayer>().myPlayer = myPlayerMovement.transform;
        myPlayerMovement.GetComponentInChildren<MapUI>().SetRoomVisited(myStartingRoomData.myX, myStartingRoomData.myY);*/
    }
}
