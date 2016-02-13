using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Threading;
using System.ComponentModel;
using System.Diagnostics;
using LogicPad2.Diagram.ArrowUserControl;
using LogicPad2.Diagram.PieMenuUserControl;
using LogicPad2.Diagram.UIGates;
using starPadSDK.Inq;
using starPadSDK.Inq.MSInkCompat;
using starPadSDK.Geom;

using Gates;

namespace LogicPad2.Diagram
{
    public delegate void TriggerPieMenuHandler(object sender, PieMenuEventArgs e);

    public delegate void HitPieMenuHandler(object sender, PieMenuHitTestEventArgs e);
 
    /// <summary>
    /// Interaction logic for GateInkCanvas.xaml
    /// 
    /// The combination of InkCanvas With DragCanvas
    /// </summary>
    public partial class GateInkCanvas : UserControl
    {
        public event TriggerPieMenuHandler triggerPieMenuHandler;
        public event HitPieMenuHandler hitPieMenuHandler;

        #region properties

        public string ICName;

        public BindingList<Gate> selected = new BindingList<Gate>();

        private ICList icl;
        private bool _ro = false;
        private Gates.Circuit c;
        private Dictionary<Gates.AbstractGate, Gate> gates = new Dictionary<Gates.AbstractGate, Gate>();
        private Dictionary<Gates.Terminal, Wire> wires = new Dictionary<Gates.Terminal, Wire>();
        private Dictionary<Gates.AbstractGate, GateLocation> oldgatepositions = new Dictionary<Gates.AbstractGate, GateLocation>();
        private DragState dragging = DragState.NONE;
        private bool ReadyToSelect = false;
        private Point mp;
        private Point sp;
        private Gate.TerminalID beginTID;
        private double _zoom = 1.0;
        private UndoRedo.Transaction moves;
        private Brush oldBackground;
        private bool _mute = false;
        private UndoRedo.UndoManager _undman;
              
        private const double ANGLE_SNAP_DEG = 10;
        private const double DELTA_SNAP = 5;
        private const double GRID_SIZE = 32;
        #endregion

        public Stroke CurrentStroke { set; get; }
        private DateTime _stylusDownTime;
        public StylusPoint StartPoint { set; get; }
        public Arrow CurrentFlickArrow { set; get; }
        private ObservableCollection<GateTest> _classes;

        public bool IsPieMenuVisible { set; get; }

        public bool onGateStroke = false;
        public bool onWireStroke = false;
        public bool onWireMoveStroke = false;

        #region Constructors

        public GateInkCanvas() : this(new Gates.Circuit(), new ICList()){ }

        public GateInkCanvas(UIGates.IC ic, ICList icl)
            : this(((Gates.IC)ic.AbGate).Circuit, icl) 
        {
            ICName = ic.AbGate.Name;

            foreach (KeyValuePair<Gates.AbstractGate, GateLocation> gp in ic.locationHints)
            {
                if (gp.Key is Gates.IC)
                {
                    // must get terminal id template
                    UIGates.IC templateic = icl.GetIC(gp.Key.Name);
                    
                    AddGate(UIGates.IC.CreateFromTemplate( (Gates.IC)gp.Key, templateic), gp.Value);
                } else
                AddGate(gp.Key, gp.Value);
            }
           // this.Loaded += ((sender, e) =>
            {
                foreach (KeyValuePair<Gates.AbstractGate, GateLocation> gp in ic.locationHints)
                {
                    for (int i = 0; i < gp.Key.NumberOfInputs; i++)
                    {
                        Gates.Terminal inp = new Gates.Terminal(i, gp.Key);
                        if (c.GetSource(inp) != null)
                        {
                            c_CircuitConnection(c, inp,
                                c.GetSource(inp));
                        }
                    }
                }
            }//);

            this.Loaded += (sender, e) => { UpdateWireConnections(); };
        }


        protected GateInkCanvas(Gates.Circuit c, ICList icl)
        {
            InitializeComponent();

            #region Initialization

            this.icl = icl;
            this.c = c;

            c.CircuitConnection += new Gates.Circuit.CircuitConnectionEventHandler(c_CircuitConnection);
            c.ListChanged += new System.ComponentModel.ListChangedEventHandler(c_ListChanged);
            c.ReplaceGates += new Gates.Circuit.ReplaceGatesEventHandler(c_ReplaceGates);

            circuitInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            circuitInkCanvas.DefaultDrawingAttributes.Width = 2;

            // Add a listener to ResultsUpdated event.
            circuitInkCanvas.InkAnalyzer.ResultsUpdated += new ResultsUpdatedEventHandler(_inkAnalyzer_ResultsUpdated);
            circuitInkCanvas.Strokes.StrokesChanged += OnStrokesChanged;

            #endregion

            //only for piemenu selector trigger
            circuitInkCanvas.StylusDown += new StylusDownEventHandler(circuitInkCanvas_StylusDown);
            circuitInkCanvas.StylusMove += new StylusEventHandler(circuitInkCanvas_StylusMove);

            circuitInkCanvas.PreviewMouseDown += new MouseButtonEventHandler(circuitInkCanvas_PreviewMouseDown);
            circuitInkCanvas.PreviewStylusMove += new StylusEventHandler(circuitInkCanvas_PreviewStylusMove);
            circuitInkCanvas.StylusUp += new StylusEventHandler(circuitInkCanvas_StylusUp);
            circuitInkCanvas.StrokeCollected += new InkCanvasStrokeCollectedEventHandler(circuitInkCanvas_StrokeCollected);
        }

        void AnimateArrowInFlickDirection(double x1, double y1, double x2, double y2)
        {
            Arrow arrow = new Arrow();
            arrow.X1 = x1;
            arrow.Y1 = y1;
            arrow.X2 = x2;
            arrow.Y2 = y2;
            arrow.HeadWidth = 18;
            arrow.HeadHeight = 10;
            arrow.Stroke = Brushes.Red;
            arrow.StrokeThickness = 10.0;

            this.circuitInkCanvas.Children.Add(arrow);
            this.CurrentFlickArrow = arrow;

            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(1.0)));
            fadeOutAnimation.Completed += new EventHandler(ArrowFadeOutAnimationCompleted);

            fadeOutAnimation.Freeze();

            arrow.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        private void ArrowFadeOutAnimationCompleted(object sender, EventArgs e)
        {
            this.circuitInkCanvas.Children.Remove(CurrentFlickArrow);
        }

        #endregion

        #region Legacy Stable Code

        public UndoRedo.UndoManager UndoProvider
        {
            get
            {
                return _undman;
            }
            set
            {
                _undman = value;
                foreach (Gate uigate in gates.Values)
                {
                    if (uigate is UIGates.UserIO)
                        ((UIGates.UserIO)uigate).UndoProvider = UndoProvider;

                    if (uigate is UIGates.Comment)
                        ((UIGates.Comment)uigate).UndoProvider = UndoProvider;
                }
            }
        }

        public Gate FindGate(Gates.AbstractGate abGate)
        {
            return gates[abGate];
        }

        public void UpdateWireConnections()
        {
            foreach (KeyValuePair<Gates.Terminal, Wire> wire in wires)
                ((ConnectedWire)(wire.Value)).Connect();
        }

        #region Circuit Events

        private void c_ReplaceGates(Gates.Circuit sender, Dictionary<Gates.AbstractGate, Gates.AbstractGate> replacements)
        {
            foreach (KeyValuePair<Gates.AbstractGate, Gates.AbstractGate> replacement in replacements)
            {
                if (oldgatepositions.ContainsKey(replacement.Key) && replacement.Value != null)
                {
                    GateLocation np = oldgatepositions[replacement.Key];
                    gates[replacement.Value].Margin = new Thickness(np.x, np.y, 0, 0);
                    ((RotateTransform)gates[replacement.Value].RenderTransform).Angle = np.angle;
                    ((GateLocation)gates[replacement.Value].Tag).x = np.x;
                    ((GateLocation)gates[replacement.Value].Tag).y = np.y;
                    ((GateLocation)gates[replacement.Value].Tag).angle = np.angle;
                }
            }
            foreach (Gate tic in gates.Values)
            {
                if (tic is UIGates.IC)
                    ((UIGates.IC)tic).UpdateLocationHints(replacements);
            }

            UpdateLayout();
            // otherwise the wires don't know where to go
            UpdateWireConnections();
        }

