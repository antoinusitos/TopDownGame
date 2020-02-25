using UnityEngine;

public class PlayerSide : MonoBehaviour
{
    public Transform    myTransform = null;

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.GetInstance().GetPauseMenuOpened())
        {
            return;
        }

        Vector2 ret = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        Vector3 dir = Vector3.one;
        if (ret.x > 0.5f)
        {
            myTransform.localScale = dir;
        }
        else if (ret.x < 0.5f)
        {
            dir.x = -1;
            myTransform.localScale = dir;
        }
    }
}
