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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media.Animation;
using System.Xml.Linq;

using LogicPad2Util;

namespace GatesWpf
{
    /// <summary>
    /// Interaction logic for DiagramRepresentation.xaml
    /// </summary>
    public partial class DiagramRepresentation : UserControl, IPassable
    {
        public static string APP_TITLE;
        public static string APP_VERSION;
        public static string APP_COPYRIGHT;

        public static string LOAD_ON_START = "";

        private ICList icl;
        private string _filename;
        private EditLevel _myEditLevel;
        private ShadowBox sbZoom, sbSpeed, sbGates;
        private UIGates.IC _basedon = null;
        private UndoRedo.Transaction moves;
        
        /// <summary>
        /// Replace the existing canvas with a new canvas, based on the IC given.
        /// This is similar to closing this window and replacing it with another window
        /// based on that IC instead.
        /// </summary>
        /// <param name="newgcic"></param>
        public void RefreshGateCanvas(UIGates.IC newgcic)
        {
            Grid1.Children.Remove(gateCanvas);

            gateCanvas.Circuit.Stop();

            gateCanvas = new GateCanvas(newgcic, icl);
            gateCanvas.UndoProvider = (UndoRedo.UndoManager)Resources["undoManager"];
            this.UnregisterName("gateCanvas");
            this.RegisterName("gateCanvas", gateCanvas);
            Grid1.Children.Insert(0 /*Grid1.Children.Count - 4*/,gateCanvas);
            Grid.SetColumn(gateCanvas, 1);
            Grid.SetRow(gateCanvas, 1);
            _basedon = newgcic;

            if (!string.IsNullOrEmpty(newgcic.AbGate.Name))
            {
                _filename = newgcic.AbGate.Name;
                spGates.ICName = newgcic.AbGate.Name;
                UpdateTitle();
            }

            if (MyEditLevel == EditLevel.FULL || MyEditLevel == EditLevel.EDIT)
            {
                // monitor the clipboard to provide cut/copy/paste visibility
                gateCanvas.selected.ListChanged += (s2, e2) =>
                {
                    btnCopy.IsEnabled = gateCanvas.selected.Count > 0;
                    btnCut.IsEnabled = gateCanvas.selected.Count > 0;
                    btnCopyAsImage.IsEnabled = gateCanvas.selected.Count > 0;
                };
            }

            if (_myEditLevel == EditLevel.FULL)
                gateCanvas.Circuit.Start();

            gateCanvas.SetCaptureICLChanges();

            gateCanvas.Zoom = slZoom.Value;

            gateCanvas.UpdateLayout();
            gateCanvas.UpdateWireConnections();

            
            
        }

        /// <summary>
        /// The "edit" permissions of a window
        /// </summary>
        public enum EditLevel
        {
            /// <summary>
            /// Full applies only to the main window.  Full indicates all application
            /// control.  Only one full window should exist.
            /// </summary>
            FULL, 

            /// <summary>
            /// Edit applies changes to be applied to the circuit, but not application-level
            /// operations like creating an IC or saving.
            /// </summary>
            EDIT, 

            /// <summary>
            /// View allows only observation of a circuit.
            /// </summary>
            VIEW
        }
        
        /// <summary>
        /// Gets the edit level of this window
        /// </summary>
        public EditLevel MyEditLevel
        {
            get
            {
                return _myEditLevel;
            }
        }

