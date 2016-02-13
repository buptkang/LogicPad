using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;

namespace LogicPad2.Diagram
{
    public class PieMenu : Panel
    {
        public static readonly DependencyProperty ClippingRadiusProperty;

        private bool ismouseleftdown = false;
        private bool isStylusDown = false;

        static PieMenu()
        {
            PieMenu.ClippingRadiusProperty = DependencyProperty.Register("ClippingRadius", typeof(double), typeof(PieMenu), new FrameworkPropertyMetadata(20.0));
        }

        protected override Size MeasureOverride(Size availablesize)
        {
            foreach (UIElement uie in Children)
                uie.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            return availablesize;
        }

        protected override Size ArrangeOverride(Size finalsize)
        {
            double radx = this.DesiredSize.Width / 2.0;
            double rady = this.DesiredSize.Height / 2.0;
            Point center = new Point(radx, rady);

            double angle = 0.0, anglestep = 0.0;

            if (this.Children.Count != 0.0)
                anglestep = 360.0 / (double)this.Children.Count;

            double deg2rad = Math.PI / 180.0;

            foreach (UIElement uie in Children)
            {
                double a = (angle + anglestep / 2.0) * deg2rad;

                uie.Arrange(new Rect(
                    Point.Add(center, new Vector((radx + (double)base.GetValue(PieMenu.ClippingRadiusProperty)) * Math.Cos(a) / 2.0 - uie.DesiredSize.Width / 2.0,
                    (rady + (double)base.GetValue(PieMenu.ClippingRadiusProperty)) * Math.Sin(a) / 2.0 - uie.DesiredSize.Height / 2.0)),
                    uie.DesiredSize));

                angle += anglestep;
            }

            return finalsize;
        }

        #region stylus event handler

        protected override void OnStylusEnter(System.Windows.Input.StylusEventArgs e)
        {
            base.OnStylusEnter(e);
            this.InvalidateVisual();
        }

        protected override void OnStylusLeave(System.Windows.Input.StylusEventArgs e)
        {

            base.OnStylusLeave(e);
            this.InvalidateVisual();
        }

        protected override void OnStylusDown(System.Windows.Input.StylusDownEventArgs e)
        {
            base.OnStylusDown(e);
            isStylusDown = true;
            this.InvalidateVisual();
        }

        protected override void OnStylusMove(System.Windows.Input.StylusEventArgs e)
        {
            base.OnStylusMove(e);
            this.InvalidateVisual();
        }

        protected override void OnStylusUp(System.Windows.Input.StylusEventArgs e)
        {
            base.OnStylusUp(e);
            isStylusDown = false;
            this.InvalidateVisual();
        }


        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            double radx = this.DesiredSize.Width / 2.0;
            double rady = this.DesiredSize.Height / 2.0;
            Point center = new Point(radx, rady);

            double angle = 0.0, anglestep = 0.0;

            if (this.Children.Count != 0.0)
                anglestep = 360.0 / (double)this.Children.Count;

            double deg2rad = Math.PI / 180.0;

            dc.PushClip(new CombinedGeometry(GeometryCombineMode.Exclude, new EllipseGeometry(center, radx + 1, rady + 1), new EllipseGeometry(center, (double)base.GetValue(PieMenu.ClippingRadiusProperty), (double)base.GetValue(PieMenu.ClippingRadiusProperty))));

            dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(250, 250, 250)), new Pen(new SolidColorBrush(Color.FromRgb(197, 197, 197)), 1.0), center, radx, rady);

            if ((double)base.GetValue(PieMenu.ClippingRadiusProperty) > 0.0)
                dc.DrawEllipse(null, new Pen(new SolidColorBrush(Color.FromRgb(197, 197, 197)), 1.0), center, (double)base.GetValue(PieMenu.ClippingRadiusProperty) + 1, (double)base.GetValue(PieMenu.ClippingRadiusProperty) + 1);

            double sa = 0.0, ea = 0.0;

            foreach (UIElement uie in this.Children)
            {
                double a = angle * deg2rad;
                dc.DrawLine(new Pen(new SolidColorBrush(Color.FromRgb(197, 197, 197)), 1.0), center, Point.Add(center, new Vector(radx * Math.Cos(a), rady * Math.Sin(a))));

                if (this.IsStylusOver && uie.IsStylusOver)
                {
                    sa = a;
                    ea = sa + anglestep * deg2rad;
                }
                angle += anglestep;
            }

            if (this.IsStylusOver)
            {
                PathGeometry path = new PathGeometry();
                PathFigure pathfig = new PathFigure();
                pathfig.StartPoint = center;
                pathfig.Segments.Add(new LineSegment(Point.Add(center, new Vector(radx * Math.Cos(sa), rady * Math.Sin(sa))), true));
                pathfig.Segments.Add(new ArcSegment(Point.Add(center, new Vector(radx * Math.Cos(ea), rady * Math.Sin(ea))), new Size(1.0, 1.0), 0.0, false, SweepDirection.Clockwise, true));
                pathfig.Segments.Add(new LineSegment(center, true));
                path.Figures.Add(pathfig);
                dc.PushClip(path);

                LinearGradientBrush brush = new LinearGradientBrush();
                brush.StartPoint = new Point(0, 0);
                brush.EndPoint = new Point(0, 1);

                angle = 0;

                brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 254, 227), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 231, 151), 0.4));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 215, 80), 0.4));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 231, 150), 1.0));

                dc.DrawEllipse(brush, new Pen(new SolidColorBrush(Color.FromRgb(210, 192, 141)), 1.0), center, radx, rady);    
               
                dc.DrawEllipse(null, new Pen(new LinearGradientBrush(Color.FromRgb(255, 255, 247), Colors.Transparent, 45.0), 1.0), center, radx - 1, rady - 1);

                if ((double)base.GetValue(PieMenu.ClippingRadiusProperty) > 0.0)
                    dc.DrawEllipse(null, new Pen(new SolidColorBrush(Color.FromRgb(210, 192, 141)), 1.0), center, (double)base.GetValue(PieMenu.ClippingRadiusProperty) + 1, (double)base.GetValue(PieMenu.ClippingRadiusProperty) + 1);

                foreach (UIElement uie in this.Children)
                {
                    double a = angle * deg2rad;
                    dc.DrawLine(new Pen(new SolidColorBrush(Color.FromRgb(210, 192, 141)), 1.0), center, Point.Add(center, new Vector(radx * Math.Cos(a), rady * Math.Sin(a))));
                    angle += anglestep;
                }


                dc.Pop();
            }

            dc.Pop();
        }

        [Bindable(true)]
        public double ClippingRadius
        {
            get
            {
                return (double)base.GetValue(PieMenu.ClippingRadiusProperty);
            }
            set
            {
                base.SetValue(PieMenu.ClippingRadiusProperty, value);
            }
        }
    }
}