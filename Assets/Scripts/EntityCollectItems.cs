using UnityEngine;

public class EntityCollectItems : MonoBehaviour
{
    private EntityData  myEntityData = null;

    private void Awake()
    {
        myEntityData = GetComponent<EntityData>();
    }

    private void OnTriggerEnter2D(Collider2D aCollider)
    {
        Collectible c = aCollider.GetComponent<Collectible>();
        if (c != null)
        {
            myEntityData.AddToInventory(c);
            Destroy(aCollider.gameObject);
        }
    }
}
