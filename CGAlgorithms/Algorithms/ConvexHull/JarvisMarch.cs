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

            bool check_on_segment_or_out = false;
            bool check_turn = false;
            bool check_colinear = false;
            Point StartPointOnHull = points.OrderBy(point => point.X).FirstOrDefault();//minim x
            int j = 0;
            Point Endpoint = null;
            List<Point> extrem_points = new List<Point>();
            while (j < points.Count)//points numper//h
            {

                extrem_points.Add(StartPointOnHull);
                Endpoint = points.FirstOrDefault();

                for (int i = 0; i < points.Count; i++)//points number//n
                {
                    if (i == 0) continue;
                    bool equality = (StartPointOnHull == Endpoint);
                    if (equality)
                    {
                        Endpoint = points[i];
                    }
                    else if (StartPointOnHull != Endpoint)
                    {
                        check_turn = HelperMethods.CheckTurn(new Line(StartPointOnHull, Endpoint), points[i]) == Enums.TurnType.Left;
                        if (check_turn == false && StartPointOnHull != Endpoint)
                        {
                            check_colinear = HelperMethods.CheckTurn(new Line(StartPointOnHull, points[i]), Endpoint) == Enums.TurnType.Colinear;
                            if (check_colinear)
                            {
                                Line l1 = new Line(StartPointOnHull, points[i]);

                                Line l2 = new Line(StartPointOnHull, Endpoint);

                                Point v1 = HelperMethods.GetVector(l1);

                                Point v2 = HelperMethods.GetVector(l2);

                                check_on_segment_or_out = v1.Magnitude() > v2.Magnitude();//ab>bc for example
                            }
                        }
                    }


                    if ((check_turn) || (check_colinear && check_on_segment_or_out))
                    {
                        Endpoint = points[i];
                    }
                }
                outLines.Add(new Line(StartPointOnHull, Endpoint));
                StartPointOnHull = Endpoint;
                if (Endpoint == extrem_points.FirstOrDefault()) break;

                j++;
            }
            outPoints = extrem_points;






        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
