using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static GridGen;

public class TileController : MonoBehaviour, IPoolable
{
    [Header("Visual Details")]
    public MeshRenderer Renderer;
    public MeshFilter MeshFilter;
    public Material BaseTileMaterial;
    public Transform TileModifierSpriteHolder;
    public Dictionary<ETileModifier, TileSprite> TileSpritesDict;
    public TileSpriteAsset TileSpriteAsset;


    [Header("Tile Details")]
    public Tile TileDetails;
    public float width;
    public float height;
    public Coordinates Coords;
    public GridGen GridGen;
    public Hex Hex;

    [Header("Debug")]
    public bool Debug;
    public TextMeshPro DebugTextMesh;

    public void Init(Coordinates coordinates, List<ETileType> possibleTypes, GridGen gridGen)
    {
        Coords = coordinates;
        TileDetails.AvailableTypes = possibleTypes.ToList();
        TileDetails.AcceptableNeighbours = possibleTypes.ToList();
        GridGen = gridGen;
        Hex = new Hex(Coords.X, Coords.Y);
        TileSpritesDict = new Dictionary<ETileModifier, TileSprite>();
    }

    public void DefineTile(Tile newDetails)
    {
        TileDetails = newDetails.ShallowCopy();
        TileDetails.AvailableTypes.Clear();
        Renderer.material = newDetails.TileMaterial;
    }

    public void RemoveOption(ETileType typeToRemove)
    {
        TileDetails.AvailableTypes.Remove(typeToRemove);
    }

    public void UpdateNeighbours(List<ETileType> possibleTypes)
    {
        TileDetails.AcceptableNeighbours = possibleTypes.ToList();
    }

    public void Update()
    {
        var viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        var inView = viewportPos.x is >= 0f and <= 1f && viewportPos.y is >= 0f and <= 1f;

        if (!Debug)
        {
            if (inView)
            {
                if (DebugTextMesh.gameObject.activeSelf)
                    DebugTextMesh.gameObject.SetActive(false);
            }
            else if (DebugTextMesh.gameObject.activeSelf)
                DebugTextMesh.gameObject.SetActive(false);

            return;
        }

        if (inView)
        {
            if (!DebugTextMesh.gameObject.activeSelf)
                DebugTextMesh.gameObject.SetActive(true);
        }
        else if (DebugTextMesh.gameObject.activeSelf)
            DebugTextMesh.gameObject.SetActive(false);

        DebugTextMesh.text = $"Type: {TileDetails.TileType}\n" +
                             $"Temperature: {TileDetails.GameplayDetails.TileTemperature}\n" +
                             $"Moisture: {TileDetails.GameplayDetails.TileMoisture}\n" +
                             $"Altitude: {TileDetails.GameplayDetails.TileAltitude}\n";
    }
    
    public void ResetTile()
    {
        TileDetails.ClearDetails();
        Renderer.material = BaseTileMaterial;
        GridGen = null;

        var modifierList = TileSpritesDict.ToList();

        for (var i = modifierList.Count - 1; i >= 0; i--)
        {
            var item = modifierList[i];
            item.Value.Release();
            TileSpritesDict.Remove(item.Key);
        }
    }

    public void UpdatePosition()
    {
        transform.position = Hex.PositionFromCamera(Camera.main.transform.position, GridGen.GridHeight, GridGen.GridWidth
        );
    }

    public void UpdateMoisture(int moistureDelta)
    {
        TileDetails.GameplayDetails.TileMoisture += moistureDelta;

        if (TileDetails.GameplayDetails.TileMoisture < 0)
            TileDetails.GameplayDetails.TileMoisture = 0;
    }

    public void SetTemperature(float temperature)
    {
        TileDetails.GameplayDetails.TileTemperature = temperature;
    }

    public void SetAltitude(int altitude)
    {
        TileDetails.GameplayDetails.TileAltitude = altitude;
    }

    public void UpdateTileDetails(bool updateVerts = false)
    {
        if (TileDetails.GameplayDetails.TileMoisture <= TileConsts.AridMoistureAmount)
        {
            AddModifier(ETileModifier.Arid);
            RemoveModifier(ETileModifier.Moist);
            RemoveModifier(ETileModifier.Rainy);
        }
        else if (TileDetails.GameplayDetails.TileMoisture <= TileConsts.MoistMoistureAmount)
        {
            RemoveModifier(ETileModifier.Arid);
            AddModifier(ETileModifier.Moist);
            RemoveModifier(ETileModifier.Rainy);
        }
        else if (TileDetails.GameplayDetails.TileMoisture >= TileConsts.RainyMoistureAmount)
        {
            RemoveModifier(ETileModifier.Arid);
            RemoveModifier(ETileModifier.Moist);
            AddModifier(ETileModifier.Rainy);
        }

        //stops system from having mutually exclusive modifiers
        if (TileDetails.TileModifiers.HasFlag(ETileModifier.Mountain))
        {
            RemoveModifier(ETileModifier.Flat);
            RemoveModifier(ETileModifier.Rocky);
        }

        if (TileDetails.TileModifiers.HasFlag(ETileModifier.Lake))
        {
            RemoveModifier(ETileModifier.River);
        }

        //check for mountain and river and other sprites.
        UpdateTileSprites(ETileModifier.Mountain);
        UpdateTileSprites(ETileModifier.River);
        UpdateTileSprites(ETileModifier.Lake);


        //Ideally, I'd modify the tiles so that they go to a worldspace y pos determined by their altitude
        //and then adjust their hex verts to give the system a 3d nature, but so far I've been unable to figure out a good way to do it.
    }

    private void UpdateTileSprites(ETileModifier modifierToCheck)
    {
        if (TileDetails.TileModifiers.HasFlag(modifierToCheck) && !TileSpritesDict.ContainsKey(modifierToCheck))
        {
            var tileSprite = SpritePool.Instance.GetPoolableObject();
            var asset = TileSpriteAsset.TileSpriteAssets.FirstOrDefault(x => x.Modifier == modifierToCheck);
            tileSprite.SetSprite(asset, TileModifierSpriteHolder);
            tileSprite.gameObject.SetActive(true);
            TileSpritesDict.Add(modifierToCheck, tileSprite);

        }
        else if (!TileDetails.TileModifiers.HasFlag(modifierToCheck) && TileSpritesDict.ContainsKey(modifierToCheck))
        {
            var tileSprite = TileSpritesDict[modifierToCheck];
            tileSprite.Release();
            TileSpritesDict.Remove(modifierToCheck);
        }
    }

    public void AddModifier(ETileModifier modifierToAdd)
    {
        TileDetails.TileModifiers |= modifierToAdd;
    }

    public void RemoveModifier(ETileModifier modifierToRemove)
    {
        TileDetails.TileModifiers &= ~modifierToRemove;
    }
    
    public static bool IsWaterTile(ETileType tileType)
    {
        return tileType is ETileType.Ocean or ETileType.CoastalWater;
    }
}
