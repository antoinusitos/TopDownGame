using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager   _myInstance = null;

    public Slider                   myProgressSlider = null;
    public Text                     myTextProgressSlider = null;

    public GameObject               myPauseMenu = null;

    private int                     myNumberToSpawn = 0;
    private int                     myCurrentNumberSpawned = 0;
    private bool                    myIsLoading = true;

    private WorldGeneration         myWorldGeneration = null;

    private void Awake()
    {
        _myInstance = this;
        myWorldGeneration = FindObjectOfType<WorldGeneration>();
    }

    private void Update()
    {
        if(!myIsLoading)
        {
            return;
        }

        myProgressSlider.value = (float)myCurrentNumberSpawned / myNumberToSpawn;
        myTextProgressSlider.text = "Loading... (" + myCurrentNumberSpawned.ToString() + "/" + myNumberToSpawn.ToString() + ")";

        if(myProgressSlider.value >= 1)
        {
            myIsLoading = false;
            myProgressSlider.gameObject.SetActive(false);
        }
    }

    public static LoadingManager GetInstance()
    {
        return _myInstance;
    }

    public void AddToSpawn()
    {
        myNumberToSpawn++;
    }

    public void AddSpawned()
    {
        myCurrentNumberSpawned++;
    }

    public void LoadWorld()
    {
        StartCoroutine("LoadSave");   
    }

    private IEnumerator LoadSave()
    {
        Debug.Log("Seed:" + PlayerPrefs.GetInt("Seed"));

        myWorldGeneration.myUseSeed = true;
        myWorldGeneration.myGivenSeed = PlayerPrefs.GetInt("Seed");

        myWorldGeneration.GenerateWorld();

        Debug.Log("PlayerRoomX:" + PlayerPrefs.GetInt("PlayerRoomX"));
        Debug.Log("PlayerRoomY:" + PlayerPrefs.GetInt("PlayerRoomY"));
        Debug.Log("PlayerBiomeX:" + PlayerPrefs.GetInt("PlayerBiomeX"));
        Debug.Log("PlayerBiomeY:" + PlayerPrefs.GetInt("PlayerBiomeY"));

        while(!myWorldGeneration.myGenerationFinished)
        {
            yield return null;
        }

        myWorldGeneration.ActivateBiome(PlayerPrefs.GetInt("PlayerBiomeX"), PlayerPrefs.GetInt("PlayerBiomeY"));
        Room theRoom = myWorldGeneration.ActivateRoom(PlayerPrefs.GetInt("PlayerRoomX"), PlayerPrefs.GetInt("PlayerRoomY"));


        myWorldGeneration.PlacePlayerInRoom(theRoom);

        Debug.Log("PlayerX:" + PlayerPrefs.GetFloat("PlayerX"));
        Debug.Log("PlayerY:" + PlayerPrefs.GetFloat("PlayerY"));

        myWorldGeneration.PlacePlayerAt(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"));

        MapUI mapUI = FindObjectOfType<MapUI>();

        int roomVisited = PlayerPrefs.GetInt("RoomsVisited");

        for (int i = 0; i < roomVisited; ++i)
        {
            Debug.Log("want biome " + PlayerPrefs.GetInt("Visited" + i + "BiomeX") + ":" + PlayerPrefs.GetInt("Visited" + i + "BiomeY"));
            Debug.Log("want room " + PlayerPrefs.GetInt("Visited" + i + "RoomX") + ":" + PlayerPrefs.GetInt("Visited" + i + "RoomY"));

            Biome biome = myWorldGeneration.GetBiome(PlayerPrefs.GetInt("Visited" + i + "BiomeX"), PlayerPrefs.GetInt("Visited" + i + "BiomeY"));

            mapUI.SetRoomVisited(PlayerPrefs.GetInt("Visited" + i + "RoomX"), PlayerPrefs.GetInt("Visited" + i + "RoomY"), biome);
        }

        /*PlayerPrefs.SetInt("Seed", myWorldGeneration.GetSeed());

        Biome[] biomes = myWorldGeneration.GetBiomes();

        List<ResourceUsable> resourceToSave = new List<ResourceUsable>();

        for (int i = 0; i < biomes.Length; ++i)
        {
            List<ResourceUsable> newResouces = biomes[i].GetResourceUsablesToSave();
            for (int j = 0; j < newResouces.Count; ++j)
            {
                resourceToSave.Add(newResouces[j]);
            }
        }

        PlayerPrefs.SetInt("PlayerRoomX", myWorldGeneration.GetPlayerMovement().GetCurrentRoom().myRoomData.myX);
        PlayerPrefs.SetInt("PlayerRoomY", myWorldGeneration.GetPlayerMovement().GetCurrentRoom().myRoomData.myY);
        PlayerPrefs.SetInt("PlayerBiomeX", myWorldGeneration.GetPlayerMovement().GetCurrentRoom().GetBiome().myX);
        PlayerPrefs.SetInt("PlayerBiomeY", myWorldGeneration.GetPlayerMovement().GetCurrentRoom().GetBiome().myY);
        PlayerPrefs.SetFloat("PlayerX", myWorldGeneration.GetPlayerMovement().transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", myWorldGeneration.GetPlayerMovement().transform.position.y);

        PlayerPrefs.SetInt("ResourceNum", resourceToSave.Count);

        for (int i = 0; i < resourceToSave.Count; ++i)
        {
            PlayerPrefs.SetInt("Resource" + i + "Life", resourceToSave[i].GetCurrentLife());
            PlayerPrefs.SetFloat("Resource" + i + "Time", resourceToSave[i].GetCurrentTimeToRefill());
            PlayerPrefs.SetInt("Resource" + i + "X", resourceToSave[i].GetTile().myTileData.myX);
            PlayerPrefs.SetInt("Resource" + i + "Y", resourceToSave[i].GetTile().myTileData.myY);
            PlayerPrefs.SetInt("Resource" + i + "BiomeX", resourceToSave[i].GetTile().myParentRoom.GetBiome().myX);
            PlayerPrefs.SetInt("Resource" + i + "BiomeY", resourceToSave[i].GetTile().myParentRoom.GetBiome().myY);
        }

        PlayerPrefs.Save();*/
    }
}
