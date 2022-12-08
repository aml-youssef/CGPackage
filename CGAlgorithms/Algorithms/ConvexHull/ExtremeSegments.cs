using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons,
            ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            double[] directions = new double[3];
            directions[0] = 0;//right
            directions[1] = 0;//left
            directions[2] = 0;//colinear
           int size_of_points = points.Count;

            if (points.Count >= 2)
            {
                for (int i = 0; i < size_of_points; i++)
                {
                    for (int j = 0; j < size_of_points; j++)
                    {

                        directions[0] = 0;//right
                        directions[1] = 0;//left
                        directions[2] = 0;//colinear
                        //  double right = 0, left = 0, colinear = 0;
                        if (i != j)
                        {
                            for (int k = 0; k < size_of_points; k++)
                            {
                                bool point_almost_in_middle = HelperMethods.PointOnSegment(points[k], points[i], points[j]);
                                
                                if (k == i && k == j)
                                    continue;
                                else
                                {
                                    if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Right)
                                        directions[0]++;
                                    else if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Left)
                                        directions[1]++;
                                    if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Colinear&&!point_almost_in_middle)
                                        directions[2]++;
                                }
                            }
                            if (directions[2] > 0)
                                continue;
                            if (directions[0] == 0 || directions[1] == 0)
                            {//this segment contained from i and j is extrem segment
                                //outLines.Add(new Line(points[i], points[j]));
                                bool foundp1 = outPoints.Contains(points[i]);
                                bool foundp2 = outPoints.Contains(points[j]);

                                if (foundp1)
                                    continue;
                                else
                                    outPoints.Add(points[i]);
                                if (foundp2)
                                    continue;
                                else
                                    outPoints.Add(points[j]);
                            }
                           
                        }
                    }
                }

            }
            if (points.Count < 2)
                outPoints = points;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
