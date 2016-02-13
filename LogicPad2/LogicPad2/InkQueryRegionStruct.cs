using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace LogicPad2
{
    public class InkQueryRegionStruct : FrameworkElement
    {
        private VisualCollection _children;
        public VisualCollection Children
        {
            get { return _children; }
        }
            
        private StrokeCollection _equalStrokes;
        public StrokeCollection EqualStrokes
        {
            get { return _equalStrokes; }
            set { _equalStrokes = value; }
        }
        private Rect _equalRect;
        public Rect EqualRect
        {
            get { return _equalRect; }
            set { _equalRect = value; }
        }

        private DrawingVisual _leftSideRegion;
        private Rect _leftSideRegionRect;
        public Rect LeftSideRegionRect
        {
            get { return this._leftSideRegionRect; }
            set { this._leftSideRegionRect = value; }
        }
        private UserControl _leftSideUserControl;
        public UserControl LeftSideUserControl
        {
            get { return _leftSideUserControl; }
            set { _leftSideUserControl = value; }
        }
        public DrawingVisual LeftSideRegion
        {
            get { return _leftSideRegion; }
            set { _leftSideRegion = value; }
        }

        private UserControl _rightSideUserControl;
        public UserControl RightSideUserControl
        {
            get { return _rightSideUserControl; }
            set { _rightSideUserControl = value; }
        }      
        private DrawingVisual _rightSideRegion;
        private Rect _rightSideRegionRect;
        public Rect RightSideRegionRect
        {
            get { return this._rightSideRegionRect; }
            set { this._rightSideRegionRect = value; }
        }
        public DrawingVisual RightSideRegion
        {
            get { return _rightSideRegion; }
            set { _rightSideRegion = value; }
        }

        public InkQueryRegionStruct(StrokeCollection strokes)
        {
            _equalStrokes = strokes;
            _children = new VisualCollection(this);

            EqualRect = strokes.GetBounds();
            LeftSideRegionRect = new Rect(new Point(EqualRect.TopLeft.X - EqualRect.Width * 5, EqualRect.TopLeft.Y - EqualRect.Height * 2), new Size(EqualRect.Width * 4, EqualRect.Height * 4));
            RightSideRegionRect = new Rect(new Point(EqualRect.BottomRight.X + EqualRect.Width, EqualRect.BottomRight.Y - EqualRect.Height * 3), new Size(EqualRect.Width * 4, EqualRect.Height * 4));
        }

        public void GenerateLeftSideRegion()
        {
            Rect equalRect = _equalStrokes.GetBounds();
            //Draw Left Region

            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Create a rectangle and draw it in the DrawingContext.
            Rect rect = new Rect(new Point(equalRect.TopLeft.X - equalRect.Width * 10, equalRect.TopLeft.Y - equalRect.Height * 4), new Size(equalRect.Width * 8, equalRect.Height * 8));
            drawingContext.DrawRectangle(Brushes.LightBlue, (Pen)null, rect);

          
            /*
            BitmapImage theImage = new BitmapImage
                ((Resources.FindName("questionMark") as Uri), UriKind.Relative);

            drawingContext.DrawImage(theImage, rect);
             */

            drawingContext.DrawText(new FormattedText("Drag Control Here",
                  CultureInfo.GetCultureInfo("en-us"),
                  FlowDirection.LeftToRight,       
                  new Typeface("Verdana"),
                  12, System.Windows.Media.Brushes.Black), new Point(rect.X, rect.Y + rect.Height / 2));

            // Persist the drawing content.
            drawingContext.Close();

            _leftSideRegion = drawingVisual;

            _children.Add(_leftSideRegion);
        }

        public void GenerateRightSideRegion()
        {
            Rect equalRect = _equalStrokes.GetBounds();

            ////////////////////////////////////////////////////
           
            //Draw Right Region
            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Create a rectangle and draw it in the DrawingContext.
            Rect rect = new Rect(new Point(equalRect.TopLeft.X + equalRect.Width * 3, equalRect.TopLeft.Y - equalRect.Height * 4), new Size(equalRect.Width * 8, equalRect.Height * 8));
            drawingContext.DrawRectangle(Brushes.LightBlue, (Pen)null, rect);

            drawingContext.DrawText(new FormattedText("Drag Control Here",
                  CultureInfo.GetCultureInfo("en-us"),
                  FlowDirection.LeftToRight,
                  new Typeface("Verdana"),
                  12, System.Windows.Media.Brushes.Black), new Point(rect.X, rect.Y + rect.Height / 2));


            // Persist the drawing content.
            drawingContext.Close();

            _rightSideRegion = drawingVisual;

            _children.Add(_rightSideRegion);
        }

        public void AddUserControlInInkRegion(UserControl control, bool isRightSide)
        {
            if (control is LogicPad2.Diagram.UserControl1)
            {
                LogicPad2.Diagram.UserControl1 diagramUserControl = control as LogicPad2.Diagram.UserControl1;
                if (isRightSide)
                {
                    //substitute right side drawingvisual with user control    
                    this.Children.Remove(this.RightSideRegion);
                    diagramUserControl.UserControlX = this.EqualStrokes.GetBounds().TopRight.X + this.EqualStrokes.GetBounds().Width;
                    diagramUserControl.UserControlY = this.EqualStrokes.GetBounds().TopRight.Y - diagramUserControl.ActualHeight * diagramUserControl.UserControlScaleY/ 2;
                    this.RightSideUserControl = diagramUserControl;
                }
                else {
                    //substitute left side DrawingVisual with user control
                    this.Children.Remove(this.LeftSideRegion);
                    diagramUserControl.UserControlX = this.EqualStrokes.GetBounds().TopLeft.X - diagramUserControl.ActualWidth * diagramUserControl.UserControlScaleX - this.EqualStrokes.GetBounds().Width;
                    diagramUserControl.UserControlY = this.EqualStrokes.GetBounds().TopLeft.Y - diagramUserControl.ActualHeight * diagramUserControl.UserControlScaleY / 2 + this.EqualStrokes.GetBounds().Height;
                    this.LeftSideUserControl = diagramUserControl;
                }
            }
            else if (control is LogicPad2.Expression.UserControl1)
            {
                LogicPad2.Expression.UserControl1 expressionUserControl = control as LogicPad2.Expression.UserControl1;
                if (isRightSide)
                {
                    //substitute right side drawingvisual with user control    
                    this.Children.Remove(this.RightSideRegion);
                    expressionUserControl.UserControlX = this.EqualStrokes.GetBounds().TopRight.X + this.EqualStrokes.GetBounds().Width;
                    expressionUserControl.UserControlY = this.EqualStrokes.GetBounds().TopRight.Y - expressionUserControl.ActualHeight * expressionUserControl.UserControlScaleY / 2;
                    this.RightSideUserControl = expressionUserControl;
                }
                else
                {
                    //substitute left side DrawingVisual with user control
                    this.Children.Remove(this.LeftSideRegion);
                    expressionUserControl.UserControlX = this.EqualStrokes.GetBounds().TopLeft.X - expressionUserControl.ActualWidth * expressionUserControl.UserControlScaleX - this.EqualStrokes.GetBounds().Width;
                    expressionUserControl.UserControlY = this.EqualStrokes.GetBounds().TopLeft.Y - expressionUserControl.ActualHeight * expressionUserControl.UserControlScaleY / 2 + this.EqualStrokes.GetBounds().Height;
                    this.LeftSideUserControl = expressionUserControl;
                }
            }
            else if (control is LogicPad2.TruthTable.UserControl1)
            {
                LogicPad2.TruthTable.UserControl1 truthTableUserControl = control as LogicPad2.TruthTable.UserControl1;
                if (isRightSide)
                {
                    //substitute right side drawingvisual with user control    
                    this.Children.Remove(this.RightSideRegion);
                    truthTableUserControl.UserControlX = this.EqualStrokes.GetBounds().TopRight.X + this.EqualStrokes.GetBounds().Width;
                    truthTableUserControl.UserControlY = this.EqualStrokes.GetBounds().TopRight.Y - truthTableUserControl.ActualHeight * truthTableUserControl.UserControlScaleY / 2;
                    this.RightSideUserControl = truthTableUserControl;
                }
                else
                {
                    //substitute left side DrawingVisual with user control
                    this.Children.Remove(this.LeftSideRegion);
                    truthTableUserControl.UserControlX = this.EqualStrokes.GetBounds().TopLeft.X - truthTableUserControl.ActualWidth * truthTableUserControl.UserControlScaleX - this.EqualStrokes.GetBounds().Width;
                    truthTableUserControl.UserControlY = this.EqualStrokes.GetBounds().TopLeft.Y - truthTableUserControl.ActualHeight * truthTableUserControl.UserControlScaleY / 2 + this.EqualStrokes.GetBounds().Height;
                    this.LeftSideUserControl = truthTableUserControl;
                }
            }
        }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }

}
