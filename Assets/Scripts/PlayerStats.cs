using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int         myForgeLevel = 1;
    private int         myEnchantementLevel = 1;

    private PlayerData  myPlayerData = null;

    private bool        myIsInFire = false;
    private float       myTimeInFire = 0;
    private float       myCurrentTimeInFire = 0;

    private void Start()
    {
        myPlayerData = GetComponent<PlayerData>();
    }

    private void Update()
    {
        if(myIsInFire)
        {
            myCurrentTimeInFire -= Time.deltaTime;
            if(myCurrentTimeInFire <= 0)
            {
                myPlayerData.RemoveLife(5);
                myTimeInFire--;
                if(myTimeInFire > 0)
                {
                    myCurrentTimeInFire = 1;
                }
                else
                {
                    myCurrentTimeInFire = 0;
                    myIsInFire = false;
                }
            }
        }
    }

    public void SetInFire(bool aNewState, float aTime)
    {
        myIsInFire = aNewState;
        myTimeInFire = aTime;
        myCurrentTimeInFire = 1;
    }
}
