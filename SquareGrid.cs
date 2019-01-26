using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class SquareGrid
    {
        public int Width { set; get; }
        public int Height { set; get; }
        public Location Start { set; get; }
        public Location Goal { set; get; }
        public HashSet<Location> walls = new HashSet<Location>();
        public SquareGrid(int width, int height, Location start, Location goal)
        {
            Width = width;
            Height = height;
            Start = start;
            Goal = goal;
        }
        public bool InBounds(Location id)
        {
            return 0 <= id.X && id.X < Width && 0 <= id.Y && id.Y < Height;
        }
        public bool IsWallAdjacent(Location id) // "Does this point touch a wall?"
        {
            //Check above:
            if (walls.Contains(new Location(id.X, id.Y - 1)))
            {
                return true;
            }
            //Check below:
            if (walls.Contains(new Location(id.X, id.Y + 1)))
            {
                return true;
            }
            //Check to the left:
            if (walls.Contains(new Location(id.X - 1, id.Y)))
            {
                return true;
            }
            //Check to the right:
            if (walls.Contains(new Location(id.X + 1, id.Y)))
            {
                return true;
            }
            return false;
        }
        public bool Passable(Location id)
        {
            if (walls.Contains(id))
            {
                return false;
            }
            if (IsWallAdjacent(id))
            {
                return true;
            }
            if (id.Equals(Goal) || id.Equals(Start))
            {
                return true;
            }
            return false;
        }
    }
}
