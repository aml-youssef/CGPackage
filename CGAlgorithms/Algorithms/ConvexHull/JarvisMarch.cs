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
            int j = 0;
            Point ex = new Point(StartPointOnHull.X, StartPointOnHull.Y - 1);
            List<Point> extrem_points = new List<Point>();

            while (j < points.Count)//points numper//h
            {
                if (!extrem_points.Contains(StartPointOnHull))
                    extrem_points.Add(StartPointOnHull);

                Point Endpoint = points.FirstOrDefault();
                double Maximum_Angle = 0;
                double distance = 0;
                double largest_distance = 0;
                double all_round = 2 * Math.PI;
                Line l1 = new Line(ex, StartPointOnHull);
                Point v1 = HelperMethods.GetVector(l1);

                for (int i = 0; i < points.Count; i++)//points number//n
                {
                    Line l2 = new Line(StartPointOnHull, points[i]);
                    Point v2 = HelperMethods.GetVector(l2);
                    double cross = HelperMethods.CrossProduct(v1, v2);
                    double dot = (v1.X * v2.X) + (v1.Y * v2.Y);
                    double angle = Math.Atan2(cross, dot);
                    if (angle < 0)//-ve
                        angle = angle + (all_round);

                    distance = v2.Magnitude();

                    if (angle >= Maximum_Angle)
                    {
                        if (angle == Maximum_Angle)
                        {
                            if (distance >= largest_distance)
                            {
                                largest_distance = distance;
                                Endpoint = points[i];
                            }
                        }
                        else
                        {
                            largest_distance = distance;
                            Maximum_Angle = angle;
                            Endpoint = points[i];
                        }
                    }
                }
                outLines.Add(new Line(StartPointOnHull, Endpoint));
                polygons.Add(new Polygon(outLines));
                ex = StartPointOnHull;
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