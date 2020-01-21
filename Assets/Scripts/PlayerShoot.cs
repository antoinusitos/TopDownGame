using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform            myBulletSpawnPos = null;
    public Bullet               myBulletPrefab = null;

    private CameraFollowPlayer  myCameraFollowPlayer = null;
    private bool                myCanShoot = true;
    private bool                myMouseLeft = false;
    private float               myLastShot = 0;
    private const float         myTimeBetweenShots = 0.25f;
    private Vector3             myMouseVector = Vector3.zero;
    private Vector3             myMousePos = Vector3.zero;

    private void Start()
    {
        myCameraFollowPlayer = FindObjectOfType<CameraFollowPlayer>();
        myBulletSpawnPos = transform;

        GetMouseInput();
    }

    private void Update()
    {
        GetMouseInput();

        myCanShoot = (myLastShot + myTimeBetweenShots < Time.time);
        if(myCanShoot && myMouseLeft)
        {
            Bullet bullet = Instantiate(myBulletPrefab, myBulletSpawnPos.position, Quaternion.identity);
            bullet.Setup(myMouseVector);
            myLastShot = Time.time;
            myCameraFollowPlayer.Shake(myMouseVector, 1.5f, 0.05f);
        }
    }

    private void GetMouseInput()
    {
        myMouseLeft = Input.GetMouseButton(0);
        myMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //position of cursor in world
        Vector3 pos = Vector3.zero;
        pos.x = myMousePos.x;
        pos.z = myMousePos.z;
        pos.y = transform.position.y;
        myMouseVector = (pos - transform.position).normalized;
    }
}