        #region Constructors
        protected DiagramRepresentation(EditLevel e)
        {
            InitializeComponent();

            UserControlStatus = UserControlStatus.None;

            _myEditLevel = e;

            EventDispatcher.BatchDispatcher = Dispatcher;

            gateCanvas.Circuit.Start();


            // Everybody gets zoom
            sbZoom = new ShadowBox();
            sbZoom.Margin = new Thickness(20);
            Grid1.Children.Remove(spZoom);
            sbZoom.Children.Add(spZoom);
            spZoom.Background = Brushes.Transparent;
            sbZoom.VerticalAlignment = VerticalAlignment.Top;
            sbZoom.HorizontalAlignment = HorizontalAlignment.Right;
            //Grid1.Children.Add(sbZoom);
            //Grid.SetColumn(sbZoom, 1);
            //Grid.SetRow(sbZoom, 1);

            // everybody gets view keys
            this.PreviewKeyDown += new KeyEventHandler(Window1_View_KeyDown);

            Grid1.Children.Remove(spGates);
            if (e == EditLevel.FULL ||
                e == EditLevel.EDIT)
            {


                // delete for edit or full
                this.PreviewKeyDown += new KeyEventHandler(Window1_EditFull_KeyDown);

                this.PreviewKeyUp += (s2, e2) =>
                {
                    // add moves if needed
                    if (moves != null)
                        ((UndoRedo.UndoManager)Resources["undoManager"]).Add(moves);

                    moves = null;
                };

                // gates for edit or full

                sbGates = new ShadowBox();
                sbGates.Margin = new Thickness(20, 20, 20, 20);
                sbGates.Children.Add(spGates);
                spGates.Background = Brushes.Transparent;
                sbGates.VerticalAlignment = VerticalAlignment.Center;
                sbGates.HorizontalAlignment = HorizontalAlignment.Left;
                Grid1.Children.Add(sbGates);
                Grid.SetColumn(sbGates, 1);
                Grid.SetRow(sbGates, 1);

                // edit or full get undo and edit
                tbUndo.Visibility = Visibility.Visible;
                tbEdit.Visibility = Visibility.Visible;

                // monitor the clipboard to provide cut/copy/paste visibility
                gateCanvas.selected.ListChanged += (s2, e2) =>
                {
                    btnCopy.IsEnabled = gateCanvas.selected.Count > 0;
                    btnCut.IsEnabled = gateCanvas.selected.Count > 0;
                    btnCopyAsImage.IsEnabled = gateCanvas.selected.Count > 0;
                };
               
            }


            Grid1.Children.Remove(spSpeed);
            if (e == EditLevel.FULL)
            {
                // speed only for the main window
                sbSpeed = new ShadowBox();
                sbSpeed.Margin = new Thickness(20, 20, 175, 20);
                sbSpeed.Children.Add(spSpeed);
                spSpeed.Background = Brushes.Transparent;
                sbSpeed.VerticalAlignment = VerticalAlignment.Top;
                sbSpeed.HorizontalAlignment = HorizontalAlignment.Right;
                Grid1.Children.Add(sbSpeed);
                Grid.SetColumn(sbSpeed, 1);
                Grid.SetRow(sbSpeed, 1);

                // otherwise the defaults mess it up when you open a new window
                slSpeed.ValueChanged += (sender2, e2) =>
                {
                    Gates.PropagationThread.SLEEP_TIME = (int)slSpeed.Value;
                };

                // full also gets file and ic
                tbFile.Visibility = Visibility.Visible;
                tbIC.Visibility = Visibility.Visible;
            }

            if (e == EditLevel.EDIT)
            {
                // can't edit the user gates in this view
                spGates.IsReadOnly = true;
            }

            /*
            this.Loaded += (sender2, e2) => 
            {
                ((UndoRedo.UndoManager)Resources["undoManager"]).SetSavePoint(); 
                UpdateTitle();
                lblAppTitle.Text = APP_TITLE;
                lblAppVersion.Text = APP_VERSION;
                lblAppCopyright.Text = APP_COPYRIGHT;

            };
             * 
             * */

            this.PreviewMouseWheel += (sender, e2) =>
            {
                if (e2.Delta > 0)
                    slZoom.Value += 0.1;
                else
                    slZoom.Value -= 0.1;

                e2.Handled = true;

            };


            //BOKANG
            /*

            ((UndoRedo.UndoManager)Resources["undoManager"]).PropertyChanged += (sender2, e2) =>
            {
                UpdateTitle(); // look for modified or not
            };
             * 
             * */

            //BOKANG
            //InfoLine.GetInstance().PropertyChanged += InfoLine_PropertyChanged;


            //BOKANG

            if (sbSpeed != null)
                sbSpeed.Visibility = Visibility.Collapsed;

            if (myToolbarTray != null)
                myToolbarTray.Visibility = Visibility.Collapsed;

            spAppInfo.Visibility = Visibility.Collapsed;

            lblInfoLine.Visibility = Visibility.Collapsed;

            spGates.Visibility = Visibility.Collapsed;     
        }

