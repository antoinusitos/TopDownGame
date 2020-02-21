using UnityEngine;

public class GameInstance : MonoBehaviour
{
    [SerializeField]
    private int                 mySeed = -1;

    private static GameInstance _myInstance = null;

    private bool                myIsNewGame = false;

    private void Awake()
    {
        _myInstance = this;
        DontDestroyOnLoad(this);
    }

    public static GameInstance GetInstance()
    {
        return _myInstance;
    }

    public void SetSeed(int aSeed)
    {
        mySeed = aSeed;
    }

    public void SetNewGame(bool aNewState)
    {
        myIsNewGame = aNewState;
    }

    public int GetSeed()
    {
        return mySeed;
    }
}
