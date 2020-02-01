using System.Collections;
using UnityEngine;

public enum WeaponType
{
    RANGE,
    MELEE,
    MAGIC,
}

public class WeaponData : MonoBehaviour
{
    public WeaponType   myWeaponType;
    public int          myDamage = 0;
    public Bullet       myBulletPrefab = null;
    public Transform    myBulletSpawnPos = null;
    public float        myTimeBetweenShots = 0.25f;
    public float        mySwingTime = 0.2f;
    public Collider     myMeleeCollider = null;

    private bool        myCanUseWeapon = true;
    private float       myCurrentTimeToShot = 0;
    private Animation   myAnimation = null;

    private void Awake()
    {
        myAnimation = GetComponent<Animation>();
    }

    public void UseWeapon(Vector3 aMouseVector)
    {
        if(!myCanUseWeapon)
        {
            return;
        }

        switch (myWeaponType)
        {
            case WeaponType.MELEE:
                {
                    StartCoroutine("UseMelee");
                }
                break;
            case WeaponType.RANGE:
                {
                    if (myBulletPrefab != null)
                    {
                        Bullet bullet = Instantiate(myBulletPrefab, myBulletSpawnPos.position, Quaternion.identity);
                        bullet.Setup(aMouseVector);
                    }
                }
                break;
            case WeaponType.MAGIC:
                {

                }
                break;
        }
    }

    private void Update()
    {
        if(!myCanUseWeapon)
        {
            myCurrentTimeToShot += Time.deltaTime;
            if(myCurrentTimeToShot >= myTimeBetweenShots)
            {
                myCanUseWeapon = true;
                myCurrentTimeToShot = 0;
            }
        }
    }

    private IEnumerator UseMelee()
    {
        if (myAnimation != null)
            myAnimation.Play();
        myMeleeCollider.enabled = true;
        yield return new WaitForSeconds(mySwingTime);
        myMeleeCollider.enabled = false;
    }
}
