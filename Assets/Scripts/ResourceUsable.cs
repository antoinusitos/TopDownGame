using UnityEngine;

public class ResourceUsable : EntityData
{
    public GameObject           myFullSprite = null;
    public GameObject           myEmptySprite = null;

    protected bool              myCanDropResources = true;

    protected bool              myCanRefill = true;
    protected float             myTimeToRefill = 60;
    protected float             myCurrentTimeToRefill = 0;

    protected SpriteRenderer    mySpriteRenderer = null;

    private void Awake()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (myLife <= 0)
        {
            if (myCanDropResources)
            {
                myCanDropResources = false;
                DropResources();
            }
            else if (myCanRefill)
            {
                myCurrentTimeToRefill += Time.deltaTime;
                CheckRefillTime();
            }
        }
    }

    protected virtual void DropResources()
    {
        myEmptySprite.SetActive(true);
        myFullSprite.SetActive(false);
    }

    protected void CheckRefillTime()
    {
        if (myCurrentTimeToRefill >= myTimeToRefill)
        {
            myCurrentTimeToRefill = 0;
            myCanDropResources = true;
            myEmptySprite.SetActive(false);
            myFullSprite.SetActive(true);
            SetLife(100);
        }
    }

    public void OnEnteringRoom(float myDifferenceTime)
    {
        if (myLife <= 0)
        {
            if (myCanRefill)
            {
                myCurrentTimeToRefill += myDifferenceTime;
                CheckRefillTime();
            }
        }
    }
}
