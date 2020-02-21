using UnityEngine;

public class ResourceUsable : EntityData
{
    public GameObject           myFullSprite = null;
    public GameObject           myEmptySprite = null;

    [SerializeField]
    protected bool              myCanBeCrossedWhenDestroyed = false;

    protected bool              myCanDropResources = true;

    protected bool              myCanRefill = true;
    protected float             myTimeToRefill = 60;
    protected float             myCurrentTimeToRefill = 0;

    protected SpriteRenderer    mySpriteRenderer = null;

    protected bool              myMarkAsToSave = false;

    protected Tile              myTile = null;

    private void Awake()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        myTile = GetComponent<Tile>();
    }

    private void Update()
    {
        if (myLife <= 0)
        {
            if (myCanDropResources)
            {
                myCanDropResources = false;
                DropResources();
                myMarkAsToSave = true;
            }
            else if (myCanRefill)
            {
                myCurrentTimeToRefill += Time.deltaTime;
                CheckRefillTime();
                myMarkAsToSave = true;
            }
        }
    }

    protected virtual void DropResources()
    {
        myEmptySprite.SetActive(true);
        myFullSprite.SetActive(false);

        if(myCanBeCrossedWhenDestroyed)
        {
            GetComponent<Collider2D>().isTrigger = true;
        }
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

            if (myCanBeCrossedWhenDestroyed)
            {
                GetComponent<Collider2D>().isTrigger = false;
            }
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

    public float GetCurrentTimeToRefill()
    {
        return myCurrentTimeToRefill;
    }

    public void ResetMarkToSave()
    {
        myMarkAsToSave = false;
    }

    public bool GetMarkAsToSave()
    {
        return myMarkAsToSave;
    }

    public Tile GetTile()
    {
        return myTile;
    }
}
