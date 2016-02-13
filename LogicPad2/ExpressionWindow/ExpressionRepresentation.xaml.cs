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

using LogicPad2Util;

namespace ExpressionWindow
{
    /// <summary>
    /// Interaction logic for ExpressionRepresentation.xaml
    /// </summary>
    public partial class ExpressionRepresentation : UserControl, IPassable
    {
        public ExpressionRepresentation(String Output)
        {
            InitializeComponent();
            UserControlStatus = UserControlStatus.None;
            RenderCanvas.Content = Output;
        }
        
        #region Properties 

        private UserControl _owner;

        public UserControl Owner
        {
            set { _owner = value; }
            get { return _owner; }
        }

        public Label RenderCanvas
        {
            get { return expressionRenderer; }
            set { expressionRenderer = value; }
        }

        public double UserControlWidth
        {
            get { return this.Grid0.ActualWidth; }
        }

        public double UserControlHeight
        {
            get { return this.Grid0.ActualHeight; }
        }

        public double UserControlX
        {
            get { return this.UserControlXY.X; }
            set { this.UserControlXY.X = value; }
        }

        public double UserControlY
        {
            get { return this.UserControlXY.Y; }
            set { this.UserControlXY.Y = value; }
        }

        public double UserControlScaleX
        {
            get { return this.UserControlScaleXY.ScaleX; }
            set { this.UserControlScaleXY.ScaleX = value; }
        }

        public double UserControlScaleY
        {
            get { return this.UserControlScaleXY.ScaleY; }
            set { this.UserControlScaleXY.ScaleY = value; }
        }

        public StackPanel ControlRegion
        {
            get
            {
                return this.controlRegion;
            }
        }

        public Button CancelButton
        {
            get
            {
                return this.btnCancel;
            }
        }

        public Button ScaleButton
        {
            get
            {
                return this.btnScale;
            }
        }

        public Button TransformButton
        {
            get
            {
                return this.btnTransform;
            }
        }

        public UserControlStatus UserControlStatus
        {
            get { return this._userControlStatus; }
            set { this._userControlStatus = value; }
        }

        private UserControlStatus _userControlStatus;

        public double InitBtmX
        {
            get { return this._initBtmX; }
            set { this._initBtmX = value; }
        }

        private double _initBtmX;

        #endregion

        #region IPassable Interface

        public new void StylusDown(object sender, StylusDownEventArgs e)
        {
            
        }

        public new void StylusMove(object sender, StylusEventArgs e)
        {
            
        }

        public new void StylusUp(object sender, StylusEventArgs e)
        {
            
        }

        public void StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            
        }

        public new void PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            
        }

        public new void PreviewStylusMove(object sender, StylusEventArgs e)
        {
            
        }

        public new void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        public new void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            
        }

        public new void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        public new void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        #endregion
    }
}
