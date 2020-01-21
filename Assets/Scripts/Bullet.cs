using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3     myDirection = Vector3.zero;
    private float       myLife = 0.5f;
    private Transform   myTransform = null;
    private float       mySpeed = 20.0f;
    private const float myBaseSpeed = 20.0f;
    private const float mySpeedDecay = 2f;
    private const float myMinSpeed = 0.1f;

    private void Start()
    {
        myTransform = transform;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        mySpeed -= mySpeedDecay * mySpeed * Time.fixedDeltaTime;
        if (mySpeed < myMinSpeed)
        {
            mySpeed = 0; //clamp down speed so it doesnt take too long to stop
        }
        Vector3 tempPos = transform.position; //capture current position
        tempPos += myDirection * mySpeed * Time.fixedDeltaTime; //find new position
        transform.position = tempPos; //update position
    }

    public void Setup(Vector3 aDirection)
    {
        myDirection = aDirection;
        mySpeed = myBaseSpeed;
    }
}
