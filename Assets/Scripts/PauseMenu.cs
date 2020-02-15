using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool        myMenuOpen = false;
    private GameObject  myPauseMenu = null;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(myPauseMenu == null)
            {
                myPauseMenu = LoadingManager.GetInstance().myPauseMenu;
            }
            myMenuOpen = !myMenuOpen;

            if(myMenuOpen)
            {
                myPauseMenu.SetActive(true);
            }
            else
            {
                myPauseMenu.SetActive(true);
            }
        }
    }
}
