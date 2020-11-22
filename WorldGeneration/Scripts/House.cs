using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class House
{
    public House[] connectedHouses;
    public Vector2Int position = new Vector2Int(0, 0);
    public Vector2Int dimensions = new Vector2Int(1, 1);
    public float buffer;

    public Vector2Int ToInt2(Vector2 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }

    float BufferCalc(float dimsize)
    {
        return dimsize / 2 + 3;
    }
    float BufferCalc()
    {
        return dimensions.magnitude / 2 + 3;
    }
    public House()
    {
        connectedHouses = new House[2];
        dimensions = new Vector2Int(1, 1);
        buffer = BufferCalc(dimensions.magnitude);
    }
    public House(Vector2Int size)
    {
        connectedHouses = new House[2];
        dimensions = size;
        buffer = BufferCalc(dimensions.magnitude);
    }
    public House(Vector2Int size, House[] connections)
    {
        connectedHouses = connections;
        dimensions = size;
        buffer = BufferCalc(dimensions.magnitude);
    }
    public House(int[] roomSize)
    {
        connectedHouses = new House[2];
        dimensions = new Vector2Int(Random.Range(roomSize[0], roomSize[1]), Random.Range(roomSize[0], roomSize[1])); ;
        buffer = BufferCalc(dimensions.magnitude);
    }
    public House(int[] roomSize, House[] connections)
    {
        connectedHouses = connections;
        dimensions = new Vector2Int(Random.Range(roomSize[0], roomSize[1]), Random.Range(roomSize[0], roomSize[1]));
        buffer = BufferCalc(dimensions.magnitude);
    }
    //finnur hvort línan sem fer á milli tveggja húsa fer inn í húsið lárétt eða lóðrétt.
    public bool Verticality(Vector2 exitAngle, Vector2 dimensions)
    {
        Vector2 angle = dimensions / exitAngle;
        return Mathf.Abs(angle.x) < Mathf.Abs(angle.y);
    }
    //finna þann stað þar sem exitangle sker vegg hússins
    public Vector2 DetExitPoint(Vector2 exitAngle, Vector2 houseDimensions)
    {
        Vector2 exitPoint = default;
        Vector2 scaledBox = houseDimensions / exitAngle / 2;
        if (Mathf.Abs(scaledBox.y) > Mathf.Abs(scaledBox.x))
        {
            exitPoint.y = exitAngle.y * Mathf.Abs(houseDimensions.x / exitAngle.x);
            exitPoint.x = houseDimensions.x * Mathf.Sign(exitAngle.x);
        }
        else
        {
            exitPoint.y = houseDimensions.y * Mathf.Sign(exitAngle.y);
            exitPoint.x = exitAngle.x * Mathf.Abs(houseDimensions.y / exitAngle.y);
        }
        return exitPoint / 2;
    }
    public Vector2 DetExitPoint(Vector2 exitAngle)
    {
        return DetExitPoint(exitAngle, this.dimensions);
    }
    //finnur þann reit þar sem exitangle sker vegg hússins
    public Vector2Int DetExitTile(Vector2 exitAngle, Vector2Int roomDimensions)
    {
        exitAngle.Normalize();
        Vector2Int exitPoint = default;

        Vector2Int scaledBox = ToInt2(roomDimensions / exitAngle / 2);
        if (scaledBox.y - 1 > scaledBox.y) scaledBox.y += 1;
        if (scaledBox.x - 1 > scaledBox.x) scaledBox.x += 1;
        if (Mathf.Abs(scaledBox.y) > Mathf.Abs(scaledBox.x))
        {
            exitPoint.y = (int)(exitAngle.y * Mathf.Abs(roomDimensions.x / exitAngle.x) / 2);
            exitPoint.x = (int)(roomDimensions.x * Mathf.Sign(exitAngle.x) / 2);
        }
        else
        {
            exitPoint.y = (int)(roomDimensions.y * Mathf.Sign(exitAngle.y) / 2);
            exitPoint.x = (int)(exitAngle.x * Mathf.Abs(roomDimensions.y / exitAngle.y) / 2);
        }
        return exitPoint;
    }
    public Vector2Int DetExitTile(Vector2 exitAngle)
    {
        return DetExitTile(exitAngle, this.dimensions);
    }
    public Vector2Int DetRoomDimensions(int[] roomSize)
    {
        return new Vector2Int(Random.Range(roomSize[0], roomSize[1]), Random.Range(roomSize[0], roomSize[1]));
    }
    public bool Verticality(Vector2 target)
    {
        Vector2 roomDiff = target - this.position;
        Vector2 angle = (this.dimensions / roomDiff.normalized);
        return Mathf.Abs(angle.x) < Mathf.Abs(angle.y);
    }
}

