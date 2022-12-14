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
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // double[] directions = new double[3];
            Dictionary<string, int> dir = new Dictionary<string, int>();
            dir["Right"] = 0;//right
            dir["Left"] = 0;//left
            dir["Colinear"] = 0;//colinear
            if (points.Count >= 2)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        dir["Right"] = 0;//right
                        dir["Left"] = 0;//left
                        dir["Colinear"] = 0;//colinear
                        if (i != j)
                        {
                            for (int k = 0; k < points.Count; k++)
                            {
                                bool point_almost_in_middle = HelperMethods.PointOnSegment(points[k], points[i], points[j]);
                                if (k == i && k == j)
                                    continue;
                                else
                                {
                                    if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Right)
                                        dir["Right"]++;
                                    else if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Left)
                                        dir["Left"]++;
                                    if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Colinear && !point_almost_in_middle)
                                        dir["Colinear"]++;
                                }
                            }
                            if (dir["Right"] == 0 || dir["Left"] == 0 || dir["Colinear"] > 0)
                            {
                                if (dir["Colinear"] > 0)
                                    continue;
                                else
                                {
                                    Line l = new Line(points[i], points[j]);
                                    outLines.Add(l);
                                }
                            }
                        }
                    }
                }
            }
            if (points.Count < 2)
                outPoints = points;
            Polygon pol = new Polygon(outLines);
            polygons.Add(pol);
            for (int p = 0; p < outLines.Count; p++)
            {
                if (!outPoints.Contains(outLines[p].Start))
                {
                    outPoints.Add(outLines[p].Start);
                }
                if (!outPoints.Contains(outLines[p].End))
                {
                    outPoints.Add(outLines[p].End);

                }
            }
        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
