using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding
{
    class Location
    {
        public Location(int x, int y) //constructor that takes in an x and y value
        {
            X = x;
            Y = y;
        }
        public Location() //default point is (0,0)
        {
            X = 0;
            Y = 0;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public override bool Equals(object obj) // overrides Equals(); checks if the x and y values are the same
        {
            if (obj == null) return false;
            Location objAsLoc = obj as Location;
            if (objAsLoc == null) return false;
            else return (objAsLoc.X == this.X && objAsLoc.Y == this.Y);
        }
        /*public bool Equals(Location other) // see above
        {
            if (other == null) return false;
            return (other.X == X && other.Y == Y);
        }*/
        public static bool operator ==(Location loc1, Location loc2) // checks if either is null, if so, return false, if not, checks if their x and y values are the same
        {
            if (ReferenceEquals(loc1, null))
            {
                return false;
            }
            if (ReferenceEquals(loc2, null))
            {
                return false;
            }
            return (loc1.X == loc2.X && loc1.Y == loc2.Y);
        }
        public static bool operator !=(Location loc1, Location loc2) // negates above
        {
            return !(loc1 == loc2);
        }
        public override string ToString() // prints in a (x,y) fasion
        {
            return $"({X}, {Y})";
        }
        public override int GetHashCode() // ¯\_(ツ)_/¯
        {
            return X ^ Y;
        }
    }
}