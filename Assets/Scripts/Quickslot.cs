using UnityEngine;
using UnityEngine.UI;

public class Quickslot : MonoBehaviour
{
    public int      myID = -1;
    public Item     myItem;
    public bool     myIsFilled = false;
    public Image    mySlotImage = null;
    public Text     mySlotQuantity = null;

    private void Start()
    {
        mySlotImage = transform.GetChild(0).GetComponent<Image>();
        mySlotQuantity = transform.GetComponentInChildren<Text>();
    }
}
