using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Sprite Asset", menuName = "ScriptableObjects/Assets/Tile Sprites", order = 1)]
public class TileSpriteAsset : ScriptableObject
{
    public List<SpriteAsset> TileSpriteAssets;

    [System.Serializable]
    public struct SpriteAsset
    {
        public Sprite Sprite;
        public Color SpriteColor;
        public ETileModifier Modifier;
    }
}
