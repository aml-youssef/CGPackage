using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public static double dotproduct(Point a, Point b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }
        //minumum point
        public static Point minumum_point(List<Point> points)
        {
            double MIN = 99999;
            int index = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < MIN)
                {
                    MIN = points[i].Y;
                    index = i;
                }
            }
            Point minY = points[index];
            return minY;
        }
        //sort by angels
        public static List<KeyValuePair<Point, double>> Calc_angels_And_sort(Line Horizontal_Line, List<Point> points,Point minY)
        {
            List<KeyValuePair<Point, double>> Sorted_Points = new List<KeyValuePair<Point, double>>();
            double crossProduct, dotProduct, radAngel, degAngel;
            //start point
            Point start_point = new Point((Horizontal_Line.End.X - Horizontal_Line.Start.X), (Horizontal_Line.End.Y - Horizontal_Line.Start.Y));
            //loop on points to sort with angels
            for (int i = 0; i < points.Count; i++)
            {
                Point tmp = new Point((points[i].X - Horizontal_Line.Start.X), (points[i].Y - Horizontal_Line.Start.Y));
                crossProduct = CGUtilities.HelperMethods.CrossProduct(start_point, tmp);
                dotProduct = dotproduct(start_point, tmp);
                //radian angel
                radAngel = Math.Atan2(dotProduct, crossProduct);
                //convert to degrees
                degAngel = (180 / Math.PI) * (radAngel);
                //add to list
                Sorted_Points.Add(new KeyValuePair<Point, double>(points[i], degAngel));
            }
            //sort the list
            Sorted_Points.Sort((x, y) => x.Value.CompareTo(y.Value));
            Sorted_Points.Add(new KeyValuePair<Point, double>(minY, 0));
            return Sorted_Points;

        }
        
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //minumun point
            Point minY = minumum_point(points);
            //intialization of the line
            Point intiPoint = new Point(minY.X + 1, minY.Y);
            Line Horizontal_Line = new Line(minY, intiPoint);
            //remove the minumim from list of points
            points.Remove(minY);
            //sorted points
            List<KeyValuePair<Point, double>> Sorted_Points = Calc_angels_And_sort(Horizontal_Line, points, minY);
            //algorithm

            //intialize stack
            Stack<Point> hull = new Stack<Point>();
            hull.Push(minY);
            hull.Push(Sorted_Points[0].Key);
            Point top, preTop;
            //looping on sorted points

            for (int i = 1; i < Sorted_Points.Count; i++)
            {
                top = hull.Pop();
                preTop = hull.Pop();
                hull.Push(preTop);
                hull.Push(top);
                Line segment = new Line(top, preTop);
                while (hull.Count > 2 && CGUtilities.HelperMethods.CheckTurn(segment, Sorted_Points[i].Key) != CGUtilities.Enums.TurnType.Left)
                {
                    hull.Pop();
                    top = hull.Pop();
                    preTop = hull.Pop();
                    hull.Push(preTop);
                    hull.Push(top);
                    segment = new Line(top, preTop);
                }
                hull.Push(Sorted_Points[i].Key);
            }
            while (hull.Count > 0)
                outPoints.Add(hull.Pop());
            outPoints.RemoveAt(outPoints.Count - 1);
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
