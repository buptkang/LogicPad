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
using System.Xml.Linq;

using LogicPad2Util;
using LogicPadParser;

using starPadSDK.Inq;
using starPadSDK.Inq.MSInkCompat;
using starPadSDK.MathRecognizer;
using starPadSDK.Geom;
using starPadSDK.MathUI;
using starPadSDK.MathExpr;
using starPadSDK.CharRecognizer;
using starPadSDK.Utils;
using starPadSDK.UnicodeNs;
using CuspDetector = starPadSDK.Inq.BobsCusps.FeaturePointDetector;

namespace LogicPad2.Expression
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl, IPassable
    {
        public UserControl1()
        {
            InitializeComponent();
            _userControlStatus = UserControlStatus.None;

            _mrec = new MathRecognition(_mathStroqs);
            _mrec.EnsureLoaded(); // this is optional, and should only be called once per program run
            _mrec.ParseUpdated += _mrec_ParseUpdated;

            _altsMenuCrea = new AlternatesMenuCreator(alternatesMenu, _mrec);
        
            //inqCanvas.StroqCollected += inqCanvas_StroqCollected;
            //inqCanvas.PreviewStylusDown += inqCanvas_PreviewStylusDown;
            //inqCanvas.PreviewMouseLeftButtonDown += inqCanvas_PreviewMouseLeftButtonDown;
            //inqCanvas.PreviewMouseMove += inqCanvas_PreviewMouseMove;
            //inqCanvas.PreviewMouseLeftButtonUp += inqCanvas_PreviewMouseLeftButtonUp;
            //inqCanvas.PreviewStylusButtonUp += inqCanvas_PreviewStylusUp;

            inqCanvas.DefaultDrawingAttributes.Width = 1;

            /* for the rest of this method, try to ensure more stuff is loaded at startup to avoid a long pause after first stroke */
            // load unicode stuff (may not be that long?)
            Console.WriteLine(Unicode.NameOf('a'));

            // load drawing wpf stuff and create initial math font stuff            
            DrawingVisual dv = new DrawingVisual();
            var dc = dv.RenderOpen();
            Rct nombb = starPadSDK.MathExpr.ExprWPF.EWPF.DrawTop(new LetterSym('1'), 22, dc, Colors.Blue, new Pt(0, 0), true);
            dc.Close();
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

        private bool _isDiagramRepreVisible;

        public bool IsDiagramRepreVisible
        {
            set { _isDiagramRepreVisible = value; }
            get { return _isDiagramRepreVisible; }
        }

        private UserControl _diagramRepr;

        public UserControl DiagramRepr 
        {
            set { _diagramRepr = value; }
            get { return _diagramRepr; }
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
            get {
                return this.controlRegion;        
            }
        }

        public Button SaveButton
        {
            get {
                return this.btnSave;
            }
        }

        public Button CancelButton
        {
            get {
                return this.btnCancel;
            }
        }

        public Button ScaleButton
        {
            get {
                return this.btnScale;
            }
        }

        public Button TransformButton
        {
            get {
                return this.btnTransform;
            }
        }

        public InqCanvas ExpressionInkCanvas
        {
            get {
                return this.inqCanvas;
            }
        }

        public ToolBar StarPadToolBar
        {
            get { return this.alternatesMenu; }
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

        public Border UserControlBorder
        {
            get { return this.controlBorder; }
        }

        //Layout Issue
        private Rect _currentStrokesBoundingBox;
        public Rect CurrentStrokesBoudningBox
        {
            set { _currentStrokesBoundingBox = value; }
            get { return _currentStrokesBoundingBox; }
        }
       
        #region IPassable Impl

        #region Un-used Event Handler

        void IPassable.StylusDown(object sender, StylusDownEventArgs e)
        {
            
        }

        void IPassable.StylusMove(object sender, StylusEventArgs e)
        {
            
        }

        void IPassable.StylusUp(object sender, StylusEventArgs e)
        {
            
        }

        public new void PreviewStylusMove(object sender, StylusEventArgs e)
        {
        }

        public new void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        #endregion

        public new void PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Point tapPoint = e.GetPosition(inqCanvas);

            if (Selected.Contents != null && Selected.Contents.Outline != null && Selected.Contents.Outline.GetBounds().Contains(tapPoint))
            {
                StartMove(tapPoint);
                inqCanvas.InkEnabled = false;
                e.Handled = true;
            }
        }

        public new void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*
            Matrix m = new Matrix();
            m.Translate(-UserControlX, -UserControlY);
            Point tapPoint = m.Transform(e.GetPosition(inqCanvas));
            */

            Point tapPoint = e.GetPosition(inqCanvas);

            if (_moving == true)
            { // could be set by stylus going down
                Mouse.Capture(inqCanvas); // stylus doesn't capture mouse
                e.Handled = true;
                return;
            }
            if (Selected.Contents != null && Selected.Contents.Outline != null && Selected.Contents.Outline.GetBounds().Contains(tapPoint))
            {
                Mouse.Capture(inqCanvas);
                StartMove(tapPoint);
                e.Handled = true;
            }
        }

        public new void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            /*
            Matrix m = new Matrix();
            m.Translate(-UserControlX, -UserControlY);
            Point tapPoint = m.Transform(e.GetPosition(inqCanvas));
            */
            Point tapPoint = e.GetPosition(inqCanvas);

            if (_moving)
            {
                using (_mrec.BatchEditNoRecog(false))
                {
                    try
                    {
                        Selected.Contents.MoveTo(tapPoint);
                    }
                    catch { }
                }
                e.Handled = true;
            }
        }

        public new void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*
            Matrix m = new Matrix();
            m.Translate(-UserControlX, -UserControlY);
            Point tapPoint = m.Transform(e.GetPosition(inqCanvas));
            */
            Point tapPoint = e.GetPosition(inqCanvas);

            if (_moving)
            {
                _moving = false;
                using (_mrec.BatchEditNoRecog(true))
                {
                    Selected.Contents.MoveTo(tapPoint);
                }
                Selected.Contents.EndMove();
                if (_movingLock != null)
                {
                    _movingLock.Dispose(); // this will call Parse itself
                    _movingLock = null;
                }
                else Selected.Contents.Reparse(_mrec); // make sure math is updated and do a full rerecog just in case; we have only been doing non-re-recognition parses for speed
                Deselect();
                Mouse.Capture(null);
                e.Handled = true;
                inqCanvas.InkEnabled = true;
            }
        }

        void IPassable.StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            //Probably need to change the stroke to the coordinates of this canvas
            e.Stroke.DrawingAttributes.Color = Colors.Red;
            //inqCanvas.Stroqs.Add(e.Stroke);

            Matrix m = new Matrix();
            m.Translate(-UserControlX, -UserControlY);
            m.Scale(1 / UserControlScaleX, 1 / UserControlScaleY);
            e.Stroke.Transform(m, false);

            Stroq stroq = new Stroq(e.Stroke);
            inqCanvas.Stroqs.Add(stroq);

            /* If we get here, it's a real stroke (not movement), so deselect any selection */
            Deselect();

            /* check for scribble delete */
            if (ScribbleDelete(stroq)) return;

            /* check for lassos/circles around stuff */
            if (LassoSelect(stroq)) return;

            _mathStroqs.Add(stroq);
        }
        #endregion


        //StarPad for Logic Expression Recognition
        #region StarPad


        public bool HitTestAltsMenuCreator(InkCanvas parentInkCanvas, Point center)
        {
            HitTestResult result;
            foreach (Object obj in alternatesMenu.Items)
            {
                if (obj is MenuItem)
                {
                    MenuItem _menuItem = (MenuItem)obj;
                    result = VisualTreeHelper.HitTest(_menuItem, parentInkCanvas.TransformToDescendant(_menuItem).Transform(center));
                    if (result != null)
                    {
                        AltsMenuCreator.ChooseAlternate(_menuItem);
                        return true;
                    }
                }
            }
            return false;
        }

        #region properties

        private StroqCollection _mathStroqs = new StroqCollection();

        private MathRecognition _mrec;

        private starPadSDK.MathUI.InkColorizer _colorizer = new starPadSDK.MathUI.InkColorizer();

        private AlternatesMenuCreator _altsMenuCrea;

        public AlternatesMenuCreator AltsMenuCreator
        {
            get { return _altsMenuCrea; }
        }

        public Selection Selected = new Selection();

        private bool _moving = false;

        private BatchLock _movingLock = null;

        #endregion


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _mrec.Charreco.InkPixel = (float)OldStrokeStuff.Scale;
        }

        public void ClearMathInkCanvas()
        {
            _mathStroqs.Clear();
            inqCanvas.Stroqs.Clear();
            inqCanvas.Children.Clear();
            underlay.Children.Clear();
            _colorizer.Reset();
        }

        public Rct bbox(Microsoft.Ink.Strokes stks)
        {
            return _mrec.Sim[stks].Aggregate(Rct.Null, (Rct r, Stroq s) => r.Union(s.GetBounds()));
        }

        private void _mrec_ParseUpdated(MathRecognition source, Recognition chchanged, bool updateMath)
        {
            /* Evaluate math if necessary */
            if (updateMath)
                try
                {
                    Evaluator.UpdateMath(_mrec.Ranges.Select((Parser.Range r) => r.Parse));
                }
                catch { }

            /* reset geometry displayed: range displays, etc */
            underlay.Children.Clear();
            inqCanvas.Children.Clear();

            /* set up to draw background yellow thing for range displays */
            Brush fill3 = new SolidColorBrush(Color.FromArgb(50, 255, 255, 180));
            Brush fill2 = new SolidColorBrush(Color.FromArgb(75, 255, 255, 180));
            Brush fill1 = new SolidColorBrush(Color.FromArgb(100, 255, 255, 180));
            Brush sqr3 = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
            Brush sqr2 = new SolidColorBrush(Color.FromArgb(75, 0, 255, 0));
            Brush sqr1 = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0));
            foreach (Parser.Range rrr in _mrec.Ranges)
            {
                Rct rangebbox = bbox(rrr.Strokes);
                CurrentStrokesBoudningBox = rangebbox;

                Rct box = rangebbox.Inflate(8, 8);

                /* draw yellow box */
                DrawingVisual dv = new DrawingVisual();
                DrawingContext dc = dv.RenderOpen();
                dc.DrawRoundedRectangle(fill3, null, box, 4, 4);
                dc.DrawRoundedRectangle(fill2, null, box.Inflate(-4, -4), 4, 4);
                dc.DrawRoundedRectangle(fill1, null, box.Inflate(-8, -8), 4, 4);
                dc.Close();
                underlay.Children.Add(dv);

                if (rrr.Parse != null)
                {
                    /* draw interpretation of entry */
                    if (rrr.Parse.expr != null)
                    {
                        dv = new DrawingVisual();
                        dc = dv.RenderOpen();
                        // this is an example of normal drawing of an expr
                        Rct nombb = starPadSDK.MathExpr.ExprWPF.EWPF.DrawTop(rrr.Parse.expr, 22, dc, Colors.Blue, new Pt(box.Left, box.Bottom + 24), true);
                        dc.Close();
                        underlay.Children.Add(dv);
                    }

                    /* draw result of computation, if any */
                    if (rrr.Parse.finalSimp != null)
                    {
                        Rct nombb;
                        Expr result = rrr.Parse.matrixOperationResult == null ? rrr.Parse.finalSimp : rrr.Parse.matrixOperationResult;
                        // this is an example of drawing an expr by getting a geometry of it first, so can be used for special effects, etc.
                        Geometry g = starPadSDK.MathExpr.ExprWPF.EWPF.ComputeGeometry(result, 22, out nombb);
                        System.Windows.Shapes.Path p = new System.Windows.Shapes.Path();
                        p.Data = g;
                        p.Stroke = Brushes.Red;
                        p.Fill = Brushes.Transparent;
                        p.StrokeThickness = 1;
                        p.RenderTransform = new TranslateTransform(box.Right + 10, box.Center.Y);
                        inqCanvas.Children.Add(p);
                    }

                    /* colorize ink. Ideally we would have kept track of which ink strokes had changes and only update colorization in those ranges affected
                     * by the changes. */
                    if (rrr.Parse.root != null) _colorizer.Colorize(rrr.Parse.root, rrr.Strokes, _mrec);
                }
            }

            /* Update alternates menu if user wrote a char */
            if (chchanged != null)
            {
                showSidebarAlts(new[] { chchanged }, new StroqCollection(_mrec.Sim[chchanged.strokes]));
            }
