using UnityEngine;

public enum StateTypes
{
    FIRE,
    WATER,
    POISON
}

public class StateChanger : MonoBehaviour
{
    public StateTypes   myStateType = StateTypes.FIRE;
    public float        myTime = 3;

    private void OnTriggerEnter(Collider aCollider)
    {
        PlayerStats playerStats = aCollider.GetComponent<PlayerStats>();
        if(playerStats != null)
        {
            playerStats.SetInFire(true, myTime);
        }
    }
}
