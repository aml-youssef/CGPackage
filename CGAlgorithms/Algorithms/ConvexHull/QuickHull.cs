using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    struct lineEquation
    {
        public double slope;
        public double b;
    }

    public class QuickHull : Algorithm
    {
        private lineEquation getLineEquation(Point p1, Point p2)
        {
            lineEquation equation = new lineEquation();
            double slope = (p2.Y - p1.Y) / (p2.X - p1.X);
            equation.slope = slope;
            double b = p1.Y - slope * p1.X;
            equation.b = b;
            return equation;
        }

        private double getLength(Point p1, Point p2, Point point)
        {
            lineEquation equation = getLineEquation(p1, p2);
            double perpendicularSlope, b, x, y;
            if (equation.slope == 0)
            {
                x = point.X;
                y = equation.b;
            }
            else
            {
                perpendicularSlope = -(1 / equation.slope);
                b = point.Y - perpendicularSlope * point.X;
                //intersection point
                x = (b - equation.b) / (equation.slope - perpendicularSlope);
                y = equation.slope * x + equation.b;
            }
            double distance = Math.Sqrt(Math.Pow((point.Y - y), 2) + Math.Pow((point.X - x), 2));
            return distance;
        }

        private List<Point> quickhull(List<Point> points, Point left, Point right)
        {
            double maxLength = 0;
            int maxLengthIndex = -1;
            List<Point> outList = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                Line line = new Line(left, right);
                double currentLength = getLength(left, right, points[i]);
                if (currentLength > maxLength && HelperMethods.CheckTurn(line, points[i]) == Enums.TurnType.Left)
                {
                    maxLength = currentLength;
                    maxLengthIndex = i;
                }
            }
            List<Point> leftPoints;
            List<Point> rightPoints;
            if (maxLengthIndex < 0)
            {
                outList.Add(left);
                outList.Add(right);
                return outList;
            }
            else
            {
                Point maxPoint = points[maxLengthIndex];
                points.RemoveAt(maxLengthIndex);
                leftPoints = quickhull(points, left, maxPoint);
                rightPoints = quickhull(points, maxPoint, right);
            }
            for(int i = 0; i < leftPoints.Count; i++)
            {
                rightPoints.Add(leftPoints[i]);
            }
            return rightPoints;
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons,
            ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 2)
            {
                outPoints = points;
            }
            else
            {
                //Find the extreme points in the horizontal directions.
                Point maxX = new Point(-999, 0);
                Point minX = new Point(999, 0);
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].X > maxX.X)
                    {
                        maxX = points[i];
                    }
                    if (points[i].X < minX.X)
                    {
                        minX = points[i];
                    }
                }

                outPoints.Add(minX);
                outPoints.Add(maxX);
                points.Remove(minX);
                points.Remove(maxX);

                List<Point> left = quickhull(points, minX, maxX);
                List<Point> right = quickhull(points, maxX, minX);

                for (int i = 0; i < right.Count; i++)
                {
                    left.Add(right[i]);
                }

                for (int i = 0; i < left.Count; i++)
                {
                    if(outPoints.Contains(left[i]) == false)
                    {
                        outPoints.Add(left[i]);
                    }
                }

            }
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}