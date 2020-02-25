using UnityEngine;
using UnityEngine.UI;

public class UIShowSoulsCollected : MonoBehaviour
{
    [SerializeField]
    private PlayerData  myPlayerData = null;

    [SerializeField]
    private Text        myTextShowing = null;

    private void Update()
    {
        myTextShowing.text = myPlayerData.GetSoulsCollected().ToString();
    }
}
