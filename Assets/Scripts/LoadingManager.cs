using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager   _myInstance = null;

    public Slider                   myProgressSlider = null;
    public Text                     myTextProgressSlider = null;

    private int                     myNumberToSpawn = 0;
    private int                     myCurrentNumberSpawned = 0;
    private bool                    myIsLoading = true;

    private void Awake()
    {
        _myInstance = this;
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
}
