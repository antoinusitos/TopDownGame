using UnityEngine;

public class Transition : MonoBehaviour
{
    public Transition   myNextTransition = null;
    public Tile         myNextRoomTile = null;

    private bool        myCanDoTransition = true;

    private void OnTriggerEnter2D(Collider2D aCollider)
    {
        if(myCanDoTransition)
        {
            PlayerMovement pm = aCollider.GetComponent<PlayerMovement>();
            if(pm != null)
            {
                Tile tile = GetComponent<Tile>();
                tile.myParentRoom.gameObject.SetActive(false);
                tile.myParentRoom.GetBiome().gameObject.SetActive(false);

                Tile tile2 = myNextTransition.GetComponent<Tile>();
                tile2.myParentRoom.gameObject.SetActive(true);
                tile2.myParentRoom.GetBiome().gameObject.SetActive(true);

                pm.transform.position = myNextRoomTile.transform.position;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D aCollider)
    {
        if (!myCanDoTransition)
        {
            PlayerMovement pm = aCollider.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                SetCanDoTransition(true);
            }
        }
    }

    public void SetCanDoTransition(bool aNewState)
    {
        myCanDoTransition = aNewState;
    }
}
