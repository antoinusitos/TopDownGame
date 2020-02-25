using UnityEngine;

public class PlayerData : LivingEntityData
{
    private int     myForgeLevel = 1;
    private int     myEnchantementLevel = 1;

    private bool    myIsInFire = false;
    private float   myTimeInFire = 0;
    private float   myCurrentTimeInFire = 0;

    private int     mySoulsCollected = 0;

    private void Update()
    {
        if (myIsInFire)
        {
            myCurrentTimeInFire -= Time.deltaTime;
            if (myCurrentTimeInFire <= 0)
            {
                RemoveLife(5);
                myTimeInFire--;
                if (myTimeInFire > 0)
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

    public void AddSoulsCollected(int anAmount)
    {
        mySoulsCollected += anAmount;
    }

    public void SetSoulsCollected(int anAmount)
    {
        mySoulsCollected = anAmount;
    }

    public int GetSoulsCollected()
    {
        return mySoulsCollected;
    }
}
