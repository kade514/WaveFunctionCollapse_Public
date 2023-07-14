using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ETileModifier
{
    None = 0,
    Arid = 1,
    Moist = 2,
    Rainy = 4,
    Flat = 8,
    Rocky = 16,
    Mountain = 32,
    River = 64,
    Lake = 128,
}
