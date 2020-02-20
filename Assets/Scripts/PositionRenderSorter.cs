using UnityEngine;

public class PositionRenderSorter : MonoBehaviour
{
    [SerializeField]
    private int         mySortingOrderBase = 5000;
    [SerializeField]
    private int         myOffset = 0;
    [SerializeField]
    private bool        myRunOnlyOnce = false;

    private Renderer    myRenderer = null;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        myRenderer.sortingOrder = (int)(mySortingOrderBase - transform.position.y - myOffset);
        if(myRunOnlyOnce)
        {
            Destroy(this);
        }
    }
}