#if false
            /* print out log of current 1st-level parse, for debugging */
            List<string> resstrs = new List<string>();
            foreach(Parser.Range r in _mrec.Ranges) {
                Parser.ParseResult p = r.Parse;
                if(p != null && p.root != null)
                    resstrs.Add(p.root.Print());
            }
            if(resstrs.Count > 0) Console.WriteLine(resstrs.Aggregate((string a, string b) => a + " // " + b));
            foreach(Parser.Range r in _mrec.Ranges) {
                Parser.ParseResult pr = r.Parse;
                if(pr != null && pr.expr != null) Console.WriteLine(Text.Convert(pr.expr));
            }
#endif
        }

        private void showSidebarAlts(ICollection<Recognition> recogs, StroqCollection stroqs)
        {
            if (_altsMenuCrea != null)
            {
                _altsMenuCrea.Populate(recogs, stroqs);
            }
        }


        void inqCanvas_StroqCollected(object sender, InqCanvas.StroqCollectedEventArgs e)
        {
            /* filter out gestures before taking everything else as math */

            //BOKANG, check user's tap gesture in order to trigger the animation
            /*
            if (Selected.Contents != null)
            {
                if (isTapGesture(e))
                {
                    triggerAnimationForLogicExpression(Selected.Contents);
                    _mathStroqs.Remove(e.Stroq);
                    inqCanvas.Stroqs.Remove(e.Stroq);
                    return;
                }
            }
             * */

            /* If we get here, it's a real stroke (not movement), so deselect any selection */
            Deselect();

            /* check for scribble delete */
            if (ScribbleDelete(e)) return;

            /* check for lassos/circles around stuff */
            if (LassoSelect(e)) return;

            _mathStroqs.Add(e.Stroq);
        }

        //BOKANG TODO, check for the tap gesture
        public bool isTapGesture(InqCanvas.StroqCollectedEventArgs e)
        {
            Stroq currentStroq = e.Stroq;

            Recognition rtempt = _mrec.ClassifyOneTemp(currentStroq);

            if (rtempt != null && rtempt.alts.Contains<Recognition.Result>(Unicode.F.FULL_STOP))
            {
                return true;
            }

            return false;
        }

        public void Deselect()
        {
            Selected.Contents = null;
            hideSidebarAlts();
        }

        private void hideSidebarAlts()
        {
            if (_altsMenuCrea != null)
            {
                _altsMenuCrea.Clear();
            }
        }

        private bool ScribbleDelete(Stroq stroq)
        {
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
                IEnumerable<Pt> hull = stroq.ConvexHull();
                StroqCollection stqs = inqCanvas.Stroqs.HitTest(hull, 1);
                if (stqs.Count > 1)
                {
                    inqCanvas.Stroqs.Remove(stqs);
                    _mathStroqs.Remove(stqs);

                    inqCanvas.Stroqs.Remove(stroq);
                    return true;
                }
            }
            return false;
        }

        private bool ScribbleDelete(InqCanvas.StroqCollectedEventArgs e)
        {
            bool canBeScribble = e.Stroq.OldPolylineCusps().Length > 4;
            if (e.Stroq.OldPolylineCusps().Length == 4)
            {
                int[] pcusps = e.Stroq.OldPolylineCusps();
                Deg a1 = fpdangle(e.Stroq[0], e.Stroq[pcusps[1]], e.Stroq[pcusps[2]] - e.Stroq[pcusps[1]]);
                Deg a2 = fpdangle(e.Stroq[pcusps[1]], e.Stroq[pcusps[1]], e.Stroq[pcusps[3]] - e.Stroq[pcusps[1]]);
                if (a1 < 35 && a2 < 35)
                    canBeScribble = e.Stroq.BackingStroke.HitTest(e.Stroq.ConvexHull().First(), 1);
            }
            if (canBeScribble)
            {
                IEnumerable<Pt> hull = e.Stroq.ConvexHull();
                StroqCollection stqs = inqCanvas.Stroqs.HitTest(hull, 1);
                if (stqs.Count > 1)
                {
                    inqCanvas.Stroqs.Remove(stqs);
                    _mathStroqs.Remove(stqs);

                    inqCanvas.Stroqs.Remove(e.Stroq);
                    return true;
                }
            }
            return false;
        }

        private bool LassoSelect(Stroq stroq)
        {
            if (stroq.OldPolylineCusps().Length <= 4 && stroq.Count > 4)
            {
                Stroq estroq = stroq;
                CuspDetector.CuspSet cs = CuspDetector.FeaturePoints(estroq);

                Pt[] first = new Pt[cs.pts.Count / 2];
                for (int i = 0; i < first.Length; i++)
                    if (cs.distances[i] > cs.dist / 2)
                        break;
                    else first[i] = cs.pts[i];
                Pt[] second = new Pt[cs.pts.Count - first.Length];
                for (int j = 0; j < second.Length; j++) second[j] = cs.pts[first.Length + j];
                Stroq s1 = new Stroq(first);
                Stroq s2 = new Stroq(second);
                float d1, d2;
                s1.OldNearestPoint(s2[-1], out d1);
                s2.OldNearestPoint(s1[0], out d2);
                if (Math.Min(d1, d2) / Math.Max(estroq.GetBounds().Width, estroq.GetBounds().Height) < 0.3f)
                {
                    StroqCollection stqs = _mathStroqs.HitTest(estroq, 50);
                    StroqCollection stqs2 = _mathStroqs.HitTest(estroq.Reverse1(), 50);
                    if (stqs2.Count > stqs.Count)
                        stqs = stqs2;
                    stqs.Remove(estroq);
                    StroqCollection stqs3 = new StroqCollection(stqs.Where((Stroq s) => _mrec.Charreco.Classification(_mrec.Sim[s]) != null));
                    stqs = stqs3;
                    Recognition rtemp = _mrec.ClassifyOneTemp(estroq);
                    if (stqs.Count > 0 && (rtemp == null || !rtemp.alts.Contains(new Recognition.Result(Unicode.S.SQUARE_ROOT))))
                    {
                        if (rtemp != null) Console.WriteLine("select recognized for " + rtemp.allograph);
                        Deselect();

                        estroq.BackingStroke.DrawingAttributes.Color = Colors.Purple;
                        Selected.Contents = new StroqSel(stqs, estroq, (Stroq s) => _mrec.Charreco.Classification(_mrec.Sim[s]),
                            (Recognition r) => _mrec.Sim[r.strokes], inqCanvas.Stroqs);
                        StroqSel Sel = (StroqSel)Selected.Contents;
                        HashSet<Recognition> recogs = new HashSet<Recognition>(Sel.AllStroqs.Select((Stroq s) => _mrec.Charreco.Classification(_mrec.Sim[s]))
                            .Where((Recognition r) => r != null));
                        if (recogs.Count != 0) showSidebarAlts(recogs, Sel.AllStroqs);

                        return true;
                    }
                    else
                    {
                        // Generic additional selections would be called here.
                        return false;
                    }
                }
            }
            return false;
        }

        private bool LassoSelect(InqCanvas.StroqCollectedEventArgs e)
        {
            if (e.Stroq.OldPolylineCusps().Length <= 4 && e.Stroq.Count > 4)
            {
                Stroq estroq = e.Stroq;
                CuspDetector.CuspSet cs = CuspDetector.FeaturePoints(estroq);

                Pt[] first = new Pt[cs.pts.Count / 2];
                for (int i = 0; i < first.Length; i++)
                    if (cs.distances[i] > cs.dist / 2)
                        break;
                    else first[i] = cs.pts[i];
                Pt[] second = new Pt[cs.pts.Count - first.Length];
                for (int j = 0; j < second.Length; j++) second[j] = cs.pts[first.Length + j];
                Stroq s1 = new Stroq(first);
                Stroq s2 = new Stroq(second);
                float d1, d2;
                s1.OldNearestPoint(s2[-1], out d1);
                s2.OldNearestPoint(s1[0], out d2);
                if (Math.Min(d1, d2) / Math.Max(estroq.GetBounds().Width, estroq.GetBounds().Height) < 0.3f)
                {
                    StroqCollection stqs = _mathStroqs.HitTest(estroq, 50);
                    StroqCollection stqs2 = _mathStroqs.HitTest(estroq.Reverse1(), 50);
                    if (stqs2.Count > stqs.Count)
                        stqs = stqs2;
                    stqs.Remove(estroq);
                    StroqCollection stqs3 = new StroqCollection(stqs.Where((Stroq s) => _mrec.Charreco.Classification(_mrec.Sim[s]) != null));
                    stqs = stqs3;
                    Recognition rtemp = _mrec.ClassifyOneTemp(estroq);
                    if (stqs.Count > 0 && (rtemp == null || !rtemp.alts.Contains(new Recognition.Result(Unicode.S.SQUARE_ROOT))))
                    {
                        if (rtemp != null) Console.WriteLine("select recognized for " + rtemp.allograph);
                        Deselect();

                        estroq.BackingStroke.DrawingAttributes.Color = Colors.Purple;
                        Selected.Contents = new StroqSel(stqs, estroq, (Stroq s) => _mrec.Charreco.Classification(_mrec.Sim[s]),
                            (Recognition r) => _mrec.Sim[r.strokes], inqCanvas.Stroqs);
                        StroqSel Sel = (StroqSel)Selected.Contents;
                        HashSet<Recognition> recogs = new HashSet<Recognition>(Sel.AllStroqs.Select((Stroq s) => _mrec.Charreco.Classification(_mrec.Sim[s]))
                            .Where((Recognition r) => r != null));
                        if (recogs.Count != 0) showSidebarAlts(recogs, Sel.AllStroqs);

                        return true;
                    }
                    else
                    {
                        // Generic additional selections would be called here.
                        return false;
                    }
                }
            }
            return false;
        }

        void inqCanvas_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            if (Selected.Contents != null && Selected.Contents.Outline != null && Selected.Contents.Outline.GetBounds().Contains(e.GetPosition(inqCanvas)))
            {
                StartMove(e.GetPosition(inqCanvas));
                inqCanvas.InkEnabled = false;
                e.Handled = true;
            }
        }

        void inqCanvas_PreviewStylusUp(object sender, StylusButtonEventArgs e)
        {
            if (_moving)
            {
                _moving = false;
                using (_mrec.BatchEditNoRecog(true))
                {
                    Selected.Contents.MoveTo(e.GetPosition(inqCanvas));
                }
                Selected.Contents.EndMove();
                if (_movingLock != null)
                {
                    _movingLock.Dispose(); // this will call Parse itself
                    _movingLock = null;
                }
                else Selected.Contents.Reparse(_mrec); // make sure math is updated and do a full rerecog just in case; we have only been doing non-re-recognition parses for speed
                Deselect();
                Mouse.Capture(null);
                e.Handled = true;
                inqCanvas.InkEnabled = true;
            }
        }

        void inqCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_moving)
            {
                _moving = false;
                using (_mrec.BatchEditNoRecog(true))
                {
                    Selected.Contents.MoveTo(e.GetPosition(inqCanvas));
                }
                Selected.Contents.EndMove();
                if (_movingLock != null)
                {
                    _movingLock.Dispose(); // this will call Parse itself
                    _movingLock = null;
                }
                else Selected.Contents.Reparse(_mrec); // make sure math is updated and do a full rerecog just in case; we have only been doing non-re-recognition parses for speed
                Deselect();
                Mouse.Capture(null);
                e.Handled = true;
                inqCanvas.InkEnabled = true;
            }
        }

        void inqCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_moving)
            {
                using (_mrec.BatchEditNoRecog(false))
                {
                    Selected.Contents.MoveTo(e.GetPosition(inqCanvas));
                }
                e.Handled = true;
            }
        }

        void inqCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_moving == true)
            { // could be set by stylus going down
                Mouse.Capture(inqCanvas); // stylus doesn't capture mouse
                e.Handled = true;
                return;
            }
            if (Selected.Contents != null && Selected.Contents.Outline != null && Selected.Contents.Outline.GetBounds().Contains(e.GetPosition(inqCanvas)))
            {
                Mouse.Capture(inqCanvas);
                StartMove(e.GetPosition(inqCanvas));
                e.Handled = true;
            }
        }

        void StartMove(Pt p)
        {
            _moving = true;
            StroqSel ss = Selected.Contents as StroqSel;
            if (ss != null && ss.AllStroqs.Count > 10) _movingLock = _mrec.BatchEdit();
            Selected.Contents.StartMove(p);
            inqCanvas.Stroqs.Remove(Selected.Contents.Outline);
        }

        Deg fpdangle(Pt a, Pt b, Vec v)
        {
            return (a - b).Normalized().UnsignedAngle(v.Normalized());
        }

        public XElement Parse(ref int output)
        {
            XElement generatedXElement = null;

            foreach (Parser.Range rrr in _mrec.Ranges)
            {
                if (rrr.Parse != null)
                {
                    //draw interpretation
                    if (rrr.Parse.expr != null)
                    {
                        FilterOneAndZero(ref rrr.Parse.expr, ref output);
                        if (output == 1 || output == 0)
                        {
                            return null;
                        }

                        generatedXElement = LogicPadParser.LogicPadParser.Instance.ParseStarPadExpr(rrr.Parse.expr);
                    }
                }
            }
            return generatedXElement;
        }

        private void FilterOneAndZero(ref Expr expr, ref int output)
        {
            if (expr == null)
            {
                return;
            }
            if (output == 0 || output == 1)
            {
                return;
            }

            if (expr is starPadSDK.MathExpr.LetterSym || expr is starPadSDK.MathExpr.IntegerNumber)
            {
                return; 
            }
            else if (expr is starPadSDK.MathExpr.CompositeExpr)
            {
                starPadSDK.MathExpr.CompositeExpr compositeExpr = expr as starPadSDK.MathExpr.CompositeExpr;
                if (compositeExpr.Head == starPadSDK.MathExpr.WellKnownSym.equals)
                {
                    FilterOneAndZero(ref compositeExpr.Args[1], ref output);
                }

                for (int i = 0; i < compositeExpr.Args.Length; i++)
                {
                    if (compositeExpr.Args[i] is starPadSDK.MathExpr.IntegerNumber)
                    {
                        starPadSDK.MathExpr.IntegerNumber number = compositeExpr.Args[i] as starPadSDK.MathExpr.IntegerNumber;
                        if (number.Num.IsOne)
                        {
                            if (compositeExpr.Head == starPadSDK.MathExpr.WellKnownSym.plus)
                            {
                                //OR
                                output = 1;
                                return;
                            }
                            else if (compositeExpr.Head == starPadSDK.MathExpr.WellKnownSym.times)
                            {
                                //AND
                                if (compositeExpr.Args.Length > 2)
                                {
                                    Expr[] myArgs = new Expr[compositeExpr.Args.Length - 1];
                                    Array.Copy(compositeExpr.Args, 0, myArgs, 0, i);
                                    Array.Copy(compositeExpr.Args, i + 1, myArgs, i, compositeExpr.Args.Length - i - 1);

                                    CompositeExpr notExpr = new CompositeExpr(starPadSDK.MathExpr.WellKnownSym.times, myArgs);
                                    expr = notExpr;

                                }
                                else if (compositeExpr.Args.Length == 2)
                                {
                                    if (i == 0)
                                    {
                                        expr = compositeExpr.Args[1];
                                    }
                                    else
                                    {
                                        expr = compositeExpr.Args[0];
                                    }
                                }
                                output = 2;

                            }
                            else if (compositeExpr.Head == starPadSDK.MathExpr.WellKnownSym.xorGate)
                            {
                                //XOR
                                Expr[] myArgs = new Expr[compositeExpr.Args.Length -1];
                                int t = 0;
                                for(int j =0; j< compositeExpr.Args.Length; j++)
                                {
                                    if(compositeExpr.Args[j] != compositeExpr.Args[i])
                                    {
                                        myArgs[t++] = compositeExpr.Args[j];
                                    }
                                }

                                CompositeExpr notExpr = new CompositeExpr(starPadSDK.MathExpr.WellKnownSym.power, myArgs);
                                expr = notExpr;
                                output = 2;
                            }
                        }
                        else if (number.Num.IsZero)
                        {
                            if (compositeExpr.Head == starPadSDK.MathExpr.WellKnownSym.plus)
                            {
                                //OR 
                                if (compositeExpr.Args.Length > 2)
                                {
                                    Expr[] myArgs = new Expr[compositeExpr.Args.Length - 1];
                                    Array.Copy(compositeExpr.Args, 0, myArgs, 0, i);
                                    Array.Copy(compositeExpr.Args, i + 1, myArgs, i, compositeExpr.Args.Length - i - 1);

                                    CompositeExpr notExpr = new CompositeExpr(starPadSDK.MathExpr.WellKnownSym.plus, myArgs);
                                    expr = notExpr;

                                }
                                else if (compositeExpr.Args.Length == 2)
                                {
                                    if (i == 0)
                                    {
                                        expr = compositeExpr.Args[1];
                                    }
                                    else {
                                        expr = compositeExpr.Args[0];
                                    } 
                                }
                                output = 2;
                            }
                            else if (compositeExpr.Head == starPadSDK.MathExpr.WellKnownSym.times)
                            {
                                //AND
                                output = 0;
                                return;
                            }
                            else if (compositeExpr.Head == starPadSDK.MathExpr.WellKnownSym.xorGate)
                            {
                                //XOR
                                if (compositeExpr.Args.Length > 2)
                                {
                                    Expr[] myArgs = new Expr[compositeExpr.Args.Length - 1];
                                    Array.Copy(compositeExpr.Args, 0, myArgs, 0, i);
                                    Array.Copy(compositeExpr.Args, i + 1, myArgs, i, compositeExpr.Args.Length - i - 1);

                                    CompositeExpr notExpr = new CompositeExpr(starPadSDK.MathExpr.WellKnownSym.xorGate, myArgs);
                                    expr = notExpr;

                                }
                                else if (compositeExpr.Args.Length == 2)
                                {
                                    if (i == 0)
                                    {
                                        expr = compositeExpr.Args[1];
                                    }
                                    else
                                    {
                                        expr = compositeExpr.Args[0];
                                    }
                                }
                                output = 2;
                            }
                        }
                    }
                    else {
                        FilterOneAndZero(ref compositeExpr.Args[i], ref output);
                    }
                }
            }


        
        }

        #endregion

        private void GCScroller_LayoutUpdated(object sender, EventArgs e)
        {
            SizeCanvas();
        }

        private void SizeCanvas()
        {
            double maxx = GCScroller.ViewportWidth, maxy = GCScroller.ViewportHeight;

            maxx = Math.Max(maxx, CurrentStrokesBoudningBox.Left + CurrentStrokesBoudningBox.Width + 100);
            maxy = Math.Max(maxy, CurrentStrokesBoudningBox.Top + CurrentStrokesBoudningBox.Height + 100);
            
            inqCanvas.Width = maxx;
            inqCanvas.Height = maxy;
        }

    }

}