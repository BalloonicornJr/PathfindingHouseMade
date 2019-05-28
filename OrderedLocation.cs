using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    class OrderedLocation : Location , IComparable<OrderedLocation> // An OrderedLocation is simply a Location object, but with an order. Who'd've thought?
    {
        public OrderedLocation(double o, int x, int y) //constructor that takes in an x and y value
        {
            Order = o;
            this.X = x;
            this.Y = y;
        }
        public OrderedLocation() //default point is (0,0) with an order of -1
        {
            Order = -1;
            this.X = 0;
            this.Y = 0;
        }
        public int CompareTo(OrderedLocation otherWp) // used to sort locations by order
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
        public double Order { get; set; }
    }
}
