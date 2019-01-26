using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class Circle // A circle defined by its center and radius.
    {
        public Circle(Location center, double radius)
        {
            Center = center;
            Radius = radius;
        }
        public Circle()
        {
            Center = new Location(0, 0);
            Radius = 0;
        }

        public override string ToString()
        {
            return $"A circle of center ({Center.X}, {Center.Y}) and radius {Radius}.";
        }
        public Location Center { get; set; }
        public double Radius { get; set; }
    }
}
