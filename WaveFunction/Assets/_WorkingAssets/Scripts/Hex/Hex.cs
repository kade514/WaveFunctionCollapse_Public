using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex
{
    //Q+R+S must = 0
    //S = -(Q+R)

    public readonly int Q; //Column
    public readonly int R; //Row
    public readonly int S; //Sum

    public static float WidthMultiplier = Mathf.Sqrt(3f) / 2f;

    public Hex(int q, int r)
    {
        Q = q;
        R = r;
        S = -(Q + R);
    }

    float radius = 1f;

    public Vector3 Position()
    {
        //Check out this for more info https://www.redblobgames.com/grids/hexagons/

        //need to swap R/2 

        var shouldOffset = R % 2 == 0;
        var offsetAmount = 0f;

        if (shouldOffset)
            offsetAmount = HexWidth() / 2f;

        return new Vector3((HexHorizontalSpacing() * Q) + offsetAmount, 0, HexVerticalSpacing() * R);
    }

    public float HexHeight()
    {
        return radius * 2f;
    }

    public float HexWidth()
    {
        return HexHeight() * WidthMultiplier;
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }
    public float HexHorizontalSpacing()
    {
        return HexWidth();
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numRows, float numColumns)
    {
        var mapWidth = numColumns * HexHorizontalSpacing();
        var position = Position();
        
        var howManyWidthsFromCamera = Mathf.Round((position.x - cameraPosition.x) / mapWidth);
        var howManyWidthToFix = (int)howManyWidthsFromCamera;

        position.x -= howManyWidthToFix * mapWidth;

        return position;
    }

}
