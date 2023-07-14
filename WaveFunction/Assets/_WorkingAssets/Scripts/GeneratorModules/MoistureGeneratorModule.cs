using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridGen;

public class MoistureGeneratorModule : GeneratorModule
{
    public int MoistureGainedFromWater;
    //Forces wind to drop X amount of moisture on a tile where X = target tile altitude / by this value.
    public int HeightStepMoistureDrop;
    public List<RainCheck> RainChecks;

    public override IEnumerator Run()
    {
        yield return UpdateCoastalMoisture();

        yield return CheckRainfallPatterns();

        yield return IncreaseRiverAndLakeMoisture();
    }

    private IEnumerator CheckRainfallPatterns()
    {
        RainChecks.Clear();
        RainChecks = new List<RainCheck>(GridGen.GridHeight);

        yield return WestEastChecks();
    }
    
    private IEnumerator WestEastChecks()
    {
        for (var row = 0; row < GridGen.GridHeight; row++)
        {
            var coords = new Coordinates(0, row);

            if (!GridGen.TileDictionary.ContainsKey(coords))
                continue;

            var item = GridGen.TileDictionary[coords];

            RainChecks.Add(new RainCheck(coords, TileController.IsWaterTile(item.TileDetails.TileType) ? MoistureGainedFromWater : 0));
        }

        var count = 0;

        for (var column = 0; column < GridGen.GridWidth; column++)
        {
            for (var index = RainChecks.Count - 1; index >= 0; index--)
            {
                var check = RainChecks[index];
                var newCoords = check.CurrentCoords;

                if (newCoords.X + 1 >= GridGen.GridWidth)
                    newCoords.X = 0;
                else
                    newCoords.X += 1;

                var newCheck = new RainCheck(newCoords, check.MoisturePoints);

                if (!GridGen.TileDictionary.ContainsKey(newCoords))
                    continue;

                var item = GridGen.TileDictionary[newCoords];
                
                if (TileController.IsWaterTile(item.TileDetails.TileType))
                    newCheck.MoisturePoints = MoistureGainedFromWater;
                else
                {
                    if (newCheck.MoisturePoints <= 0)
                    {
                        RainChecks[index] = newCheck;
                        continue;
                    }
                    
                    var moistureToDrop = (item.TileDetails.GameplayDetails.TileAltitude / HeightStepMoistureDrop) + 1;

                    newCheck.MoisturePoints = newCheck.MoisturePoints - moistureToDrop < 0 ? 0 : newCheck.MoisturePoints - moistureToDrop;
                    item.UpdateMoisture(moistureToDrop);
                }

                RainChecks[index] = newCheck;
            }

            count++;

            if (count <= 5)
                continue;

            count = 0;
            yield return new WaitForEndOfFrame();
        }
    }
    
    private IEnumerator UpdateCoastalMoisture()
    {
        var rowCount = 0;

        for (var y = 0; y < GridGen.GridHeight; y++)
        {
            rowCount++;

            if (rowCount >= 10)
            {
                yield return new WaitForEndOfFrame();
                rowCount = 0;
            }

            var columnCount = 0;

            for (var x = 0; x < GridGen.GridWidth; x++)
            {
                columnCount++;

                if (columnCount >= 100)
                {
                    yield return new WaitForEndOfFrame();
                    columnCount = 0;
                }

                var coord = new Coordinates
                {
                    X = x,
                    Y = y
                };

                if (!GridGen.TileDictionary.ContainsKey(coord))
                    continue;

                var item = GridGen.TileDictionary[coord];

                //we don't care about adding moisute to water tiles
                if (TileController.IsWaterTile(item.TileDetails.TileType))
                    continue;

                var tileNeighbours = GridGen.GetNeighbours(item.Coords, 1, true);
                var hasWaterNeighbour = tileNeighbours.Any(x => TileController.IsWaterTile(x.TileDetails.TileType));

                if (!hasWaterNeighbour)
                    continue;

                item.UpdateMoisture(1);
            }
        }
    }

    private IEnumerator IncreaseRiverAndLakeMoisture()
    {
        var allRiverAndLakeTiles = GridGen.TileDictionary.Where(x =>
            x.Value.TileDetails.TileModifiers.HasFlag(ETileModifier.River) ||
            x.Value.TileDetails.TileModifiers.HasFlag(ETileModifier.Lake));

        var count = 0;

        foreach (var tileKVP in allRiverAndLakeTiles)
        {
            tileKVP.Value.UpdateMoisture(1);

            if (count < 40)
            {
                count++;
            }
            else
            {
                count = 0;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
