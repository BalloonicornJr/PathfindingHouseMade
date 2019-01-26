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
            if ((B.X - A.X) == 0)
            {
                return new Line(0, 0, true, A.X);
            }
            double slope = ((B.Y - A.Y) / (B.X - A.X));
            double yint = ((-slope * A.X) + A.Y);
            Line returningline = new Line(slope, yint);
            return returningline;
        }
        public static bool LineIntersectsCircle(Location A, Location B, Circle C) // Determines if the line between two Locations intersects a Circle.
        {
            Line L = LineFromPoints(A, B);
            double x = 0;
            if (L.IsVertical(ref x)) // For vertical lines
            {
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
                double rad2 = (Math.Pow(2 * C.Center.X, 2) - (4 * (Math.Pow(C.Center.X, 2) + Math.Pow(L.YInt - C.Center.Y, 2) - Math.Pow(C.Radius, 2))));
                if (rad2 >= 0)
                {
                    double negb = -(2 * C.Center.X);
                    double numeratorp = negb + Math.Sqrt(rad2);
                    double numeratorn = negb - Math.Sqrt(rad2);
                    double xval1 = numeratorp / 2;
                    double xval2 = numeratorn / 2;
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
                    return false;
                }
            }
            Console.WriteLine("Somehow, nothing happened!");
            return false;
        }
        public static void SortByDistance(Location[] points, Location goal)
        {
            //todo: Sort by distance
            //replace Location[] points with List?
        }
    }
}
