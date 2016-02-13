using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Globalization;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;

using System.Windows.Automation.Peers;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using starPadSDK.Inq;
using starPadSDK.Inq.MSInkCompat;
using starPadSDK.Geom;

using LogicPad2.Diagram;
using LogicPad2.Expression;
using LogicPad2.TruthTable;

using LogicPad2Util;

namespace LogicPad2
{
    public delegate void ChangeMenuRepresentationHandler(object sender, MenuEventArgs args);

    public class MSInkAnalysisCanvas : System.Windows.Controls.InkCanvas
    {
        private bool _touchEnabled;
        public bool TouchEnabled 
        {
            get { return _touchEnabled; }
            set { 
                //TODO
                _touchEnabled = value;
            }
        }

        public TabletDeviceType InputDevice { set; get; } 

        public event ChangeMenuRepresentationHandler ChangeMainMenuRepresentationOptions;

        public MSInkAnalysisCanvas()
        {
            InitInkAnalysis();
        }

        #region Event Handler

        /*
         * Expression User Control: No
         * TruthTable User Control: Yes
         * Diagram User Control: Yes
         *  
         */
        protected override void OnStylusDown(StylusDownEventArgs e)
        {
            if (e.StylusDevice.TabletDevice.Type == TabletDeviceType.Touch)
            {
                if (!TouchEnabled)
                {
                    InputDevice = e.StylusDevice.TabletDevice.Type;
                    return;
                }
            }else {
                InputDevice = e.StylusDevice.TabletDevice.Type;
            }
            
            base.OnStylusDown(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Expression.UserControl1)
                    {
                        #region Expression User Control
                       
                        //HitTest
                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.Expression.UserControl1 expressionUserControl = child as LogicPad2.Expression.UserControl1;

                            CurrentDrawingControl = LogicCanvasType.Expression;
                            HighLightCurrentUserControl(CurrentDrawingControl, expressionUserControl as UserControl);

                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(expressionUserControl.ExpressionInkCanvas, this.TransformToDescendant(expressionUserControl.ExpressionInkCanvas).Transform(pt));
                            if (result != null)
                            {
                                expressionUserControl.UserControlStatus = UserControlStatus.Inking;
                                (child as IPassable).StylusDown(this, e);
                                return;
                            }
                            
                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(expressionUserControl.ScaleButton, this.TransformToDescendant(expressionUserControl.ScaleButton).Transform(pt));
                            if (result != null)
                            {
                                expressionUserControl.CaptureStylus();
                                expressionUserControl.UserControlStatus = UserControlStatus.Scale;
                                expressionUserControl.InitBtmX = e.GetPosition(this).X;
                                return;
                            }

                            //Check HitTest Result on Transform Button
                            result = VisualTreeHelper.HitTest(expressionUserControl.TransformButton, this.TransformToDescendant(expressionUserControl.TransformButton).Transform(pt));
                            if (result != null)
                            {
                                expressionUserControl.CaptureStylus();
                                expressionUserControl.UserControlStatus = UserControlStatus.Transform;
                                return;
                            }
                            return;
                        }
                        #endregion
                    }
                    else if (child is LogicPad2.TruthTable.UserControl1)
                    {
                        #region Truth Table User Control

                        //HitTest
                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.TruthTable.UserControl1 truthTableUserControl = child as LogicPad2.TruthTable.UserControl1;

                            CurrentDrawingControl = LogicCanvasType.TruthTable;
                            HighLightCurrentUserControl(CurrentDrawingControl, truthTableUserControl as UserControl);

                            //Check HitTest Result on InkCanvas                            
                            result = VisualTreeHelper.HitTest(truthTableUserControl.TruthTableInkCanvas, this.TransformToDescendant(truthTableUserControl.TruthTableInkCanvas).Transform(pt));
                            if (result != null)
                            {
                                truthTableUserControl.UserControlStatus = UserControlStatus.Inking;
                                (child as IPassable).StylusDown(this, e);
                                return;
                            }
                     
                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(truthTableUserControl.ScaleButton, this.TransformToDescendant(truthTableUserControl.ScaleButton).Transform(pt));
                            if (result != null)
                            {
                                truthTableUserControl.UserControlStatus = UserControlStatus.Scale;

                                truthTableUserControl.InitBtmX = e.GetPosition(this).X;
                                return;
                            }

                            //Check HitTest Result on Transform Button
                            result = VisualTreeHelper.HitTest(truthTableUserControl.TransformButton, this.TransformToDescendant(truthTableUserControl.TransformButton).Transform(pt));
                            if (result != null)
                            {
                                truthTableUserControl.UserControlStatus = UserControlStatus.Transform;
                                return;
                            }
                            return;
                        }
                        #endregion
                    }
                    else if (child is LogicPad2.Diagram.UserControl1)
                    {
                        #region Diagram User Control

                        //HitTest
                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.Diagram.UserControl1 diagramUserControl = child as LogicPad2.Diagram.UserControl1;

                            CurrentDrawingControl = LogicCanvasType.Diagram;
                            HighLightCurrentUserControl(LogicCanvasType.Diagram, diagramUserControl);

                            //Check HitTest Result on InkCanvas   
                            result = VisualTreeHelper.HitTest(diagramUserControl.DiagramInkCanvas, this.TransformToDescendant(diagramUserControl.DiagramInkCanvas).Transform(pt));
                            if (result != null)
                            {
                                diagramUserControl.UserControlStatus = UserControlStatus.Inking;
                                (child as IPassable).StylusDown(this, e);
                                return;
                            }
                           
                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(diagramUserControl.ScaleButton, this.TransformToDescendant(diagramUserControl.ScaleButton).Transform(pt));
                            if (result != null)
                            {
                                diagramUserControl.UserControlStatus = UserControlStatus.Scale;

                                diagramUserControl.InitBtmX = e.GetPosition(this).X;
                                return;
                            }

                            //Check HitTest Result on Transform Button
                            result = VisualTreeHelper.HitTest(diagramUserControl.TransformButton, this.TransformToDescendant(diagramUserControl.TransformButton).Transform(pt));
                            if (result != null)
                            {
                                diagramUserControl.UserControlStatus = UserControlStatus.Transform;
                                diagramUserControl.InitBtmX = e.GetPosition(this).X;
                                diagramUserControl.InitBtmY = e.GetPosition(this).Y;
                                return;
                            }
                            return;
                        }
                        #endregion
                    }
                    else if (child is ExpressionWindow.ExpressionRepresentation)
                    {
                        #region Expression Representation
                        //HitTest
                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            ExpressionWindow.ExpressionRepresentation expressionRepr = child as ExpressionWindow.ExpressionRepresentation;

                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(expressionRepr.ScaleButton, this.TransformToDescendant(expressionRepr.ScaleButton).Transform(pt));
                            if (result != null)
                            {
                                expressionRepr.UserControlStatus = UserControlStatus.Scale;
                                expressionRepr.InitBtmX = e.GetPosition(this).X;
                                return;
                            }

                            //Check HitTest Result on Transform Button
                            result = VisualTreeHelper.HitTest(expressionRepr.TransformButton, this.TransformToDescendant(expressionRepr.TransformButton).Transform(pt));
                            if (result != null)
                            {
                                expressionRepr.UserControlStatus = UserControlStatus.Transform;
                                return;
                            }
                            return;
                        }
                        #endregion
                    }
                    else if (child is TruthTableWindow.TruthTableRepresentation)
                    {
                        #region TruthTable Representation
                        //HitTest
                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            TruthTableWindow.TruthTableRepresentation truthTableRepr = child as TruthTableWindow.TruthTableRepresentation;

                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(truthTableRepr.ScaleButton, this.TransformToDescendant(truthTableRepr.ScaleButton).Transform(pt));
                            if (result != null)
                            {
                                truthTableRepr.UserControlStatus = UserControlStatus.Scale;
                                truthTableRepr.InitBtmX = e.GetPosition(this).X;
                                return;
                            }

                            //Check HitTest Result on Transform Button
                            result = VisualTreeHelper.HitTest(truthTableRepr.TransformButton, this.TransformToDescendant(truthTableRepr.TransformButton).Transform(pt));
                            if (result != null)
                            {
                                truthTableRepr.UserControlStatus = UserControlStatus.Transform;
                                return;
                            }
                            return;
                        }
                        #endregion
                    }
                    else if (child is GatesWpf.DiagramRepresentation)
                    {
                        #region Diagram Representation
                        //HitTest
                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            GatesWpf.DiagramRepresentation diagramRepr = child as GatesWpf.DiagramRepresentation;

                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(diagramRepr.ScaleButton, this.TransformToDescendant(diagramRepr.ScaleButton).Transform(pt));
                            if (result != null)
                            {
                                diagramRepr.UserControlStatus = UserControlStatus.Scale;
                                diagramRepr.InitBtmX = e.GetPosition(this).X;
                                return;
                            }

                            //Check HitTest Result on Transform Button
                            result = VisualTreeHelper.HitTest(diagramRepr.TransformButton, this.TransformToDescendant(diagramRepr.TransformButton).Transform(pt));
                            if (result != null)
                            {
                                diagramRepr.UserControlStatus = UserControlStatus.Transform;
                                return;
                            }
                            return;
                        }
                        #endregion
                    }

                }
            }
            CurrentDrawingControl = LogicCanvasType.Main;
            ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.Main));
        }

        /*
        * Expression User Control: No
        * TruthTable User Control: No
        * Diagram User Control: Yes
        * 
        */
        protected override void OnStylusMove(StylusEventArgs e)
        {
            base.OnStylusMove(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Expression.UserControl1)
                    {
                        #region Expression User Control
                        Point pt = e.GetPosition(this);

                        LogicPad2.Expression.UserControl1 expressionUserControl = child as LogicPad2.Expression.UserControl1;

                        //Scale User Control Operation
                        if (expressionUserControl.UserControlStatus == UserControlStatus.Scale)
                        {
                            double newBtmX = pt.X;
                            expressionUserControl.CaptureStylus();
                            double scaler = -(expressionUserControl.InitBtmX - newBtmX) / expressionUserControl.ActualWidth;
                            expressionUserControl.InitBtmX = newBtmX;
                            expressionUserControl.UserControlScaleX += scaler;
                            expressionUserControl.UserControlScaleY += scaler;
                            return;
                        }
                        else if (expressionUserControl.UserControlStatus == UserControlStatus.Transform)
                        {
                            expressionUserControl.CaptureStylus();
                            expressionUserControl.UserControlX = e.GetPosition(this).X - expressionUserControl.ActualWidth * expressionUserControl.UserControlScaleX;
                            expressionUserControl.UserControlY = e.GetPosition(this).Y - expressionUserControl.ActualHeight * expressionUserControl.UserControlScaleY;
                          
                            return;
                        }

                        #endregion
                    }
                    else if (child is LogicPad2.TruthTable.UserControl1)
                    {
                        #region Truth Table User Control

                        Point pt = e.GetPosition(this);

                        LogicPad2.TruthTable.UserControl1 truthTableUserControl = child as LogicPad2.TruthTable.UserControl1;

                        //Scale User Control Operation
                        if (truthTableUserControl.UserControlStatus == UserControlStatus.Scale)
                        {
                            double newBtmX = pt.X;
                            truthTableUserControl.CaptureStylus();
                            double scaler = -(truthTableUserControl.InitBtmX - newBtmX) / truthTableUserControl.ActualWidth;
                            truthTableUserControl.InitBtmX = newBtmX;
                            truthTableUserControl.UserControlScaleX += scaler;
                            truthTableUserControl.UserControlScaleY += scaler;
                            
                            return;
                        }
                        else if (truthTableUserControl.UserControlStatus == UserControlStatus.Transform)
                        {
                            truthTableUserControl.CaptureStylus();
                            truthTableUserControl.UserControlX = e.GetPosition(this).X - truthTableUserControl.ActualWidth * truthTableUserControl.UserControlScaleX;
                            truthTableUserControl.UserControlY = e.GetPosition(this).Y - truthTableUserControl.ActualHeight * truthTableUserControl.UserControlScaleY;
                            return;
                        }
                        #endregion
                    }
                    else if (child is LogicPad2.Diagram.UserControl1)
                    { 
                        #region Diagram User Control

                        Point pt = e.GetPosition(this);

                        LogicPad2.Diagram.UserControl1 diagramUserControl = child as LogicPad2.Diagram.UserControl1;

                        double newBtmX = pt.X;
                        double newBtmY = pt.Y;

                        //Scale User Control Operation
                        if (diagramUserControl.UserControlStatus == UserControlStatus.Scale)
                        {
                            diagramUserControl.CaptureStylus();
                            double scaler = -(diagramUserControl.InitBtmX - newBtmX) / diagramUserControl.ActualWidth;
                            diagramUserControl.InitBtmX = newBtmX;
                            diagramUserControl.UserControlScaleX += scaler;
                            diagramUserControl.UserControlScaleY += scaler;
                            return;
                        }
                        else if (diagramUserControl.UserControlStatus == UserControlStatus.Transform)
                        {
                            diagramUserControl.CaptureStylus();

                            double deltax = -(diagramUserControl.InitBtmX - newBtmX);
                            double deltay = -(diagramUserControl.InitBtmY - newBtmY);
                            //diagramUserControl.UserControlX = e.GetPosition(this).X - diagramUserControl.ActualWidth * diagramUserControl.UserControlScaleX;
                            diagramUserControl.UserControlX += deltax; 
                            //diagramUserControl.UserControlY = e.GetPosition(this).Y - diagramUserControl.ActualHeight * diagramUserControl.UserControlScaleY;
                            diagramUserControl.UserControlY += deltay;
                            diagramUserControl.InitBtmX = newBtmX;
                            diagramUserControl.InitBtmY = newBtmY;
                            return;
                        }else if(diagramUserControl.UserControlStatus == UserControlStatus.Inking)
                        {
                            //Check HitTest Result on InkCanvas
                            HitTestResult result = VisualTreeHelper.HitTest(diagramUserControl.DiagramInkCanvas, this.TransformToDescendant(diagramUserControl.DiagramInkCanvas).Transform(pt));
                            if (result != null)
                            {
                                diagramUserControl.UserControlStatus = UserControlStatus.Inking;
                                (child as IPassable).StylusMove(this, e);
                                return;
                            }
                        }
                        #endregion
                    }
                    else if (child is ExpressionWindow.ExpressionRepresentation)
                    {
                        #region Expression Representation
                        Point pt = e.GetPosition(this);

                        ExpressionWindow.ExpressionRepresentation expressionRepr = child as ExpressionWindow.ExpressionRepresentation;

                        //Scale User Control Operation
                        if (expressionRepr.UserControlStatus == UserControlStatus.Scale)
                        {
                            double newBtmX = pt.X;
                            expressionRepr.CaptureStylus();
                            double scaler = -(expressionRepr.InitBtmX - newBtmX) / expressionRepr.ActualWidth;
                            expressionRepr.InitBtmX = newBtmX;
                            expressionRepr.UserControlScaleX += scaler;
                            expressionRepr.UserControlScaleY += scaler;
                            return;
                        }
                        else if (expressionRepr.UserControlStatus == UserControlStatus.Transform)
                        {
                            expressionRepr.CaptureStylus();
                            expressionRepr.UserControlX = e.GetPosition(this).X - expressionRepr.ActualWidth * expressionRepr.UserControlScaleX;
                            expressionRepr.UserControlY = e.GetPosition(this).Y - expressionRepr.ActualHeight * expressionRepr.UserControlScaleY;

                            return;
                        }

                        #endregion
                    }
                    else if (child is TruthTableWindow.TruthTableRepresentation)
                    {
                        #region TruthTable Representation
                        Point pt = e.GetPosition(this);

                        TruthTableWindow.TruthTableRepresentation truthTableRepr = child as TruthTableWindow.TruthTableRepresentation;

                        //Scale User Control Operation
                        if (truthTableRepr.UserControlStatus == UserControlStatus.Scale)
                        {
                            double newBtmX = pt.X;
                            truthTableRepr.CaptureStylus();
                            double scaler = -(truthTableRepr.InitBtmX - newBtmX) / truthTableRepr.ActualWidth;
                            truthTableRepr.InitBtmX = newBtmX;
                            truthTableRepr.UserControlScaleX += scaler;
                            truthTableRepr.UserControlScaleY += scaler;
                            return;
                        }
                        else if (truthTableRepr.UserControlStatus == UserControlStatus.Transform)
                        {
                            truthTableRepr.CaptureStylus();
                            truthTableRepr.UserControlX = e.GetPosition(this).X - truthTableRepr.ActualWidth * truthTableRepr.UserControlScaleX;
                            truthTableRepr.UserControlY = e.GetPosition(this).Y - truthTableRepr.ActualHeight * truthTableRepr.UserControlScaleY;

                            return;
                        }

                        #endregion    
                    }
                    else if (child is GatesWpf.DiagramRepresentation)
                    {
                        #region Diagram Representation

                        Point pt = e.GetPosition(this);

                        GatesWpf.DiagramRepresentation diagramRepr = child as GatesWpf.DiagramRepresentation;

                        //Scale User Control Operation
                        if (diagramRepr.UserControlStatus == UserControlStatus.Scale)
                        {
                            double newBtmX = pt.X;
                            diagramRepr.CaptureStylus();
                            double scaler = -(diagramRepr.InitBtmX - newBtmX) / diagramRepr.ActualWidth;
                            diagramRepr.InitBtmX = newBtmX;
                            diagramRepr.UserControlScaleX += scaler;
                            diagramRepr.UserControlScaleY += scaler;
                            return;
                        }
                        else if (diagramRepr.UserControlStatus == UserControlStatus.Transform)
                        {
                            diagramRepr.CaptureStylus();
                            diagramRepr.UserControlX = e.GetPosition(this).X - diagramRepr.ActualWidth * diagramRepr.UserControlScaleX;
                            diagramRepr.UserControlY = e.GetPosition(this).Y - diagramRepr.ActualHeight * diagramRepr.UserControlScaleY;

                            return;
                        }

                        #endregion
                    }
                  }
              }
        }

        /*
        * Expression User Control: No
        * TruthTable User Control: No
        * Diagram User Control: Yes
        * 
        */
        protected override void OnStylusUp(StylusEventArgs e)
        {
            base.OnStylusUp(e);
        
            InkQueryRegionStruct removedRegion = null;

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.TruthTable.UserControl1)
                    {
                        #region TruthTable User Control
                        LogicPad2.TruthTable.UserControl1 truthTableUserControl = child as LogicPad2.TruthTable.UserControl1;

                        //HitTest
                        Point pt = e.GetPosition(this);

                        if (truthTableUserControl.UserControlStatus != UserControlStatus.None)
                        {
                            //Check HitTest With Other InkCanvas InkQueryRegionStruct
                            removedRegion = HitTestInkQueryRegionStruct(truthTableUserControl);

                            truthTableUserControl.UserControlStatus = UserControlStatus.None;
                        }

                        truthTableUserControl.ReleaseStylusCapture();

                        #endregion
                    }
                    else if (child is LogicPad2.Diagram.UserControl1)
                    {
                        #region Diagram User Control

                        LogicPad2.Diagram.UserControl1 diagramUserControl = child as LogicPad2.Diagram.UserControl1;

                        //HitTest
                        Point pt = e.GetPosition(this);

                        if (diagramUserControl.UserControlStatus == UserControlStatus.Scale
                            || diagramUserControl.UserControlStatus == UserControlStatus.Transform)
                        {
                            //HitTest With Other InkCanvas InkQueryRegionStruct
                            removedRegion = HitTestInkQueryRegionStruct(diagramUserControl);
                        }
                        else if (diagramUserControl.UserControlStatus == UserControlStatus.Inking)
                        {
                            //Check HitTest Result on InkCanvas
                            HitTestResult result = VisualTreeHelper.HitTest(diagramUserControl.DiagramInkCanvas, this.TransformToDescendant(diagramUserControl.DiagramInkCanvas).Transform(pt));
                            if (result != null)
                            {
                                (child as IPassable).StylusUp(this, e);
                                return;
                            }
                        }

                        diagramUserControl.UserControlStatus = UserControlStatus.None;

                        diagramUserControl.ReleaseStylusCapture();

                        #endregion
                    }
                    else if (child is LogicPad2.Expression.UserControl1)
                    {
                        #region Expression User Control

                        LogicPad2.Expression.UserControl1 expressionUserControl = child as LogicPad2.Expression.UserControl1;

                        //HitTest
                        Point pt = e.GetPosition(this);

                        if (expressionUserControl.UserControlStatus != UserControlStatus.None)
                        {
                            //Check HitTest With Other InkCanvas InkQueryRegionStruct
                            removedRegion = HitTestInkQueryRegionStruct(expressionUserControl);

                            expressionUserControl.UserControlStatus = UserControlStatus.None;
                        }

                        expressionUserControl.ReleaseStylusCapture();

                        #endregion
                    }
                    else if (child is ExpressionWindow.ExpressionRepresentation)
                    {
                        #region Expression Representation
                        ExpressionWindow.ExpressionRepresentation expressionRepr = child as ExpressionWindow.ExpressionRepresentation;

                        //HitTest
                        Point pt = e.GetPosition(this);

                        if (expressionRepr.UserControlStatus != UserControlStatus.None)
                        {
                            expressionRepr.UserControlStatus = UserControlStatus.None;
                        }

                        expressionRepr.ReleaseStylusCapture();

                        #endregion
                    }
                    else if (child is TruthTableWindow.TruthTableRepresentation)
                    {
                        #region TruthTable Representation
                        TruthTableWindow.TruthTableRepresentation truthTableRepr = child as TruthTableWindow.TruthTableRepresentation;

                        //HitTest
                        Point pt = e.GetPosition(this);

                        if (truthTableRepr.UserControlStatus != UserControlStatus.None)
                        {
                            truthTableRepr.UserControlStatus = UserControlStatus.None;
                        }

                        truthTableRepr.ReleaseStylusCapture();

                        #endregion
                    }
                    else if (child is GatesWpf.DiagramRepresentation)
                    {
                        #region Diagram Representation
                        GatesWpf.DiagramRepresentation diagramRepr = child as GatesWpf.DiagramRepresentation;

                        //HitTest
                        Point pt = e.GetPosition(this);
                        if (diagramRepr.UserControlStatus != UserControlStatus.None)
                        {
                            diagramRepr.UserControlStatus = UserControlStatus.None;
                        }
                        diagramRepr.ReleaseStylusCapture();

                        #endregion
                    }
                }
            }

            if (removedRegion != null)
            {
                this.Children.Remove(removedRegion);
            }
        }
        
        /*
        * Expression User Control: Yes
        * TruthTable User Control: Yes
        * Diagram User Control: Yes
        * 
        */
        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            if (InputDevice == TabletDeviceType.Touch)
            {
                this.Strokes.Remove(e.Stroke);
                return;
            }

            base.OnStrokeCollected(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Expression.UserControl1)
                    {
                        #region Expression User Control
                        LogicPad2.Expression.UserControl1 expressionUserControl = child as LogicPad2.Expression.UserControl1;

                        if (expressionUserControl.UserControlStatus == UserControlStatus.Scale)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }
                        else if (expressionUserControl.UserControlStatus == UserControlStatus.Transform)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        //HitTest
                        //Calculate center of stroke
                        Rect strokeRect = e.Stroke.GetBounds();
                        Point center = new Point(strokeRect.Left + strokeRect.Width / 2d,
                            strokeRect.Top + strokeRect.Height / 2d);

                        //Point transformPoint = child.RenderTransform.Inverse.Transform(center);
                        //HitTestResult result = VisualTreeHelper.HitTest(child, center);

                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(center));

                        if (result != null)
                        {
                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(expressionUserControl.ExpressionInkCanvas, this.TransformToDescendant(expressionUserControl.ExpressionInkCanvas).Transform(center));
                            if (result != null)
                            {
                                (child as IPassable).StrokeCollected(this, e);
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Save Button
                            result = VisualTreeHelper.HitTest(expressionUserControl.SaveButton, this.TransformToDescendant(expressionUserControl.SaveButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                MessageBox.Show("Save Command!!!");
                                return;
                            }

                            //Check HitTest Result on Cancel Button
                            result = VisualTreeHelper.HitTest(expressionUserControl.CancelButton, this.TransformToDescendant(expressionUserControl.CancelButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                CheckDeletedControlOnInkQueryRegionStruct(expressionUserControl);
                                this.Children.Remove(expressionUserControl);
                                if(CurrentEditingUserControl != null && CurrentEditingUserControl == expressionUserControl)
                                {
                                    ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.Main));
                                }
                                return;
                            }

                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(expressionUserControl.ScaleButton, this.TransformToDescendant(expressionUserControl.ScaleButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Transform Region
                            result = VisualTreeHelper.HitTest(expressionUserControl.TransformButton, this.TransformToDescendant(expressionUserControl.TransformButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on StarPad Alternate MenuItems
                           
                            if (expressionUserControl.HitTestAltsMenuCreator(this, center))
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }
                           
                            //For control region part Ad-Hoc
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }
                        #endregion
                    }
                    else if (child is LogicPad2.TruthTable.UserControl1)
                    {
                        #region Truth Table User Control

                        LogicPad2.TruthTable.UserControl1 truthTableUserControl = child as LogicPad2.TruthTable.UserControl1;

                        if (truthTableUserControl.UserControlStatus == UserControlStatus.Scale)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }
                        else if (truthTableUserControl.UserControlStatus == UserControlStatus.Transform)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        //HitTest
                        //Calculate center of stroke
                        Rect strokeRect = e.Stroke.GetBounds();
                        Point center = new Point(strokeRect.Left + strokeRect.Width / 2d,
                            strokeRect.Top + strokeRect.Height / 2d);

                        //Point transformPoint = child.RenderTransform.Inverse.Transform(center);
                        //HitTestResult result = VisualTreeHelper.HitTest(child, center);

                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(center));

                        if (result != null)
                        {
                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(truthTableUserControl.TruthTableInkCanvas, this.TransformToDescendant(truthTableUserControl.TruthTableInkCanvas).Transform(center));
                            if (result != null)
                            {
                                (child as IPassable).StrokeCollected(this, e);
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Increase Term Button 
                            result = VisualTreeHelper.HitTest(truthTableUserControl.IncreaseTermButton, this.TransformToDescendant(truthTableUserControl.IncreaseTermButton).Transform(center));
                            if (result != null)
                            {
                                truthTableUserControl.IncreaseTerm();
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Decrease Term Button
                            result = VisualTreeHelper.HitTest(truthTableUserControl.DecreaseTermButton, this.TransformToDescendant(truthTableUserControl.DecreaseTermButton).Transform(center));
                            if (result != null)
                            {
                                truthTableUserControl.DecreaseTerm();
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Save Button
                            result = VisualTreeHelper.HitTest(truthTableUserControl.SaveButton, this.TransformToDescendant(truthTableUserControl.SaveButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                MessageBox.Show("Save Command!!!");
                                return;
                            }

                            //Check HitTest Result on Cancel Button
                            result = VisualTreeHelper.HitTest(truthTableUserControl.CancelButton, this.TransformToDescendant(truthTableUserControl.CancelButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                CheckDeletedControlOnInkQueryRegionStruct(truthTableUserControl);
                                this.Children.Remove(truthTableUserControl);
                                if (CurrentEditingUserControl != null && CurrentEditingUserControl == truthTableUserControl)
                                {
                                    ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.Main));
                                }
                                
                                return;
                            }

                            //For control region part Ad-Hoc
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        #endregion
                    }
                    else if (child is LogicPad2.Diagram.UserControl1)
                    {
                        #region Diagram User Control

                        LogicPad2.Diagram.UserControl1 diagramUserControl = child as LogicPad2.Diagram.UserControl1;

                        if (diagramUserControl.UserControlStatus == UserControlStatus.Scale)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }
                        else if (diagramUserControl.UserControlStatus == UserControlStatus.Transform)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        //HitTest
                        //Calculate center of stroke
                        Rect strokeRect = e.Stroke.GetBounds();
                        Point center = new Point(strokeRect.Left + strokeRect.Width / 2d,
                            strokeRect.Top + strokeRect.Height / 2d);

                        //Point transformPoint = child.RenderTransform.Inverse.Transform(center);
                        //HitTestResult result = VisualTreeHelper.HitTest(child, center);

                    
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(center));

                        if (result != null)
                        {
                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(diagramUserControl.DiagramInkCanvas, this.TransformToDescendant(diagramUserControl.DiagramInkCanvas).Transform(center));
                            if (result != null)
                            {
                                Matrix m = new Matrix();
                                m.Translate(-diagramUserControl.UserControlX - 6, -diagramUserControl.UserControlY - 6);
                                //m.Translate(-UserControlX, -UserControlY);
                                m.Scale(1/diagramUserControl.UserControlScaleX, 1 / diagramUserControl.UserControlScaleY);
                                e.Stroke.Transform(m, false);
                                (child as IPassable).StrokeCollected(this, e);
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Save Button
                            result = VisualTreeHelper.HitTest(diagramUserControl.SaveButton, this.TransformToDescendant(diagramUserControl.SaveButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                MessageBox.Show("Save Command!!!");
                                return;
                            }

                            //Check HitTest Result on Cancel Button
                            result = VisualTreeHelper.HitTest(diagramUserControl.CancelButton, this.TransformToDescendant(diagramUserControl.CancelButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                CheckDeletedControlOnInkQueryRegionStruct(diagramUserControl);
                                this.Children.Remove(diagramUserControl);
                                if (CurrentEditingUserControl != null && CurrentEditingUserControl == diagramUserControl)
                                {
                                    ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.Main));
                                }
                                
                                return;
                            }

                            //For control region part Ad-Hoc
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        #endregion
                    }
                    else if (child is ExpressionWindow.ExpressionRepresentation)
                    {
                        #region Expression Representation

                        ExpressionWindow.ExpressionRepresentation expressionRepr = child as ExpressionWindow.ExpressionRepresentation;

                        if (expressionRepr.UserControlStatus == UserControlStatus.Scale)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }
                        else if (expressionRepr.UserControlStatus == UserControlStatus.Transform)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        //HitTest
                        //Calculate center of stroke
                        Rect strokeRect = e.Stroke.GetBounds();
                        Point center = new Point(strokeRect.Left + strokeRect.Width / 2d,
                            strokeRect.Top + strokeRect.Height / 2d);

                        //Point transformPoint = child.RenderTransform.Inverse.Transform(center);
                        //HitTestResult result = VisualTreeHelper.HitTest(child, center);

                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(center));

                        if (result != null)
                        {
                            //Check HitTest Result on Cancel Button
                            result = VisualTreeHelper.HitTest(expressionRepr.CancelButton, this.TransformToDescendant(expressionRepr.CancelButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                //CheckDeletedControlOnInkQueryRegionStruct(expressionRepr);
                                if (this.Children.Contains(expressionRepr.Owner))
                                {
                                    if (expressionRepr.Owner is LogicPad2.Diagram.UserControl1)
                                    {
                                        LogicPad2.Diagram.UserControl1 userControl1 = expressionRepr.Owner as LogicPad2.Diagram.UserControl1;
                                        userControl1.IsExpressionRepreVisible = false;
                                        ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.Diagram, userControl1));
                                    }
                                    else if (expressionRepr.Owner is LogicPad2.TruthTable.UserControl1)
                                    {
                                        LogicPad2.TruthTable.UserControl1 userControl2 = expressionRepr.Owner as LogicPad2.TruthTable.UserControl1;
                                        userControl2.IsExpressionRepreVisible = false;
                                        ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.TruthTable, userControl2));
                                    }
                                }
                                this.Children.Remove(expressionRepr);
                                return;
                            }

                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(expressionRepr.ScaleButton, this.TransformToDescendant(expressionRepr.ScaleButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Transform Region
                            result = VisualTreeHelper.HitTest(expressionRepr.TransformButton, this.TransformToDescendant(expressionRepr.TransformButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //For control region part Ad-Hoc
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        #endregion
                    }
                    else if (child is TruthTableWindow.TruthTableRepresentation)
                    {
                        #region TruthTable Representation

                        TruthTableWindow.TruthTableRepresentation truthTableRepr = child as TruthTableWindow.TruthTableRepresentation;

                        if (truthTableRepr.UserControlStatus == UserControlStatus.Scale)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }
                        else if (truthTableRepr.UserControlStatus == UserControlStatus.Transform)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        //HitTest
                        //Calculate center of stroke
                        Rect strokeRect = e.Stroke.GetBounds();
                        Point center = new Point(strokeRect.Left + strokeRect.Width / 2d,
                            strokeRect.Top + strokeRect.Height / 2d);

                        //Point transformPoint = child.RenderTransform.Inverse.Transform(center);
                        //HitTestResult result = VisualTreeHelper.HitTest(child, center);

                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(center));

                        if (result != null)
                        {
                            //Check HitTest Result on Cancel Button
                            result = VisualTreeHelper.HitTest(truthTableRepr.CancelButton, this.TransformToDescendant(truthTableRepr.CancelButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                //CheckDeletedControlOnInkQueryRegionStruct(truthTableRepr);
                                if (this.Children.Contains(truthTableRepr.Owner))
                                {
                                    if (truthTableRepr.Owner is LogicPad2.Diagram.UserControl1)
                                    {
                                        LogicPad2.Diagram.UserControl1 userControl1 = truthTableRepr.Owner as LogicPad2.Diagram.UserControl1;
                                        userControl1.IsTruthTableRepreVisible = false;
                                        ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.Diagram, userControl1));
                                    }
                                    else if (truthTableRepr.Owner is LogicPad2.Expression.UserControl1)
                                    {
                                        LogicPad2.Expression.UserControl1 userControl2 = truthTableRepr.Owner as LogicPad2.Expression.UserControl1;
                                        userControl2.IsTruthTableRepreVisible = false;
                                        ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.Expression, userControl2));
                                    }
                                }
                                this.Children.Remove(truthTableRepr);
                                return;
                            }

                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(truthTableRepr.ScaleButton, this.TransformToDescendant(truthTableRepr.ScaleButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Transform Region
                            result = VisualTreeHelper.HitTest(truthTableRepr.TransformButton, this.TransformToDescendant(truthTableRepr.TransformButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //For control region part Ad-Hoc
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        #endregion
                    }
                    else if (child is GatesWpf.DiagramRepresentation)
                    {
                        #region Diagram Representation

                        GatesWpf.DiagramRepresentation diagramRepr = child as GatesWpf.DiagramRepresentation;

                        if (diagramRepr.UserControlStatus == UserControlStatus.Scale)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }
                        else if (diagramRepr.UserControlStatus == UserControlStatus.Transform)
                        {
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        //HitTest
                        //Calculate center of stroke
                        Rect strokeRect = e.Stroke.GetBounds();
                        Point center = new Point(strokeRect.Left + strokeRect.Width / 2d,
                            strokeRect.Top + strokeRect.Height / 2d);

                        //Point transformPoint = child.RenderTransform.Inverse.Transform(center);
                        //HitTestResult result = VisualTreeHelper.HitTest(child, center);

                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(center));

                        if (result != null)
                        {
                            //Check HitTest Result on Cancel Button
                            result = VisualTreeHelper.HitTest(diagramRepr.CancelButton, this.TransformToDescendant(diagramRepr.CancelButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                //CheckDeletedControlOnInkQueryRegionStruct(truthTableRepr);
                                if (this.Children.Contains(diagramRepr.Owner))
                                {
                                    if (diagramRepr.Owner is LogicPad2.TruthTable.UserControl1)
                                    {
                                        LogicPad2.TruthTable.UserControl1 userControl1 = diagramRepr.Owner as LogicPad2.TruthTable.UserControl1;
                                        userControl1.IsDiagramRepreVisible = false;
                                        ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.TruthTable, userControl1));
                                    }
                                    else if (diagramRepr.Owner is LogicPad2.Expression.UserControl1)
                                    {
                                        LogicPad2.Expression.UserControl1 userControl2 = diagramRepr.Owner as LogicPad2.Expression.UserControl1;
                                        userControl2.IsDiagramRepreVisible = false;
                                        ChangeMainMenuRepresentationOptions(this, new MenuEventArgs(LogicCanvasType.Expression, userControl2));
                                    }
                                }
                                this.Children.Remove(diagramRepr);
                                return;
                            }

                            //Check HitTest Result on Scale Button
                            result = VisualTreeHelper.HitTest(diagramRepr.ScaleButton, this.TransformToDescendant(diagramRepr.ScaleButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //Check HitTest Result on Transform Region
                            result = VisualTreeHelper.HitTest(diagramRepr.TransformButton, this.TransformToDescendant(diagramRepr.TransformButton).Transform(center));
                            if (result != null)
                            {
                                this.Strokes.Remove(e.Stroke);
                                return;
                            }

                            //For control region part Ad-Hoc
                            this.Strokes.Remove(e.Stroke);
                            return;
                        }

                        #endregion
                    }
                }
            }

            Stroke currentStroke = e.Stroke;

            /* check for scribble delete */
            if (ScribbleDelete(currentStroke))
            {
                CheckInkEqualSignRegion();
                return;
            } 

            _inkAnalyzer.AddStroke(currentStroke);
            _inkAnalyzer.BackgroundAnalyze();

            this.ShowInkAnalysisFeedback = true;
        }


        private void HighLightCurrentUserControl(LogicCanvasType type, UserControl currentEditingControl)
        {
            CurrentEditingUserControl = currentEditingControl;

            Border border;

            foreach (UIElement child in Children)
            {
                if (child is UserControl)
                {
                    if (child == currentEditingControl)
                    {
                        switch (type)
                        {
                            case LogicCanvasType.Diagram:
                                LogicPad2.Diagram.UserControl1 ucd = child as LogicPad2.Diagram.UserControl1;
                                border = ucd.UserControlBorder;
                                border.BorderBrush = Brushes.Yellow;
                                border.BorderThickness = new Thickness(5);

                                ChangeMainMenuRepresentationOptions(ucd, new MenuEventArgs(LogicCanvasType.Diagram, ucd));
                                break;
                            case LogicCanvasType.Expression:
                                LogicPad2.Expression.UserControl1 uce = child as LogicPad2.Expression.UserControl1;
                                border = uce.UserControlBorder;
                                border.BorderBrush = Brushes.Yellow;
                                border.BorderThickness = new Thickness(5);
                                ChangeMainMenuRepresentationOptions(uce, new MenuEventArgs(LogicCanvasType.Expression, uce));
                                break;
                            case LogicCanvasType.TruthTable:
                                LogicPad2.TruthTable.UserControl1 uct = child as LogicPad2.TruthTable.UserControl1;
                                border = uct.UserControlBorder;
                                border.BorderBrush = Brushes.Yellow;
                                border.BorderThickness = new Thickness(5);
                                ChangeMainMenuRepresentationOptions(uct, new MenuEventArgs(LogicCanvasType.TruthTable, uct));
                                break;
                        }
                    }else
                    {
                        //Reset Others
                       if(child is LogicPad2.Diagram.UserControl1)
                       {
                            LogicPad2.Diagram.UserControl1 ucd = child as LogicPad2.Diagram.UserControl1;
                            border = ucd.UserControlBorder;
                            border.BorderBrush = Brushes.Black;
                            border.BorderThickness = new Thickness(1);
                       }
                       else if (child is LogicPad2.Expression.UserControl1)
                       {
                           LogicPad2.Expression.UserControl1 uce = child as LogicPad2.Expression.UserControl1;
                           border = uce.UserControlBorder;
                           border.BorderBrush = Brushes.Black;
                           border.BorderThickness = new Thickness(1);

                       }
                       else if (child is LogicPad2.TruthTable.UserControl1)
                       {
                           LogicPad2.TruthTable.UserControl1 uct = child as LogicPad2.TruthTable.UserControl1;
                           border = uct.UserControlBorder;
                           border.BorderBrush = Brushes.Black;
                           border.BorderThickness = new Thickness(1);
                       }
                    }
                }
            }

        }
    
        /*
        * Expression User Control: Yes
        * TruthTable User Control: No
        * Diagram User Control: No
        * 
        */
        protected override void OnPreviewStylusDown(StylusDownEventArgs e)
        {
            base.OnPreviewStylusDown(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Expression.UserControl1)
                    {
                        #region Expression User Control

                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.Expression.UserControl1 expressionUserControl = child as LogicPad2.Expression.UserControl1;

                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(expressionUserControl.ExpressionInkCanvas, this.TransformToDescendant(expressionUserControl.ExpressionInkCanvas).Transform(pt));
                            if (result != null)
                            {
                                expressionUserControl.UserControlStatus = UserControlStatus.Inking;
                                (child as IPassable).PreviewStylusDown(this, e);
                                return;
                            }
                        }

                        #endregion
                    }
                }
            }
        }

        /*
         * Expression User Control: No
         * TruthTable User Control: No
         * Diagram User Control: Yes
         * 
         */

        protected override void OnPreviewStylusMove(StylusEventArgs e)
        {
            base.OnPreviewStylusMove(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Diagram.UserControl1)
                    {
                        #region Diagram User Control

                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.Diagram.UserControl1 diagramUserControl = child as LogicPad2.Diagram.UserControl1;

                            if (diagramUserControl.UserControlStatus == UserControlStatus.Inking)
                            {
                                #region obsolete code
                                /* 
                                //Adhoc to hittest
                                for (int i = 0; i < diagramUserControl.DiagramInkCanvas.Children.Count; i++)
                                {
                                    UIElement uie = diagramUserControl.DiagramInkCanvas.Children[i];

                                    if (uie is ConnectedWire)
                                    {
                                        result = VisualTreeHelper.HitTest(uie, this.TransformToDescendant(diagramUserControl.DiagramInkCanvas).Transform(pt));
                                        if (result != null)
                                        {
                                            ConnectedWire myWire = uie as ConnectedWire;

                                            if (HitTestWireDict.ContainsKey(myWire))
                                            {
                                                HitTestWireDict[myWire]++;
                                            }
                                            else
                                            {
                                                HitTestWireDict.Add(myWire, 1);
                                            }

                                            
                                            if(HitTestWireDict[myWire] == 3)
                                            {
                                                HitTestWireDict.Clear();

                                                Debug.WriteLine("This wire is " + myWire.ToString());

                                                (child as IPassable).PreviewStylusMove(uie, e);
                                            }
                                            

                                            Debug.WriteLine("HitTest count " + HitTestWireDict[myWire].ToString());
                                        }
                                    }
                                }

                                */
                                #endregion

                                //Check HitTest Result on InkCanvas
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramInkCanvas, this.TransformToDescendant(diagramUserControl.DiagramInkCanvas).Transform(pt));
                                if (result != null)
                                {
                                    diagramUserControl.UserControlStatus = UserControlStatus.Inking;
                                    (child as IPassable).PreviewStylusMove(this, e);
                                    return;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        /*
        * Expression User Control: No
        * TruthTable User Control: No
        * Diagram User Control: Yes
        * 
        * 
        */
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Diagram.UserControl1)
                    {
                        #region Diagram User Control

                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.Diagram.UserControl1 diagramUserControl = child as LogicPad2.Diagram.UserControl1;

                            #region Pie Menu Gate Selector Hit Test
                            //Check HitTest Result on DiagramPieMenuGateSelector
                            result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector).Transform(pt));
                            if (result != null)
                            {
                                //HitTest on OR Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.OrGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.OrGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonDown(diagramUserControl.DiagramPieMenuGateSelector.OrGate, e);
                                    return;
                                }

                                //HitTest on AND Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.AndGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.AndGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonDown(diagramUserControl.DiagramPieMenuGateSelector.AndGate, e);
                                    return;
                                }

                                //HitTest on NOT Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.NotGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.NotGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonDown(diagramUserControl.DiagramPieMenuGateSelector.NotGate, e);
                                    return;
                                }

                                //HitTest on XOR Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.XorGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.XorGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonDown(diagramUserControl.DiagramPieMenuGateSelector.XorGate, e);
                                    return;
                                }

                                //HitTest on UserInput Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.InputGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.InputGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonDown(diagramUserControl.DiagramPieMenuGateSelector.InputGate, e);
                                    return;
                                }

                                //HitTest on UserOutput Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.OutputGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.OutputGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonDown(diagramUserControl.DiagramPieMenuGateSelector.OutputGate, e);
                                    return;
                                }

                                
                            }

