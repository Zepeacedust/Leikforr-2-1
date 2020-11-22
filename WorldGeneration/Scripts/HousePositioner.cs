using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HousePositioner : MonoBehaviour
{
    //skil ekki afhverju þetta er ekki builtin, breytir vector2 í vector2int. hjálpar með tilemap
    public Vector2Int ToInt2(Vector2 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }


    //blueprint tiles
    public Tile HouseWall, Outside, Inside, Path, OutsideWall;
    public Tilemap tilemap;

    private List<House> Houses = new List<House>();
    public int HouseNum = 10;
    public int Seed = 1;
    public int[] roomSize = new int[2];

    

    //fann þetta á netinu, er notað til að finna hvert húsin eiga að fara.
    private Vector2[] FindTangentCircle(House p0, House p1, House NewRoom)
    {
        float x0 = p0.position.x;
        float y0 = p0.position.y;
        float x1 = p1.position.x;
        float y1 = p1.position.y;
        float r0 = p0.buffer + NewRoom.buffer;
        float r1 = p1.buffer + NewRoom.buffer;
        float a, dx, dy, d, h, rx, ry;
        float x2, y2;

        /* dx and dy are the vertical and horizontal distances between
         * the circle centers.
         */
        dx = x1 - x0;
        dy = y1 - y0;

        /* Determine the straight-line distance between the centers. */
        d = Mathf.Sqrt((dy * dy) + (dx * dx));


        /* 'point 2' is the point where the line through the circle
         * intersection points crosses the line between the circle
         * centers.  
         */

        /* Determine the distance from point 0 to point 2. */
        a = ((r0 * r0) - (r1 * r1) + (d * d)) / (2 * d);

        /* Determine the coordinates of point 2. */
        x2 = x0 + (dx * a / d);
        y2 = y0 + (dy * a / d);

        /* Determine the distance from point 2 to either of the
         * intersection points.
         */
        h = Mathf.Sqrt((r0 * r0) - (a * a));

        /* Now determine the offsets of the intersection points from
         * point 2.
         */
        rx = -dy * (h / d);
        ry = dx * (h / d);

        /* Determine the absolute intersection points. */
        /*
        *xi = x2 + rx;
        *xi_prime = x2 - rx;
        *yi = y2 + ry;
        *yi_prime = y2 - ry;
        */
        Vector2[] points = new Vector2[2] {
                new Vector2(x2 + rx, y2 + ry),
                new Vector2(x2 - rx, y2 - ry)
            };
        return points;
    }
    public void Start()
    {
        Random.InitState(Seed);

        // setja fyrsta herbergi niður
        Houses.Add(new House(roomSize));

        //finna stað fyrir næsta herbergi, algorythminn þerf 2 herbergi til að virka, þaðann getur það gert restina
        Vector2 dir = Random.insideUnitCircle.normalized;
        House last = Houses[0];
        House newHouse = new House(roomSize, new House[1] { last });
        newHouse.position = ToInt2(last.position + (last.buffer + newHouse.buffer) * dir);
        Houses.Add(newHouse);

        for (int i = 0; i < HouseNum - 2; i++)
        {
            //setja niður restina af herbergjunum
            int startPoint = 0;
            int p1Index = 0;
            bool ended ;
            do
            {
                ended = true;
                House p0 = Houses[startPoint];
                House p1;
                newHouse = new House(roomSize);


                do // selecting p1 so it is close enough to fit Newroom between it and p0
                {
                    if (p1Index == startPoint)
                    {
                        p1Index++;
                    }
                    p1Index %= Houses.Count;
                    p1 = Houses[p1Index];
                    p1Index++;
                    //loopið hleypir ekki út fyrr en það er búið að finna rétt hús.
                } while (p1.position.Equals(p0.position) || (p1.position - p0.position).sqrMagnitude > Mathf.Pow(p0.buffer + p1.buffer + newHouse.buffer, 2));
                

                //staðirnir sem húsið getur verið eru þeir punktar þar sem það snertir bæði p1 og p0
                Vector2[] points = FindTangentCircle(p0, p1, newHouse);
                Vector2 selpoint;

                if (++startPoint >= Houses.Count) startPoint = 0;
                //reyna að nota þann punkt sem er nær miðju
                if (points[0].sqrMagnitude < points[1].sqrMagnitude)
                {
                    selpoint = points[0];
                }
                else
                {
                    selpoint = points[1];
                }
                foreach (var Room in Houses)
                {
                    // passa að sá punktur virki fyrir Húsið
                    if ((selpoint - Room.position).sqrMagnitude < Mathf.Pow(Room.buffer + newHouse.buffer, 2))
                    {
                        ended = false;
                        break;
                    }
                }//ef það virkar ekki nota hinn punktinn
                if (ended == false)
                {
                    ended = true;
                    if (points[0].sqrMagnitude > points[1].sqrMagnitude)
                    {
                        selpoint = points[0];
                    }
                    else
                    {
                        selpoint = points[1];
                    }
                    foreach (var Room in Houses)
                    {
                        if ((selpoint - Room.position).sqrMagnitude < Mathf.Pow(Room.buffer + newHouse.buffer, 2))
                        {
                            ended = false;
                            break;
                        }
                    }
                }
                //notað til að tengja húsin saman með stígum
                newHouse.connectedHouses = new House[2] { p0, p1 };
                newHouse.position = ToInt2(selpoint);
            } while (!ended);
            Houses.Add(newHouse);
        }
        
        foreach (House house in Houses)
        {
            //passa að húsið passar inn í tilemappið
            if ( Mathf.Abs(tilemap.size.x + tilemap.origin.x) < Mathf.Abs(house.position.x) + house.dimensions.x / 2)
            {
                tilemap.size = new Vector3Int(
                    (Mathf.Abs(house.position.x) + house.dimensions.x - tilemap.origin.x) * 2 + 2,
                    tilemap.size.y, 1);
            }
            if (Mathf.Abs(tilemap.size.y + tilemap.origin.y) < Mathf.Abs(house.position.y) + house.dimensions.y / 2)
            {
                tilemap.size = new Vector3Int(tilemap.size.x,
                    (Mathf.Abs(house.position.y) + house.dimensions.y - tilemap.origin.y) * 2 + 2,
                    1);
            }

            tilemap.origin = -(tilemap.size / 2);

            if (house.connectedHouses[0] != null)
            {
                foreach (House otherHouse in house.connectedHouses)
                {
                    if (otherHouse == null)
                    {
                        break;
                    }
                    ///<summary>
                    /// Determine whether the side of a given rectangle that intersects the line between the two given points is vertical
                    /// </summary>
                    bool[] verticality = new bool[2] { house.Verticality(otherHouse.position), otherHouse.Verticality(house.position) };

                    Vector2Int[] WallPoints;

                    if (verticality[0] == verticality[1])
                    {
                        WallPoints = new Vector2Int[4];
                        //the first and last tunnel nodes are the points where the lines intersect with the exits of the rooms.
                        WallPoints[0] = house.position + house.DetExitTile(otherHouse.position - house.position);
                        WallPoints[3] = otherHouse.position + otherHouse.DetExitTile(house.position - otherHouse.position);

                        if (verticality[0])
                        {
                            //the other two nodes are the in the middle of them
                            WallPoints[1] = new Vector2Int((WallPoints[0].x + WallPoints[3].x) / 2, WallPoints[0].y);
                            WallPoints[2] = new Vector2Int((WallPoints[0].x + WallPoints[3].x) / 2, WallPoints[3].y);
                        }
                        else
                        {
                            WallPoints[1] = new Vector2Int(WallPoints[0].x, (WallPoints[0].y + WallPoints[3].y) / 2);
                            WallPoints[2] = new Vector2Int(WallPoints[3].x, (WallPoints[0].y + WallPoints[3].y) / 2);
                        }
                    }
                    else
                    {
                        WallPoints = new Vector2Int[3];

                        WallPoints[0] = house.position + house.DetExitTile(otherHouse.position - house.position);
                        WallPoints[2] = otherHouse.position + otherHouse.DetExitTile(house.position - otherHouse.position);

                        if (verticality[0] && !verticality[1])
                        {
                            WallPoints[1] = new Vector2Int(WallPoints[2].x, WallPoints[0].y);

                        }
                        else
                        {
                            WallPoints[1] = new Vector2Int(WallPoints[0].x, WallPoints[2].y);
                        }
                    }

                    Vector2Int lastPoint = default;
                    foreach (Vector2Int wallPoint in WallPoints)
                    {
                        if (lastPoint != default)
                        {
                            ///TODO: Optimize
                            Vector2Int direction = lastPoint - wallPoint;
                            direction = ToInt2(((Vector2)direction).normalized);
                            Vector2Int currentPoint = wallPoint;
                            while (!currentPoint.Equals(lastPoint))
                            {
                                if (tilemap.GetTile((Vector3Int)currentPoint) == null)
                                    tilemap.SetTile((Vector3Int)currentPoint, Path);
                                currentPoint += direction;
                            }
                        }
                        lastPoint = wallPoint;
                    }

                }
            }
            //ástæðan fyrir því að deila dimension með tvemur er að dimension er öll lengdin a fhúsinu en við þurfum bara lengdina frá miðjunni
            tilemap.BoxFill((Vector3Int)house.position, HouseWall,
                house.position.x - house.dimensions.x / 2,
                house.position.y - house.dimensions.y / 2,
                house.position.x + house.dimensions.x / 2,
                house.position.y + house.dimensions.y / 2);
            tilemap.BoxFill((Vector3Int)house.position, Inside,
                house.position.x - house.dimensions.x / 2 + 1,
                house.position.y - house.dimensions.y / 2 + 1,
                house.position.x + house.dimensions.x / 2 - 1,
                house.position.y + house.dimensions.y / 2 - 1);

        }
        //minkar minnið sem leikurinn tekur
        tilemap.CompressBounds();
        tilemap.size = new Vector3Int(tilemap.size.x + 12, tilemap.size.y + 12, 1);
        //fillir inn á milli húsa 
        for (int iteration = 0; iteration < 6; iteration++)
        {
            List<Vector3Int> newTiles = new List<Vector3Int>();
            for (int x = -tilemap.size.x; x < tilemap.size.x; x++)
            {
                for (int y = -tilemap.size.y; y < tilemap.size.y; y++)
                {
                    if (tilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                    {
                        if (tilemap.GetTile(new Vector3Int(x + 1, y, 0)) != null ||
                            tilemap.GetTile(new Vector3Int(x - 1, y, 0)) != null ||
                            tilemap.GetTile(new Vector3Int(x, y + 1, 0)) != null ||
                            tilemap.GetTile(new Vector3Int(x, y - 1, 0)) != null)
                        {
                            newTiles.Add(new Vector3Int(x, y, 0));
                        }
                    }
                }
            }
            foreach (Vector3Int newTile in newTiles)
            {
                if (iteration > 3)
                    tilemap.SetTile(newTile, OutsideWall); 
                else
                    tilemap.SetTile(newTile, Outside);
            }
        }
    }
}
