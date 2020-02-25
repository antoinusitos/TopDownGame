using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseManager.GetInstance().SwitchPauseState();
        }
    }
}
