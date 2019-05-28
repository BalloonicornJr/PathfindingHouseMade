using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class Program
    {
        static List<Waypoint> GrabMission() //Reads the mission from the location described in line 13.
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
        static List<Waypoint> GetBoundaries() //Gets the boundaries from the location described in line 31.
        {
            StreamReader sr = new StreamReader(@"C:\Users\Public\Downloads\InteropCode\interop\boundary_points.txt");
            string entireString = sr.ReadToEnd();
            string[] pointArrayString = entireString.Split(',');
            double[] pointArrayDouble = new double[pointArrayString.Length];
            for (int i = 0; i < pointArrayString.Length; i++)
            {
                pointArrayDouble[i] = double.Parse(pointArrayString[i]);
            }
            List<Waypoint> Boundaries = new List<Waypoint>();
            for (int i = 0; i < pointArrayDouble.Length; i += 3)
            {
                Boundaries.Add(new Waypoint(pointArrayDouble[i], pointArrayDouble[i + 1], pointArrayDouble[i + 2], -1));
            }
            Boundaries.Sort();
            return Boundaries;
        }
        static List<ParsedObstacle> ParseObstacles(double altitude) //Gets obstacles from the location described in line 49.
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
                    ParsedObstacles.Add(new ParsedObstacle(pointArrayDouble[i], pointArrayDouble[i + 1], ((pointArrayDouble[i + 2]* 0.3048000) + 6.096), pointArrayDouble[i + 3]));
                }
            }
            return ParsedObstacles;
        }
        static void DrawGrid(SquareGrid field) //Prints the playing field to console. FOR DEBUGGING PURPOSES ONLY; DO NOT USE WHEN TESTING
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

        static void DrawPassable(SquareGrid field) // Prints the playing field to console and highlights which points are passable. See warning above.
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
        static SquareGrid GenerateField(List<Waypoint> Waypoints, List<ParsedObstacle> ParsedObstacles, List<Waypoint> Boundaries, ref double downmost, ref double upmost, ref double leftmost, ref double rightmost) //Generates the playing field given all necessary information.
        {
            double californiaLatToMeter = 1.111785102E5;
            double californiaLonToMeter = 9.226910619E4;
            /* To do: Add Maryland conversions */
            double latitudeconversion = californiaLatToMeter;
            double longitudeconversion = californiaLonToMeter;
            upmost = Double.MinValue;
            downmost = Double.MaxValue;
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
            int height = (int)(deltaHeight * longitudeconversion) + 10;
            double deltaWidth = rightmost - leftmost;
            int width = (int)(deltaWidth * latitudeconversion) + 10;
            rightmost *= latitudeconversion;
            leftmost *= latitudeconversion;
            upmost *= longitudeconversion;
            downmost *= longitudeconversion;
            //Console.WriteLine(Waypoints[0].Latitude * latitudeconversion); // for debugging
            double swrappedlat = (Waypoints[0].Latitude * latitudeconversion) - leftmost;
            //Console.WriteLine(Waypoints[0].Longitude * longitudeconversion); // for debugging
            double swrappedlon = (Waypoints[0].Longitude * longitudeconversion) - downmost;
            Location start = new Location(Convert.ToInt32(swrappedlat) + 5, Convert.ToInt32(swrappedlon) + 5);
            //Console.WriteLine(Waypoints[1].Latitude * latitudeconversion); // for debugging
            double gwrappedlat = (Waypoints[1].Latitude * latitudeconversion) - leftmost;
            //Console.WriteLine(Waypoints[1].Longitude * longitudeconversion); // for debugging
            double gwrappedlon = (Waypoints[1].Longitude * longitudeconversion) - downmost;
            Location goal = new Location(Convert.ToInt32(gwrappedlat) +5, Convert.ToInt32(gwrappedlon) + 5);
            SquareGrid playingField = new SquareGrid(Waypoints[0].Altitude, width, height, start, goal, Waypoints[0].Order);
            foreach(ParsedObstacle po in ParsedObstacles)
            {
                double wrappedlat = (po.Latitude * latitudeconversion) - leftmost;
                double wrappedlon = (po.Longitude * longitudeconversion) - downmost;
                Location center = new Location(Convert.ToInt32(wrappedlat) + 5, Convert.ToInt32(wrappedlon) + 5);
                playingField.obstacles.Add(new Circle(center, po.Radius));
            }
            foreach(Waypoint boundarypoint in Boundaries)
            {
                double wrappedlat = (boundarypoint.Latitude * latitudeconversion) - leftmost;
                double wrappedlon = (boundarypoint.Longitude * longitudeconversion) - downmost;
                playingField.boundaries.Add(new OrderedLocation(boundarypoint.Order, Convert.ToInt32(wrappedlat), Convert.ToInt32(wrappedlon)));
                playingField.boundaries.Sort();
            }
            return playingField;
        }
        static void Main(string[] args) //I realize there's a lot going on here. Due to time constraints, I can't really clean this up.
        {
            double californiaLatToMeter = 1.111785102E5;
            double californiaLonToMeter = 9.226910619E4;
            List<Waypoint> Waypoints = GrabMission();
            List<Waypoint> Boundaries = GetBoundaries();

            /*foreach (Waypoint w in Waypoints)
            {
                Console.WriteLine(w);
            }
            Console.WriteLine();*/ // for debugging
            //DrawPassable(playingField);

            /*Console.WriteLine($"Width: {playingField.Width}; Height: {playingField.Height}");
            Console.WriteLine($"Start = {playingField.Start}");
            Console.WriteLine($"Goal = {playingField.Goal}");*/ // DO NOT REMOVE OR UNCOMMENT: This part visualizes the grid by printing it out. For use when Length & Width < 30.

            List<ParsedObstacle> ParsedObstacles = ParseObstacles(Waypoints[0].Altitude);
            double upmost = 0, leftmost = 0, rightmost = 0, downmost = 0;
            SquareGrid playingField = GenerateField(Waypoints, ParsedObstacles, Boundaries, ref downmost, ref upmost, ref leftmost, ref rightmost);
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
                    Waypoints.Add(new Waypoint(order, latitude, longitude, (Waypoints[0].Altitude + Waypoints[1].Altitude)/2));
                }
            }
            Waypoints.Sort();
            for(int i = 2; i <= Waypoints[Waypoints.Count -1].Order; i++)
            {
                path.Clear();
                int index = Util.GetIndexOfOrder(i, Waypoints);
                //Console.WriteLine(Waypoints[index - 1].Latitude * californiaLatToMeter); // for debugging
                double swrappedlat = (Waypoints[index - 1].Latitude * californiaLatToMeter) - leftmost;
                //Console.WriteLine(Waypoints[index - 1].Longitude * californiaLonToMeter); // for debugging
                double swrappedlon = (Waypoints[index - 1].Longitude * californiaLonToMeter) - downmost;
                playingField.Start = new Location(Convert.ToInt32(swrappedlat) + 5, Convert.ToInt32(swrappedlon) + 5);
                //Console.WriteLine(Waypoints[index].Latitude * californiaLatToMeter); // for debugging
                double gwrappedlat = (Waypoints[index].Latitude * californiaLatToMeter) - leftmost;
                //Console.WriteLine(Waypoints[index].Longitude * californiaLonToMeter); // for debugging
                double gwrappedlon = (Waypoints[index].Longitude * californiaLonToMeter) - downmost;
                playingField.Goal = new Location(Convert.ToInt32(gwrappedlat) + 5, Convert.ToInt32(gwrappedlon) + 5);
                playingField.FirstOrder = i - 1;
                playingField.Altitude = Waypoints[i - 1].Altitude;
                path = playingField.GetShortestPath();
                foreach (Location p in path)
                {
                    if (p != playingField.Goal && p != playingField.Start)
                    {
                        double order = playingField.FirstOrder + count;
                        count += 0.1;
                        double latitude = (p.X + leftmost - 5) / californiaLatToMeter;
                        double longitude = (p.Y + downmost - 5) / californiaLonToMeter;
                        Waypoints.Add(new Waypoint(order, latitude, longitude, (Waypoints[i-1].Altitude + Waypoints[i].Altitude)/2));
                    }
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
        }
    }
}
