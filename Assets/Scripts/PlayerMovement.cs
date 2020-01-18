using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody   myRigidbody = null;
    private float       mySpeed = 5.0f;
    private Vector3     myLastDirection = Vector3.zero;

    // Start is called before the first frame update
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        myLastDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.Q))
        {
            myLastDirection -= Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            myLastDirection += Vector3.right;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            myLastDirection += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            myLastDirection -= Vector3.forward;
        }

        myLastDirection.Normalize();
    }

    private void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + myLastDirection * mySpeed * Time.deltaTime);
    }
}
