using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RainCheck
{
    [HideInInspector]
    public string name;
    public GridGen.Coordinates CurrentCoords;
    public int MoisturePoints;

    public RainCheck(int x, int y, int startingMoisture)
    {
        CurrentCoords.X = x;
        CurrentCoords.Y = y;
        MoisturePoints = startingMoisture;
        name = $"{CurrentCoords} - {MoisturePoints}";
    }

    public RainCheck(GridGen.Coordinates coords, int startingMoisture)
    {
        CurrentCoords = coords;
        MoisturePoints = startingMoisture;
        name = $"{CurrentCoords} - {MoisturePoints}";
    }
}
