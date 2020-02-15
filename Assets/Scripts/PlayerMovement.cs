using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D     myRigidbody = null;
    private float           mySpeed = 5.0f;
    private Vector2         myLastDirection = Vector3.zero;

    private Room            myCurrentRoom = null;

    // Start is called before the first frame update
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        myLastDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.Q))
        {
            myLastDirection -= Vector2.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            myLastDirection += Vector2.right;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            myLastDirection += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            myLastDirection -= Vector2.up;
        }

        myLastDirection.Normalize();
    }

    private void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + myLastDirection * mySpeed * Time.fixedDeltaTime);
    }

    public void SetCurrentRoom(Room aRoom)
    {
        myCurrentRoom = aRoom;
    }

    public Room GetCurrentRoom()
    {
        return myCurrentRoom;
    }
}
