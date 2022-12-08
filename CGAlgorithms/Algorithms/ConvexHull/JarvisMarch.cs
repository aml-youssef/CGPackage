using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Point StartPointOnHull = points.OrderBy(point => point.X).FirstOrDefault();//minim x
            Point Endpoint;
            while (true)
            {
                outPoints.Add(StartPointOnHull);
                Endpoint = points[0];

                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 0)
                        continue;
                    Line l1 = new Line(StartPointOnHull, points[i]);
                    Line l2 = new Line(StartPointOnHull, Endpoint);
                    Point v1 = HelperMethods.GetVector(l1);
                    Point v2 = HelperMethods.GetVector(l2);
                    bool check = v1.Magnitude() > v2.Magnitude();//ab>bc for example
                    bool check_turn = HelperMethods.CheckTurn(new Line(StartPointOnHull, Endpoint), points[i]) == Enums.TurnType.Left;
                    bool check_colinear = HelperMethods.CheckTurn(new Line(StartPointOnHull, points[i]), Endpoint) == Enums.TurnType.Colinear && check;
                    ////use from 28 to 32 in colinear distance to nknow if point in or out 
                    if ((check_turn) || (check_colinear))
                    {
                        Endpoint = points[i];
                    }
                }

                StartPointOnHull = Endpoint;
                if (Endpoint == outPoints.FirstOrDefault())
                    break;
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}