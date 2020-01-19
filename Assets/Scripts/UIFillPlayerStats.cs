using UnityEngine;
using UnityEngine.UI;

public class UIFillPlayerStats : MonoBehaviour
{
    public Slider       myPlayerLifeBar = null;
    public Slider       myPlayerManaBar = null;

    private PlayerData  myPlayerData = null;

    private void Start()
    {
        myPlayerData = GetComponentInParent<PlayerData>();
    }

    // Update is called once per frame
    private void Update()
    {
        myPlayerLifeBar.value = myPlayerData.GetLifeRatio();
        myPlayerManaBar.value = myPlayerData.GetManaRatio();
    }
}
