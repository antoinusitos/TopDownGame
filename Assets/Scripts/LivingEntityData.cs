using UnityEngine;

public class LivingEntityData : EntityData
{
    protected bool myIsAlive = true;

    public bool GetIsAlive()
    {
        return myIsAlive;
    }

    public void SetIsAlive(bool aNewState)
    {
        myIsAlive = aNewState;
    }
}
