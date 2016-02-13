using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace LogicPad2.Diagram
{
    /// <summary>
    /// Interaction logic for Wire.xaml
    /// </summary>
    public partial class Wire : UserControl
    {
        private TranslateTransform flowOffset;
        private LinearGradientBrush flow;
        private BezierSegment bz;
        private PathFigure pf;
        private DoubleAnimation da;
        private Point _original, _dest;
        private delegate void SetFillDelegate(bool value);

        public Wire()
        {
            InitializeComponent();

            flow = new LinearGradientBrush(Colors.White, Colors.Red, 0);
            flow.MappingMode = BrushMappingMode.Absolute;
            flow.SpreadMethod = GradientSpreadMethod.Repeat;

            pf = new PathFigure();
            bz = new BezierSegment();
            pf.Segments.Add(bz);
            PathGeometry pg = new PathGeometry(new PathFigure[]{pf});
            Inner.Data = pg;
            Outer.Data = pg;

            Inner.Stroke = Brushes.White;
        }

        private void Recompute()
        {
            double x1, y1, x2, y2;
            x1 = _original.X;
            y1 = _original.Y;
            x2 = _dest.X;
            y2 = _dest.Y;
            pf.StartPoint = new Point(x1, y1);
            bz.Point1 = new Point(x1 * 0.6 + x2 * 0.4, y1);
            bz.Point2 = new Point(x1 * 0.4 + x2 * 0.6, y2);
            bz.Point3 = new Point(x2, y2);

            // if x2 == x1 or y2 == y1 then the flow will not happen
            // so we can "override" those cases with slightly fake values
            if (x2 == x1)
                x2 += 0.00001;
            if (y2 == y1)
                y2 += 0.00001;

            flow.StartPoint = pf.StartPoint;
            flow.EndPoint = bz.Point3;

            TransformGroup rt = new TransformGroup();
            rt.Children.Add(new ScaleTransform(10.0 / Math.Abs(x2 - x1), 10.0 / Math.Abs(y2 - y1)));
            
            flowOffset = new TranslateTransform(0, 0);

            if (x1 < x2)
                da = new DoubleAnimation(0.0, 1.0, new Duration(new TimeSpan(0, 0, (int)Math.Abs(x2 - x1) / 10)));
            else
                da = new DoubleAnimation(1.0, 0.0, new Duration(new TimeSpan(0, 0, (int)Math.Abs(x2 - x1) / 10)));
            
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.IsCumulative = true;
            flowOffset.BeginAnimation(TranslateTransform.XProperty, da);

            if (y1 < y2)
                da = new DoubleAnimation(0.0, 1.0, new Duration(new TimeSpan(0, 0, (int)Math.Abs(y2 - y1) / 10)));
            else
                da = new DoubleAnimation(1.0, 0.0, new Duration(new TimeSpan(0, 0, (int)Math.Abs(y2 - y1) / 10)));

            da.RepeatBehavior = RepeatBehavior.Forever;
            da.IsCumulative = true;
            flowOffset.BeginAnimation(TranslateTransform.YProperty, da);

            rt.Children.Add(flowOffset);
            flow.RelativeTransform = rt;
        }

        private void setFill(bool value)
        {
            if (value)
            {
                Inner.Stroke = flow;
            }
            else {
                Inner.Stroke = Brushes.White;
            }
        }

        public bool Value
        {
            set
            {
                setFill(value);
            }
        }

        public Point Origin
        {
            get {
                return _original;
            }
            set {
                _original = value;
                Recompute();
            }
        }

        public Point Destination
        {
            get {
                return _dest;
            }
            set {
                _dest = value;
                Recompute();
            }        
        }

    }
}
