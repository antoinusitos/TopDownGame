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

    private bool        myWantToUse = false;

    public void UseWeapon(Vector3 aMouseVector)
    {
        switch (myWeaponType)
        {
            case WeaponType.MELEE:
                {
                    myWantToUse = true;
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
}
