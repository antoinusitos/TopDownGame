using UnityEngine;

public enum TriggerPlace
{
    LEFT,
    CENTER,
    RIGHT
}

public class TriggerNextRoom : MonoBehaviour
{
    public int              myTransitionType = 0;
    public TriggerPlace     myTriggerPlace;

    private Room            myNextRoom = null;
    private Room            myActualRoom = null;

    public void SetNextRoom(Room aNextRoom)
    {
        myNextRoom = aNextRoom;
    }

    public void SetActualRoom(Room aActualRoom)
    {
        myActualRoom = aActualRoom;
    }

    private void OnTriggerEnter2D(Collider2D aCollider)
    {
        if(myNextRoom == null)
        {
            return;
        }

        PlayerMovement playerMovement = aCollider.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            myNextRoom.gameObject.SetActive(true);
            myNextRoom.OnEnteringRoom();

            if (myTransitionType == 0)
            {
                aCollider.transform.position = myNextRoom.myBottomSpawningTile.transform.position;
            }
            else if (myTransitionType == 1)
            {
                aCollider.transform.position = myNextRoom.myLeftSpawningTile.transform.position;
            }
            else if (myTransitionType == 2)
            {
                aCollider.transform.position = myNextRoom.myTopSpawningTile.transform.position;
            }
            else if (myTransitionType == 3)
            {
                aCollider.transform.position = myNextRoom.myRightSpawningTile.transform.position;
            }
            myActualRoom.OnLeavingRoom();
            myActualRoom.gameObject.SetActive(false);

            playerMovement.GetComponentInChildren<MapUI>().SetRoomVisited(myNextRoom.myRoomData.myX, myNextRoom.myRoomData.myY);
        }
    }
}
