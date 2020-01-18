using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform lol = null;

    private PlayerMovement  myPlayerMovement = null;
    private Transform       myTransform = null;
    private Transform       myCameraOffsetTransform = null;
    private Transform       myPlayerTransform = null;
    private const float     mySpeed = 2.0f;
    public Vector3          myOffset = new Vector3(0, 5.0f, -3.0f);
    public Vector3          myMouseCameraOffset = new Vector3(0, 5, -3);
    private Camera          myCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        myCamera = GetComponentInChildren<Camera>();
        myCameraOffsetTransform = myCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(myPlayerMovement == null)
        {
            return;
        }

        /*RaycastHit hit;
        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 pos = hit.point - myPlayerTransform.position;
            pos.y = 0;

            float dist = Vector3.Distance(hit.point, myPlayerTransform.position);
            if(dist > 3)
            {
                pos = myPlayerTransform.position + Vector3.zero;
            }
            else
            {
                pos = myPlayerTransform.position + (hit.point - myPlayerTransform.position).normalized * 3;
            }

            pos.y = 0;

            lol.transform.position = pos;

            myCameraOffsetTransform.position = Vector3.Lerp(myCameraOffsetTransform.position, pos + myMouseCameraOffset, Time.deltaTime * mySpeed);
        }
        else
        {
            myCameraOffsetTransform.localPosition = Vector3.Lerp(myCameraOffsetTransform.localPosition, Vector3.zero, Time.deltaTime * mySpeed);
        }*/

        myTransform.position = Vector3.Lerp(myTransform.position, myPlayerTransform.position + myOffset, Time.deltaTime * mySpeed);
    }

    public void SetPlayerMovement(PlayerMovement aPlayerMovement)
    {
        myPlayerMovement = aPlayerMovement;
        myPlayerTransform = myPlayerMovement.transform;
    }
}
