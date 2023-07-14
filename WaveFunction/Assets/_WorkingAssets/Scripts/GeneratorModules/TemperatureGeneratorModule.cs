using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureGeneratorModule : GeneratorModule
{
    public AnimationCurve TemperatureCurve;

    public override IEnumerator Run()
    {
        var equatorRow = GridGen.GridHeight / 2;
        var normalisedTileDistance = 1f / (GridGen.GridHeight/2f);

        var count = 0;

        for (var i = equatorRow; i < GridGen.GridHeight; i++)
        {
            var rowRelHeight = i - equatorRow;

            AssignTempPerRow(rowRelHeight, normalisedTileDistance, i);

            count++;

            if (count < 20) 
                continue;

            count = 0;
            yield return new WaitForEndOfFrame();

        }

        count = 0;

        for (var i = equatorRow; i >= 0; i--)
        {
            var rowRelHeight = equatorRow - i;

            AssignTempPerRow(rowRelHeight, normalisedTileDistance, i);

            count++;

            if (count < 20)
                continue;

            count = 0;
            yield return new WaitForEndOfFrame();
        }
    }

    private void AssignTempPerRow(int rowRelHeight, float normalisedTileDistance, int row)
    {
        for (var column = 0; column < GridGen.GridWidth; column++)
        {
            var coord = new GridGen.Coordinates(column, row);
            var tile = GridGen.TileDictionary[coord];

            var curvePos = rowRelHeight * normalisedTileDistance;

            tile.SetTemperature(TemperatureCurve.Evaluate(curvePos));
        }
    }
}
