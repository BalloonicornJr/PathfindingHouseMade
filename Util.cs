using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class Util
    {
        public static double Distance(Location a, Location b) // Standard distance formula
        {
            if (a.Equals(b))
            {
                return 0;
            }
            double xtotal = b.X - a.X;
            double ytotal = b.Y - a.Y;
            return Math.Sqrt(Math.Pow(xtotal, 2) + Math.Pow(ytotal, 2));
        }
        public static Line LineFromPoints(Location A, Location B) // Creates a Line object from two Locations.
        {
            if((B.X - A.X) == 0)
            {
                return new Line(0, 0, true, A.X);
            }
            double slope = ((double)(B.Y - A.Y) / (double)(B.X - A.X));
            double yint = ((-slope * A.X) + A.Y);
            Line returningline = new Line(slope, yint);
            return returningline;
        }
        public static bool LineIntersectsCircle(Location A, Location B, Circle C) // Determines if the line between two Locations intersects a Circle.
        {
            Line L = LineFromPoints(A, B);
            //Console.WriteLine(L); // for debugging
            double x = 0;
            if (L.IsVertical(ref x)) // For vertical lines
            {
                //Console.WriteLine("The line is vertical!"); //for debugging
                double rad3 = Math.Pow(C.Radius, 2) - Math.Pow((x - C.Center.X), 2);
                if (rad3 >= 0)
                {
                    double yval1 = C.Center.Y + Math.Sqrt(rad3);
                    double yval2 = C.Center.Y - Math.Sqrt(rad3);
                    if ((yval1 < A.Y && yval1 > B.Y) || (yval1 < B.Y && yval1 > A.Y))
                    {
                        return true;
                    }
                    else if ((yval2 < A.Y && yval2 > B.Y) || (yval2 < B.Y && yval2 > A.Y))
                    {
                        return true;
                    }
                    else return false;
                }
                else if (rad3 < 0)
                {
                    return false;
                }
            }
            else if (L.Slope != 0) // For lines with nonzero slope
            {
                //Console.WriteLine("The slope is nonzero!"); //for debugging
                double rad1 = (Math.Pow((2 * L.Slope * (L.YInt - C.Center.Y) - (2 * C.Center.X)), 2) - (4 * (1 + Math.Pow(L.Slope, 2)) * (Math.Pow(C.Center.X, 2) + Math.Pow((L.YInt - C.Center.Y), 2) - Math.Pow(C.Radius, 2))));
                if (rad1 >= 0)
                {
                    double nb = -(2 * L.Slope * (L.YInt - C.Center.Y) - (2 * C.Center.X));
                    double numeratorp = nb + Math.Sqrt(rad1);
                    double numeratorn = nb - Math.Sqrt(rad1);
                    double twoa = 2 * (1 + Math.Pow(L.Slope, 2));
                    double xval1 = numeratorp / twoa;
                    double xval2 = numeratorn / twoa;
                    if ((xval1 < A.X && xval1 > B.X) || (xval1 < B.X && xval1 > A.X))
                    {
                        return true;
                    }
                    else if ((xval2 < A.X && xval2 > B.X) || (xval2 < B.X && xval2 > A.X))
                    {
                        return true;
                    }
                    else return false;
                }
                if (rad1 < 0)
                {
                    return false;
                }
            }
            else if (L.Slope == 0) // For lines with slope = zero
            {
                //Console.WriteLine("The slope is horizontal!");
                double rad2 = (Math.Pow(2 * C.Center.X, 2) - (4 * (Math.Pow(C.Center.X, 2) + Math.Pow(L.YInt - C.Center.Y, 2) - Math.Pow(C.Radius, 2))));
                if (rad2 >= 0)
                {
                    //Console.WriteLine("Rad2 is greater than 0!"); //for debugging
                    double negb = (2 * (double)C.Center.X);
                    double numeratorp = negb + Math.Sqrt(rad2);
                    double numeratorn = negb - Math.Sqrt(rad2);
                    double xval1 = numeratorp / 2;
                    //Console.WriteLine($"Xval1 = {xval1}"); //for debugging
                    double xval2 = numeratorn / 2;
                    //Console.WriteLine($"Xval2 = {xval2}"); //for debugging
                    if ((xval1 < A.X && xval1 > B.X) || (xval1 < B.X && xval1 > A.X))
                    {
                        return true;
                    }
                    else if ((xval2 < A.X && xval2 > B.X) || (xval2 < B.X && xval2 > A.X))
                    {
                        return true;
                    }
                    else return false;
                }
                else if (rad2 < 0)
                {
                    //Console.WriteLine("Rad2 is less than 0!"); //for debugging
                    return false;
                }
            }
            //Console.WriteLine("Somehow, nothing happened!");
            return false;
        }
        public static bool ContainsLoc(HashSet<Location> points, Location point)
        {
            foreach (Location x in points)
            {
                if (point.Equals(x))
                {
                    return true;
                }
            }
            return false;
        }
        public static int LocIndex(HashSet<Location> points, Location point)
        {
            int count = 0;
            foreach (Location x in points)
            {
                if (point.Equals(x))
                {
                    return count;
                }
                count++;
            }
            return -1;
        }
        public static void PrintLocArray(HashSet<Location> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Console.Write($"{points.ElementAt(i)} ");
            }
        }
        public static void RemovePoint(HashSet<Location> points, Location point)
        {
            foreach (Location x in points)
            {
                if (point.Equals(x))
                {
                    Location test = x;
                    points.Remove(test);
                }
            }
        }
        public static Location GetClosest(HashSet<Location> points, Location goal)
        {
            double min = Double.MaxValue;
            int place = -1;
            for (int i = 0; i < points.Count; i++)
            {
                if (Distance(points.ElementAt(i), goal) < min)
                {
                    min = Distance(points.ElementAt(i), goal);
                    place = i;
                }
            }
            return points.ElementAt(place);
        }
        public static string LocationArrayToString(Location[] arr) // returns a long string that's every Location array element put together
        {
            if (arr.Length == 0 || arr == null)
            {
                return "";
            }
            string run = "";
            foreach (Location obj in arr)
            {
                run += obj.ToString();
                run += " ";
            }
            return run;
        }
        public static int GetNodeAtLoc(List<Node> nodes, Location loc) // finds the index of the node in the list of nodes that is at a certain position. assume all nodes are unique.
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Point == loc)
                {
                    return i;
                }
            }
            return -1;
        }
        public static void CopyHashSetToArray(HashSet<Location> hash, ref Location[] arr)
        {
            if (hash.Count != arr.Length)
            {
                throw new Exception("Error in CopyHashSetToArray: The hashset and array were of different lengths.");
            }

            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            for (int i = 0; i < hash.Count; i++)
            {
                arr[i] = hash.ElementAt(i);
            }
        }
    }
}