/*
 * /// <summary>
    /// Determine whether the side of a given rectangle that intersects the line between the two given points is vertical
    /// </summary>
    /// <param name="target"></param>
    /// <param name="position"></param>
    /// <param name="dimensions"></param>
    /// <returns>bool Verticality </returns>
    private bool Verticality(Vector2 target, Vector2 position, Vector2 dimensions)
    {
        Vector2 roomDiff = target - position;
        Vector2 angle = (dimensions / roomDiff.normalized);
        return Mathf.Abs(angle.x) < Mathf.Abs(angle.y);
    }
    //finnur hvort línan sem fer á milli tveggja húsa fer inn í húsið lárétt eða lóðrétt.
    private bool Verticality(Vector2 exitAngle, Vector2 dimensions)
    {
        Vector2 angle = dimensions / exitAngle;
        return Mathf.Abs(angle.x) < Mathf.Abs(angle.y);
    }
    //finna þann stað þar sem exitangle sker vegg hússins
    private Vector2 DetExitPoint(Vector2 exitAngle, Vector2 roomDimensions)
    {
        Vector2 exitPoint = default;
        Vector2 scaledBox = roomDimensions / exitAngle / 2;
        if (Mathf.Abs(scaledBox.y) > Mathf.Abs(scaledBox.x))
        {
            exitPoint.y = exitAngle.y * Mathf.Abs(roomDimensions.x / exitAngle.x);
            exitPoint.x = roomDimensions.x * Mathf.Sign(exitAngle.x);
        }
        else
        {
            exitPoint.y = roomDimensions.y * Mathf.Sign(exitAngle.y);
            exitPoint.x = exitAngle.x * Mathf.Abs(roomDimensions.y / exitAngle.y);
        }
        return exitPoint / 2;
    }
    //finnur þann reit þar sem exitangle sker vegg hússins
    private Vector2Int DetExitTile(Vector2 exitAngle, Vector2Int roomDimensions)
    {
        exitAngle.Normalize();
        Vector2Int exitPoint = default;
        
        Vector2Int scaledBox = ToInt2(roomDimensions / exitAngle / 2);
        if (scaledBox.y - 1 > scaledBox.y) scaledBox.y += 1;
        if (scaledBox.x - 1 > scaledBox.x) scaledBox.x += 1;
        if (Mathf.Abs(scaledBox.y) > Mathf.Abs(scaledBox.x))
        {
            exitPoint.y = (int)(exitAngle.y * Mathf.Abs(roomDimensions.x / exitAngle.x) / 2);
            exitPoint.x = (int)(roomDimensions.x * Mathf.Sign(exitAngle.x) / 2);
        }
        else
        {
            exitPoint.y = (int)(roomDimensions.y * Mathf.Sign(exitAngle.y) / 2);
            exitPoint.x = (int)(exitAngle.x * Mathf.Abs(roomDimensions.y / exitAngle.y) / 2);
        }
        return exitPoint;
    }
    private Vector2Int DetRoomDimensions(int[] roomSize)
    {
        return new Vector2Int(Random.Range(roomSize[0], roomSize[1]), Random.Range(roomSize[0], roomSize[1]));
    }
 * 
 **/
