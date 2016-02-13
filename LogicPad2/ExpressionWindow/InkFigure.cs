using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Media;


namespace ExpressionWindow
{
    public class InkFigure : FrameworkElement
    {
        #region Properties
        //figure text
        private FormattedText ftext;
        public FormattedText Text
        {
            get
            {
                return ftext;
            }
        }

        //center of the figure to be drawn
        private Point center;
        public Point Center
        {
            get
            {
                return center;
            }
        }

        //color of the figure
        private Brush color;
        public Brush Color
        {
            get
            {
                return color;
            }
        }

        private Rect bound;

        public Rect Bound
        {
            get { return bound; }
            set { bound = value; }
        }
        #endregion

        public InkFigure() { }

        public InkFigure(string name, Rect boundbox, Point center, Brush color):
            this(name, boundbox, center, color, 30)
        { 
        }

        public InkFigure(string name, Rect boundbox, Point center, Brush color, double size)
        {
            init(name, boundbox, center, color, size);
        }

        void init(string name, Rect boundbox, Point center, Brush color, double size)
        {
            bound = boundbox;
            ftext = new FormattedText(
                name,
                System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(new FontFamily(), FontStyles.Italic, FontWeights.Heavy, FontStretches.Normal),
                size,
                color);
            this.color = color;
            this.center = center;
        }

        public Rect GetBounds()
        {
            return bound;
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            center.X -= ftext.Width / 2;
            center.Y -= ftext.Height / 2;
            drawingContext.DrawText(ftext, center);
        }
    }
}
