using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class Waypoint : IComparable<Waypoint>
    {
        public Waypoint(double o, double lat, double lon, double alt)
        {
            Order = o;
            Latitude = lat;
            Longitude = lon;
            Altitude = alt;
        }
        public Waypoint()
        {
            Order = -1;
            Latitude = 0;
            Longitude = 0;
            Altitude = -1;
        }
        public int CompareTo(Waypoint otherWp) // used to sort nodes by cost
        {
            if (otherWp == null)
            {
                return 1;
            }
            else
            {
                return this.Order.CompareTo(otherWp.Order);
            }
        }
        public override string ToString()
        {
            return $"{Order},{Latitude},{Longitude},{Altitude}";
        }
        public double Order { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
    }
}
