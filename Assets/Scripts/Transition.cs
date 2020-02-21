using UnityEngine;

public class Transition : MonoBehaviour
{
    public Transition   myNextTransition = null;
    public Tile         myNextRoomTile = null;

    private bool        myCanDoTransition = true;
    private float       myTransitionTimeAvailable = 0;

    private void OnTriggerEnter2D(Collider2D aCollider)
    {
        if(myCanDoTransition)
        {
            PlayerMovement playerMovement = aCollider.GetComponentInParent<PlayerMovement>();
            if(playerMovement != null)
            {
                Tile tile = GetComponent<Tile>();
                Room actualRoom = tile.myParentRoom;

                Tile tile2 = myNextTransition.GetComponent<Tile>();
                myNextTransition.SetCanDoTransition(false);
                Room nextRoom = tile2.myParentRoom;

                playerMovement.transform.position = myNextRoomTile.transform.position;

                nextRoom.gameObject.SetActive(true);
                nextRoom.OnEnteringRoom();

                actualRoom.OnLeavingRoom();
                actualRoom.gameObject.SetActive(false);

                FindObjectOfType<WorldGeneration>().SetCurrentActiveBiome(nextRoom.GetBiome());

                if(actualRoom.GetBiome() != nextRoom.GetBiome())
                {
                    actualRoom.GetBiome().gameObject.SetActive(false);

                    playerMovement.GetComponentInChildren<MapUI>().UpdateBiomeOnMap();
                }

                nextRoom.GetBiome().gameObject.SetActive(true);

                playerMovement.GetComponentInChildren<MapUI>().SetRoomVisited(nextRoom.myRoomData.myX, nextRoom.myRoomData.myY, nextRoom.GetBiome());

                playerMovement.SetCurrentRoom(nextRoom);
            }
        }
    }

    private void Update()
    {
        if (!myCanDoTransition)
        {
            myTransitionTimeAvailable += Time.deltaTime;
            if(myTransitionTimeAvailable >= 0.1f)
            {
                myTransitionTimeAvailable = 0;
                myCanDoTransition = true;
            }
        }
    }

    public void SetCanDoTransition(bool aNewState)
    {
        myCanDoTransition = aNewState;
    }
}
