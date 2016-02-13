using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


/*
 *  Copyright (C) 2010 Steve Kollmansberger
 * 
 *  This file is part of Logic Gate Simulator.
 *
 *  Logic Gate Simulator is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Logic Gate Simulator is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Logic Gate Simulator.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace GatesWpf
{
    /// <summary>
    /// Serves the same purpose as a Point class, but expanded to include angle
    /// </summary>
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
