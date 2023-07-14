using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexPool : BasePool<TileController>
{
    public override TileController GetPoolableObject()
    {
        var hex = InactiveObjects.FirstOrDefault();

        if (hex == null)
        {
            SpawnItem<TileController>();
            hex = InactiveObjects.FirstOrDefault();
        }

        InactiveObjects.Remove(hex);
        ActiveObjects.Add(hex);
        return hex;
    }

    public override void ReturnPoolableObject(TileController instance)
    {
        ActiveObjects.Remove(instance);
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform);
        instance.ResetTile();
        InactiveObjects.Add(instance);
    }
}
