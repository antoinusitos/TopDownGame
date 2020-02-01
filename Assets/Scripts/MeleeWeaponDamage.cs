using UnityEngine;

public class MeleeWeaponDamage : MonoBehaviour
{
    public int  myDamage = 40;

    private void OnTriggerEnter2D(Collider2D aCollider)
    {
        EntityData ed = aCollider.gameObject.GetComponent<EntityData>();
        if (ed != null)
        {
            ed.RemoveLife(myDamage);
        }
    }
}
