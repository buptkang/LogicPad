using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.ComponentModel;
using System.Xml.Linq;

using LogicPad2Util;
using LogicPadParser;
using LogicPad2.Diagram.DragDrop;

namespace LogicPad2.Diagram
{
    public delegate void DisableInkHandler(object sender, DisableInkEventArgs e);

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl, IPassable
    {
        public event DisableInkHandler disableInkHandler;

        public UserControl1()
        {
            InitializeComponent();
            _userControlStatus = UserControlStatus.None;

            _myEditLevel = EditLevel.FULL;
            EventDispatcher.BatchDispatcher = Dispatcher;
            inkCanvas.Circuit.Start();
            inkCanvas.triggerPieMenuHandler += new TriggerPieMenuHandler(inkCanvas_triggerPieMenuHandler); 
            inkCanvas.hitPieMenuHandler += inkCanvas_hitPieMenuHandler;
            
            LogicPad2.Diagram.DragDrop.DragDropHelper.ItemDropped += new EventHandler<LogicPad2.Diagram.DragDrop.DragDropEventArgs>(DragDropHelper_ItemDropped);
            initPieMenuOnCanvas();

            icl = new ICList();
            inkCanvas.ICL = icl;
            inkCanvas.SetCaptureICLChanges();
            pieMenuGateSelector.ICList = icl;
        }

        #region Properties

        private bool _isExpressionRepreVisible;

        public bool IsExpressionRepreVisible
        {
            set { _isExpressionRepreVisible = value; }
            get { return _isExpressionRepreVisible; }
        }

        private UserControl _expressionRepr;

        public UserControl ExpressionRepr
        {
            set { _expressionRepr = value; }
            get { return _expressionRepr; }
        }

        private bool _isTruthTableRepreVisible;

        public bool IsTruthTableRepreVisible
        {
            set { _isTruthTableRepreVisible = value; }
            get { return _isTruthTableRepreVisible; }
        }

        private UserControl _truthTableRepr;

        public UserControl TruthTablePepr
        {
            set { _truthTableRepr = value; }
            get { return _truthTableRepr; }
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

        public Button SaveButton
        {
            get
            {
                return this.btnSave;
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

        public double InitBtmY
        {
            get { return this._initBtmY; }
            set { this._initBtmY = value; }
        }

        private double _initBtmY;

        public InkCanvas DiagramInkCanvas
        {
            get { return this.inkCanvas.circuitInkCanvas; }
        }

        public PieMenuGateSelector DiagramPieMenuGateSelector
        {
            get { return this.pieMenuGateSelector; }
        }

        public Border UserControlBorder
        {
            get { return this.controlBorder; }
        }

        private ICList icl;
        private string _filename;
        private EditLevel _myEditLevel;
        private UIGates.IC _basedon = null;

        public enum EditLevel
        {
            FULL, EDIT, VIEW
        }

        public EditLevel MyEditLevel
        {
            get
            {
                return _myEditLevel;
            }
        }

        #endregion

        #region IPassable Event Handler

        public new void StylusDown(object sender, StylusDownEventArgs e)
        {
            this.inkCanvas.circuitInkCanvas_StylusDown(sender, e);
        }

        public new void StylusMove(object sender, StylusEventArgs e)
        {
            this.inkCanvas.circuitInkCanvas_StylusMove(sender, e);
        }

        public new void StylusUp(object sender, StylusEventArgs e)
        {
            this.inkCanvas.circuitInkCanvas_StylusUp(sender, e);
        }

        public void StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            //Matrix m = new Matrix();
            //m.Translate(-UserControlX, -UserControlY);
            //m.Scale(1 / UserControlScaleX, 1 / UserControlScaleY);
            //e.Stroke.Transform(m, false);
           
            this.inkCanvas.circuitInkCanvas_StrokeCollected(sender, e);
        }

        public new void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.inkCanvas.circuitInkCanvas_PreviewMouseDown(sender, e);
        }

        public new void PreviewStylusMove(object sender, StylusEventArgs e)
        {
            this.inkCanvas.circuitInkCanvas_PreviewStylusMove(sender, e);
        }

        //For PieMenu Gate Selector Only
        public new void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragDropHelper.Instance.DragSource_UserControl_PreviewMouseLeftButtonDown(this, sender, e);
        }

        //For PieMenu Gate Selector Only
        public new void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            DragDropHelper.Instance.DragSource_UserControl_PreviewMouseMove(sender, e);
        }

