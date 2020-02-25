using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager  myInstance = null;

    private WorldGeneration     myWorldGeneration = null;

    private void Awake()
    {
        myInstance = this;
        myWorldGeneration = FindObjectOfType<WorldGeneration>();
    }

    public static SaveManager GetInstance()
    {
        return myInstance;
    }

    public void SaveWorld()
    {
        PlayerPrefs.SetInt("Seed", myWorldGeneration.GetSeed());

        Biome[] biomes = myWorldGeneration.GetBiomes();

        List<ResourceUsable> resourceToSave = new List<ResourceUsable>();

        for(int i = 0; i < biomes.Length; ++i)
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
        PlayerPrefs.SetInt("PlayerSouls", myWorldGeneration.GetPlayerMovement().GetComponent<PlayerData>().GetSoulsCollected());

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

        MapUI mapUI = FindObjectOfType<MapUI>();

        List<Room> rooms = mapUI.GetVisitedRooms();

        PlayerPrefs.SetInt("RoomsVisited", rooms.Count);

        for (int i = 0; i < rooms.Count; ++i)
        {
            PlayerPrefs.SetInt("Visited"+ i + "BiomeX", rooms[i].GetBiome().myX);
            PlayerPrefs.SetInt("Visited"+ i + "BiomeY", rooms[i].GetBiome().myY);
            PlayerPrefs.SetInt("Visited"+ i + "RoomX", rooms[i].myRoomData.myX);
            PlayerPrefs.SetInt("Visited"+ i + "RoomY", rooms[i].myRoomData.myY);
        }

        PlayerPrefs.Save();
    }
}
