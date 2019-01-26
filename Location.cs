using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class Location // Basic (x,y) coordinate system
    {
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Location()
        {
            X = 0;
            Y = 0;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Equals(Location b)
        {
            return (b.X == X && b.Y == Y);
        }
        public override string ToString()
        {
            return $"({X}, {Y}";
        }
    }
}
