using UnityEngine;

public class ResourceUsable : EntityData
{
    public Sprite               myFullSprite = null;
    public Sprite               myEmptySprite = null;

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
                if(myCurrentTimeToRefill >= myTimeToRefill)
                {
                    myCurrentTimeToRefill = 0;
                    myCanDropResources = true;
                    mySpriteRenderer.sprite = myFullSprite;
                    SetLife(100);
                }
            }
        }
    }

    protected virtual void DropResources()
    {
        mySpriteRenderer.sprite = myEmptySprite;
    }
}
