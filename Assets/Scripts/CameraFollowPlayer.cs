using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    private PlayerMovement  myPlayerMovement = null;
    private Transform       myTransform = null;
    private Transform       myPlayerTransform = null;
    private const float     mySpeed = 2.0f;
    private Vector3         myOffset = new Vector3(0, 5.0f, -5.0f);

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(myPlayerMovement == null)
        {
            return;
        }

        myTransform.position = Vector3.Lerp(myTransform.position, myPlayerTransform.position + myOffset, Time.deltaTime * mySpeed);
    }

    public void SetPlayerMovement(PlayerMovement aPlayerMovement)
    {
        myPlayerMovement = aPlayerMovement;
        myPlayerTransform = myPlayerMovement.transform;
    }
}
