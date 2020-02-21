using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private float               myTimeToFade = 0.25f;

    private bool                myFadeDirection = true;

    [SerializeField]
    private Image               myImage = null;

    private static FadeManager  myInstance = null;

    private float               myTransitionFadeSpeed = 2.0f;

    private void Awake()
    {
        myInstance = this;
    }

    public static FadeManager GetInstance()
    {
        return myInstance;
    }

    public void LaunchTransitionFade()
    {
        StartCoroutine("TransitionFade");
    }

    private IEnumerator TransitionFade()
    {
        float timer = 0;

        Color col = myImage.color;
        col.a = 1;
        myImage.color = col;

        timer = 0;

        while (timer < 1)
        {
            Color col2 = myImage.color;
            col2.a = 1 - timer;
            myImage.color = col2;

            timer += Time.deltaTime * myTransitionFadeSpeed;
            yield return null;
        }
        timer = 1;
        col = myImage.color;
        col.a = 1 - timer;
        myImage.color = col;
    }

    public void LaunchBeginTransition()
    {
        StartCoroutine("TransitionBegin");
    }

    private IEnumerator TransitionBegin()
    {
        float timer = 0;
        while(timer < 1)
        {
            Color col = myImage.color;
            col.a = 1 - timer;
            myImage.color = col;

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 1;

        Color col2 = myImage.color;
        col2.a = 1 - timer;
        myImage.color = col2;
    }

}
