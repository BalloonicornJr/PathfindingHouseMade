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
        public double Altitude { set; get; }
        public double FirstOrder { set; get; }
        public HashSet<Location> walls = new HashSet<Location>();
        public HashSet<Circle> obstacles = new HashSet<Circle>();
        public HashSet<Location> wallDistant = new HashSet<Location>();
        public SquareGrid(double altitude, int width, int height, Location start, Location goal, double firstorder)
        {
            Altitude = altitude;
            Width = width;
            Height = height;
            Start = start;
            Goal = goal;
            FirstOrder = firstorder;
        }
        public bool InBounds(Location id)
        {
            return 0 <= id.X && id.X < Width && 0 <= id.Y && id.Y < Height;
        }
        public bool IsWallAdjacent(Location id) // defunct
        {
            //Check above:
            if (Util.ContainsLoc(walls, new Location(id.X, id.Y - 1)))
            {
                return true;
            }
            //Check below:
            if (Util.ContainsLoc(walls, new Location(id.X, id.Y + 1)))
            {
                return true;
            }
            //Check to the left:
            if (Util.ContainsLoc(walls, new Location(id.X - 1, id.Y)))
            {
                return true;
            }
            //Check to the right:
            if (Util.ContainsLoc(walls, new Location(id.X + 1, id.Y)))
            {
                return true;
            }
            return false;
        }
        public void GenerateDistantPoints() //This exists to account for turning radius. Passable points are 1.25x farther from the center of the obstacle than the radius. And floor'd or ceiling'd.
        {
            foreach (Circle obstacle in obstacles)
            {
                double multiplier = 1.5;
                int posX = (int)Math.Ceiling((obstacle.Center.X + (obstacle.Radius * multiplier)));
                int negX = (int)Math.Floor((obstacle.Center.X - (obstacle.Radius * multiplier)));
                int posY = (int)Math.Ceiling((obstacle.Center.Y + (obstacle.Radius * multiplier)));
                int negY = (int)Math.Floor((obstacle.Center.Y - (obstacle.Radius * multiplier)));
                wallDistant.Add(new Location(posX, obstacle.Center.Y));
                wallDistant.Add(new Location(obstacle.Center.X, posY));
                wallDistant.Add(new Location(negX, obstacle.Center.Y));
                wallDistant.Add(new Location(obstacle.Center.X, negY));
            }
        }
        public bool Passable(Location id)
        {
            GenerateDistantPoints();
            if (!InBounds(id))
            {
                return false;
            }
            if (id.Equals(Goal) || id.Equals(Start))
            {
                return true;
            }
            if (Util.ContainsLoc(walls, id))
            {
                return false;
            }
            if (Util.ContainsLoc(wallDistant, id))
            {
                return true;
            }
            return false;
        }
        public HashSet<Location> GeneratePath() //defunct
        {
            HashSet<Location> passablePoints = new HashSet<Location>();
            HashSet<Location> been = new HashSet<Location>();
            for (int y = 0; y <= Width; y++)
            {
                for (int x = 0; x <= Height; x++)
                {
                    if (Passable(new Location(x, y)))
                    {
                        passablePoints.Add(new Location(x, y));
                        //Console.WriteLine($"Added {new Location(x, y)}!"); // For debugging
                    }
                }
            }
            Location current = Start;
            been.Add(current);
            while (!current.Equals(Goal))
            {
                //Console.WriteLine($"\nNow at {current}."); // For debugging
                HashSet<Location> consider = new HashSet<Location>();
                HashSet<Location> nextPossible = new HashSet<Location>();
                foreach (Location test in passablePoints)
                {
                    bool testIsPassable = true;
                    //Console.WriteLine($"Testing {test}!"); // For debugging
                    foreach (Location wall in walls)
                    {
                        if (Util.LineIntersectsCircle(current, test, new Circle(wall, 0.5)))
                        {
                            testIsPassable = false;
                        }
                    }
                    Console.WriteLine($"{test} = {testIsPassable}");
                    if (testIsPassable && !Util.ContainsLoc(been, test))
                    {
                        consider.Add(test);
                    }
                }
                if (consider.Count == 0)
                {
                    Console.WriteLine("No path found!");
                    break;
                }
                been.Add(Util.GetClosest(consider, Goal));
                current = Util.GetClosest(consider, Goal);
                //Console.WriteLine($"Moving to {current}!"); // For debugging
            }
            return been;
        }
        public HashSet<Location> GetShortestPath()
        {
            CopyObstaclesToWalls();
            /* This was replaced by GenerateNodes. */
            List<Node> nodes = GenerateNodes();
            nodes.Sort();
            /*foreach (Node n in nodes)
            {
                Console.WriteLine($"{n}");
            }*/ // For debugging
            if (Start == Goal)
            {
                Console.WriteLine("Start and goal are equal!");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                return new HashSet<Location>();
            }
            Node current = nodes[0]; //assume the start node is always nodes[0]
            //Console.WriteLine($"{nodes[0]}");
            while (!current.Visited && current.Point != Goal)
            {
                for (int i = 0; i < current.Attached.Length; i++)
                {
                    int p = Util.GetNodeAtLoc(nodes, current.Attached[i]);
                    double distance = Util.Distance(current.Point, current.Attached[i]);
                    if (nodes[p].Cost > (current.Cost + distance) && !nodes[p].Visited)
                    {
                        nodes[p].Cost = (current.Cost + distance);
                        nodes[p].Path.Clear();
                        foreach (Location loc in current.Path)
                        {
                            //Console.WriteLine($"Adding a point at {loc}!");
                            nodes[p].Path.Add(loc);
                        }
                        nodes[p].Path.Add(current.Point);
                        //Console.WriteLine($"The node at location {nodes[p].Point} now has a cost of {nodes[p].Cost}."); // For debugging
                    }
                }
                current.Visited = true;
                nodes[Util.GetNodeAtLoc(nodes, current.Point)] = current;
                nodes.Sort();
                current = nodes[0];
                int index = 0;
                if (current.Visited)
                {
                    //Console.WriteLine("The next candidate was visted already!");
                    while (current.Visited)
                    {
                        index++;
                        if (index >= nodes.Count) break;
                        current = nodes[index];
                    }
                }
                //Console.WriteLine($"Now considering the node at {current.Point}."); // For debugging
            }
            //Console.WriteLine("The optimal path is:");
            HashSet<Location> been = new HashSet<Location>();
            if(Util.GetNodeAtLoc(nodes, Goal) >= nodes.Count || Util.GetNodeAtLoc(nodes, Goal) < 0)
            {
                Console.WriteLine("Path incomputable:");
                if (!Passable(Goal))
                {
                    Console.WriteLine("Goal not a passable point!");
                }
                else
                {
                    Console.WriteLine("Unknown reason!");
                }
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                return been;
            }
            foreach (Location loc in nodes[Util.GetNodeAtLoc(nodes, Goal)].Path)
            {
                been.Add(loc);
            }
            return been;
        }
        public HashSet<Location> GetAllPassablePoints()
        {
            HashSet<Location> passablePoints = new HashSet<Location>();
            for (int y = 0; y <= Height; y++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    if (Passable(new Location(x, y)))
                    {
                        passablePoints.Add(new Location(x, y));
                    }
                }
            }
            return passablePoints;
        }
        public List<Node> GenerateNodes()
        {
            HashSet<Location> passablePoints = GetAllPassablePoints();
            HashSet<Location> startPassable = new HashSet<Location>();
            foreach (Location passable in passablePoints)
            {
                bool stest = true;
                foreach (Circle obstacle in obstacles)
                {
                    if (Util.LineIntersectsCircle(Start, passable, obstacle))
                    {
                        stest = false;
                    }
                }
                if (stest) startPassable.Add(passable);
            }
            Location[] startPassableArr = new Location[startPassable.Count];
            Util.CopyHashSetToArray(startPassable, ref startPassableArr);
            Node start = new Node(Start, startPassableArr, 0);
            List<Node> nodes = new List<Node>();
            nodes.Add(start);
            HashSet<Location> considerPassable = new HashSet<Location>();
            foreach (Location consider in passablePoints)
            {
                if (consider != Start)
                {
                    foreach (Location passable in passablePoints)
                    {
                        bool test = true;
                        foreach (Circle obstacle in obstacles)
                        {
                            if (Util.LineIntersectsCircle(consider, passable, obstacle))
                            {
                                test = false;
                                //Console.WriteLine($"{consider} is connected to {passable}, even with {new Circle(wall, 0.5)} in the way!");
                                break;
                            }
                        }
                        if (test) considerPassable.Add(passable);
                    }
                    Location[] considerPassableArr = new Location[considerPassable.Count];
                    Util.CopyHashSetToArray(considerPassable, ref considerPassableArr);
                    nodes.Add(new Node(consider, considerPassableArr, Double.MaxValue));
                    considerPassable.Clear();
                }
            }
            nodes.Sort();
            return nodes;
        }
        public void CopyObstaclesToWalls()
        {
            walls.Clear();
            foreach (Circle obst in obstacles)
            {
                walls.Add(obst.Center);
            }
        }
    }
}