        //For PieMenu Gate Selector Only
        public new void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DragDropHelper.Instance.DragSource_PreviewMouseLeftButtonUp(sender, e);
        }

        #region Un-used Event Handler
            
        public new void PreviewStylusDown(object sender, StylusDownEventArgs e)
            {
            
            }

        #endregion

        #endregion

        #region PieMenu Timer

        private BackgroundWorker worker;

        void initPieMenuOnCanvas()
        {
            double initTime = Environment.TickCount;

            pieMenuGateSelector.Visibility = Visibility.Visible;

            worker = new BackgroundWorker();

            //Pie Menu Trigger 
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                while (Environment.TickCount - initTime < 100)
                {
                    continue;
                }
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                pieMenuGateSelector.Visibility = Visibility.Collapsed;
            };

            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        void inkCanvas_hitPieMenuHandler(object sender, PieMenuHitTestEventArgs e)
        {
           if(e.DragAndDropEventType == PieMenuHitTestEventArgs.EventType.Down)
           {
               DragDropHelper.Instance.DragSourcePieMenuPreviewMouseLeftButtonDown(this, e.EventArgs);
           }
           else if (e.DragAndDropEventType == PieMenuHitTestEventArgs.EventType.Up)
           {
               DragDropHelper.Instance.DragSourcePieMenuPreviewMouseLeftButtonUp(e);
           }

           foreach(UIElement uie in pieMenuGateSelector.pieMenu.Children)
           {
               if (this.IsStylusOver && uie.IsStylusOver)
               {
                    if(e.DragAndDropEventType==PieMenuHitTestEventArgs.EventType.Move)
                    {        
                        DragDropHelper.Instance.DragSourcePieMenuPreviewMouseMove(uie, e.EventArgs);
                    }
               }
           }
        }

        void inkCanvas_triggerPieMenuHandler(object sender, PieMenuEventArgs e)
        {
            DisableInkEventArgs args; 
            
            if (e.IsVisible)
            {
                args = new DisableInkEventArgs(true);
                disableInkHandler(sender, args);
                pieMenuGateSelector.Background = Brushes.Transparent;
                pieMenuGateSelector.Visibility = Visibility.Visible;
                pieMenuGateSelector.UserControlToolTipX = e.Position.X - pieMenuGateSelector.ActualWidth * 0.5; 
                pieMenuGateSelector.UserControlToolTipY = e.Position.Y - pieMenuGateSelector.ActualHeight * 0.5;
                
                //pieMenuGateSelector.UserControlToolTipX = e.Position.X;
                //pieMenuGateSelector.UserControlToolTipY = e.Position.Y;
            }
            else
            {
                args = new DisableInkEventArgs(false);
                disableInkHandler(sender, args);
                pieMenuGateSelector.Visibility = Visibility.Collapsed;                    
            }
        }

        #endregion

        void DragDropHelper_ItemDropped(object sender, LogicPad2.Diagram.DragDrop.DragDropEventArgs e)
        {
            if (e.DropTarget.IsDescendantOf(inkCanvas))
            {
                Gate newGate = null;
                newGate = ((Gate)e.Content).CreateUserInstance();

                inkCanvas.AddGate(newGate, new GateLocation(inkCanvas.GetNearestSnapTo(inkCanvas.TranslateScrolledPoint(e.Position))));
                inkCanvas.UpdateLayout();
            }
        }

        public void Parse(out string outString)
        {
            CircuitXML cxml = new CircuitXML(icl);
            XElement root = cxml.Save("Temp.gcg", this.inkCanvas, true);
            outString = LogicPadParser.LogicPadParser.Instance.ParseDiagramXElement(root);
        }

        public XElement GetRootElement()
        {
            CircuitXML cxml = new CircuitXML(icl);
            XElement root = cxml.Save("Temp.gcg", this.inkCanvas, true);
            return root;
        }
    }

    public class DisableInkEventArgs : EventArgs
    {
       public bool IsDisabled {set; get;}

       public DisableInkEventArgs(bool isDisabled)
            : base()
        {
            IsDisabled = isDisabled;
        }
    }
}
