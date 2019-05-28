using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class Line // An object containing all vital information of a line between two points
    {
        private bool vertical; // "Is this line vertical?"
        private double xintvertical; // If vertical, this is used to keep the x-intercept
        public Line()
        {
            Slope = 0;
            YInt = 0;
            vertical = false;
        }
        public Line(double slope, double yint)
        {
            Slope = slope;
            YInt = yint;
            vertical = false;
        }
        public Line(double slope, double yint, bool isVertical, double xint)
        {
            Slope = slope;
            YInt = yint;
            vertical = isVertical;
            xintvertical = xint;
        }
        public bool IsVertical(ref double x) // Outputs if the line is vertical (as a bool) and the x-int (as a double, passed by reference)
        {
            x = xintvertical;
            return vertical;
        }
        public double Slope { get; set; }
        public double YInt { get; set; }
        public override string ToString()
        {
            return $"A Line with slope {Slope} and Y-Intercept {YInt}";
        }
    }
}