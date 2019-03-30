using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class ParsedObstacle
    {
        public ParsedObstacle(double lat, double lon, double rad, double alt)
        {
            Latitude = lat;
            Longitude = lon;
            Radius = rad;
            Altitude = alt;
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public double Altitude { get; set; }
        public override string ToString()
        {
            return $"Latitude:{Latitude}\nLongitude:{Longitude}\nRadius:{Radius}\nAltitude:{Altitude}";
        }
    }
}
