using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public void DevideAndConquer(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int size = points.Count;
            if (size <= 5)
            {
                JarvisMarch alg = new JarvisMarch();

                alg.Run(points, lines, polygons, ref outPoints, ref outLines, ref outPolygons);
                outPoints.Reverse();
                return;
            }

            //points.Sort((p1, p2) => p1.X.CompareTo(p2.X));
            int median = size / 2;
            List<Point> left = new List<Point>();
            List<Point> right = new List<Point>();

            for (int i = 0; i < median; i++)
            {
                left.Add(points[i]);
            }
            for (int i = median; i < size; i++)
            {
                right.Add(points[i]);
            }
            //left part
            List<Point> left_outpoints = new List<Point>();

            DevideAndConquer(left, lines, polygons, ref left_outpoints, ref outLines, ref outPolygons);

            //right part
            List<Point> right_outpoints = new List<Point>();

            DevideAndConquer(right, lines, polygons, ref right_outpoints, ref outLines, ref outPolygons);

            //sorting polygon pionts to be counter clockwise 
            //left_outpoints = FormAntiClock_polygon(left_outlines, left_outpoints);
            //right_outpoints = FormAntiClock_polygon(right_outlines, right_outpoints);

            outPoints = Merg(left_outpoints, right_outpoints);
            return;

        }
        public List<Point> FormAntiClock_polygon(List<Line> lines, List<Point> pionts)

        {
            int i = 0;
            int size = pionts.Count;
            List<Point> anti_pionts = new List<Point>();

            while (true)
            {
                if (i == 0)
                {
                    if (anti_pionts.Count == 0)
                    {
                        anti_pionts.Add(lines[i].Start);
                        anti_pionts.Add(lines[i].End);
                    }
                }
                else if (lines[i].Start == lines[i - 1].End)
                {
                    if (!anti_pionts.Contains(lines[i].End))
                    {
                        anti_pionts.Add(lines[i].End);
                    }
                }
                if (anti_pionts.Count == pionts.Count)
                {
                    break;
                }
                i = (i + 1) % pionts.Count;
            }
            int size_anti = anti_pionts.Count();
            Double sum = 0;

            for (int j = 0; j < anti_pionts.Count; j++)
            {
                sum += (anti_pionts[(j + 1) % size_anti].X - anti_pionts[j].X) * (anti_pionts[(j + 1) % size_anti].Y + anti_pionts[j].Y);
            }
            if (sum > 0)
            {
                anti_pionts.Reverse();
            }

            return anti_pionts;

        }


        public List<int> GetStartPoints(List<Point> left, List<Point> right)
        {
            int start_left = 0, start_right = 0;
            double max = -10000, min = 10000;

            for (int i = 0; i < left.Count; i++)
            {
                if (left[i].X > max)
                {
                    max = left[i].X;
                    start_left = i;
                }
            }
            for (int i = 0; i < right.Count; i++)
            {
                if (right[i].X < min)
                {
                    min = right[i].X;
                    start_right = i;
                }
            }

            List<int> starts = new List<int>();
            starts.Add(start_left);
            starts.Add(start_right);
            return starts;
        }

        public List<int> GetUpper(List<Point> left, int start_left, List<Point> right, int start_right)
        {
            int top_left = start_left, top_right = start_right;
            bool left_mod = false, right_mod = false;
            int ffa = 0, ffb = 0;

            while (true)
            {
                left_mod = false;
                right_mod = false;
                if (left.Count == 2 && (left[0].Y == left[1].Y))
                {
                    double min = 10000;
                    for (int i = 0; i < left.Count; i++)
                    {
                        if (left[i].X < min)
                        {
                            min = left[i].X;
                            top_left = i;
                        }
                    }
                }
                else
                {
                    int co = 0;
                    while (HelperMethods.CheckTurn(new Line(right[top_right], left[top_left]), left[(top_left + 1) % left.Count]) == Enums.TurnType.Right || HelperMethods.CheckTurn(new Line(right[top_right], left[top_left]), left[(top_left + 1) % left.Count]) == Enums.TurnType.Colinear)
                    {
                        left_mod = true;
                        top_left = (top_left + 1) % left.Count;
                        if (co > left.Count)
                        {
                            /*
                            double min = 10000;
                            for (int i = 0; i < left.Count; i++)
                            {
                                if (left[i].X < min)
                                {
                                    min = left[i].X;
                                    top_left = i;
                                }
                            }
                            */
                            top_left = start_left;
                            ffa = 1;
                            break;
                        }
                        co++;
                    }
                }

                //right
                if (right.Count == 2 && (right[0].Y == right[1].Y))
                {
                    double max = -1000;
                    for (int i = 0; i < right.Count; i++)
                    {
                        if (right[i].X > max)
                        {
                            max = right[i].X;
                            top_right = i;
                        }
                    }
                }
                else
                {
                    int co = 0;
                    while (HelperMethods.CheckTurn(new Line(left[top_left], right[top_right]), right[(top_right - 1 + right.Count) % right.Count]) == Enums.TurnType.Left || HelperMethods.CheckTurn(new Line(left[top_left], right[top_right]), right[(top_right - 1 + right.Count) % right.Count]) == Enums.TurnType.Colinear)
                    {
                        right_mod = true;
                        top_right = (top_right - 1 + right.Count) % right.Count;
                        if (co > right.Count)
                        {
                            /*
                            double max = -1000;
                            for (int i = 0; i < right.Count; i++)
                            {
                                if (right[i].X > max)
                                {
                                    max = right[i].X;
                                    top_right = i;
                                }
                            }
                            */
                            top_right = start_right;
                            ffb = 1;
                            break;
                        }
                        co++;
                    }
                }
                if (left_mod == false && right_mod == false)
                {
                    break;
                }
                if (ffa == 1 || ffb == 1)
                {
                    break;
                }
            }
            List<int> upper = new List<int>();
            upper.Add(top_left);
            upper.Add(top_right);

            return upper;
        }

        public List<int> GetBottom(List<Point> left, int start_left, List<Point> right, int start_right)
        {
            int bottom_left = start_left, bottom_right = start_right;
            bool left_mod = false, right_mod = false;

            while (true)
            {
                left_mod = false;
                right_mod = false;
                int ffa = 0, ffb = 0;
                if (left.Count == 2 && (left[0].Y == left[1].Y))
                {
                    bottom_left = start_left;
                }
                else
                {
                    int co = 0;
                    while (HelperMethods.CheckTurn(new Line(right[bottom_right], left[bottom_left]), left[(bottom_left - 1 + left.Count) % left.Count]) == Enums.TurnType.Left || HelperMethods.CheckTurn(new Line(right[bottom_right], left[bottom_left]), left[(bottom_left - 1 + left.Count) % left.Count]) == Enums.TurnType.Colinear)
                    {
                        left_mod = true;
                        bottom_left = (bottom_left - 1 + left.Count) % left.Count;
                        if (co > left.Count)
                        {
                            bottom_left = start_left;
                            ffa = 1;
                            break;
                        }
                        co++;
                    }

                }
                //right
                if (right.Count == 2 && right[0].Y == right[1].Y)
                {
                    bottom_right = start_right;
                }
                else
                {
                    int co = 0;
                    while (HelperMethods.CheckTurn(new Line(left[bottom_left], right[bottom_right]), right[(bottom_right + 1) % right.Count]) == Enums.TurnType.Right || HelperMethods.CheckTurn(new Line(left[bottom_left], right[bottom_right]), right[(bottom_right + 1) % right.Count]) == Enums.TurnType.Colinear)
                    {
                        right_mod = true;
                        bottom_right = (bottom_right + 1) % right.Count;

                        if (co > right.Count)
                        {
                            bottom_right = start_right;
                            ffb = 1;
                            break;
                        }
                        co++;
                    }
                }
                if (left_mod == false && right_mod == false)
                {
                    break;
                }
                if (ffa == 1 || ffb == 1)
                {
                    break;
                }

            }
            List<int> bottoms = new List<int>();
            bottoms.Add(bottom_left);
            bottoms.Add(bottom_right);
            return bottoms;
        }

        public List<Point> Tmerg(List<Point> left, int top_left, int bottom_left, List<Point> right, int top_right, int bottom_right)
        {
            List<Point> mrg = new List<Point>();
            int l_size = left.Count, r_size = right.Count;

            int left_index = top_left;
            while (left_index != bottom_left)
            {
                mrg.Add(left[left_index]);
                left_index = (left_index + 1) % l_size;
            }
            mrg.Add(left[bottom_left]);

            int right_index = bottom_right;
            while (right_index != top_right)
            {
                mrg.Add(right[right_index]);
                right_index = (right_index + 1) % r_size;
            }
            mrg.Add(right[top_right]);
            left.Clear(); right.Clear();
            left = null; right = null;
            return mrg;
        }

        public List<Point> RemoveLinear(List<Point> merged)
        {
            List<int> remove = new List<int>();

            int count = 0, index = 1;
            while (count < merged.Count)
            {
                if (merged.Count - remove.Count <= 2)
                {
                    break;
                }
                if (HelperMethods.CheckTurn(new Line(merged[(index - 1 + merged.Count) % merged.Count], merged[index]), merged[(index + 1) % merged.Count]) == Enums.TurnType.Colinear)
                {
                    remove.Add(index);
                }
                count++;
                index = (index + 1) % merged.Count;
            }
            List<Point> r_mrg = new List<Point>();
            for (int i = 0; i < merged.Count; i++)
            {
                if (remove.Contains(i))
                {
                    continue;
                }
                r_mrg.Add(merged[i]);
            }
            return r_mrg;
        }

        public List<Point> Merg(List<Point> left, List<Point> right)
        {
            int start_left, start_right;
            List<int> starts = new List<int>();
            starts = GetStartPoints(left, right);
            start_left = starts[0];
            start_right = starts[1];

            int top_left, top_right;
            List<int> upper_points = new List<int>();
            upper_points = GetUpper(left, start_left, right, start_right);
            top_left = upper_points[0];
            top_right = upper_points[1];

            int bottom_left, bottom_right;
            List<int> bottom_points = new List<int>();
            bottom_points = GetBottom(left, start_left, right, start_right);
            bottom_left = bottom_points[0];
            bottom_right = bottom_points[1];

            List<Point> merged = new List<Point>();
            merged = Tmerg(left, top_left, bottom_left, right, top_right, bottom_right);

            List<Point> r_merged = new List<Point>();
            r_merged = RemoveLinear(merged);

            return r_merged;
        }
        public List<Point> Merge1(List<Point> left, List<Point> right)
        {
            double max = -1000000000000;
            double min = 10000000000000;
            int start_left = 0;
            int start_right = 0;
            bool left_modified = false;
            bool right_modified = false;
            int bottom_left = 0;
            int bottom_right = 0;
            int bottom_left_minus;
            int bottom_right_plus;


            for (int i = 0; i < left.Count; i++)
            {
                if (left[i].X > max)
                {
                    max = left[i].X;
                    start_left = i;
                }
            }

            for (int i = 0; i < right.Count; i++)
            {
                if (right[i].X < min)
                {
                    min = right[i].X;
                    start_right = i;
                }

            }

            int top_right = start_right;
            int top_left = start_left;
            int top_left_plus;
            int top_right_minus;
            bool isLinear_left = false;
            bool islinear_right = false;
            int num_p_on_line = 0;
            int i_minus;

            for (int i = 0; i < left.Count; i++)
            {
                i_minus = i - 1;

                if (i_minus < 0)
                {
                    i_minus += left.Count;
                }

                if (HelperMethods.CheckTurn(new Line(left[i_minus], left[i]), left[(i + 1) % left.Count]) == Enums.TurnType.Colinear)
                {
                    num_p_on_line++;
                }
            }

            if (num_p_on_line == left.Count)
            {
                isLinear_left = true;
                top_left = start_left;
                double min2 = -10000;
                for (int i = 0; i < left.Count; i++)
                {
                    if (left[i].X < min2)
                    {
                        min2 = left[i].X;
                        bottom_left = i;
                    }
                }
            }

            num_p_on_line = 0;
            for (int i = 0; i < right.Count; i++)
            {
                i_minus = i - 1;

                if (i_minus < 0)
                {
                    i_minus += right.Count;
                }

                if (HelperMethods.CheckTurn(new Line(right[i_minus], right[i]), right[(i + 1) % right.Count]) == Enums.TurnType.Colinear)
                {
                    num_p_on_line++;
                }
            }

            if (num_p_on_line == right.Count)
            {
                islinear_right = true;
                top_right = start_right;
                double max2 = 10000;
                for (int i = 0; i < right.Count; i++)
                {
                    if (left[i].X > max2)
                    {
                        max2 = right[i].X;
                        bottom_right = i;
                    }
                }
            }

            int ffa = 0, ffb = 0;
            if (!isLinear_left && !islinear_right)
            {
                top_left = start_left;
                top_right = start_right;

                bottom_left = start_left;
                bottom_right = start_right;
                while (true)
                {
                    int co = 0;
                    top_left_plus = (top_left + 1) % left.Count;
                    left_modified = false;
                    right_modified = false;

                    //update top left
                    while ((HelperMethods.CheckTurn(new Line(right[top_right], left[top_left]), left[top_left_plus]) == Enums.TurnType.Right) || (HelperMethods.CheckTurn(new Line(right[top_right], left[top_left]), left[top_left_plus]) == Enums.TurnType.Colinear))
                    {

                        left_modified = true;
                        top_left = top_left_plus;
                        top_left_plus = (top_left + 1) % left.Count;

                        if (co > left.Count)
                        {
                            top_left = start_left;
                            ffa = 1;
                            break;
                        }
                        co++;

                    }
                    co = 0;

                    top_right_minus = top_right - 1;
                    if (top_right_minus < 0)
                    {
                        top_right_minus += right.Count;
                    }
                    //update top right
                    while (HelperMethods.CheckTurn(new Line(left[top_left], right[top_right]), right[top_right_minus]) == Enums.TurnType.Left)
                    {
                        right_modified = true;
                        top_right = top_right_minus;

                        top_right_minus = top_right - 1;

                        if (top_right_minus < 0)
                        {
                            top_right_minus += right.Count;
                        }

                        if (co > right.Count)
                        {
                            top_right = start_right;
                            ffb = 1;
                            break;
                        }

                        co++;
                    }

                    if (left_modified == false && right_modified == false)
                    {
                        break;
                    }

                    if (ffa == 1 || ffb == 1)
                    {
                        break;
                    }

                }

                bottom_left = start_left;
                bottom_right = start_right;
                ffa = 0;
                ffb = 0;
                while (true)
                {
                    bottom_left_minus = bottom_left - 1;
                    left_modified = false;
                    right_modified = false;
                    int co = 0;

                    if (bottom_left_minus < 0)
                    {
                        bottom_left_minus += left.Count;
                    }
                    //update bottom left


                    bottom_right_plus = (bottom_right + 1) % right.Count;

                    //update bottom right
                    while (HelperMethods.CheckTurn(new Line(left[bottom_left], right[bottom_right]), right[bottom_right_plus]) == Enums.TurnType.Right)
                    {


                        right_modified = true;
                        bottom_right = bottom_right_plus;

                        bottom_right_plus = (bottom_right + 1) % right.Count;

                        if (co > right.Count)
                        {
                            bottom_right = start_right;
                            ffb = 1;
                            break;
                        }
                        co++;

                    }

                    if (left_modified == false && right_modified == false)
                    {
                        break;
                    }
                    if (ffa == 1 || ffb == 1)
                    {
                        break;
                    }

                }
            }
            int l_size = left.Count;
            List<Point> mergeed = new List<Point>();
            int left_idx = top_left;

            while (left_idx != bottom_left)
            {
                mergeed.Add(left[left_idx]);
                left_idx = (left_idx + 1) % l_size;
            }
            mergeed.Add(left[bottom_left]);


            int r_size = right.Count;


            int right_idx = bottom_right;


            while (right_idx != top_right)
            {
                mergeed.Add(right[right_idx]);
                right_idx = (right_idx + 1) % r_size;
            }
            mergeed.Add(right[top_right]);

            List<int> remove = new List<int>();
            int index_minus;
            int end = 0;


            for (int index = 1; end < mergeed.Count; index = (index + 1) % mergeed.Count)
            {
                if (mergeed.Count - remove.Count == 2)
                {
                    break;

                }

                index_minus = index - 1;

                if (index_minus < 0)
                {
                    index_minus += mergeed.Count;
                }

                if (HelperMethods.CheckTurn(new Line(mergeed[index_minus], mergeed[index]), mergeed[(index + 1) % mergeed.Count]) == Enums.TurnType.Colinear)
                {
                    remove.Add(index);
                }

                end++;
            }




            List<Point> R_merg = new List<Point>();

            for (int i = 0; i < mergeed.Count; i++)
            {
                if (remove.Contains(i))
                {
                    continue;
                }
                R_merg.Add(mergeed[i]);
                //  mergeed.RemoveAt(remove[i]);
            }
            return R_merg;
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points.Sort(delegate (Point p1, Point p2) { return (p1.X != p2.X) ? p1.X.CompareTo(p2.X) : p1.Y.CompareTo(p2.Y); });
            DevideAndConquer(points, lines, polygons, ref outPoints, ref outLines, ref outPolygons);
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}