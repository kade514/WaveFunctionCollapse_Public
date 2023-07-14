using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using GD.MinMaxSlider;
using Random = UnityEngine.Random;

public class AltitudeGeneratorModule : GeneratorModule
{
    public float NoiseScale;
    public AnimationCurve AltitudeCurve;

    [MinMaxSlider(0, 30)]
    public Vector2Int NumMountainChains;
    
    [Tooltip("How many tiles the mountain range should be")]
    [MinMaxSlider(0, 20)]
    public Vector2Int MountainChainLength;
    
    public override IEnumerator Run()
    {
        yield return AdjustTileAltitudeCoro();

        foreach (var keyValuePair in GridGen.TileDictionary)
        {
            keyValuePair.Value.AddModifier(ETileModifier.Flat);
        }

        yield return SpawnMountainsCoro();
    }

    private IEnumerator AdjustTileAltitudeCoro()
    {
        var count = 0;

        for (var row = 0; row < GridGen.GridHeight; row++)
        {
            for (var column = 0; column < GridGen.GridWidth; column++)
            {
                var coord = new GridGen.Coordinates(column, row);

                if (!GridGen.TileDictionary.ContainsKey(coord))
                    continue;

                var tile = GridGen.TileDictionary[coord];

                if (TileController.IsWaterTile(tile.TileDetails.TileType))
                    continue;

                var noiseX = ((float)coord.X / GridGen.GridWidth) * NoiseScale;
                var noiseY = ((float)coord.Y / GridGen.GridHeight) * NoiseScale;

                var pos = new float2(noiseX, noiseY);
                var celValue = noise.cellular(pos);

                //I'm not 100% sure, but I'm pretty certain that the x value of this is what we want.
                //The documentation provided for the Unity Mathematics package is unclear on this point.
                var altInHundreds = (int)AltitudeCurve.Evaluate(Mathf.Clamp(celValue.x, 0f, 1.0f)) * 100;

                tile.SetAltitude(altInHundreds);
            }

            count++;

            if (count <= 10)
                continue;

            count = 0;

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator SpawnMountainsCoro()
    {
        var numMountainsToSpawn = Random.Range(NumMountainChains.x, NumMountainChains.y);
        var allValidTiles =
            GridGen.TileDictionary.Where(x => !TileController.IsWaterTile(x.Value.TileDetails.TileType)).ToList();

        for (var mountainChainCount = 0; mountainChainCount < numMountainsToSpawn; mountainChainCount++)
        {
            var chainLength = Random.Range(MountainChainLength.x, MountainChainLength.y);
            var curTile = allValidTiles[Random.Range(0, allValidTiles.Count)].Value;
            curTile.AddModifier(ETileModifier.Mountain);

            var altInHundreds = (int)AltitudeCurve.Evaluate(Random.Range(0.8f,1f)) * 500;
            curTile.SetAltitude(altInHundreds);


            var direction = new GridGen.Coordinates(Random.Range(-1, 2), Random.Range(-1, 2));

            //safety check just in case it tries to not move
            if (direction.X == 0 && direction.Y == 0)
            {
                direction.X = 1;
            }

            for (var mountainCount = 0; mountainCount < chainLength; mountainCount++)
            {
                var newCoords = GridGen.GetWrappedCoordinates(new GridGen.Coordinates(curTile.Coords.X + direction.X, curTile.Coords.Y + direction.Y));
                var isWater = TileController.IsWaterTile(GridGen.TileDictionary[newCoords].TileDetails.TileType);

                //basically: If we try to make a mountain in the water, check to see if there are clear spaces above and below that coordinate instead
                if (isWater)
                {
                    var valToAdd = Random.Range(0, 2) == 0 ? 1 : -1;
                    newCoords = GridGen.GetWrappedCoordinates(new GridGen.Coordinates(newCoords.X, newCoords.Y + valToAdd));
                    isWater = TileController.IsWaterTile(GridGen.TileDictionary[newCoords].TileDetails.TileType);

                    if (isWater)
                    {
                        newCoords = GridGen.GetWrappedCoordinates(new GridGen.Coordinates(newCoords.X, newCoords.Y - valToAdd));
                        valToAdd = -valToAdd;
                        newCoords = GridGen.GetWrappedCoordinates(new GridGen.Coordinates(newCoords.X, newCoords.Y + valToAdd));
                        isWater = TileController.IsWaterTile(GridGen.TileDictionary[newCoords].TileDetails.TileType);
                    }
                    else
                    {
                        curTile = GridGen.TileDictionary[newCoords];
                        curTile.AddModifier(ETileModifier.Mountain);

                        altInHundreds = (int)AltitudeCurve.Evaluate(Random.Range(0.8f, 1f)) * 500;
                        curTile.SetAltitude(altInHundreds);

                        direction.Y = Random.Range(-1, 2);

                        if (direction.X == 0 && direction.Y == 0)
                        {
                            direction.X = 1;
                        }
                    }
                }
                else
                {
                    curTile = GridGen.TileDictionary[newCoords];
                    curTile.AddModifier(ETileModifier.Mountain);
                    altInHundreds = (int)AltitudeCurve.Evaluate(Random.Range(0.8f, 1f)) * 500;
                    curTile.SetAltitude(altInHundreds);

                    direction.Y = Random.Range(-1, 2);

                    if (direction.X == 0 && direction.Y == 0)
                    {
                        direction.X = 1;
                    }
                }

            }
        }

        yield break;
    }
}
