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
        PlayerData playerData = aCollider.GetComponent<PlayerData>();
        if(playerData != null)
        {
            playerData.SetInFire(true, myTime);
        }
    }
}