#endregion

                            //Adhoc to hittest
                            for (int i = 0; i < diagramUserControl.DiagramInkCanvas.Children.Count; i++ )
                            {
                                UIElement uie = diagramUserControl.DiagramInkCanvas.Children[i];

                                if (uie is ConnectedWire)
                                {
                                    result = VisualTreeHelper.HitTest(uie, this.TransformToDescendant(diagramUserControl.DiagramInkCanvas).Transform(pt));
                                    if (result != null)
                                    {
                                        (child as IPassable).PreviewMouseDown(uie, e);
                                    }
                                }
                            }
                          
                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(diagramUserControl.DiagramInkCanvas, this.TransformToDescendant(diagramUserControl.DiagramInkCanvas).Transform(pt));
                            if (result != null)
                            { 
                                (child as IPassable).PreviewMouseDown(this, e);
                                return;
                            }
                        }
                        #endregion
                    }
                }
            }
        }


        /*
          * Expression User Control: Yes
          * TruthTable User Control: No
          * Diagram User Control: Yes(PieMenuSelector)
          * 
          * 
          */
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Expression.UserControl1)
                    {
                        #region Expression User Control

                        Point pt = e.GetPosition(this);

                        LogicPad2.Expression.UserControl1 expressionUserControl = child as LogicPad2.Expression.UserControl1;

                        //Scale User Control Operation
                        if (expressionUserControl.UserControlStatus == UserControlStatus.Inking)
                        {
                            //Check HitTest Result on InkCanvas
                            HitTestResult result = VisualTreeHelper.HitTest(expressionUserControl.ExpressionInkCanvas, this.TransformToDescendant(expressionUserControl.ExpressionInkCanvas).Transform(pt));
                            if (result != null)
                            {
                                expressionUserControl.UserControlStatus = UserControlStatus.Inking;
                                (child as IPassable).PreviewMouseMove(this, e);
                                return;
                            }
                            else
                            {
                                expressionUserControl.UserControlStatus = UserControlStatus.None;
                            }
                        }
                        #endregion
                    }
                    else if (child is LogicPad2.Diagram.UserControl1)
                    {
                        #region Diagram User Control

                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.Diagram.UserControl1 diagramUserControl = child as LogicPad2.Diagram.UserControl1;

                            if (diagramUserControl.DiagramPieMenuGateSelector.Visibility == Visibility.Collapsed)
                            {
                                return;
                            }

                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector).Transform(pt));

                            if (result != null)
                            {
                                //HitTest on OR Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.OrGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.OrGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseMove(diagramUserControl.DiagramPieMenuGateSelector.OrGate, e);
                                    return;
                                }

                                //HitTest on AND Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.AndGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.AndGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseMove(diagramUserControl.DiagramPieMenuGateSelector.AndGate, e);
                                    return;
                                }

                                //HitTest on NOT Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.NotGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.NotGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseMove(diagramUserControl.DiagramPieMenuGateSelector.NotGate, e);
                                    return;
                                }

                                //HitTest on XOR Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.XorGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.XorGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseMove(diagramUserControl.DiagramPieMenuGateSelector.XorGate, e);
                                    return;
                                }

                                //HitTest on UserInput Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.InputGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.InputGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseMove(diagramUserControl.DiagramPieMenuGateSelector.InputGate, e);
                                    return;
                                }

                                //HitTest on UserOutput Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.OutputGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.OutputGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseMove(diagramUserControl.DiagramPieMenuGateSelector.OutputGate, e);
                                    return;
                                }
                            } 
                        }
                        #endregion
                    }
                }
            }
        }

        /*
        * Expression User Control: Yes
        * TruthTable User Control: No
        * Diagram User Control: Yes(PieMenuSelector)
        * 
        * 
        */
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        { 
            base.OnPreviewMouseLeftButtonDown(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Expression.UserControl1)
                    {
                        #region Expression User Control

                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.Expression.UserControl1 expressionUserControl = child as LogicPad2.Expression.UserControl1;

                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(expressionUserControl.ExpressionInkCanvas, this.TransformToDescendant(expressionUserControl.ExpressionInkCanvas).Transform(pt));
                            if (result != null)
                            {
                                expressionUserControl.UserControlStatus = UserControlStatus.Inking;
                                (child as IPassable).PreviewMouseLeftButtonDown(this, e);
                                return;
                            }
                        }
                        
                        #endregion
                    }
                    else if (child is LogicPad2.Diagram.UserControl1)
                    {
                        #region Diagram User Control

                        #endregion
                    }
                }
            }
        }

        /*
        * Expression User Control: Yes
        * TruthTable User Control: No
        * Diagram User Control: Yes(PieMenuSelector)
        *
        */
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            foreach (UIElement child in Children)
            {
                if (child is IPassable)
                {
                    if (child is LogicPad2.Expression.UserControl1)
                    {
                        #region Expression User Control

                        Point pt = e.GetPosition(this);

                        LogicPad2.Expression.UserControl1 expressionUserControl = child as LogicPad2.Expression.UserControl1;

                        //Scale User Control Operation
                        if (expressionUserControl.UserControlStatus != UserControlStatus.None)
                        {
                            expressionUserControl.UserControlStatus = UserControlStatus.None;
                            (child as IPassable).PreviewMouseLeftButtonUp(this, e);
                            expressionUserControl.ReleaseStylusCapture();
                            return;
                        }

                        #endregion
                    }
                    else if (child is LogicPad2.Diagram.UserControl1)
                    {
                        #region Diagram User Control

                        Point pt = e.GetPosition(this);
                        HitTestResult result = VisualTreeHelper.HitTest(child, this.TransformToDescendant(child).Transform(pt));

                        if (result != null)
                        {
                            LogicPad2.Diagram.UserControl1 diagramUserControl = child as LogicPad2.Diagram.UserControl1;
                           
                            if (diagramUserControl.DiagramPieMenuGateSelector.Visibility == Visibility.Collapsed)
                            {
                                return;
                            }
                           
                            //Check HitTest Result on InkCanvas
                            result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector).Transform(pt));
                            if (result != null)
                            {
                                //HitTest on OR Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.OrGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.OrGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonUp(diagramUserControl.DiagramPieMenuGateSelector.OrGate, e);
                                    diagramUserControl.ReleaseStylusCapture();
                                    return;
                                }

                                //HitTest on AND Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.AndGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.AndGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonUp(diagramUserControl.DiagramPieMenuGateSelector.AndGate, e);
                                    diagramUserControl.ReleaseStylusCapture();
                                    return;
                                }

                                //HitTest on NOT Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.NotGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.NotGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonUp(diagramUserControl.DiagramPieMenuGateSelector.NotGate, e);
                                    diagramUserControl.ReleaseStylusCapture();
                                    return;
                                }

                                //HitTest on XOR Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.XorGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.XorGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonUp(diagramUserControl.DiagramPieMenuGateSelector.XorGate, e);
                                    diagramUserControl.ReleaseStylusCapture();
                                    return;
                                }

                                //HitTest on UserInput Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.InputGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.InputGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonUp(diagramUserControl.DiagramPieMenuGateSelector.InputGate, e);
                                    diagramUserControl.ReleaseStylusCapture();
                                    return;
                                }

                                //HitTest on UserOutput Gate
                                result = VisualTreeHelper.HitTest(diagramUserControl.DiagramPieMenuGateSelector.OutputGate, this.TransformToDescendant(diagramUserControl.DiagramPieMenuGateSelector.OutputGate).Transform(pt));
                                if (result != null)
                                {
                                    (child as IPassable).PreviewMouseLeftButtonUp(diagramUserControl.DiagramPieMenuGateSelector.OutputGate, e);
                                    diagramUserControl.ReleaseStylusCapture();
                                    return;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
        }


        #endregion

        #region Main InkCanvas 

        public void InitInkAnalysis()
        {
            _inkAnalyzer = new InkAnalyzer(this.Dispatcher);
            // Add a listener to StrokesChanged event of InkAnalysis.Strokes collection.

            // Add a listener to ResultsUpdated event.
            _inkAnalyzer.ResultsUpdated += OnInkAnalyzerResultsUpdated;

            this.ShowInkAnalysisFeedback = true;           
        }

        public void DeconstructInkAnalysis()
        {
            _inkAnalyzer.Dispose();
            _inkAnalyzer = null;
        }

        #region scribble function

        private bool ScribbleDelete(Stroke stroke)
        {
            starPadSDK.Inq.Stroq stroq = new starPadSDK.Inq.Stroq(stroke);
            bool canBeScribble = stroq.OldPolylineCusps().Length > 4;
            if (stroq.OldPolylineCusps().Length == 4)
            {
                int[] pcusps = stroq.OldPolylineCusps();
                Deg a1 = fpdangle(stroq[0], stroq[pcusps[1]], stroq[pcusps[2]] - stroq[pcusps[1]]);
                Deg a2 = fpdangle(stroq[pcusps[1]], stroq[pcusps[1]], stroq[pcusps[3]] - stroq[pcusps[1]]);
                if (a1 < 35 && a2 < 35)
                    canBeScribble = stroq.BackingStroke.HitTest(stroq.ConvexHull().First(), 1);
            }

            if (canBeScribble)
            {
                IEnumerable<starPadSDK.Geom.Pt> hull = stroq.ConvexHull();

                List<Point> hullTemp = new List<Point>();
                foreach (starPadSDK.Geom.Pt pt in hull)
                {
                    hullTemp.Add(new Point(pt.X, pt.Y));
                }

                StrokeCollection stks = this.Strokes.HitTest(hullTemp, 50);
                if (stks.Count > 1)
                {
                    //inqCanvas.Stroqs.Remove(stqs);
                    this.Strokes.Remove(stks);
                    if (stks.Contains(stroke))
                    {
                        stks.Remove(stroke);
                    }
                    this.Strokes.Remove(stroke);

                    _inkAnalyzer.RemoveStrokes(stks);
                    // AnalyzedRegion.Strokes.Remove(stks);
                }

                //if (this.Strokes.Contains(stroke))
                //{
                //{
                //    this.Strokes.Remove(stroke);
                    //AnalyzedRegion.Strokes.Remove(stroke);
                //}

                this.ShowInkAnalysisFeedback = false;

                return true;
            }
            return false;
        }

        Deg fpdangle(Pt a, Pt b, Vec v)
        {
            return (a - b).Normalized().UnsignedAngle(v.Normalized());
        }

        #endregion

        private void OnInkAnalyzerResultsUpdated(object sender, ResultsUpdatedEventArgs e)
        {
            if (_feedbackAdorner != null)
            {
                //cause the feedback adorner to repaint itself
                _feedbackAdorner.InvalidateVisual();
            }

            // If the user has made edits while analysis was being performed, trigger
            // BackgroundAnalyze again to analyze these changes
            if (!_inkAnalyzer.DirtyRegion.IsEmpty)
            {
                _inkAnalyzer.BackgroundAnalyze();
            }
        }

        public bool ShowInkAnalysisFeedback
        {
            get { return _showInkAnalysisFeedback; }
            set
            {
                _showInkAnalysisFeedback = value;

                if (_adornerDecorator == null)
                {

                    Console.Write(this.VisualChildrenCount);
                    //We want to adorn the InkCanvas's inner canvas with an adorner 
                    //that we use to display the parse and recognition results
                    _adornerDecorator = (AdornerDecorator)GetVisualChild(0);

                    DependencyObject inkPresenter = VisualTreeHelper.GetChild(_adornerDecorator, 0);
                    DependencyObject innerCanvas = VisualTreeHelper.GetChild(inkPresenter, 0);

                    _feedbackAdorner = new InkAnalysisFeedbackAdorner((UIElement)innerCanvas, _inkAnalyzer);
                    _feedbackAdorner._subInkCanvasGenerated += GenerateSubInkCanvas;
                    _adornerDecorator.AdornerLayer.Add(_feedbackAdorner);
                }

                if (_showInkAnalysisFeedback)
                {
                    _feedbackAdorner.Visibility = Visibility.Visible;
                }
                else
                {
                    _feedbackAdorner.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void DeleteRecognizedStrokesOnInkCanvas(StrokeCollection strokes)
        {
            _inkAnalyzer.RemoveStrokes(strokes);
            this.Strokes.Remove(strokes);
            ShowInkAnalysisFeedback = false;
        }

        public void GenerateSubInkCanvas(object sender, SubCanvasEventArgs args)
        {
            switch (args.LogicCanvasType)
            { 
                case LogicCanvasType.Diagram:
                    
                    Diagram.UserControl1 diagramUserControl1 = new Diagram.UserControl1();

                    diagramUserControl1.UserControlX = args.Central.X;
                    diagramUserControl1.UserControlY = args.Central.Y;

                    this.Children.Add(diagramUserControl1);

                    //Ad-hoc
                    diagramUserControl1.disableInkHandler += new DisableInkHandler(diagramUserControl1_disableInkCanvasInkHandler);
                    
                    if(args.MyStrokes != null)
                        DeleteRecognizedStrokesOnInkCanvas(args.MyStrokes);
                    
                    break;
                case LogicCanvasType.Expression:    
                    Expression.UserControl1 expressionUserControl = new Expression.UserControl1();

                    expressionUserControl.UserControlX = args.Central.X - expressionUserControl.UserControlWidth / 2;
                    expressionUserControl.UserControlY = args.Central.Y - expressionUserControl.UserControlHeight / 2;
                  
                    this.Children.Add(expressionUserControl);

                    if(args.MyStrokes != null)
                        DeleteRecognizedStrokesOnInkCanvas(args.MyStrokes);
                   
                    break;
                case LogicCanvasType.TruthTable:
                    TruthTable.UserControl1 truthTableUserControl = new TruthTable.UserControl1();

                    truthTableUserControl.UserControlX = args.Central.X - truthTableUserControl.UserControlWidth / 2;
                    truthTableUserControl.UserControlY = args.Central.Y - truthTableUserControl.UserControlHeight / 2;

                    this.Children.Add(truthTableUserControl);

                    if(args.MyStrokes != null)
                        DeleteRecognizedStrokesOnInkCanvas(args.MyStrokes);

                    break;
                case LogicCanvasType.EqualSign:
                    if (!HasEqualSignInkRegionAdded(args.MyStrokes))
                    {
                        InkQueryRegionStruct equalRegion = new InkQueryRegionStruct(args.MyStrokes);

                        if (!HasControlInLeftRegion(ref equalRegion))
                        {
                            equalRegion.GenerateLeftSideRegion();
                        }
                        
                        if (!HasControlInRightRegion(ref equalRegion))
                        {
                            equalRegion.GenerateRightSideRegion();
                        }

                        //Check Parsing Current InkQueryRegionStruct
                        if (equalRegion.LeftSideUserControl != null && equalRegion.RightSideUserControl != null)
                        {
                            if (LogicParser1.Instance.MatchTwoUserControl(equalRegion.LeftSideUserControl, equalRegion.RightSideUserControl))
                            {
                                this.Dispatcher.BeginInvoke((Action)(() => { MessageBox.Show("Match On Two Representations"); }));
                            }
                            else
                            {

                                this.Dispatcher.BeginInvoke((Action)(() => { MessageBox.Show("Not Match on Two Representations"); }));
                            }
                            this.Strokes.Remove(equalRegion.EqualStrokes);
                        }
                        else {
                            this.Children.Add(equalRegion);
                        }
                        
                    }
                    _inkAnalyzer.RemoveStrokes(args.MyStrokes);
                    ShowInkAnalysisFeedback = false;
                    break;
                case LogicCanvasType.Question:
                    this.Dispatcher.BeginInvoke((Action)(() => { MessageBox.Show("Basic User Manual"); }));
                    break;
            }
        }

        void diagramUserControl1_disableInkCanvasInkHandler(object sender, DisableInkEventArgs e)
        {
            if (e.IsDisabled)
                this.EditingMode = InkCanvasEditingMode.None;
            else
                this.EditingMode = InkCanvasEditingMode.Ink;
        }

        private bool HasControlInLeftRegion(ref InkQueryRegionStruct inkRegion)
        {          
            foreach (UIElement element in this.Children)
            {
                if (element is UserControl)
                {
                    if (UserControlIntersectWithInkRegion(element as UserControl, ref inkRegion, false))
                    {
                        if (inkRegion.LeftSideUserControl == null)
                        {
                            inkRegion.LeftSideUserControl = element as UserControl;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool HasControlInRightRegion(ref InkQueryRegionStruct inkRegion)
        {
            foreach (UIElement element in this.Children)
            {
                if (element is UserControl)
                {
                    if (UserControlIntersectWithInkRegion(element as UserControl, ref inkRegion, true))
                    {
                        if (inkRegion.RightSideUserControl == null)
                        {
                            inkRegion.RightSideUserControl = element as UserControl;
                            return true;
                        }
                    }
                }   
            }
            return false;
        }

        private bool UserControlIntersectWithInkRegion(UserControl element, ref InkQueryRegionStruct inkRegion, bool isRightSide)
        {
            Rect rect;

            if (element is LogicPad2.Diagram.UserControl1)
            {
                #region Diagram
                LogicPad2.Diagram.UserControl1 diagramUserControl = element as LogicPad2.Diagram.UserControl1;
                Rect userControlRect = new Rect(diagramUserControl.UserControlX,
                            diagramUserControl.UserControlY,
                            diagramUserControl.ActualWidth * diagramUserControl.UserControlScaleX,
                            diagramUserControl.ActualHeight * diagramUserControl.UserControlScaleY);

                if (isRightSide)
                {
                    //rect = inkRegion.RightSideRegionRect;
                    rect = new Rect(new Point(inkRegion.EqualRect.TopRight.X, inkRegion.EqualRect.TopRight.Y),
                            new Size(diagramUserControl.ActualWidth * diagramUserControl.UserControlScaleX, diagramUserControl.ActualHeight * diagramUserControl.UserControlScaleY));
                }
                else
                {
                    //rect = inkRegion.LeftSideRegionRect;
                    rect = new Rect(new Point(inkRegion.EqualRect.TopLeft.X - diagramUserControl.ActualWidth * diagramUserControl.UserControlScaleX, inkRegion.EqualRect.TopLeft.Y),
                           new Size(diagramUserControl.ActualWidth * diagramUserControl.UserControlScaleX, diagramUserControl.ActualHeight * diagramUserControl.UserControlScaleY));
                }

                if (userControlRect.IntersectsWith(rect))
                {
                    return true;
                }
                #endregion
            }
            else if (element is LogicPad2.Expression.UserControl1)
            {
                #region Expression

                LogicPad2.Expression.UserControl1 expressionUserControl = element as LogicPad2.Expression.UserControl1;
                Rect userControlRect = new Rect(expressionUserControl.UserControlX,
                            expressionUserControl.UserControlY,
                            expressionUserControl.ActualWidth * expressionUserControl.UserControlScaleX,
                            expressionUserControl.ActualHeight * expressionUserControl.UserControlScaleY);

                if (isRightSide)
                {
                    //rect = inkRegion.RightSideRegionRect;
                    rect = new Rect(new Point(inkRegion.EqualRect.TopRight.X, inkRegion.EqualRect.TopRight.Y),
                            new Size(expressionUserControl.ActualWidth * expressionUserControl.UserControlScaleX, expressionUserControl.ActualHeight * expressionUserControl.UserControlScaleY));
                }
                else
                {
                    //rect = inkRegion.LeftSideRegionRect;
                    rect = new Rect(new Point(inkRegion.EqualRect.TopLeft.X - expressionUserControl.ActualWidth * expressionUserControl.UserControlScaleX, inkRegion.EqualRect.TopLeft.Y),
                           new Size(expressionUserControl.ActualWidth * expressionUserControl.UserControlScaleX, expressionUserControl.ActualHeight * expressionUserControl.UserControlScaleY));
                }

                if (userControlRect.IntersectsWith(rect))
                {
                    return true;
                }

                #endregion
            }
            else if (element is LogicPad2.TruthTable.UserControl1)
            {
                #region TruthTable

                LogicPad2.TruthTable.UserControl1 truthTableUserControl = element as LogicPad2.TruthTable.UserControl1;
                Rect userControlRect = new Rect(truthTableUserControl.UserControlX,
                            truthTableUserControl.UserControlY,
                            truthTableUserControl.ActualWidth * truthTableUserControl.UserControlScaleX,
                            truthTableUserControl.ActualHeight * truthTableUserControl.UserControlScaleY);

                if (isRightSide)
                {
                    //rect = inkRegion.RightSideRegionRect;
                    rect = new Rect(new Point(inkRegion.EqualRect.TopRight.X, inkRegion.EqualRect.TopRight.Y),
                            new Size(truthTableUserControl.ActualWidth * truthTableUserControl.UserControlScaleX, truthTableUserControl.ActualHeight * truthTableUserControl.UserControlScaleY));
                }
                else
                {
                    //rect = inkRegion.LeftSideRegionRect;
                    rect = new Rect(new Point(inkRegion.EqualRect.TopLeft.X - truthTableUserControl.ActualWidth * truthTableUserControl.UserControlScaleX, inkRegion.EqualRect.TopLeft.Y),
                           new Size(truthTableUserControl.ActualWidth * truthTableUserControl.UserControlScaleX, truthTableUserControl.ActualHeight * truthTableUserControl.UserControlScaleY));
                }

                if (userControlRect.IntersectsWith(rect))
                {
                    return true;
                }

                #endregion
            }
            return false;
        }

        //Avoid Calculate Equal Sign Every Time
        private bool HasEqualSignInkRegionAdded(StrokeCollection strokes)
        {
            foreach (UIElement element in this.Children)
            {
                if (element is InkQueryRegionStruct)
                {
                    InkQueryRegionStruct equalRegion = element as InkQueryRegionStruct;
                    foreach (Stroke stroke in strokes)
                    {
                        if (equalRegion.EqualStrokes.Contains(stroke))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //Scribble Delete use only
        private void CheckInkEqualSignRegion()
        {
            foreach (UIElement element in this.Children)
            {
                if (element is InkQueryRegionStruct)
                {
                    InkQueryRegionStruct region = element as InkQueryRegionStruct;

                    foreach (Stroke stroke in region.EqualStrokes)
                    {
                        if (!this.Strokes.Contains(stroke))
                        {
                            this.Children.Remove(region);

                            /*
                            if (region.LeftSideUserControl != null &&
                                this.Children.Contains(region.LeftSideUserControl))
                            {
                                this.Children.Remove(region.LeftSideUserControl);
                            }
                            if (region.RightSideUserControl != null &&
                                this.Children.Contains(region.RightSideUserControl))
                            {
                                this.Children.Remove(region.RightSideUserControl);
                            }
                             * */
                            return;
                        }    
                    }
                }
            }
        }


        private void CheckDeletedControlOnInkQueryRegionStruct(UserControl control)
        {
            foreach (UIElement element in this.Children)
            {
                if (element is InkQueryRegionStruct)
                {
                    InkQueryRegionStruct region = element as InkQueryRegionStruct;
                    if (control == region.LeftSideUserControl)
                    {
                        region.LeftSideUserControl = null;
                        region.GenerateLeftSideRegion();
                    }
                    else if (control == region.RightSideUserControl)
                    {
                        region.RightSideUserControl = null;
                        region.GenerateRightSideRegion();
                    }
                }
            }
        }

        //Detector for inkRegion
        private InkQueryRegionStruct HitTestInkQueryRegionStruct(UserControl control)
        {
            InkQueryRegionStruct result = null;
            foreach (UIElement element in this.Children)
            {
                if (element is InkQueryRegionStruct)
                {
                    InkQueryRegionStruct region = element as InkQueryRegionStruct;
 
                    if (UserControlIntersectWithInkRegion(control, ref region, false))
                    { 
                        //Left Side
                        if (region.LeftSideUserControl == null && region.RightSideUserControl != control)
                        { 
                            region.AddUserControlInInkRegion(control, false);    
                        }
                        
                    }else
                    {
                        if (control == region.LeftSideUserControl)
                        {
                            region.GenerateLeftSideRegion();
                            region.LeftSideUserControl = null;
                        }
                    }

                    if (UserControlIntersectWithInkRegion(control, ref region, true))
                    {
                        //Right Side
                        if (region.RightSideUserControl == null && region.LeftSideUserControl != control)
                        {
                            region.AddUserControlInInkRegion(control, true);
                        }
                    }else 
                    {
                        if (control == region.RightSideUserControl)
                        {
                            region.GenerateRightSideRegion();
                            region.RightSideUserControl = null;
                        }
                    }

                    //Check Parsing Current InkQueryRegionStruct
                    if (region.LeftSideUserControl != null && region.RightSideUserControl != null)
                    {
                     
                        if (LogicParser1.Instance.MatchTwoUserControl(region.LeftSideUserControl, region.RightSideUserControl))
                        {
                            MessageBox.Show("Match On Two Representations");
                        }
                        else {
                            MessageBox.Show("Not Match on Two Representations");
                        }
                        result = region;
                        this.Strokes.Remove(region.EqualStrokes);
                        break;
                    }
                }
            }
            return result;
        }

        //Oboselete
        public bool IntersectWithOtherChildernInCanvas(Rect rect)
        {
            foreach (UIElement element in this.Children)
            {
                if (element is UserControl)
                {
                    if (element is LogicPad2.Diagram.UserControl1)
                    {
                        LogicPad2.Diagram.UserControl1 userControl = element as LogicPad2.Diagram.UserControl1;
                        Rect userControlRect = new Rect(userControl.UserControlX,
                            userControl.UserControlY,
                            userControl.ActualWidth,
                            userControl.ActualHeight);
                        if (rect.IntersectsWith(userControlRect))
                        {
                            return true;
                        }
                    }
                    else if (element is LogicPad2.Expression.UserControl1)
                    {
                        LogicPad2.Expression.UserControl1 userControl = element as LogicPad2.Expression.UserControl1;
                        Rect userControlRect = new Rect(userControl.UserControlX,
                            userControl.UserControlY,
                            userControl.ActualWidth,
                            userControl.ActualHeight);
                        if (rect.IntersectsWith(userControlRect))
                        {
                            return true;
                        }
                    }
                    else if (element is LogicPad2.TruthTable.UserControl1)
                    {
                        LogicPad2.TruthTable.UserControl1 userControl = element as LogicPad2.TruthTable.UserControl1;
                        Rect userControlRect = new Rect(userControl.UserControlX,
                            userControl.UserControlY,
                            userControl.ActualWidth,
                            userControl.ActualHeight);
                        if (rect.IntersectsWith(userControlRect))
                        {
                            return true;
                        }
                    }
                    else if (element is ExpressionWindow.ExpressionRepresentation)
                    {
                        ExpressionWindow.ExpressionRepresentation userControl = element as ExpressionWindow.ExpressionRepresentation;
                        Rect userControlRect = new Rect(userControl.UserControlX,
                            userControl.UserControlY,
                            userControl.ActualWidth,
                            userControl.ActualHeight);
                        if (rect.IntersectsWith(userControlRect))
                        {
                            return true;
                        }
                    }
                    else if (element is TruthTableWindow.TruthTableRepresentation)
                    {
                        TruthTableWindow.TruthTableRepresentation userControl = element as TruthTableWindow.TruthTableRepresentation;
                        Rect userControlRect = new Rect(userControl.UserControlX,
                            userControl.UserControlY,
                            userControl.ActualWidth,
                            userControl.ActualHeight);
                        if (rect.IntersectsWith(userControlRect))
                        {
                            return true;
                        }
                    }
                    else if (element is GatesWpf.DiagramRepresentation)
                    {
                        GatesWpf.DiagramRepresentation userControl = element as GatesWpf.DiagramRepresentation;
                        Rect userControlRect = new Rect(userControl.UserControlX,
                            userControl.UserControlY,
                            userControl.ActualWidth,
                            userControl.ActualHeight);
                        if (rect.IntersectsWith(userControlRect))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        #endregion

        #region Properties

        private LogicCanvasType _currentDrawingControl = LogicCanvasType.Main;

        public LogicCanvasType CurrentDrawingControl
        {
            get { return this._currentDrawingControl; }
            set { this._currentDrawingControl = value; }
        }

        private UserControl _currentEditingUserControl;

        public UserControl CurrentEditingUserControl
        {
            set { _currentEditingUserControl = value; }
            get { return _currentEditingUserControl; }
        }

        private InkAnalyzer _inkAnalyzer;

        public InkAnalyzer InkAnalyzer
        {
            get { return _inkAnalyzer; }
            set { _inkAnalyzer = value; }
        }

        /// <summary>
        /// Flag set via ShowInkAnalysisFeedback that determines if 
        /// we should show parsing structure feedback and analysis results 
        /// overlayed on the strokes
        /// </summary>
        private bool _showInkAnalysisFeedback = true;

        /// <summary>
        /// The private AdornerDecorator InkCanvas uses to render selection feedback.
        /// We use it to display feedback for InkAnalysis
        /// </summary>
        private AdornerDecorator _adornerDecorator = null;
        private InkAnalysisFeedbackAdorner _feedbackAdorner = null;

        public InkAnalysisFeedbackAdorner FeedbackAdorner
        {
            get { return _feedbackAdorner; }
            set { _feedbackAdorner = value; }
        }

        #endregion

    }

    public class MenuEventArgs: EventArgs
    {   
        private LogicCanvasType _currentEditingCanvas;
        public LogicCanvasType CurrentEditingCanvas
        {
            get { return _currentEditingCanvas; }
            set { _currentEditingCanvas = value; }
        }

        private UserControl _currentEditingUserControl;
        public UserControl CurrentEditingUserControl
        {
            set { _currentEditingUserControl = value; }
            get { return _currentEditingUserControl; }
        }

        public MenuEventArgs(LogicCanvasType type, UserControl userControl)
        {
            _currentEditingCanvas = type;
            _currentEditingUserControl = userControl;
        }

        public MenuEventArgs(LogicCanvasType type)
        {
            _currentEditingCanvas = type;
        }
    }
}
