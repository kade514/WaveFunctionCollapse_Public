using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSprite : MonoBehaviour, IPoolable
{
    public SpriteRenderer SpriteRenderer;

    public void SetSprite(TileSpriteAsset.SpriteAsset spriteAsset, Transform parent = null)
    {
        SpriteRenderer.sprite = spriteAsset.Sprite;
        SpriteRenderer.color = spriteAsset.SpriteColor;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public void Release()
    {
        SpritePool.Instance.ReturnPoolableObject(this); 
    }
}
