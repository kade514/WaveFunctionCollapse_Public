using GD.MinMaxSlider;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiverGeneratorModule : GeneratorModule
{
    [MinMaxSlider(0, 30)] 
    public Vector2Int PermittedRiverSteps;

    public LineRenderer LineRendererPrefab;

    public override IEnumerator Run()
    {
        yield return GenerateRiversCoro();
    }

    private IEnumerator GenerateRiversCoro()
    {
        var allMountains = GridGen.TileDictionary
            .Where(x => x.Value.TileDetails.TileModifiers.HasFlag(ETileModifier.Mountain)).ToList();

        foreach (var mountain in allMountains)
        {
            EstablishRiverSystem(mountain.Value);
        }

        yield return new WaitForEndOfFrame();
    }

    //BUG: Seems to randomly not generate rivers. Need to give the code another pass.
    private void EstablishRiverSystem(TileController tile)
    {
        var curTile = tile;
        curTile.AddModifier(ETileModifier.River);
        var maxRiverSteps = Random.Range(PermittedRiverSteps.x, PermittedRiverSteps.y);

        var lineRend = Instantiate(LineRendererPrefab, curTile.transform);
        lineRend.transform.localPosition = new Vector3(0, 0, -0.1f);

        var tileList = new List<Transform> { curTile.transform };

        for (var stepsLeft = maxRiverSteps - 1; stepsLeft >= 0; stepsLeft--)
        {
            var tileNeighbours = GridGen.GetNeighbours(curTile.Coords, 1, true);

            tileNeighbours = tileNeighbours.OrderBy(x => x.TileDetails.GameplayDetails.TileAltitude).ToList();

            var sameHeightTiles = new List<TileController>();
            for (var i = 0; i < tileNeighbours.Count; i++)
            {
                if (i == 0 || tileNeighbours[i].TileDetails.GameplayDetails.TileAltitude <=
                    sameHeightTiles[0].TileDetails.GameplayDetails.TileAltitude)
                {
                    sameHeightTiles.Add(tileNeighbours[i]);
                }
                else
                {
                    break;
                }
            }

            var nextTile = sameHeightTiles[Random.Range(0, sameHeightTiles.Count)];

            if (TileController.IsWaterTile(nextTile.TileDetails.TileType) ||
                nextTile.TileDetails.TileModifiers.HasFlag(ETileModifier.Lake))
            {
                tileList.Add(nextTile.transform);
                break;
            }

            if (nextTile.TileDetails.GameplayDetails.TileAltitude > curTile.TileDetails.GameplayDetails.TileAltitude)
            {
                curTile.AddModifier(ETileModifier.Lake);
                break;
            }

            curTile = nextTile;

            tileList.Add(curTile.transform);

            var useLake = stepsLeft <= 0;

            if (curTile.TileDetails.TileModifiers.HasFlag(ETileModifier.River))
            {
                useLake = false;
                stepsLeft = 0;
            }
            else if(curTile.TileDetails.TileModifiers.HasFlag(ETileModifier.Lake))
            {
                stepsLeft = 0;
            }

            curTile.AddModifier(useLake ? ETileModifier.Lake : ETileModifier.River);
        }

        lineRend.positionCount = tileList.Count;

        for (var i = 0; i < tileList.Count; i++)
        {
            lineRend.SetPosition(i, tile.transform.InverseTransformPoint(tileList[i].position) + new Vector3(0, 0, 0.1f));
        }
    }
}
