using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
	public Transform        myPlayer;

	private Vector3         myTarget;
	private Vector3         myMousePos;
	private Vector3         myRefVel;
	private Vector3         myShakeOffset;
    private const float     myCameraDist = 3f;
    private float           mySmoothTime = 0.2f;
	private float           myYStart = 7;
    private Vector3         myCamOffset = new Vector3(0, 0, -4);
	private float           myShakeMag;
	private float           myShakeTimeEnd;
	private Vector3         myShakeVector;
    private bool            myShaking;
	
	private void Start()
	{
		myTarget = myPlayer.position;
	}
	
	private void Update()
	{
        if (myPlayer == null)
        {
            return;
        }

		myMousePos = CaptureMousePos();
        myShakeOffset = UpdateShake();
		myTarget = UpdateTargetPos();
		UpdateCameraPosition();
	}
	
	private Vector3 CaptureMousePos()
	{
		Vector2 ret = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		ret *= 2;
		ret -= Vector2.one;
		float max = 0.9f;
		if(Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
		{
			ret = ret.normalized;
		}
		
		return ret;
	}
	
	private Vector3 UpdateTargetPos()
	{
        Vector3 mouseOffset = Vector3.zero;
        mouseOffset.x = myMousePos.x * myCameraDist;
        mouseOffset.z = myMousePos.y * myCameraDist;
        Vector3 ret = myPlayer.position + mouseOffset + myCamOffset;
        ret += myShakeOffset;
		ret.y = myYStart;
		return ret;
	}
	
	private void UpdateCameraPosition()
	{
		Vector3 tempPos;
		tempPos = Vector3.SmoothDamp(transform.position, myTarget, ref myRefVel, mySmoothTime);
		transform.position = tempPos;
	}

    public void Shake(Vector3 aDirection, float aMagnitude, float aLength)
    {
        myShaking = true;
        myShakeVector = aDirection;
        myShakeMag = aMagnitude;
        myShakeTimeEnd = Time.time + aLength;
    }

    private Vector3 UpdateShake()
    {
        if(!myShaking || Time.time > myShakeTimeEnd)
        {
            myShaking = false;
            return Vector3.zero;
        }
        Vector3 tempOffset = myShakeVector;
        tempOffset *= myShakeMag;
        return tempOffset;
    }

    public Vector3 GetMousePos()
    {
        return myMousePos;
    }
}