        //BOKANG 
        public DiagramRepresentation() : this(EditLevel.EDIT)
        {

            AssemblyTitleAttribute title;
            AssemblyCopyrightAttribute copyright;
            Assembly aAssembly = Assembly.GetExecutingAssembly();
            
            
            title = (AssemblyTitleAttribute)
                    AssemblyTitleAttribute.GetCustomAttribute(
                aAssembly, typeof(AssemblyTitleAttribute));

            copyright = (AssemblyCopyrightAttribute)
                    AssemblyCopyrightAttribute.GetCustomAttribute(
                aAssembly, typeof(AssemblyCopyrightAttribute));
            APP_TITLE = title.Title;
            APP_VERSION = aAssembly.GetName().Version.ToString();
            APP_COPYRIGHT = copyright.Copyright;
            
            icl = new ICList();
            
            gateCanvas.ICL = icl;
            gateCanvas.UndoProvider = (UndoRedo.UndoManager)Resources["undoManager"];
            gateCanvas.SetCaptureICLChanges();
            spGates.ICList = icl;
            spGates.UndoProvider = (UndoRedo.UndoManager)Resources["undoManager"];

            this.Loaded += (s2, e2) => { Gates.IOGates.Clock.CalculatePrecession(); };

            if (!string.IsNullOrEmpty(LOAD_ON_START))
            {
                try
                {
                    CircuitXML cxml = new CircuitXML(icl);
                    RefreshGateCanvas(cxml.Load(LOAD_ON_START, icl.Add));

                    btnSave.IsEnabled = true;
                    _filename = LOAD_ON_START;
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load requested circuit, reason: " + ex.ToString());
                }

            }
            
        }

