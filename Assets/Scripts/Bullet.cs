using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private SpriteRenderer  mySpriteRenderer = null;
    private Vector3         myDirection = Vector3.zero;
    private Transform       myTransform = null;
    private float           mySpeed = 20.0f;
    private const float     myBaseSpeed = 20.0f;
    private const float     mySpeedDecay = 2f;
    private const float     myMinSpeed = 0.1f;
    private bool            myDisappearing = false;

    private void Start()
    {
        myTransform = transform;
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Move();
        CheckDisappear();
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            mySpeed = 0; //stop if it hits a wall
        }
    }

    void CheckDisappear()
    {
        if (mySpeed == 0 && !myDisappearing)
        { //disappear and destroy when stopped
            myDisappearing = true; //so we dont continuelly call the coroutine
            StartCoroutine(Disappear());
        }
    }

    private IEnumerator Disappear()
    {
        float curAlpha = 1; //start at full alpha
        float disSpeed = 3f; //take 1/3 seconds to disappear
        Color disColor = mySpriteRenderer.color; //capture color to edit its alpha
        do
        {
            curAlpha -= disSpeed * Time.deltaTime; //find new alpha
            disColor.a = curAlpha; //apply alpha to color
            mySpriteRenderer.color = disColor; // apply color to bullet
            yield return null;
        } while (curAlpha > 0); //end when the bullet is transparent
        Destroy(gameObject); //get rid of bullet now that it can't be seen
    }
}