        private void c_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemAdded)
            {
                if (!gates.ContainsKey(c[e.NewIndex]))
                    AddGate(c[e.NewIndex], new GateLocation());
            }
            if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
            {
                List<Gates.AbstractGate> toRemove = new List<Gates.AbstractGate>();
                // find and remove from canvas any removed gates
                foreach (KeyValuePair<Gates.AbstractGate, Gate> abg in gates)
                {
                    if (!c.Contains(abg.Key))
                    {
                        circuitInkCanvas.Children.Remove(abg.Value);
                        toRemove.Add(abg.Key);
                        oldgatepositions[abg.Key] = new GateLocation(abg.Value.Margin.Left, abg.Value.Margin.Top, ((RotateTransform)abg.Value.RenderTransform).Angle);
                    }
                }

                foreach (Gates.AbstractGate ab in toRemove)
                    gates.Remove(ab);

                // find and remove any stray wires (although should be none)
                List<Gates.Terminal> toRemoveW = new List<Gates.Terminal>();

                foreach (KeyValuePair<Gates.Terminal, Wire> wire in wires)
                {
                    if (!c.Contains(wire.Key.gate))
                    {
                        toRemoveW.Add(wire.Key);
                    }
                }

                foreach (Gates.Terminal t in toRemoveW)
                    wires.Remove(t);

            }
        }

        private void c_CircuitConnection(Gates.Circuit sender, Gates.Terminal input, Gates.Terminal output)
        {
            Wire cw = wires.ContainsKey(input) ? wires[input] : null;
            if (cw != null)
            {
                circuitInkCanvas.Children.Remove(cw);
                wires.Remove(input);
            }
            if (output != null)
            {
                Gate inp = gates[input.gate];
                Gate outp = gates[output.gate];
                ConnectedWire ncw = new ConnectedWire(output.gate, outp.FindTerminal(false, output.portNumber),
                    input.gate, inp.FindTerminal(true, input.portNumber));
                ncw.Value = output.gate.Output[output.portNumber];
                wires[input] = ncw;
                circuitInkCanvas.Children.Insert(0, ncw);
                ncw.Connect();

                ncw.MouseDown += (snd, e) =>
                {
                    if (!IsReadOnly)
                    {

                        Gates.Terminal t = c.GetSource(input);
                        c.Disconnect(input);

                        
                        if (UndoProvider != null)
                            UndoProvider.Add(new UndoRedo.Reverse(new UndoRedo.ConnectWire(c, t, input), "Disconnect Wire"));                        
                    }
                };
            }
        }

        #endregion

        public Gates.Circuit Circuit
        {
            get
            {
                return c;
            }
        }

        private void SetInfoLine(Gate g)
        {
            if (_ro)
            {
            }
            else
            {
                string infoline = "Left-drag to move, right-drag to rotate, left-drag terminals to connect";

                if (g is UIGates.IC)
                    infoline += ", double-click to view, right-click for options";

                if (g is UIGates.UserIO)
                    infoline += ", right-click to rename";

                if (g is UIGates.Numeric)
                    infoline += ", click on the representation to change";

                if (g is UIGates.Numeric || g is UIGates.Clock || g is UIGates.Comment)
                    infoline += ", click and type to enter a value";

                if (g.AbGate is Gates.IVariableInputs)
                {
                    // can only remove if more than 2
                    infoline += ", right-click to add" + (g.AbGate.NumberOfInputs > 2 ? "/remove" : "") + " inputs";
                }

            }
        }

        #region Add and Remove Gates

        public void RemoveGate(Gate uigate)
        {
            c.Remove(uigate.AbGate);
            uigate.MouseDown -= new MouseButtonEventHandler(uigate_MouseDown);
            //uigate.MouseUp -= new MouseButtonEventHandler(uigate_MouseUp);
            uigate.StylusUp -= new StylusEventHandler(uigate_StylusUp);

            if (uigate is UIGates.UserIO)
            {
                if (uigate is UIGates.UserInput)
                {
                    string inputLabel = ((UIGates.UserInput)uigate).UserIOLabel;
                    UserIOLabelList.inputLabelList.Add(inputLabel);
                }
                else if (uigate is UIGates.UserOutput)
                {
                    string outputLabel = ((UIGates.UserOutput)uigate).UserIOLabel;
                    UserIOLabelList.outputLabelList.Add(outputLabel);
                }
            }

            /*
            if (uigate is UIGates.IC)
                uigate.MouseDoubleClick -= new MouseButtonEventHandler(uigate_MouseDoubleClick);
            */

        }

        public void AddGate(Gate uigate, GateLocation pos)
        {
            Gates.AbstractGate gate = uigate.AbGate;

            gates[gate] = uigate;
            uigate.Margin = new Thickness(pos.x, pos.y, 0, 0);
            circuitInkCanvas.Children.Add(uigate);
            if (!c.Contains(gate))
                c.Add(gate);

            uigate.RenderTransform = new RotateTransform(pos.angle, uigate.Width / 2.0, uigate.Height / 2.0);
            uigate.Tag = new GateLocation() { x = pos.x, y = pos.y, angle = pos.angle };

            // NOTE that we need a separate angle and transform
            // for the snap-to to work properly
            // so I am using the tag to store the angle
            uigate.MouseDown += new MouseButtonEventHandler(uigate_MouseDown);
            //uigate.MouseUp += new MouseButtonEventHandler(uigate_MouseUp);
            uigate.StylusUp +=new StylusEventHandler(uigate_StylusUp);

          /*
            if (uigate is UIGates.IC)
            {
                uigate.MouseDoubleClick += new MouseButtonEventHandler(uigate_MouseDoubleClick);
                uigate.ContextMenu = new ContextMenu();
                MenuItem inline = new MenuItem();
                inline.Header = "Inline Circuit";
                inline.Tag = uigate;
                uigate.ContextMenu.Items.Add(inline);
                inline.Click += new RoutedEventHandler(inline_Click);
                uigate.ContextMenu.IsEnabled = !IsReadOnly;

            }
             * */

            // can add inputs
            /*
            if (uigate.AbGate is Gates.IVariableInputs)
            {
                uigate.ContextMenu = new ContextMenu();
                MenuItem addInput = new MenuItem();
                addInput.Header = "Add Input";
                addInput.Tag = uigate;
                uigate.ContextMenu.Items.Add(addInput);
                addInput.Click += (sender2, e2) =>
                {
                    Gates.AbstractGate newgate = ((Gates.IVariableInputs)uigate.AbGate).Clone(uigate.AbGate.NumberOfInputs + 1);
                    c.ReplaceGate(uigate.AbGate, newgate);
                   
                    if (UndoProvider != null)
                        UndoProvider.Add(new UndoRedo.ChangeNumInputs(c, uigate.AbGate, newgate));
                };
                if (uigate.AbGate.NumberOfInputs > 2)
                {
                    MenuItem removeInput = new MenuItem();
                    removeInput.Header = "Remove Input";
                    removeInput.Tag = uigate;
                    uigate.ContextMenu.Items.Add(removeInput);
                    removeInput.Click += (sender2, e2) =>
                    {
                        UndoRedo.Transaction removeInp = new UndoRedo.Transaction("Remove Input");

                        // remember wires connected to removed input
                        Gates.Terminal dest = new Gates.Terminal(uigate.AbGate.NumberOfInputs - 1, uigate.AbGate);
                        Gates.Terminal origin = c.GetSource(dest);
                        if (origin != null)
                            removeInp.Add(new UndoRedo.Reverse(new UndoRedo.ConnectWire(c, origin, dest)));

                        Gates.AbstractGate newgate = ((Gates.IVariableInputs)uigate.AbGate).Clone(uigate.AbGate.NumberOfInputs - 1);
                        c.ReplaceGate(uigate.AbGate, newgate);

                        removeInp.Add(new UndoRedo.ChangeNumInputs(c, uigate.AbGate, newgate));
                      
                        if (UndoProvider != null)
                            UndoProvider.Add(removeInp);
                    };
                }

                uigate.ContextMenu.IsEnabled = !IsReadOnly;
            }
             * 
             * */

            //BOKANG
            /*
            if (uigate is UIGates.UserIO) {
                ((UIGates.UserIO)uigate).UndoProvider = UndoProvider;
                if (uigate is UIGates.UserInput)
                {
                    string inputLabel = UserIOLabelList.inputLabelList[0];
                    ((UIGates.UserInput)uigate).UserIOLabel = inputLabel;
                    UserIOLabelList.inputLabelList.Remove(inputLabel);
                }
                else if (uigate is UIGates.UserOutput)
                {
                    string outputLabel = UserIOLabelList.outputLabelList[0];
                    ((UIGates.UserOutput)uigate).UserIOLabel = outputLabel;
                    UserIOLabelList.outputLabelList.Remove(outputLabel);
                }
            }
             * */
            
            if (uigate is UIGates.Comment)
                ((UIGates.Comment)uigate).UndoProvider = UndoProvider;
             SetInfoLine(uigate);
        }

        public void AddGate(Gates.AbstractGate gate, GateLocation pos)
        {
            // maybe we could use extension methods
            // to add a method to create a UIGate for each AbstractGate?

            Gate uigate;
            if (gate is Gates.BasicGates.And)
            {
                uigate = new UIGates.And((Gates.BasicGates.And)gate);
            }
            else if (gate is Gates.BasicGates.Not)
            {
                uigate = new UIGates.Not((Gates.BasicGates.Not)gate);
            }
            else if (gate is Gates.BasicGates.Or)
            {
                uigate = new UIGates.Or((Gates.BasicGates.Or)gate);
            }
            else if (gate is Gates.BasicGates.Nand)
            {
                uigate = new UIGates.Nand((Gates.BasicGates.Nand)gate);
            }
            else if (gate is Gates.BasicGates.Nor)
            {
                uigate = new UIGates.Nor((Gates.BasicGates.Nor)gate);
            }
            else if (gate is Gates.BasicGates.Xor)
            {
                uigate = new UIGates.Xor((Gates.BasicGates.Xor)gate);
            }
            else if (gate is Gates.BasicGates.Xnor)
            {
                uigate = new UIGates.Xnor((Gates.BasicGates.Xnor)gate);
            }
            else if (gate is Gates.BasicGates.Buffer)
            {
                uigate = new UIGates.Buffer((Gates.BasicGates.Buffer)gate);
            }
            else if (gate is Gates.IOGates.UserInput)
            {
                uigate = new UIGates.UserInput((Gates.IOGates.UserInput)gate);
            }
            else if (gate is Gates.IOGates.UserOutput)
            {
                uigate = new UIGates.UserOutput((Gates.IOGates.UserOutput)gate);
            }
            else if (gate is Gates.IOGates.AbstractNumeric)
            {
                uigate = new UIGates.Numeric((Gates.IOGates.AbstractNumeric)gate);
            }
            else if (gate is Gates.IOGates.Clock)
            {
                uigate = new UIGates.Clock((Gates.IOGates.Clock)gate);
            }
            else if (gate is Gates.IOGates.Comment)
            {
                uigate = new UIGates.Comment((Gates.IOGates.Comment)gate);
            }
            else if (gate is Gates.IC)
            {
                uigate = UIGates.IC.CreateFromTemplate((Gates.IC)gate, icl.GetIC(gate.Name));
            }
            else throw new ArgumentException("gate not of known subclass");

            AddGate(uigate, pos);
        }


        #endregion

        public bool IsReadOnly
        {
            get
            {
                return _ro;
            }
            set
            {
                _ro = value;
                foreach (Gate g in gates.Values)
                {
                    g.IsReadOnly = value;
                    if (g is UIGates.IC || g.AbGate is Gates.IVariableInputs)
                        g.ContextMenu.IsEnabled = !value;

                    SetInfoLine(g);
                }
            }
        }

        #region ICL
        /// <summary>
        /// Provide an IC List for this canvas to work with.
        /// After this value is set, consider called SetCaptureICLChanges
        /// </summary>
        public ICList ICL
        {
            set
            {
                icl.ChangeIC -= new ICList.ChangeICEventHandler(icl_ChangeIC);
                icl = value;
                //icl.ChangeIC += new ICList.ChangeICEventHandler(icl_ChangeIC);
            }
        }

        /// <summary>
        /// If any IC is replaced or changed in the ICL, the gate canvas should
        /// replace its visual representations with the new representation.
        /// Call this method to enable catching the ChangeIC event from the given icl.
        /// </summary>
        public void SetCaptureICLChanges()
        {
            icl.ChangeIC += new ICList.ChangeICEventHandler(icl_ChangeIC);

        }

        private void icl_ChangeIC(object sender, ICList.ChangeICEventArgs e)
        {

            if (e.newic == null)
                c.ReplaceICs(e.original.AbGate.Name, null);
            else
                c.ReplaceICs(e.original.AbGate.Name, (Gates.IC)e.newic.AbGate);
        }
        #endregion

        void _inkAnalyzer_ResultsUpdated(object sender, ResultsUpdatedEventArgs e)
        {
            if (circuitInkCanvas.InkFeedbackAdorner != null)
            {
                //cause the feedback adorner to repaint itself
                circuitInkCanvas.InkFeedbackAdorner.InvalidateVisual();
            }

            // If the user has made edits while analysis was being performed, trigger
            // BackgroundAnalyze again to analyze these changes
            if (!circuitInkCanvas.InkAnalyzer.DirtyRegion.IsEmpty)
            {
                circuitInkCanvas.InkAnalyzer.BackgroundAnalyze();
            }
        }

        private void OnStrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            // Update the ink data of the ink analyzer.
            if (e.Removed.Count > 0)
            {
                foreach (Stroke stroke in e.Removed)
                {
                    //we're removing this stroke so we don't need to listen 
                    //to StylusPointsChanged anymore
                    stroke.StylusPointsChanged -= OnStrokeStylusPointsChanged;
                }
                circuitInkCanvas.InkAnalyzer.RemoveStrokes(e.Removed);


            }
            if (e.Added.Count > 0)
            {
                foreach (Stroke stroke in e.Added)
                {
                    //listen for StylusPointsChanged, which can happen
                    //during move and resize operations
                    stroke.StylusPointsChanged += OnStrokeStylusPointsChanged;
                }
                circuitInkCanvas.InkAnalyzer.AddStrokes(e.Added);
                circuitInkCanvas.InkAnalyzer.SetStrokesType(e.Added, StrokeType.Unspecified);
            }
            circuitInkCanvas.InkAnalyzer.BackgroundAnalyze();
        }

        private void OnStrokeStylusPointsChanged(object sender, EventArgs e)
        {
            Stroke changedStroke = (Stroke)sender;

            //a stroke's StylusPoints have changed we need to find
            //all affected contextNodes's and mark the dirty region with them
            StrokeCollection strokesThatChanged = new StrokeCollection();
            strokesThatChanged.Add(changedStroke);
            ContextNodeCollection dirtyNodes =
                circuitInkCanvas.InkAnalyzer.FindInkLeafNodes(strokesThatChanged);

            foreach (ContextNode dirtyNode in dirtyNodes)
            {
                //let the analyzer know that where the stroke previously 
                //existed is now dirty
                circuitInkCanvas.InkAnalyzer.DirtyRegion.Union(dirtyNode.Location.GetBounds());
            }

            //let the analyzer know that the stroke data is no longer valid
            circuitInkCanvas.InkAnalyzer.ClearStrokeData(changedStroke);

            //finally, make the region where the stroke now exists dirty also
            circuitInkCanvas.InkAnalyzer.DirtyRegion.Union(changedStroke.GetBounds());

            circuitInkCanvas.InkAnalyzer.BackgroundAnalyze();
        }

        /// <summary>
        /// Given a point on the canvas, find the nearest "snap" point.  Useful
        /// for placing new gates.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point GetNearestSnapTo(Point p)
        {
            return new Point(Math.Round(p.X / GRID_SIZE) * GRID_SIZE,
                                        Math.Round(p.Y / GRID_SIZE) * GRID_SIZE);
        }

        /// <summary>
        /// Given a relative point on the GateCanvas control,
        /// adjust it using the current zoom and scroll settings
        /// to reflect an actual point on the unscrolled, unzoomed canvas.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point TranslateScrolledPoint(Point p)
        {
            Point np = new Point();
            np.X = GCScroller.HorizontalOffset + p.X;
            np.Y = GCScroller.VerticalOffset + p.Y;
            np.X /= _zoom;
            np.Y /= _zoom;
            return np;
        }

        //private static Rect GetBounds(IEnumerable<Gate> gts, double padding)
        //{
        //    double minx = 0, maxx = 0, miny = 0, maxy = 0;
        //    bool fst = true;
        //    foreach (Gate g in gts)
        //    {
        //        if (fst)
        //        {
        //            minx = g.Margin.Left;
        //            maxx = g.Margin.Left + g.Width;
        //            miny = g.Margin.Top;
        //            maxy = g.Margin.Top + g.Height;
        //            fst = false;
        //        }

        //        minx = Math.Min(minx, g.Margin.Left - padding);
        //        maxx = Math.Max(maxx, g.Margin.Left + g.Width + padding);
        //        miny = Math.Min(miny, g.Margin.Top - padding);
        //        maxy = Math.Max(maxy, g.Margin.Top + g.Height + padding);
        //    }

        //    return new Rect(minx, miny, maxx - minx, maxy - miny);
        //}

        ///// <summary>
        ///// Determine the extent of visual gates on this canvas, taking into account
        ///// any requested padding, and whether all gates should be considered or
        ///// only selected gates.
        ///// </summary>
        ///// <param name="padding"></param>
        ///// <param name="selectedOnly"></param>
        ///// <returns></returns>
        //public Rect GetBounds(double padding, bool selectedOnly)
        //{
        //    if (selectedOnly)
        //        return GetBounds(selected, padding);
        //    else
        //        return GetBounds(gates.Values, padding);
        //}


        public void SelectAll()
        {
            ClearSelection();

            foreach (Gate g in gates.Values)
            {
                g.Selected = true;
                selected.Add(g);
            }
        }

        private void SizeCanvas()
        {
            double maxx = GCScroller.ViewportWidth / _zoom, maxy = GCScroller.ViewportHeight / _zoom;
            foreach (Gate g in gates.Values)
            {
                maxx = Math.Max(maxx, g.Margin.Left + g.Width + 64);
                maxy = Math.Max(maxy, g.Margin.Top + g.Height + 64);
            }
            circuitInkCanvas.Width = maxx;
            circuitInkCanvas.Height = maxy;
        }

        public static readonly DependencyProperty ZoomCenterProperty =
            DependencyProperty.Register("ZoomCenter", typeof(Point), typeof(GateInkCanvas));

        public Point ZoomCenter
        {

            get
            {
                return (Point)GetValue(ZoomCenterProperty);
            }
            set
            {
                SetValue(ZoomCenterProperty, value);
            }
        }

        /// <summary>
        /// Calculate the center of the displayed area which would make a zoom center
        ///  to zoom "straight" in or out.
        /// </summary>
        public void SetZoomCenter()
        {
            double centerX = (GCScroller.HorizontalOffset + GCScroller.ViewportWidth / 2.0) / _zoom;
            double centerY = (GCScroller.VerticalOffset + GCScroller.ViewportHeight / 2.0) / _zoom;
            ZoomCenter = new Point(centerX, centerY);
        }

        /// <summary>
        /// Indicate if the user provided value in ZoomCenter should be used
        /// </summary>
        public bool UseZoomCenter { get; set; }

        /// <summary>
        /// Sets the zoom factor.  Changes to zoom occur based on the center of the displayed area,
        /// or, if UseZoomCenter is set to true, around the user defined zoom center.
        /// </summary>

        public double Zoom
        {
            set
            {

                double centerX = (GCScroller.HorizontalOffset + GCScroller.ViewportWidth / 2.0) / _zoom;
                double centerY = (GCScroller.VerticalOffset + GCScroller.ViewportHeight / 2.0) / _zoom;
                _zoom = value;
                circuitInkCanvas.LayoutTransform = new ScaleTransform(value, value);
                if (UseZoomCenter)
                {
                    centerX = ZoomCenter.X;
                    centerY = ZoomCenter.Y;
                }

                if (!double.IsNaN(centerX))
                {
                    GCScroller.ScrollToHorizontalOffset((centerX * _zoom) - GCScroller.ViewportWidth / 2.0);
                    GCScroller.ScrollToVerticalOffset((centerY * _zoom) - GCScroller.ViewportHeight / 2.0);
                }
            }
        }

        private void GCScroller_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_mute)
                SizeCanvas();
        }

        private void GCScroller_LayoutUpdated(object sender, EventArgs e)
        {
            if (!_mute)
                SizeCanvas();
        }

        private void GCScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (dragging == DragState.MOVE)
            {
                System.Threading.Thread.Sleep(100); // don't let them scroll themselves into oblivion
            }
        }

        public UIGates.IC CreateIC(string name, SELECTED_GATES selectedOnly)
        {
            Gates.Circuit nc;
            Dictionary<int, int> idxmap = null;

            if ((selected.Count > 1 && selectedOnly == SELECTED_GATES.SELECTED_IF_TWO_OR_MORE) || selectedOnly == SELECTED_GATES.SELECTED)
                nc = c.Clone(selected.ToList().ConvertAll(g => g.AbGate), out idxmap);
            else
                nc = c.Clone();

            ICBuilder icb = new ICBuilder(nc, new ICBuilder.GatePosition(gate =>
            {
                int idx = nc.IndexOf(gate);
                if (idxmap != null)
                {
                    foreach (KeyValuePair<int, int> kv in idxmap)
                        if (kv.Value == idx)
                        {
                            idx = kv.Key;
                            break;
                        }// this makes me want to cry
                    // there must be a better way to look up by value
                    // because the value is the new circuit idx
                    // and the key is the old circuit idx
                    // don't just reverse it on create
                    // we use it the other way too

                }

                Gates.AbstractGate oldgate = c[idx];
                return new GateLocation(gates[oldgate].Margin.Left, gates[oldgate].Margin.Top, ((RotateTransform)gates[oldgate].RenderTransform).Angle);
            }));
            return icb.CreateIC(name);
        }

        public UIGates.IC CreateIC()
        {
            return CreateIC(ICName, SELECTED_GATES.ALL);
        }

        protected enum DragState
        {
            NONE, MOVE, CONNECT_TO, CONNECT_FROM
        };

        public enum SELECTED_GATES
        {
            ALL, SELECTED, SELECTED_IF_TWO_OR_MORE
        };

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
                //BOKANG
               
                bool removeCurrentStroke = CheckRemoveGate(stroke);

                IEnumerable<starPadSDK.Geom.Pt> hull = stroq.ConvexHull();

                List<Point> hullTemp = new List<Point>();
                foreach (starPadSDK.Geom.Pt pt in hull)
                {
                    hullTemp.Add(new Point(pt.X, pt.Y));
                }

                StrokeCollection stks = circuitInkCanvas.Strokes.HitTest(hullTemp, 50);
                //StroqCollection stqs = inqCanvas.Stroqs.HitTest(hull, 1);
                if (stks.Count > 1)
                {
                    //inqCanvas.Stroqs.Remove(stqs);
                    circuitInkCanvas.Strokes.Remove(stks);
                    if (stks.Contains(stroke))
                    {
                        stks.Remove(stroke);
                    }

                    if(circuitInkCanvas.Strokes.Contains(stroke))
                    {
                        circuitInkCanvas.Strokes.Remove(stroke);
                    }
                    //inqCanvas.Stroqs.Remove(stroq);
                    return true;
                }

                if (removeCurrentStroke)
                {
                    if(circuitInkCanvas.Strokes.Contains(stroke))
                    {
                        circuitInkCanvas.Strokes.Remove(stroke);
                    }
                }

            }
            return false;
        }

        Deg fpdangle(Pt a, Pt b, Vec v)
        {
            return (a - b).Normalized().UnsignedAngle(v.Normalized());
        }

        public void ClearSelection()
        {
            foreach (Gate g in selected)
                g.Selected = false;

            selected.Clear();
        }

        #endregion
    
        private Rect GetTerminalBounds(Gate tg, Rect gRect, Gate.TerminalID tid)
        {
            //Warning: Without Rotation support right now
            //All the heuristic values
            
            //TODO tg rotation
            if(tg is UIGates.UserInput)
            {
               return new Rect(gRect.Left  + (gRect.Width * 3) / 4, gRect.Top, gRect.Width / 4, gRect.Height);
            }
            else if (tg is UIGates.UserOutput)
            {
                return new Rect(gRect.Left - 10, gRect.Top, gRect.Width / 4 + 10, gRect.Height);
            }
            else if(tg is UIGates.Not)
            {  
                if(tid.isInput)
                {
                    return new Rect(gRect.Left - 10, gRect.Top, gRect.Width / 4 + 10, gRect.Height);
                }
                else
                {
                    return new Rect(gRect.Left + (gRect.Width * 3) / 4, gRect.Top, gRect.Width / 4 + 10, gRect.Height);
                }
            }else
            {
                //AND,XOR and OR
                //Input Terminal
                if(tid.isInput)
                {
                    if (tid.ID == 0)
                    {
                        //Down input side
                        return new Rect(gRect.Left - 10, gRect.Top + gRect.Height / 2, gRect.Width / 4 + 10, gRect.Height / 2);
                    }
                    else
                    {
                        //Upper input side
                        return new Rect(gRect.Left - 10, gRect.Top, gRect.Width / 4 + 10, gRect.Height / 2);
                    }
                    
                }
                else
                {
                    return new Rect(gRect.Left + (gRect.Width * 3) / 4, gRect.Top, gRect.Width / 4 + 10, gRect.Height);
                }
               
            }
        }
        
        void uigate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mp2 = e.GetPosition(circuitInkCanvas);

            if (IsReadOnly)
                return;

            Gate tg = (Gate)sender;
            // to avoid sticking on other gates, move this one to the top
            circuitInkCanvas.Children.Remove(tg);
            circuitInkCanvas.Children.Add(tg);

            if (!tg.Selected)
            {
                selected.Add(tg);
                ((GateLocation)tg.Tag).x = tg.Margin.Left;
                ((GateLocation)tg.Tag).y = tg.Margin.Top;
                tg.Selected = true;
            }

            Rect gRect = new Rect(tg.Margin.Left, tg.Margin.Top, tg.Width, tg.Height);
            
            foreach (Gate.TerminalID tid in tg)
            {
                Rect tRect = GetTerminalBounds(tg, gRect, tid);
                
                bool condition = tRect.Contains(mp2);
                // if (tid.t.IsMouseOver)
                //if (mp2.X >= tRect.Value.Left - 10 && mp2.X <= tRect.Value.Right + 10 
                //   && mp2.Y >= tRect.Value.Top - 10 && mp2.Y <= tRect.Value.Bottom + 10)
                if(condition)
                {
                    // ok, so are we connecting to or from
                    // is this an input or output?
                    if (tid.isInput)
                    {
                        // can only connect from an input
                        // if there is no other connection here
                        if (wires.ContainsKey(new Gates.Terminal(tid.ID, tid.abgate)))
                            continue;

                        dragging = DragState.CONNECT_TO;
                        dragWire.Value = false;

                        // highlight all terminals which provide output
                        foreach (Gates.AbstractGate ag in gates.Keys)
                        {
                            for (int i = 0; i < ag.Output.Length; i++)
                            {
                                gates[ag].FindTerminal(false, i).t.Highlight = true;
                            }
                        }
                    }
                    else
                    {
                        dragging = DragState.CONNECT_FROM;
                        // TODO: if the value of the output changes
                        // while being dragged, this won't update
                        dragWire.Value = tid.abgate.Output[tid.ID];


                        // highlight all terminals which accept input
                        // note this is all inputs NOT already connected
                        foreach (Gates.AbstractGate ag in gates.Keys)
                        {
                            for (int i = 0; i < ag.NumberOfInputs; i++)
                            {
                                if (c.GetSource(new Gates.Terminal(i, ag)) == null)
                                    gates[ag].FindTerminal(true, i).t.Highlight = true;
                            }
                        }
                    }
                    beginTID = tid;

                    dragWire.Destination = tid.t.TranslatePoint(new Point(5, 5), circuitInkCanvas);
                    dragWire.Origin = tid.t.TranslatePoint(new Point(5, 5), circuitInkCanvas);

                    e.Handled = true;
                    return;

                }
            }

            dragging = DragState.MOVE;

            moves = new UndoRedo.Transaction(
                            (e.LeftButton == MouseButtonState.Pressed ?
                            "Move" : "Rotate") + " " +
                            (selected.Count == 1 ?
                            "Gate" : selected.Count.ToString() + " Gates"));
            
            e.Handled = true;
        }

        void uigate_StylusUp(object sender, StylusEventArgs e)
        {
            Point mp2 = e.GetPosition(circuitInkCanvas);
            Gate tg = (Gate)sender;

            Rect gRect = new Rect(tg.Margin.Left, tg.Margin.Top, tg.Width, tg.Height);

            if (dragging == DragState.CONNECT_FROM ||
            dragging == DragState.CONNECT_TO)
            {
                foreach (Gate.TerminalID tid in (Gate)sender)
                {
                    Rect tRect = GetTerminalBounds(tg, gRect, tid);
                    bool condition = tRect.Contains(mp2);
                    if(condition)
                    //if (mp2.X >= tRect.Value.Left - 10 && mp2.X <= tRect.Value.Right + 10
                    //   && mp2.Y >= tRect.Value.Top - 10 && mp2.Y <= tRect.Value.Bottom + 10)
                    //if (tid.t.IsMouseOver)
                    {
                        Gates.Terminal origin = null, dest = null;

                        if (tid.isInput && dragging == DragState.CONNECT_FROM &&
                            !wires.ContainsKey(new Gates.Terminal(tid.ID, tid.abgate)))
                        {
                            origin = new Gates.Terminal(beginTID.ID, beginTID.abgate);
                            dest = new Gates.Terminal(tid.ID, tid.abgate);
                        }


                        if (!tid.isInput && dragging == DragState.CONNECT_TO)
                        {
                            origin = new Gates.Terminal(tid.ID, tid.abgate);
                            dest = new Gates.Terminal(beginTID.ID, beginTID.abgate);

                        }

                        if (origin != null)
                        {
                            c[dest] = origin;
                            UndoRedo.ConnectWire cw = new UndoRedo.ConnectWire(c, origin, dest);

                            if (UndoProvider != null)
                                UndoProvider.Add(cw);
                        }

                        break;
                    }
                }
            }
        }


        //pie menu only
        public void circuitInkCanvas_StylusDown(object sender, StylusDownEventArgs e)
        {
            StylusPointCollection points = e.GetStylusPoints(this.circuitInkCanvas);
            _stylusDownTime = DateTime.Now;
            StartPoint = points[0];

            IsPieMenuVisible = false;
        }
        //pie menu only
        public void circuitInkCanvas_StylusMove(object sender, StylusEventArgs e)
        {
            DateTime now = DateTime.Now;
            StylusPointCollection points = e.GetStylusPoints(this.circuitInkCanvas);
            //check if the current stroke is trigger pie menu stroke
            //debug only
            TimeSpan testSpan = now - _stylusDownTime;

            if(!IsPieMenuVisible)
            {
                if (StrokeAnalyzer.IsTriggerPieMenuStroke(testSpan, StrokeInfo.pointDistance(StartPoint, points[points.Count - 1])))
                {
                    PieMenuEventArgs args = new PieMenuEventArgs(true);
                    args.Position = e.GetPosition(this);
                    triggerPieMenuHandler(this, args);
                    IsPieMenuVisible = true;
                
                    PieMenuHitTestEventArgs hitargs = new PieMenuHitTestEventArgs(e.GetPosition(this), e, PieMenuHitTestEventArgs.EventType.Down);
                    hitPieMenuHandler(this, hitargs);
                }
            }
           
            if(IsPieMenuVisible)
            {
                PieMenuHitTestEventArgs hitargs = new PieMenuHitTestEventArgs(e.GetPosition(this), e, PieMenuHitTestEventArgs.EventType.Move);
                hitPieMenuHandler(this, hitargs);
            }
           
        }     

        #region obsolete code

        /*
        void circuitInkCanvas_StylusMove(object sender, StylusEventArgs e)
        {
            if (IsReadOnly)
                return;

            TimeSpan ts = _stopWatch.Elapsed;
            if (ts.TotalSeconds - _lastElapsedSecs> 2)
            {
                _stopWatch.Stop();
                _stopWatch = Stopwatch.StartNew();
                MessageBox.Show("Popup");
            }

            Point mp2 = e.GetPosition(circuitInkCanvas);

            circuitInkCanvas.BringIntoView(new Rect(new Point(mp2.X - 10, mp2.Y - 10),
                new Point(mp2.X + 10, mp2.Y + 10)));

            switch (dragging)
            {
                case DragState.CONNECT_FROM:
                    dragWire.Destination = mp2;
                    break;
                case DragState.CONNECT_TO:
                    dragWire.Origin = mp2;
                    break;
                case DragState.MOVE:
                    #region DragState is Move
                    foreach (Gate g in selected)
                    {
                        //g.RenderTransform = new TranslateTransform(mp2.X, mp2.Y);
                        //Direct Move
                        
                            double dx = mp2.X - mp.X;
                            double dy = mp2.Y - mp.Y;
                            ((GateLocation)g.Tag).x = ((GateLocation)g.Tag).x + dx;
                            ((GateLocation)g.Tag).y = ((GateLocation)g.Tag).y + dy;
                            double cx = ((GateLocation)g.Tag).x % GRID_SIZE;
                            double cy = ((GateLocation)g.Tag).y % GRID_SIZE;

                            Point op = new Point(g.Margin.Left, g.Margin.Top);

                            if ((Math.Abs(cx) < DELTA_SNAP || Math.Abs(GRID_SIZE - cx) < DELTA_SNAP) &&
                                (Math.Abs(cy) < DELTA_SNAP || Math.Abs(GRID_SIZE - cy) < DELTA_SNAP))
                            {
                                g.Margin = new Thickness(Math.Round(g.Margin.Left / GRID_SIZE) * GRID_SIZE,
                                    Math.Round(g.Margin.Top / GRID_SIZE) * GRID_SIZE, 0, 0);

                            }
                            else
                            {
                                g.Margin = new Thickness(((GateLocation)g.Tag).x, ((GateLocation)g.Tag).y, 0, 0);
                            }

                            Point np = new Point(g.Margin.Left, g.Margin.Top);
                            if (op != np)
                                moves.Add(new UndoRedo.MoveGate(g, this, op, np));

                            SizeCanvas();
                            g.BringIntoView(); // still needed because gate larger than 20px block

                    }

                    UpdateWireConnections();
                    break;
                    #endregion
                case DragState.NONE:
                    #region Drag State is None
                    // not dragging
                    // creating a selection rectangle
                    if (ReadyToSelect)
                    {
                        double x1 = Math.Min(mp2.X, sp.X);
                        double width = Math.Abs(mp2.X - sp.X);

                        double y1 = Math.Min(mp2.Y, sp.Y);
                        double height = Math.Abs(mp2.Y - sp.Y);

                        dragSelect.Margin = new Thickness(x1, y1, 0, 0);
                        dragSelect.Width = width;
                        dragSelect.Height = height;
                        dragSelect.Visibility = Visibility.Visible;

                        // select any gates inside the rectangle
                        Rect select = new Rect(x1, y1, width, height);
                        foreach (Gate g in gates.Values)
                        {
                            Rect grect = new Rect(g.Margin.Left, g.Margin.Top, g.Width, g.Height);
                            if (select.IntersectsWith(grect) && !g.Selected)
                            {
                                g.Selected = true;
                                selected.Add(g);
                            }

                            // this is not the same as just "not" or else the above
                            if (!select.IntersectsWith(grect) && g.Selected)
                            {
                                g.Selected = false;
                                selected.Remove(g);
                            }
                        }
                    }
                    break;
                    #endregion
            }
            mp = mp2;
        }
         * 
         * */
        
        /*
        void circuitInkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsReadOnly)
                return;

            TimeSpan ts = _stopWatch.Elapsed;
            if (ts.TotalSeconds - _lastElapsedSecs > 2)
            {
                MessageBox.Show("Popup");
            }

            Point mp2 = e.GetPosition(circuitInkCanvas);

            circuitInkCanvas.BringIntoView(new Rect(new Point(mp2.X - 10, mp2.Y - 10),
                new Point(mp2.X + 10, mp2.Y + 10)));

            switch (dragging)
            {
                case DragState.CONNECT_FROM:
                    dragWire.Destination = mp2;
                    break;
                case DragState.CONNECT_TO:
                    dragWire.Origin = mp2;
                    break;
                case DragState.MOVE:
                    #region DragState is Move
                    foreach (Gate g in selected)
                    {
                        //g.RenderTransform = new TranslateTransform(mp2.X, mp2.Y);
                        //Direct Move
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            double dx = mp2.X - mp.X;
                            double dy = mp2.Y - mp.Y;
                            ((GateLocation)g.Tag).x = ((GateLocation)g.Tag).x + dx;
                            ((GateLocation)g.Tag).y = ((GateLocation)g.Tag).y + dy;
                            double cx = ((GateLocation)g.Tag).x % GRID_SIZE;
                            double cy = ((GateLocation)g.Tag).y % GRID_SIZE;

                            Point op = new Point(g.Margin.Left, g.Margin.Top);

                            if ((Math.Abs(cx) < DELTA_SNAP || Math.Abs(GRID_SIZE - cx) < DELTA_SNAP) &&
                                (Math.Abs(cy) < DELTA_SNAP || Math.Abs(GRID_SIZE - cy) < DELTA_SNAP))
                            {
                                g.Margin = new Thickness(Math.Round(g.Margin.Left / GRID_SIZE) * GRID_SIZE,
                                    Math.Round(g.Margin.Top / GRID_SIZE) * GRID_SIZE, 0, 0);

                            }
                            else
                            {
                                g.Margin = new Thickness(((GateLocation)g.Tag).x, ((GateLocation)g.Tag).y, 0, 0);
                            }

                            Point np = new Point(g.Margin.Left, g.Margin.Top);
                            if (op != np)
                                moves.Add(new UndoRedo.MoveGate(g, this, op, np));

                            SizeCanvas();
                            g.BringIntoView(); // still needed because gate larger than 20px block

                        }
                                               
                    }

                    UpdateWireConnections();
                    break;
                    #endregion
                case DragState.NONE:
                    #region Drag State is None
                        // not dragging
                        // creating a selection rectangle
                        if (ReadyToSelect)
                        {
                            double x1 = Math.Min(mp2.X, sp.X);
                            double width = Math.Abs(mp2.X - sp.X);

                            double y1 = Math.Min(mp2.Y, sp.Y);
                            double height = Math.Abs(mp2.Y - sp.Y);

                            dragSelect.Margin = new Thickness(x1, y1, 0, 0);
                            dragSelect.Width = width;
                            dragSelect.Height = height;
                            dragSelect.Visibility = Visibility.Visible;

                            // select any gates inside the rectangle
                            Rect select = new Rect(x1, y1, width, height);
                            foreach (Gate g in gates.Values)
                            {
                                Rect grect = new Rect(g.Margin.Left, g.Margin.Top, g.Width, g.Height);
                                if (select.IntersectsWith(grect) && !g.Selected)
                                {
                                    g.Selected = true;
                                    selected.Add(g);
                                }

                                // this is not the same as just "not" or else the above
                                if (!select.IntersectsWith(grect) && g.Selected)
                                {
                                    g.Selected = false;
                                    selected.Remove(g);
                                }
                            }
                        }
                        break;
                        #endregion 
            }
            mp = mp2;
        }
         * */

        /*
       void circuitInkCanvas_PreviewStylusDown(object sender, StylusDownEventArgs e)
       {
           if (EditingMode.currentEditingMode != EditingMode.EditingModeType.SketchLogicGate)
           {
               MessageBox.Show("Please Check Sketch Logic Diagram Radio Button", "TIP");
               return;
           }

           _lastElapsedSecs = _stopWatch.Elapsed.TotalSeconds;

           // only come here if the uigate doesn't handle it
           ClearSelection();

           Point mp2 = e.GetPosition(circuitInkCanvas);

           sp = new Point(mp2.X, mp2.Y);

           ReadyToSelect = true;

           onGateStroke = false;

           foreach (UIElement gate in circuitInkCanvas.Children)
           {
               if (gate is Gate)
               {
                   Gate g = gate as Gate;
                   Rect grect = new Rect(g.Margin.Left, g.Margin.Top, g.Width, g.Height);
                   if (mp2.X >= grect.Left && mp2.X <= grect.Right
                       && mp2.Y >= grect.Top && mp2.Y <= grect.Bottom)
                   {
                       onGateStroke = true;
                       uigate_MouseDown(g, e);

                       if (dragging == DragState.MOVE)
                       {
                           if (g is UIGates.UserInput)
                           {
                               UIGates.UserInput temp = g as UIGates.UserInput;
                               temp.r_MouseDown(this, e);
                           }
                       }
                       break;
                   }
               }
           }

           e.Handled = true;
       }
       */


        #endregion

        public void circuitInkCanvas_PreviewStylusMove(object sender, StylusEventArgs e)
        {
            if (IsReadOnly)
                return;

            Point mp2 = e.GetPosition(circuitInkCanvas);

            if (sender is ConnectedWire)
            {
                #region connectedWire Object

                onWireMoveStroke = true;

                ConnectedWire myWire = sender as ConnectedWire;

                Gate.TerminalID tid = myWire.OriginTerminalID;

                // ok, so are we connecting to or from
                // is this an input or output?
                if (tid.isInput)
                {
                    // can only connect from an input
                    // if there is no other connection here
                    //if (wires.ContainsKey(new Gates.Terminal(tid.ID, tid.abgate)))

                    dragging = DragState.CONNECT_TO;
                    dragWire.Value = false;

                    // highlight all terminals which provide output
                    foreach (Gates.AbstractGate ag in gates.Keys)
                    {
                        for (int i = 0; i < ag.Output.Length; i++)
                        {
                            gates[ag].FindTerminal(false, i).t.Highlight = true;
                        }
                    }
                }
                else
                {
                    dragging = DragState.CONNECT_FROM;
                    // TODO: if the value of the output changes
                    // while being dragged, this won't update
                    dragWire.Value = tid.abgate.Output[tid.ID];


                    // highlight all terminals which accept input
                    // note this is all inputs NOT already connected
                    foreach (Gates.AbstractGate ag in gates.Keys)
                    {
                        for (int i = 0; i < ag.NumberOfInputs; i++)
                        {
                            if (c.GetSource(new Gates.Terminal(i, ag)) == null)
                                gates[ag].FindTerminal(true, i).t.Highlight = true;
                        }
                    }
                }
                beginTID = tid;

                dragWire.Destination = tid.t.TranslatePoint(new Point(5, 5), circuitInkCanvas);
                dragWire.Origin = tid.t.TranslatePoint(new Point(5, 5), circuitInkCanvas);

                //if (dragging == DragState.MOVE)
                //{
                //    if (g is UIGates.UserInput)
                //    {
                //        UIGates.UserInput temp = g as UIGates.UserInput;
                //        temp.r_MouseDown(this, e);
                //    }
                //}
                //break;

                return;

                #endregion
            }

            circuitInkCanvas.BringIntoView(new Rect(new Point(mp2.X - 10, mp2.Y - 10),
                new Point(mp2.X + 10, mp2.Y + 10)));

            switch (dragging)
            {
                case DragState.CONNECT_FROM:

                    foreach (UIElement gate in circuitInkCanvas.Children)
                    {
                        if (gate is Gate)
                        {
                            Gate g = gate as Gate;
                            foreach (Gate.TerminalID tid in g)
                            {
                                tid.t.Background = Brushes.Transparent;
                            }
                         }
                    }
                   
                    // gate termination indication
                    foreach (UIElement gate in circuitInkCanvas.Children)
                    {
                        if (gate is Gate)
                        {
                            Gate g = gate as Gate;
                            Rect grect = new Rect(g.Margin.Left - 10, g.Margin.Top - 10, g.Width + 10, g.Height + 10);

                            bool condition = false;
                            condition = grect.Contains(mp2);
                            if (condition)
                            {
                                Rect gRect = new Rect(g.Margin.Left, g.Margin.Top, g.Width, g.Height);

                                foreach (Gate.TerminalID tid in g)
                                {
                                    Rect tRect = GetTerminalBounds(g, grect, tid);
                                    condition = tRect.Contains(mp2);
                                    if (condition)
                                    {
                                        tid.t.Background = Brushes.Yellow;
                                    }else
                                    {
                                        tid.t.Background = Brushes.Transparent;
                                    }
                                }
                            }
                        }
                    }
                    dragWire.Destination = mp2;
                    break;
                case DragState.CONNECT_TO:
                     foreach (UIElement gate in circuitInkCanvas.Children)
                    {
                        if (gate is Gate)
                        {
                            Gate g = gate as Gate;
                            foreach (Gate.TerminalID tid in g)
                            {
                                tid.t.Background = Brushes.Transparent;
                            }
                         }
                    }
                   
                    // gate termination indication
                    foreach (UIElement gate in circuitInkCanvas.Children)
                    {
                        if (gate is Gate)
                        {
                            Gate g = gate as Gate;
                            Rect grect = new Rect(g.Margin.Left - 10, g.Margin.Top - 10, g.Width + 10, g.Height + 10);

                            bool condition = false;
                            condition = grect.Contains(mp2);
                            if (condition)
                            {
                                Rect gRect = new Rect(g.Margin.Left, g.Margin.Top, g.Width, g.Height);

                                foreach (Gate.TerminalID tid in g)
                                {
                                    Rect tRect = GetTerminalBounds(g, grect, tid);
                                    condition = tRect.Contains(mp2);
                                    if (condition)
                                    {
                                        tid.t.Background = Brushes.Yellow;
                                    }else
                                    {
                                        tid.t.Background = Brushes.Transparent;
                                    }
                                }
                            }
                        }
                    }
                    dragWire.Origin = mp2;
                    break;
                case DragState.MOVE:
                    #region DragState is Move

                    foreach (Gate g in selected)
                    {
                        //g.RenderTransform = new TranslateTransform(mp2.X, mp2.Y);
                        //Direct Move

                        double dx = mp2.X - mp.X;
                        double dy = mp2.Y - mp.Y;
                        ((GateLocation)g.Tag).x = ((GateLocation)g.Tag).x + dx;
                        ((GateLocation)g.Tag).y = ((GateLocation)g.Tag).y + dy;
                        double cx = ((GateLocation)g.Tag).x % GRID_SIZE;
                        double cy = ((GateLocation)g.Tag).y % GRID_SIZE;

                        Point op = new Point(g.Margin.Left, g.Margin.Top);

                        if ((Math.Abs(cx) < DELTA_SNAP || Math.Abs(GRID_SIZE - cx) < DELTA_SNAP) &&
                            (Math.Abs(cy) < DELTA_SNAP || Math.Abs(GRID_SIZE - cy) < DELTA_SNAP))
                        {
                            g.Margin = new Thickness(Math.Round(g.Margin.Left / GRID_SIZE) * GRID_SIZE,
                                Math.Round(g.Margin.Top / GRID_SIZE) * GRID_SIZE, 0, 0);

                        }
                        else
                        {
                            g.Margin = new Thickness(((GateLocation)g.Tag).x, ((GateLocation)g.Tag).y, 0, 0);
                        }

                        Point np = new Point(g.Margin.Left, g.Margin.Top);
                        if (op != np)
                            moves.Add(new UndoRedo.MoveGate(g, this, op, np));

                        SizeCanvas();
                        g.BringIntoView(); // still needed because gate larger than 20px block

                    }

                    UpdateWireConnections();
                    break;
                    #endregion
                case DragState.NONE:
                    #region Drag State is None
                    // not dragging
                    // creating a selection rectangle
                    if (ReadyToSelect)
                    {
                        double x1 = Math.Min(mp2.X, sp.X);
                        double width = Math.Abs(mp2.X - sp.X);

                        double y1 = Math.Min(mp2.Y, sp.Y);
                        double height = Math.Abs(mp2.Y - sp.Y);

                        //dragSelect.Margin = new Thickness(x1, y1, 0, 0);
                        //dragSelect.Width = width;
                        //dragSelect.Height = height;
                        //dragSelect.Visibility = Visibility.Visible;

                        // select any gates inside the rectangle
                        Rect select = new Rect(x1, y1, width, height);
                        foreach (Gate g in gates.Values)
                        {
                            Rect grect = new Rect(g.Margin.Left, g.Margin.Top, g.Width, g.Height);
                            if (select.IntersectsWith(grect) && !g.Selected)
                            {
                                g.Selected = true;
                                selected.Add(g);
                            }

                            // this is not the same as just "not" or else the above
                            if (!select.IntersectsWith(grect) && g.Selected)
                            {
                                g.Selected = false;
                                selected.Remove(g);
                            }
                        }
                    }
                    break;
                    #endregion
            }

            mp = mp2;
        }

        public void circuitInkCanvas_StylusUp(object sender, StylusEventArgs e)
        {
            if (IsPieMenuVisible)
            {
                PieMenuHitTestEventArgs hitargs = new PieMenuHitTestEventArgs(e.GetPosition(this), e, PieMenuHitTestEventArgs.EventType.Up);
                hitPieMenuHandler(this, hitargs);
            }

            //Make Pie Menu Invisible
            PieMenuEventArgs args = new PieMenuEventArgs(false);
            triggerPieMenuHandler(this, args);

            Point mp2 = e.GetPosition(circuitInkCanvas);

            foreach (UIElement gate in circuitInkCanvas.Children)
            {
                if (gate is Gate)
                {
                    Gate g = gate as Gate;
                    Rect grect = new Rect(g.Margin.Left - 10, g.Margin.Top - 10, g.Width + 10, g.Height + 10);

                    bool condition = false;
                    condition = grect.Contains(mp2);
                    if (condition)
                    {
                        uigate_StylusUp(g ,e);
                        break;
                    }
                }else if(gate is ConnectedWire)
                {
                    HitTestResult result = VisualTreeHelper.HitTest(gate, mp2);
                    if(result != null)
                    {
                        Debug.WriteLine("Hit Test Wire circuitInkCanvas_StylusUp");
                        //Image stroke starts from one terminal and stop at this wire
                        //1. through this wire, create new wire to connect existing wire's 
                        //if stroke starts from input terminal from one gate, then search for input terminal from existing wire.
                        //otherwise stroke starts from the output termianl from one gate, then search for output terminal from existing wire. 
                        ConnectedWire myWire = gate as ConnectedWire;

                        //new wire's destination is mp2; new wire's orignal is ???
                        if (onGateStroke)
                        { 
                            //start the stroke from gate terminal
                            Gate.TerminalID tid = myWire.OriginTerminalID;

                            Gates.Terminal origin = null, dest = null;

                            if (tid.isInput && dragging == DragState.CONNECT_FROM &&
                                !wires.ContainsKey(new Gates.Terminal(tid.ID, tid.abgate)))
                            {
                                origin = new Gates.Terminal(beginTID.ID, beginTID.abgate);
                                dest = new Gates.Terminal(tid.ID, tid.abgate);
                            }


                            if (!tid.isInput && dragging == DragState.CONNECT_TO)
                            {
                                origin = new Gates.Terminal(tid.ID, tid.abgate);
                                dest = new Gates.Terminal(beginTID.ID, beginTID.abgate);

                            }

                            if (origin != null)
                            {
                                c[dest] = origin;
                                UndoRedo.ConnectWire cw = new UndoRedo.ConnectWire(c, origin, dest);

                                if (UndoProvider != null)
                                    UndoProvider.Add(cw);
                            }
                        }
                        break;
                    }
                }
            }

            dragging = DragState.NONE;
            //dragSelect.Width = 0;
            //dragSelect.Height = 0;
            //dragSelect.Margin = new Thickness(0, 0, 0, 0);
            //dragSelect.Visibility = Visibility.Hidden;

            dragWire.Destination = new Point(0, 0);
            dragWire.Origin = new Point(0, 0);

            // unhightlight all
            foreach (Gates.AbstractGate ag in gates.Keys)
            {

                for (int i = 0; i < ag.Output.Length; i++)
                {
                    gates[ag].FindTerminal(false, i).t.Highlight = false;
                }

                for (int i = 0; i < ag.NumberOfInputs; i++)
                {
                    gates[ag].FindTerminal(true, i).t.Highlight = false;
                }
            }

            if (UndoProvider != null && moves != null && moves.Count > 0)
                UndoProvider.Add(moves);
            moves = null;

            ReadyToSelect = false;
        }

        public void circuitInkCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // only come here if the uigate doesn't handle it
            ClearSelection();

            Point mp2 = e.GetPosition(circuitInkCanvas);
            sp = new Point(mp2.X, mp2.Y);

            ReadyToSelect = true;
            onGateStroke = false;
        
            foreach (UIElement gate in circuitInkCanvas.Children)
            {
                if (gate is Gate)
                {
                    Gate g = gate as Gate;
                    Rect grect = new Rect(g.Margin.Left - 10, g.Margin.Top - 10, g.Width + 10, g.Height + 10);
                    bool condition = false;
                    condition = grect.Contains(mp2);
                    if (condition)
                    {
                        onGateStroke = true;

                        uigate_MouseDown(g, e);

                        if (dragging == DragState.MOVE)
                        {
                            if (g is UIGates.UserInput)
                            {
                                UIGates.UserInput temp = g as UIGates.UserInput;
                                temp.r_MouseDown(this, e);
                            }
                        }
                        break;
                    }
                }

            }

            if (sender is ConnectedWire)
            {
                onWireStroke = true;
                //Debug.WriteLine("HitTest Wire start at " + mp2.ToString());

                ConnectedWire myWire = sender as ConnectedWire;

                Gate.TerminalID tid = myWire.OriginTerminalID;

                // ok, so are we connecting to or from
                // is this an input or output?
                if (tid.isInput)
                {
                    // can only connect from an input
                    // if there is no other connection here
                    //if (wires.ContainsKey(new Gates.Terminal(tid.ID, tid.abgate)))
                    
                    dragging = DragState.CONNECT_TO;
                    dragWire.Value = false;

                    // highlight all terminals which provide output
                    foreach (Gates.AbstractGate ag in gates.Keys)
                    {
                        for (int i = 0; i < ag.Output.Length; i++)
                        {
                            gates[ag].FindTerminal(false, i).t.Highlight = true;
                        }
                    }
                }
                else
                {
                    dragging = DragState.CONNECT_FROM;
                    // TODO: if the value of the output changes
                    // while being dragged, this won't update
                    dragWire.Value = tid.abgate.Output[tid.ID];


                    // highlight all terminals which accept input
                    // note this is all inputs NOT already connected
                    foreach (Gates.AbstractGate ag in gates.Keys)
                    {
                        for (int i = 0; i < ag.NumberOfInputs; i++)
                        {
                            if (c.GetSource(new Gates.Terminal(i, ag)) == null)
                                gates[ag].FindTerminal(true, i).t.Highlight = true;
                        }
                    }
                }
                beginTID = tid;

                dragWire.Destination = tid.t.TranslatePoint(new Point(5, 5), circuitInkCanvas);
                dragWire.Origin = tid.t.TranslatePoint(new Point(5, 5), circuitInkCanvas);

                //if (dragging == DragState.MOVE)
                //{
                //    if (g is UIGates.UserInput)
                //    {
                //        UIGates.UserInput temp = g as UIGates.UserInput;
                //        temp.r_MouseDown(this, e);
                //    }
                //}
                //break;
            }

            e.Handled = true;
        }
      
        public void circuitInkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            CurrentStroke = e.Stroke;

            if (!circuitInkCanvas.Strokes.Contains(e.Stroke))
            {
                circuitInkCanvas.Strokes.Add(e.Stroke);
            }

            #region Flick Detection

            //Flick Gesture
            TimeSpan timespan = DateTime.Now - _stylusDownTime;

            StrokeInfo stroke = new StrokeInfo(CurrentStroke, timespan);

            //Real Analysis
            if (StrokeAnalyzer.Instance.IsFlick(stroke) && 1 == 0)
            {
                FlickDirections direction = StrokeAnalyzer.Instance.DetectFlickDirection(stroke);
                StylusPoint PointA = stroke.StartPoint;
                StylusPoint PointB = stroke.EndPoint;
                AnimateArrowInFlickDirection(PointA.X, PointA.Y, PointB.X, PointB.Y);

                //Throw one gate xaml on the canvas
                Gate newGate = null;

                switch (direction)
                {
                    case FlickDirections.UpLeft:
                        newGate = new UserOutput();
                        break;
                    case FlickDirections.DownLeft:
                        newGate = new Xor();
                        //OR
                        break;
                    case FlickDirections.Down:
                        newGate = new And();
                        //AND
                        break;
                    case FlickDirections.DownRight:
                        //NOT
                        newGate = new Not();
                        break;
                    case FlickDirections.Up:
                        //INPUT
                        newGate = new Or();
                        break;
                    case FlickDirections.UpRight:
                        //OUTPUT
                        newGate = new UserInput();
                        break;
                }
                //GateLocation location = new GateLocation();
                //Heuristic
                //BO KANG??
                Point temp = new Point(PointA.X - 30, PointA.Y - 30);

                this.AddGate(newGate, new GateLocation(this.GetNearestSnapTo(this.TranslateScrolledPoint(temp))));
                //this.AddGate(newGate, location);
                this.circuitInkCanvas.UpdateLayout();
                this.circuitInkCanvas.Strokes.Remove(CurrentStroke);
            }

            #endregion

            /* check for drag Gate Stroke*/
            if (onGateStroke)
            {
                circuitInkCanvas.Strokes.Remove(e.Stroke);
                return;
            }

            /*check for draw wire stroke*/
            if(onWireStroke)
            {
                circuitInkCanvas.Strokes.Remove(e.Stroke);
                onWireStroke = false;
                return;
            }else
            {
                if(onWireMoveStroke)
                {
                    circuitInkCanvas.Strokes.Remove(e.Stroke);
                    onWireMoveStroke = false;
                    return;
                }
            }

            //Debug.WriteLine("Stroke Bound Percentage is " + stroke.BoundPercentage);
            /* Test if the current stroke is wire connect stroke */
            if (stroke.BoundPercentage > 10)
            {
                return;
            }

            //Debug.WriteLine("Stroke BoundPercentage is " + stroke.BoundPercentage.ToString());

            /* check for scribble delete */
            if (ScribbleDelete(e.Stroke)) return;
        }

        private bool CheckRemoveGate(Stroke stroke)
        {
            Rect strokeBound = stroke.GetBounds();

            bool removeGate = false;

            List<Gate> removedGates = new List<Gate>();
            
            for(int i = 0; i < circuitInkCanvas.Children.Count; i++)
            {
                UIElement gate = circuitInkCanvas.Children[i];
                
                if (gate is Gate)
                {
                    Gate g = gate as Gate;

                    Rect grect = new Rect(g.Margin.Left + 40, g.Margin.Top + 40, g.Width - 40, g.Height - 40);

                    if (strokeBound.Contains(grect))
                    {   
                        //this.RemoveGate(g);
                        removedGates.Add(g);
                        removeGate = true;
                    } 
                }else if(gate is ConnectedWire)
                {
                    ConnectedWire wire = gate as ConnectedWire;

                    //Debug.WriteLine("Stroke Rect Bound: " + strokeBound.ToString());
                    //Debug.WriteLine("Wire Origin is " + wire.Origin.ToString());
                    //Debug.WriteLine("Wire Destination is " + wire.Destination.ToString());

                    //Debug.WriteLine("Wire Outer Margin is " + wire.Outer.Margin.ToString());

                    HitTestResult result = null;
                    int count = 0;
                    foreach(StylusPoint pt in stroke.StylusPoints)
                    {
                        result = VisualTreeHelper.HitTest(wire, pt.ToPoint());
                        if(result != null)
                        {
                            count++;
                        }
                    }

                    //Debug.WriteLine("Count is " + count.ToString());
                    if(count >= 3)
                    {

                        //trigger remove wire
                        //From connectedWire to get terminal

                        Gates.Terminal t = new Gates.Terminal(wire.DestTerminalID.ID, wire.DestinationGate);
                        c.Disconnect1(t);
                        circuitInkCanvas.Children.Remove(wire);

                        /*
                        for (int j = 0; j < wire.DestinationGate.NumberOfInputs; j++)
                        {
                            Gates.Terminal t = new Gates.Terminal(j, wire.DestinationGate);

                            //Gates.Terminal t = new Gates.Terminal(j, wire.OriginGate);
                            if (t != null)
                            {
                                c.Disconnect1(t);

                                circuitInkCanvas.Children.Remove(wire);
                            }
                        }
                         */
                        
                        removeGate = true;
                    }
                }
            }
            
            if(removeGate)
            {
                foreach (Gate g in removedGates)
                {
                    this.RemoveGate(g);
                } 
                return true;
            }

            return false;
         }
    }


    public class PieMenuHitTestEventArgs : EventArgs
    {
        public Point Pt { get; set; }

        public StylusEventArgs EventArgs { get; set; }

        public EventType DragAndDropEventType;

        public static Point FinalPt { get; set; }

        public enum EventType
        {
          Down, Move, Up
        }

        public PieMenuHitTestEventArgs(Point pt, StylusEventArgs eventArgs, EventType eventType)
            : base()
        {
            Pt = pt;
            EventArgs = eventArgs;
            DragAndDropEventType = eventType;
            if(eventType == EventType.Down)
            {
                PieMenuHitTestEventArgs.FinalPt = pt;
            }
        }
    }

    public class PieMenuEventArgs : EventArgs
    {
        private bool _isVisible;

        public bool IsVisible 
        {
            get { return _isVisible; }
        }

        private Point _position;

        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public PieMenuEventArgs(bool isVisible) : base()
        {
            _isVisible = isVisible;
        }
    }
}
