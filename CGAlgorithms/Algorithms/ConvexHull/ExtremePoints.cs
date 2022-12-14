using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        int[] Not_Extrem_Point;
        public int avilable_tringle(int a, int b, int c, List<Point> points)
        {
            Line l1 = new Line(points[a], points[b]);
            Line l2 = new Line(points[b], points[c]);
            Line l3 = new Line(points[c], points[a]);

            double ab = Math.Sqrt(Math.Pow((l1.End.Y - l1.Start.Y), 2) + Math.Pow((l1.End.X - l1.Start.X), 2));
            double bc = Math.Sqrt(Math.Pow((l2.End.Y - l2.Start.Y), 2) + Math.Pow((l2.End.X - l2.Start.X), 2));
            double ca = Math.Sqrt(Math.Pow((l3.End.Y - l3.Start.Y), 2) + Math.Pow((l3.End.X - l3.Start.X), 2));

            if (bc + ca <= ab) { return c; }
            if (ab + bc <= ca) { return b; }
            if (ca + ab <= bc) { return a; }
            //the same
            //if (HelperMethods.PointOnSegment(points[c], points[a], points[b])) { return c; }
            //if (HelperMethods.PointOnSegment(points[b], points[a], points[c])) { return b; }
            //if (HelperMethods.PointOnSegment(points[a], points[b], points[c])) { return a; }
            return -5000000;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            List<Point> extrem_points = new List<Point>();
            if (points.Count == 1 || points.Count == 2)
            {
                extrem_points = points;
            }
            else
            {
                Not_Extrem_Point = new int[(points.Count)];
                for (int a = 0; a < points.Count; a++)//n
                {
                    for (int b = a + 1; b < points.Count; b++)//n
                    {
                        for (int c = b + 1; c < points.Count; c++)//n
                        {
                            //we must check if this point contribute polygone with allowed lines 
                            //من الاخر لازم اشوف الاضلاع دي تصلخ تكون اضلاع مثلث والا لا عندي حاجه بتقول لازم يكون مجموع اي ضلعين في المثلث اكبر من الضلع التالت 
                            if (avilable_tringle(a, b, c, points) == c) { Not_Extrem_Point[c] += 1; continue; }//true 
                            else if (avilable_tringle(a, b, c, points) == b) { Not_Extrem_Point[b] += 1; break; }//true
                            else if (avilable_tringle(a, b, c, points) == a) { Not_Extrem_Point[a] += 1; break; }//true
                            for (int d = 0; d < points.Count; d++)
                            {
                                if (d != a && d != b && d != c)
                                {
                                    if (HelperMethods.PointInTriangle(points[d], points[a], points[b], points[c]) == Enums.PointInPolygon.Inside)
                                    {
                                        Not_Extrem_Point[d] += 1;//true
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < points.Count; i++)
                    if (Not_Extrem_Point[i] >= 1)//any point 0 is extreme//to understand make else 
                        continue;
                    else
                    {
                        if (!extrem_points.Contains(points[i]))
                            extrem_points.Add(points[i]);
                    }
            }
            outPoints = extrem_points;

        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
