using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct Tile
{
    [Header("Generation Details")]
    public Material TileMaterial;
    public ETileType TileType;
    public List<ETileType> AcceptableNeighbours;
    public List<ETileType> AvailableTypes;

    [Header("Gameplay Details")] 
    public TileEnvironment GameplayDetails;
    [Space] public ETileModifier TileModifiers;

    public Tile ShallowCopy()
    {
        return (Tile)this.MemberwiseClone();
    }

    public void ClearDetails()
    {
        TileMaterial = null;
        TileType = ETileType.Undefined;
        AcceptableNeighbours = new List<ETileType>();
        AvailableTypes = new List<ETileType>();
    }
}