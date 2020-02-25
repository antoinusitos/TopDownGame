using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool                    myMenuOpen = false;
    private GameObject              myPauseMenu = null;

    private static PauseManager     myInstance = null;

    private void Awake()
    {
        myInstance = this;
    }

    public static PauseManager GetInstance()
    {
        return myInstance;
    }

    public bool GetPauseMenuOpened()
    {
        return myMenuOpen;
    }

    public void SwitchPauseState()
    {
        if (myPauseMenu == null)
        {
            myPauseMenu = LoadingManager.GetInstance().myPauseMenu;
        }
        myMenuOpen = !myMenuOpen;

        if (myMenuOpen)
        {
            myPauseMenu.SetActive(true);
        }
        else
        {
            myPauseMenu.SetActive(false);
        }
    }
}
