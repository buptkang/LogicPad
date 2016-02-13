using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace LogicPad2.Diagram
{
    public class GateLocation
    {
        /// <summary>
        /// The angle as directly read or saved in a RotateTransform
        /// </summary>
        public double angle;
        public double x;
        public double y;
        public GateLocation() { angle = 0; x = 0; y = 0; }
        public GateLocation(double x, double y, double angle)
        {
            this.x = x;
            this.y = y;
            this.angle = angle;
        }

        /// <summary>
        /// Create based on a Point; angle = 0
        /// </summary>
        /// <param name="p"></param>
        public GateLocation(Point p)
        {
            this.x = p.X;
            this.y = p.Y;
            angle = 0;
        }
        public Point GetPoint()
        {
            return new Point(x, y);
        }
    }
}
