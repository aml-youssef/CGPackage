using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGUtilities.DataStructures;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        double K = 1000.0;
        public Point Beee(Point p1, Point p2, Point p3)
        {
            Point B = new Point((p1.X + p2.X) / 2.0, (p1.Y + p2.Y) / 2.0);
            B.X = (B.X + p3.X) / 2.0;
            B.Y = (B.Y + p3.Y) / 2.0;
            return B;
        }
        public static double angleBetweenLines(Line line1, Line line2)
        {

            double ang2 = Math.Atan2(line2.End.Y - line2.Start.Y, line2.Start.X - line2.End.X);
            double ang1 = Math.Atan2(line1.End.Y - line1.Start.Y, line1.Start.X - line1.End.X);
            double degree = (ang1 - ang2) * (180 / Math.PI);
            if (degree < 0) degree += 360;
            return degree;

        }

        public KeyValuePair<Tuple<double, int>, Tuple<double, int>> pre_nextCalc(Point p, Point B, Line base_line, int i, CGUtilities.DataStructures.OrderedSet<Tuple<double, int>> H)
        {
            KeyValuePair<Tuple<double, int>, Tuple<double, int>> Pp = new KeyValuePair<Tuple<double, int>, Tuple<double, int>>();
            double pointangle = angleBetweenLines(base_line, new Line(B, p));
            Pp = H.DirectUpperAndLower(new Tuple<double, int>(pointangle, i));
            return Pp;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            if (points.Count > 3)
            {

                Point B = Beee(points[0], points[1], points[2]);
                //// calculating base line
                Point NB = new Point(B.X + K, B.Y);
                CGUtilities.DataStructures.OrderedSet<Tuple<double, int>> H = new CGUtilities.DataStructures.OrderedSet<Tuple<double, int>>();
                Line baseline = new Line(B, NB);
                double p0 = angleBetweenLines(baseline, new Line(B, points[0]));
                double p1 = angleBetweenLines(baseline, new Line(B, points[1]));
                double p2 = angleBetweenLines(baseline, new Line(B, points[2]));
                H.Add(new Tuple<double, int>(p0, 0));
                H.Add(new Tuple<double, int>(p1, 1));
                H.Add(new Tuple<double, int>(p2, 2));
                for (int i = 3; i < points.Count; i++)
                {
                    Point p = points[i];
                    KeyValuePair<Tuple<double, int>, Tuple<double, int>> Ppair = new KeyValuePair<Tuple<double, int>, Tuple<double, int>>();
                    double pointPangle = angleBetweenLines(baseline, new Line(B, p));
                    Ppair = H.DirectUpperAndLower(new Tuple<double, int>(pointPangle, i));
                    Tuple<double, int> Pre = Ppair.Value;
                    Tuple<double, int> Next = Ppair.Key;
                    if (Pre == null)
                        Pre = H.GetLast();
                    if (Next == null)
                        Next = H.GetFirst();
                    if (HelperMethods.CheckTurn(new Line(points[Pre.Item2], points[Next.Item2]), p) == Enums.TurnType.Right)
                    {
                        KeyValuePair<Tuple<double, int>, Tuple<double, int>> Pp = pre_nextCalc(points[Pre.Item2], B, baseline, Pre.Item2, H);
                        Tuple<double, int> newP = Pp.Value;
                        if (newP == null)
                            newP = H.GetLast();
                        while (HelperMethods.CheckTurn(new Line(p, points[Pre.Item2]), points[newP.Item2]) == Enums.TurnType.Colinear || HelperMethods.CheckTurn(new Line(p, points[Pre.Item2]), points[newP.Item2]) == Enums.TurnType.Left)
                        {
                            H.Remove(Pre);
                            Pre = newP;

                            Pp = pre_nextCalc(points[Pre.Item2], B, baseline, Pre.Item2, H);
                            newP = Pp.Value;
                            if (newP == null)
                                newP = H.GetLast();
                        }
                        KeyValuePair<Tuple<double, int>, Tuple<double, int>> Next_pair = pre_nextCalc(points[Next.Item2], B, baseline, Next.Item2, H);
                        Tuple<double, int> newN = Next_pair.Key;
                        if (newN == null)
                            newN = H.GetFirst();
                        while (HelperMethods.CheckTurn(new Line(p, points[Next.Item2]), points[newN.Item2]) == Enums.TurnType.Colinear || HelperMethods.CheckTurn(new Line(p, points[Next.Item2]), points[newN.Item2]) == Enums.TurnType.Right)
                        {
                            H.Remove(Next);
                            Next = newN;
                            Next_pair = pre_nextCalc(points[Next.Item2], B, baseline, Next.Item2, H);
                            newN = Next_pair.Key;
                            if (newN == null)
                                newN = H.GetFirst();
                        }
                        for (int j = 0; j < H.Count; j++)
                        {
                            if (HelperMethods.PointInTriangle(points[H.ElementAt(j).Item2], points[i], points[Pre.Item2], points[Next.Item2]) == Enums.PointInPolygon.Inside)
                            {
                                H.Remove(H.ElementAt(j));
                            }
                        }
                        H.Add(new Tuple<double, int>(pointPangle, i));
                    }

                }
                List<Point> l = new List<Point>();
                for (int i = 0; i < H.Count; ++i)
                {
                    l.Add(points[H.ElementAt(i).Item2]);
                }

                outPoints = l;
            }
            else
            {
                outPoints = points;
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
