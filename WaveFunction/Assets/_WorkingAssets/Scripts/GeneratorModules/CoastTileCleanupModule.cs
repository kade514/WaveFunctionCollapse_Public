using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridGen;

//This whole module exists since the rules allow for isolated coastal tiles to exist and it looks bad.
//2+ coastal tiles together looks good, but 1000+ random isolated tiles is annoying.
public class CoastTileCleanupModule : GeneratorModule
{
    public override IEnumerator Run()
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
                var tileNeighbours = GridGen.GetNeighbours(item.Coords, 1, true);
                var onlyOcean = tileNeighbours.All(x=>x.TileDetails.TileType == ETileType.Ocean);

                if (!onlyOcean)
                    continue;

                item.DefineTile(tileNeighbours[0].TileDetails);
            }
        }
    }

    private void ReplaceTile()
    {
        
    }
}
