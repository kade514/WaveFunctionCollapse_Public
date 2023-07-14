using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpritePool : BasePool<TileSprite>
{
    public override TileSprite GetPoolableObject()
    {
        var tileSprite = InactiveObjects.FirstOrDefault();

        if (tileSprite == null)
        {
            SpawnItem<TileSprite>();
            tileSprite = InactiveObjects.FirstOrDefault();
        }

        InactiveObjects.Remove(tileSprite);
        ActiveObjects.Add(tileSprite);
        return tileSprite;
    }

    public override void ReturnPoolableObject(TileSprite instance)
    {
        ActiveObjects.Remove(instance);
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform);
        instance.SpriteRenderer.color = Color.white;
        InactiveObjects.Add(instance);
    }
}
