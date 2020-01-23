using UnityEngine;

public class RockUsable : ResourceUsable
{
    public GameObject   myDropPrefab = null;

    protected override void DropResources()
    {
        base.DropResources();

        for(int i = 0; i < 3; ++i)
        {
            Transform t = Instantiate(myDropPrefab, transform.parent).transform;
            Vector3 offset = Vector2.up * Random.Range(-1.0f, 1.0f) + Vector2.right * Random.Range(-1.0f, 1.0f);
            t.position = transform.position + offset;
        }
    }
}
