using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LogicPad2.Diagram
{
    public class ICBuilder
    {
        public delegate GateLocation GatePosition(Gates.AbstractGate gate);

        private List<GTerm> posGates;
        private double avgX = 0, avgY = 0;
        private Gates.Circuit c;
        private GatePosition pos;

        private struct GTerm
        {
            public Gate.Position pos;
            public double offset;
            public Gates.AbstractGate gate;
            public GTerm(Gates.AbstractGate gate, double offset, Gate.Position pos)
            {
                this.pos = pos;
                this.offset = offset;
                this.gate = gate;
            }
        }

        private void PositionAndHold(Gates.AbstractGate g)
        {
            // determine quadrant
            Point gp = pos(g).GetPoint();
            // the goal here is to make the bracket square
            // and not rectangular if one side is longer


            double dx = gp.X - avgX;
            double dy = gp.Y - avgY;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                // left or right
                // note that we invert Y for left so it goes from bottom to top
                if (dx < 0)
                    posGates.Add(new GTerm(g, -gp.Y, Gate.Position.LEFT));
                else
                    posGates.Add(new GTerm(g, gp.Y, Gate.Position.RIGHT));
            }
            else
            {
                // top or bottom
                if (dy < 0)
                    posGates.Add(new GTerm(g, gp.X, Gate.Position.TOP));
                else
                    posGates.Add(new GTerm(g, gp.X, Gate.Position.BOTTOM));
            }

        }

        public UIGates.IC CreateIC(string name)
        {
            posGates = new List<GTerm>();

            List<Gates.IOGates.UserInput> uis = new List<Gates.IOGates.UserInput>();
            List<Gates.IOGates.UserOutput> uos = new List<Gates.IOGates.UserOutput>();

            // Determine the center-point of the X
            bool fst = true;
            double maxX = 0, maxY = 0, minX = 0, minY = 0;
            foreach (Gates.AbstractGate g in c)
            {
                Point p = pos(g).GetPoint();
                if (fst)
                {
                    maxX = p.X;
                    minX = p.X;
                    maxY = p.Y;
                    minY = p.Y;
                    fst = false;
                }
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                //avgX += p.X;
                //avgY += p.Y;
            }

            //avgX /= c.Count;
            //avgY /= c.Count;
            avgX = (maxX + minX) / 2.0;
            avgY = (maxY + minY) / 2.0;

            // For all user i/o gates, determine where they fall in the X
            foreach (Gates.AbstractGate g in c)
            {

                if (g is Gates.IOGates.UserInput)
                {
                    uis.Add((Gates.IOGates.UserInput)g);
                    PositionAndHold(g);
                }
                if (g is Gates.IOGates.UserOutput)
                {
                    uos.Add((Gates.IOGates.UserOutput)g);
                    PositionAndHold(g);
                }
            }

            // Apply a sort to order user i/o gates with respect
            // to the standard ordering that the IC uses to display its terminals
            // see Gate class
            posGates.Sort((firstVal, nextVal) =>
            {
                if (firstVal.pos != nextVal.pos)
                    return firstVal.pos.CompareTo(nextVal.pos);
                else
                    return firstVal.offset.CompareTo(nextVal.offset);
            });

            Gates.IC nic = new Gates.IC(c, uis.ToArray(), uos.ToArray(), name);
            List<Gate.TerminalID> tids = new List<Gate.TerminalID>();

            // constuct the terminal id list based on the sorted sequence
            foreach (GTerm gt in posGates)
            {
                tids.Add(new Gate.TerminalID(gt.gate is Gates.IOGates.UserInput,
                    gt.gate is Gates.IOGates.UserInput ?
                    uis.IndexOf((Gates.IOGates.UserInput)gt.gate) : uos.IndexOf((Gates.IOGates.UserOutput)gt.gate),
                    gt.pos));
            }

            UIGates.IC nuic = new UIGates.IC(nic, tids.ToArray());

            // finally, hint the IC so that we can remember
            // where things are placed visually in the future
            foreach (Gates.AbstractGate g in c)
            {
                nuic.locationHints.Add(g, pos(g));
            }

            // in some cases, the caller may be using these gates
            // so we create a clone so there is no interference
            return (UIGates.IC)nuic.CreateUserInstance();
        }

        /// <summary>
        /// Create an IC, but ignore user input and output.  This is useful if
        /// the IC is just a package for some other operation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIGates.IC CreateNonTerminaledIC(string name)
        {
            Gates.IC nic = new Gates.IC(c, new Gates.IOGates.UserInput[0],
                new Gates.IOGates.UserOutput[0], name);
            UIGates.IC nuic = new UIGates.IC(nic, new Gate.TerminalID[0]);

            // finally, hint the IC so that we can remember
            // where things are placed visually in the future
            foreach (Gates.AbstractGate g in c)
            {
                nuic.locationHints.Add(g, pos(g));
            }

            return (UIGates.IC)nuic.CreateUserInstance();
        }

        public ICBuilder(Gates.Circuit c, GatePosition pos)
        {
            this.c = c;
            this.pos = pos;
        }
    }
}
