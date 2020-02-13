using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform            myGunPivot = null;

    private CameraFollowPlayer  myCameraFollowPlayer = null;
    private bool                myCanShoot = true;
    private bool                myMouseLeft = false;
    private float               myLastShot = 0;
    private float               myTimeBetweenShots = 0.25f;
    private Vector3             myMouseVector = Vector3.zero;
    private Vector3             myMousePos = Vector3.zero;
    private Transform           myTransform = null;
    public WeaponData           myCurrentWeapon = null;

    private void Start()
    {
        myTransform = transform;
        myCameraFollowPlayer = FindObjectOfType<CameraFollowPlayer>();

        GetMouseInput();
    }

    private void Update()
    {
        GetMouseInput();

        myCanShoot = (myLastShot + myCurrentWeapon.myTimeBetweenShots < Time.time);
        if(myCanShoot && myMouseLeft)
        {
            myCurrentWeapon.UseWeapon(myMouseVector);
            myLastShot = Time.time;
            //myCameraFollowPlayer.Shake((myTransform.position - myCurrentWeapon.myBulletSpawnPos.position), 1.5f, 0.05f);
        }

        UpdateGunPivot();
    }

    private void UpdateGunPivot()
    {
        float gunAngle = -1 * Mathf.Atan2(myMouseVector.y, myMouseVector.x) * Mathf.Rad2Deg; //find angle in degrees from player to cursor
        myGunPivot.rotation = Quaternion.AngleAxis(gunAngle, Vector3.back); //rotate gun sprite around that angle
        /*gunRend.sortingOrder = playerSortingOrder - 1; //put the gun sprite bellow the player sprite
        if (gunAngle > 0)
        { //put the gun on top of player if it's at the correct angle
            gunRend.sortingOrder = playerSortingOrder + 1;
        }*/
    }

    private void GetMouseInput()
    {
        myMouseLeft = Input.GetMouseButton(0);
        myMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //position of cursor in world
        myMousePos.z = transform.position.z;
        myMouseVector = (myMousePos - transform.position).normalized;
    }

    public void EquipWeapon(WeaponData aWeaponData)
    {
        myCurrentWeapon = aWeaponData;
    }
}
