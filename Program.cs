using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class Program
    {
        static List<Waypoint> GrabMission()
        {
            StreamReader sr = new StreamReader(@"C:\Users\Public\Downloads\InteropCode\interop\MissionPointsParsed.txt");
            string entireString = sr.ReadToEnd();
            string[] pointArrayString = entireString.Split(',');
            double[] pointArrayDouble = new double[pointArrayString.Length];
            for(int i = 0; i < pointArrayString.Length; i++)
            {
                pointArrayDouble[i] = double.Parse(pointArrayString[i]);
            }
            List<Waypoint> Waypoints = new List<Waypoint>();
            for(int i = 0; i < pointArrayDouble.Length; i+= 4)
            {
                Waypoints.Add(new Waypoint(pointArrayDouble[i], pointArrayDouble[i + 1], pointArrayDouble[i + 2], pointArrayDouble[i + 3]));
            }
            Waypoints.Sort();
            return Waypoints;
        }
        static List<ParsedObstacle> ParseObstacles(double altitude)
        {
            StreamReader sr = new StreamReader(@"C:\Users\Public\Downloads\InteropCode\interop\StationaryParsed.txt");
            string entireString = sr.ReadToEnd();
            string[] pointArrayString = entireString.Split(',');
            double[] pointArrayDouble = new double[pointArrayString.Length];
            for (int i = 0; i < pointArrayString.Length; i++)
            {
                pointArrayDouble[i] = double.Parse(pointArrayString[i]);
            }
            List<ParsedObstacle> ParsedObstacles = new List<ParsedObstacle>();
            for(int i = 0; i < pointArrayDouble.Length; i+= 4)
            {
                if(pointArrayDouble[i+3] > (altitude + 20))
                {
                    ParsedObstacles.Add(new ParsedObstacle(pointArrayDouble[i], pointArrayDouble[i + 1], pointArrayDouble[i + 2], pointArrayDouble[i + 3]));
                }
            }
            return ParsedObstacles;
        }
        static void DrawGrid(SquareGrid field)
        {
            field.CopyObstaclesToWalls();
            HashSet<Location> path = field.GetShortestPath();
            for (int y = field.Width; y >= 0; y--)
            {
                for (int x = 0; x <= field.Height; x++)
                {
                    Location here = new Location(x, y);
                    if (here.Equals(field.Start))
                    {
                        Console.Write("S ");
                    }
                    else if (here.Equals(field.Goal))
                    {
                        Console.Write("G ");
                    }
                    else if (Util.ContainsLoc(field.walls, here))
                    {
                        Console.Write("# ");
                    }
                    else if (Util.ContainsLoc(path, here))
                    {
                        Console.Write($"{Util.LocIndex(path, here)} ");
                    }
                    else
                    {
                        Console.Write("* ");
                    }
                }
                Console.WriteLine();
            }
        }

        static void DrawPassable(SquareGrid field)
        {
            field.CopyObstaclesToWalls();
            for (int y = field.Width; y >= 0; y--)
            {
                for (int x = 0; x <= field.Height; x++)
                {
                    Location here = new Location(x, y);
                    if (field.Passable(here))
                    {
                        Console.Write("$ ");
                    }
                    else if (Util.ContainsLoc(field.walls, here))
                    {
                        Console.Write("# ");
                    }
                    else Console.Write("* ");
                }
                Console.WriteLine();
            }
        }
        static SquareGrid GenerateField(List<Waypoint> Waypoints, List<ParsedObstacle> ParsedObstacles, ref double downmost, ref double upmost, ref double leftmost, ref double rightmost)
        {
            //For now, assume index 12 (#13) in Waypoints is the start, and index 13 (#14) is the goal.
            int p1 = 0;
            int p2 = 1;
            double californiaLatToMeter = 1.111785102E5;
            double californiaLonToMeter = 9.226910619E4;
            downmost = Double.MaxValue;
            upmost = Double.MinValue;
            leftmost = Double.MaxValue;
            rightmost = Double.MinValue;
            foreach(Waypoint w in Waypoints)
            {
                if (w.Longitude < downmost) downmost = w.Longitude;
                if (w.Latitude < leftmost) leftmost = w.Latitude;
                if (w.Longitude > upmost) upmost = w.Longitude;
                if (w.Latitude > rightmost) rightmost = w.Latitude;
            }
            double deltaHeight = upmost - downmost;
            int height = (int)(deltaHeight * californiaLonToMeter) + 10;
            double deltaWidth = rightmost - leftmost;
            int width = (int)(deltaWidth * californiaLatToMeter) + 10;
            rightmost *= californiaLatToMeter;
            leftmost *= californiaLatToMeter;
            upmost *= californiaLonToMeter;
            downmost *= californiaLonToMeter;
            //Console.WriteLine(Waypoints[p1].Latitude * californiaLatToMeter);
            double swrappedlat = (Waypoints[p1].Latitude * californiaLatToMeter) - leftmost;
            //Console.WriteLine(Waypoints[p1].Longitude * californiaLonToMeter);
            double swrappedlon = (Waypoints[p1].Longitude * californiaLonToMeter) - downmost;
            Location start = new Location(Convert.ToInt32(swrappedlat) + 5, Convert.ToInt32(swrappedlon) + 5);
            //Console.WriteLine(Waypoints[p2].Latitude * californiaLatToMeter);
            double gwrappedlat = (Waypoints[p2].Latitude * californiaLatToMeter) - leftmost;
            //Console.WriteLine(Waypoints[p2].Longitude * californiaLonToMeter);
            double gwrappedlon = (Waypoints[p2].Longitude * californiaLonToMeter) - downmost;
            Location goal = new Location(Convert.ToInt32(gwrappedlat) +5, Convert.ToInt32(gwrappedlon) + 5);
            SquareGrid playingField = new SquareGrid(Waypoints[0].Altitude, width, height, start, goal, Waypoints[p1].Order);
            foreach(ParsedObstacle po in ParsedObstacles)
            {
                double wrappedlat = (po.Latitude * californiaLatToMeter) - leftmost;
                double wrappedlon = (po.Longitude * californiaLonToMeter)- downmost;
                Location center = new Location(Convert.ToInt32(wrappedlat) + 5, Convert.ToInt32(wrappedlon) + 5);
                playingField.obstacles.Add(new Circle(center, po.Radius));
            }
            return playingField;
        }
        static void Main(string[] args)
        {
            double californiaLatToMeter = 1.111785102E5;
            double californiaLonToMeter = 9.226910619E4;
            List<Waypoint> Waypoints = GrabMission();
            /*foreach (Waypoint w in Waypoints)
            {
                Console.WriteLine(w);
            }
            Console.WriteLine();*/
            List<ParsedObstacle> ParsedObstacles = ParseObstacles(Waypoints[0].Altitude);
            double upmost = 0, leftmost = 0, rightmost = 0, downmost = 0;
            SquareGrid playingField = GenerateField(Waypoints, ParsedObstacles, ref downmost, ref upmost, ref leftmost, ref rightmost);
            //DrawPassable(playingField);
            /*Console.WriteLine($"Width: {playingField.Width}; Height: {playingField.Height}");
            Console.WriteLine($"Start = {playingField.Start}");
            Console.WriteLine($"Goal = {playingField.Goal}");*/
            HashSet<Location> path = new HashSet<Location>();
            path = playingField.GetShortestPath();
            double count = 0.1;
            foreach(Location p in path)
            {
                if (p != playingField.Goal && p!= playingField.Start)
                {
                    double order = playingField.FirstOrder + count;
                    count += 0.1;
                    double latitude = (p.X + leftmost - 5) / californiaLatToMeter;
                    double longitude = (p.Y + downmost - 5) / californiaLonToMeter;
                    Waypoints.Add(new Waypoint(order, latitude, longitude, Waypoints[0].Altitude));
                }
            }
            Waypoints.Sort();
            String final = "";
            int counts = 1;
            foreach(Waypoint wp in Waypoints)
            {
                final += wp.ToString();
                if(counts != Waypoints.Count)
                {
                    final += ",";
                }
                counts++;
            }
            System.IO.File.WriteAllText(@"C:\Users\Public\Downloads\InteropCode\interop\MissionPointsParsedObstacle.txt", final);

            /*
            Location start = new Location(3, 9);
            Location goal = new Location(17, 18);
            SquareGrid field = new SquareGrid(300, 20, 20, start, goal, 0);
            field.obstacles.Add(new Circle(new Location(12, 14), 4));
            DrawPassable(field);
            Console.WriteLine();
            DrawGrid(field);
            //Console.WriteLine("Finished!");
            Location test = new Location(6, 2);*/ //For debugging
        }
    }
}
