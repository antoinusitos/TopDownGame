using UnityEngine;

public class TriggerNextRoom : MonoBehaviour
{
    private Room myNextRoom = null;
    private Room myActualRoom = null;
    public int myTransitionType = 0;

    public void SetNextRoom(Room aNextRoom)
    {
        myNextRoom = aNextRoom;
    }

    public void SetActualRoom(Room aActualRoom)
    {
        myActualRoom = aActualRoom;
    }

    private void OnTriggerEnter(Collider aCollider)
    {
        if(myNextRoom == null)
        {
            return;
        }

        PlayerMovement playerMovement = aCollider.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            myNextRoom.gameObject.SetActive(true);

            if(myTransitionType == 0)
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
            myActualRoom.gameObject.SetActive(false);

            playerMovement.GetComponentInChildren<MapUI>().SetRoomVisited(myNextRoom.myRoomData.myX, myNextRoom.myRoomData.myY);
        }
    }
}