        /// <summary>
        /// Create a window based on a given IC and IC List.  Provide
        /// an edit level of either View or Edit as appropriate.
        /// </summary>
        /// <param name="IC"></param>
        /// <param name="useicl"></param>
        /// <param name="el"></param>
        public DiagramRepresentation(UIGates.IC IC, ICList useicl, EditLevel el)
            : this(el)
        {
            icl = useicl;

            gateCanvas.ICL = icl;
            gateCanvas.UndoProvider = (UndoRedo.UndoManager)Resources["undoManager"];
            spGates.ICList = icl;
            _filename = IC.AbGate.Name;


            this.Loaded += (sender, e) =>
            {
                RefreshGateCanvas(IC);

                spGates.ICName = IC.AbGate.Name;
                if (el == EditLevel.VIEW)
                    gateCanvas.IsReadOnly = true;

                Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        Focus();
                    }));

            };

        }

        #endregion

        private void Window1_Closing(object sender, CancelEventArgs e)
        {
            // only for original full window

            //BOKANG
            //e.Cancel = !QuerySave();
        }

        /// <summary>
        /// Gets the IC this window is based on, if any.
        /// </summary>
        public UIGates.IC BasedOn
        {
            get
            {
                return _basedon;
            }
        }

        /// <summary>
        /// Creates an IC from this window's circuit.
        /// This just calls CreateIC in the appropriate gate canvas.
        /// </summary>
        /// <returns></returns>
        public UIGates.IC GetIC()
        {
            return gateCanvas.CreateIC();
        }

        private void KeyRotateGates(int degree)
        {
            foreach (Gate g in gateCanvas.selected)
            {
                double origin = ((RotateTransform)g.RenderTransform).Angle;
                double dest = origin + degree;
                gateCanvas.UndoProvider.Add(new UndoRedo.RotateGate(g, gateCanvas, origin, dest));

                ((RotateTransform)g.RenderTransform).Angle = dest;
                ((GateLocation)g.Tag).angle = dest;
            }
            gateCanvas.UpdateWireConnections();
        }

        private void KeyMoveGates(int dx, int dy)
        {
            foreach (Gate g in gateCanvas.selected)
            {
                moves.Add(new UndoRedo.MoveGate(g, gateCanvas, new Point(g.Margin.Left, g.Margin.Top), new Point(g.Margin.Left + dx, g.Margin.Top + dy)));
                g.Margin = new Thickness(g.Margin.Left + dx, g.Margin.Top + dy, 0, 0);
                ((GateLocation)g.Tag).x += dx;
                ((GateLocation)g.Tag).y += dy;
            }
            gateCanvas.UpdateWireConnections();
        }

        private void Window1_View_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.P)
                {
                    btnPrint_Click(sender, e);
                }

                // zoom controls
                if (e.Key == Key.D0 || e.Key == Key.NumPad0)
                {
                    btnFitToScreen_Click(sender, e);
                }
                if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                {
                    btnActualSize_Click(sender, e);
                }
                if (e.Key == Key.Add || e.Key == Key.OemPlus)
                {
                    slZoom.Value += 0.1;
                }
                if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                {
                    slZoom.Value -= 0.1;
                }
            }
        }

        private void Window1_EditFull_KeyDown(object sender, KeyEventArgs e)
        {
            // delete all selected gates
            if (e.Key == Key.Delete)
            {
                gateCanvas.UndoProvider.Add(gateCanvas.DeleteSelectedGates());
            }

            if (e.Key == Key.Escape)
            {
                DragDrop.DragDropHelper.Cancel();
            }

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.S && MyEditLevel == EditLevel.FULL)
                {
                    if (btnSave.IsEnabled)
                        btnSave_Click(sender, e);
                    else
                        btnSave_As_Click(sender, e);
                }

                if (e.Key == Key.X && btnCut.IsEnabled)
                {
                    btnCut_Click(sender, e);
                }
                if (e.Key == Key.C && btnCopy.IsEnabled)
                {
                    btnCopy_Click(sender, e);
                }
                if (e.Key == Key.V && btnPaste.IsEnabled)
                {
                    btnPaste_Click(sender, e);
                }

                if (e.Key == Key.Z && btnUndo.IsEnabled)
                {
                    btnUndo_Click(sender, e);
                }
                if (e.Key == Key.Y && btnRedo.IsEnabled)
                {
                    btnRedo_Click(sender, e);
                }
                if (e.Key == Key.A)
                {
                    gateCanvas.SelectAll();
                }

                // gate rotation
                if (e.Key == Key.Right)
                {
                    KeyRotateGates(90);
                }
                if (e.Key == Key.Left)
                {
                    KeyRotateGates(-90);
                }
                

            }
            else
            { // not using ctrl

                if ((e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.Down || e.Key == Key.Up) && gateCanvas.selected.Count > 0 && moves == null)
                {
                    moves = new UndoRedo.Transaction("Move " +
                                (gateCanvas.selected.Count == 1 ?
                                "Gate" : gateCanvas.selected.Count.ToString() + " Gates"));
                }

                // moving gates
                if (e.Key == Key.Right)
                {
                    KeyMoveGates(1, 0);
                }
                if (e.Key == Key.Left)
                {
                    KeyMoveGates(-1, 0);
                }
                if (e.Key == Key.Up)
                {
                    KeyMoveGates(0, -1);
                }
                if (e.Key == Key.Down)
                {
                    KeyMoveGates(0, 1);
                }

            }

        }

        private void btnCreateIC_Click(object sender, RoutedEventArgs e)
        {
            

            UIGates.IC nic = gateCanvas.CreateIC(icl.GenerateAvailableName("Untitled"), GateCanvas.SELECTED_GATES.SELECTED_IF_TWO_OR_MORE);

            icl.Add(nic);
            // can't call seteditname on nic directly
            // because it doesn't exist in the selector
            // because the selector makes an instance
            // so have the selector redirect the request

            // bg delay is due to animation effect time
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s2, e2) => { System.Threading.Thread.Sleep(500); };
            bg.RunWorkerCompleted += (s2, e2) => 
            {
                spGates.SetEditName(nic.AbGate.Name);
            };
            bg.RunWorkerAsync();

            ((UndoRedo.UndoManager)Resources["undoManager"]).Add(new UndoRedo.CreateIC(icl, nic));


        }

        #region Circuit File Operations

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            CircuitXML cxml = new CircuitXML(icl);
            cxml.Save(_filename, gateCanvas);

            ((UndoRedo.UndoManager)Resources["undoManager"]).SetSavePoint();
            
        }

        private void btnSave_As_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".gcg";
            dlg.Filter = "Gate Circuit Groups (.gcg)|*.gcg";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                CircuitXML cxml = new CircuitXML(icl);
                try
                {
                    cxml.Save(dlg.FileName, gateCanvas);
                    _filename = dlg.FileName;
                    btnSave.IsEnabled = true;
                    UpdateTitle();

                    ((UndoRedo.UndoManager)Resources["undoManager"]).SetSavePoint();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save circuit as requested: " + ex.ToString());
                }
            }
        }


        private void btnImportIC_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ic";
            dlg.Filter = "IC (.ic)|*.ic|All Files (*.*)|*.*";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                CircuitXML cxml = new CircuitXML(icl);
                try
                {
                    cxml.Load(dlg.FileName, icl.Add);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load circuit as requested: " + ex.ToString());
                }

            }
        }

        //BOKANG
        public void loadXMLElement(string path)
        { 
            gateCanvas.ClearSelection();
            icl.Clear();
            CircuitXML cxml = new CircuitXML(icl);
            try{
                slZoom.Value = 0.5;
                RefreshGateCanvas(cxml.Load(path, icl.Add));
            }catch (Exception ex){
                MessageBox.Show("Unable to load circuit as requested: " + ex.ToString());
            }
        }

        public void loadXMLElement(XElement circuit)
        {
            gateCanvas.ClearSelection();
            icl.Clear();
            CircuitXML cxml = new CircuitXML(icl);
            try
            {
                slZoom.Value = 1;
                RefreshGateCanvas(cxml.LoadCircuit(circuit));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load circuit as requested: " + ex.ToString());
            }
        }
        
        #endregion

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.ClearSelection();
            gateCanvas.UndoProvider.Undo();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.ClearSelection();
            gateCanvas.UndoProvider.Redo();
        }

        private void btnClearUndos_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.UndoProvider.Clear();
            GC.Collect();
        }

        private void UpdateTitle()
        {
            StringBuilder ttl = new StringBuilder();

            if (String.IsNullOrEmpty(_filename))
            {
                ttl.Append("[Untitled]");
            }
            else
            {
                ttl.Append(_filename.Substring(_filename.LastIndexOf(@"\") + 1));
            }

            if (!((UndoRedo.UndoManager)Resources["undoManager"]).IsAtSavePoint)
            {
                ttl.Append(" (Modified) ");
            }
            ttl.Append(" - ");

            switch (_myEditLevel)
            {
                case EditLevel.VIEW:
                    ttl.Append("View IC - ");
                    break;
                case EditLevel.EDIT:
                    ttl.Append("View/Edit IC - ");
                    break;
            }

            

            ttl.Append(APP_TITLE);

        }

        #region Circuit Zoom Operations

        private void slZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            gateCanvas.Zoom = slZoom.Value;
        }

        private void AnimateZoom(double dest, Point? zoomCenter)
        {
            if (dest < slZoom.Minimum)
                dest = slZoom.Minimum;

            if (dest > slZoom.Maximum)
                dest = slZoom.Maximum;

            PointAnimation pa = null;
            
            if (zoomCenter.HasValue) 
            {
                gateCanvas.SetZoomCenter();
                pa = new PointAnimation(gateCanvas.ZoomCenter, zoomCenter.Value, new Duration(new TimeSpan(0, 0, 1)));
                
                pa.AccelerationRatio = 0.2;
                pa.DecelerationRatio = 0.2;

            }
            DoubleAnimation da = new DoubleAnimation(dest, new Duration(new TimeSpan(0, 0, 1)));
            da.AccelerationRatio = 0.2;
            da.DecelerationRatio = 0.2;

            Storyboard sb = new Storyboard();
            sb.Children.Add(da);
            if (pa != null)
            {
                sb.Children.Add(pa);
                Storyboard.SetTarget(pa, gateCanvas);
                Storyboard.SetTargetProperty(pa, new PropertyPath(GateCanvas.ZoomCenterProperty));
                gateCanvas.UseZoomCenter = true;

            }
            Storyboard.SetTarget(da, slZoom);
            Storyboard.SetTargetProperty(da, new PropertyPath(Slider.ValueProperty));
            sb.FillBehavior = FillBehavior.Stop;

            
            sb.Begin();

            BackgroundWorker finishani = new BackgroundWorker();
            finishani.DoWork += (sender2, e2) =>
            {
                System.Threading.Thread.Sleep(900);
            };
            finishani.RunWorkerCompleted += (sender2, e2) =>
            {
                if (zoomCenter.HasValue)
                    gateCanvas.ZoomCenter = zoomCenter.Value;

                slZoom.Value = dest;
            };
            finishani.RunWorkerAsync();

            
        }

        public void btnActualSize_Click(object sender, RoutedEventArgs e)
        {
            AnimateZoom(1, null);
        }

        public void btnFitToScreen_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds;
            bounds = gateCanvas.GetBounds(64, gateCanvas.selected.Count > 1);
            

            double minx = bounds.Left;
            double miny = bounds.Top;
            double maxx = bounds.Right;
            double maxy = bounds.Bottom;

            double wid =  gateCanvas.ActualWidth / (maxx - minx);
            double hei = gateCanvas.ActualHeight / (maxy - miny);
            
            
            AnimateZoom(Math.Min(wid, hei), new Point(minx + (maxx - minx) / 2.0,
                miny + (maxy - miny) / 2.0));

            BackgroundWorker resetzc = new BackgroundWorker();
            resetzc.DoWork += (sender2, e2) =>
            {
                System.Threading.Thread.Sleep(1500);
            };
            resetzc.RunWorkerCompleted += (sender2, e2) =>
            {
                gateCanvas.UseZoomCenter = false;
            };
            resetzc.RunWorkerAsync();

        }
        #endregion

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
        
        #region CopyPaste
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (gateCanvas.selected.Count > 0) {
                UIGates.IC ic = gateCanvas.CreateIC("(clipboard)", GateCanvas.SELECTED_GATES.SELECTED);
                CircuitXML cx = new CircuitXML(icl);
                DataObject clipobj = new DataObject();
                Clipboard.SetData("IC", cx.CreateCircuitXML(ic).ToString());
                btnPaste.IsEnabled = true;
            }
        }

        private void btnCopyAsImage_Click(object sender, RoutedEventArgs e)
        {
            if (gateCanvas.selected.Count > 0)
            {
                UIGates.IC ic = gateCanvas.CreateIC("(clipboard)", GateCanvas.SELECTED_GATES.SELECTED);
                
                
                GateCanvas tmp = new GateCanvas(ic, icl);
                tmp.Width = tmp.GetBounds(0, false).Width;
                tmp.Height = tmp.GetBounds(0, false).Height;
                BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += (s2, e2) =>
                {
                    System.Threading.Thread.Sleep(500); // prop time
                    // don't use wait on propagation because if there is a loop
                    // it will loop forever
                };
                bg.RunWorkerCompleted += (s2, e2) =>
                {
                    tmp.Mute = true;
                    tmp.UpdateLayout();
                    tmp.UpdateWireConnections();
                    Clipboard.SetImage(tmp.CreateImage());
                    Grid1.Children.Remove(tmp);
                };
                
                Grid1.Children.Add(tmp);
                bg.RunWorkerAsync();
                
            }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsData("IC"))
            {
                string xml = Clipboard.GetData("IC") as string;
                CircuitXML cx = new CircuitXML(icl);
                try
                {
                    UIGates.IC ic = cx.LoadCircuit(System.Xml.Linq.XElement.Parse(xml));
                    gateCanvas.PasteIC(ic);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to complete paste; maybe you deleted a needed IC?");
                }
            }
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            btnCopy_Click(sender, e); // do a copy
            gateCanvas.UndoProvider.Add(gateCanvas.DeleteSelectedGates());
        }

        #endregion
        
        private void btnFlatten_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.Flatten();
        }

        private void AnimateOpacity(UIElement target, double dest)
        {
            DoubleAnimation da = new DoubleAnimation(dest, new Duration(new TimeSpan(0,0,0,0,500)));
            da.AccelerationRatio = 0.2;
            da.DecelerationRatio = 0.2;

            Storyboard sb = new Storyboard();
            sb.Children.Add(da);
            
            Storyboard.SetTarget(da, target);
            Storyboard.SetTargetProperty(da, new PropertyPath(UIElement.OpacityProperty));
            sb.FillBehavior = FillBehavior.Stop;


            sb.Begin();


            BackgroundWorker finishani = new BackgroundWorker();
            finishani.DoWork += (sender2, e2) =>
            {
                System.Threading.Thread.Sleep(500);
            };
            finishani.RunWorkerCompleted += (sender2, e2) =>
            {
                target.Opacity = dest;
            };
            finishani.RunWorkerAsync();

            
        }
        
        private void btnShowHideToolbars_Unchecked(object sender, RoutedEventArgs e)
        {
            

            
            AnimateOpacity(sbGates, 0);
            
            if (sbSpeed != null)
                AnimateOpacity(sbSpeed, 0);
            
            AnimateOpacity(sbZoom, 0);


            BackgroundWorker finishani = new BackgroundWorker();
            finishani.DoWork += (sender2, e2) =>
            {
                System.Threading.Thread.Sleep(600);
            };
            finishani.RunWorkerCompleted += (sender2, e2) =>
            {
                sbGates.Visibility = Visibility.Collapsed;

                if (sbSpeed != null)
                    sbSpeed.Visibility = Visibility.Collapsed;

                sbZoom.Visibility = Visibility.Collapsed;
            };
            finishani.RunWorkerAsync();
            
        }

        private void btnShowHideToolbars_Checked(object sender, RoutedEventArgs e)
        {

            if (sbGates == null) return; // occurs during load, not ready yet
            
                
            sbGates.Visibility = Visibility.Visible;
                
            if (sbSpeed != null)
                    sbSpeed.Visibility = Visibility.Visible;
            
            sbZoom.Visibility = Visibility.Visible;
            
            AnimateOpacity(sbGates, 1);
            
            if (sbSpeed != null)
                AnimateOpacity(sbSpeed, 1);
            
            AnimateOpacity(sbZoom, 1);



        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new System.Windows.Controls.PrintDialog();

            if (printDlg.ShowDialog() == true)
            {

                gateCanvas.Print(printDlg);

             
            }


        }

        private void btnSaveAsImage_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Image (.png)|*.png|JPEG Image (.jpg)|*.jpg|Bitmap Image (.bmp)|*.bmp";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                BitmapEncoder be = null;
                switch (dlg.FilterIndex)
                {
                    case 0:
                        be = new PngBitmapEncoder();
                        break;
                    case 1:
                        be = new JpegBitmapEncoder();
                        break;
                    case 2:
                        be = new BmpBitmapEncoder();
                        break;
                }
                gateCanvas.Mute = true;
                be.Frames.Add(BitmapFrame.Create(gateCanvas.CreateImage()));
                gateCanvas.Mute = false;
                try
                {
                    System.IO.FileStream fs = System.IO.File.Create(dlg.FileName);
                    be.Save(fs);
                    fs.Close();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save image as requested: "+ ex.ToString());
                }
            }
        }

        private void btnAlignTopLeft_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.AlignUpperLeft();
        }

        private void btnChart_Click(object sender, RoutedEventArgs e)
        {
            Charting.Chart chrt = new Charting.Chart(gateCanvas.Circuit);
            chrt.Show();
        }

        #region Properties

        private UserControl _owner;

        public UserControl Owner
        {
            set { _owner = value; }
            get { return _owner; }
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
