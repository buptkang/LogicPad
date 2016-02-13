using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Documents;
using System.Windows.Media;
using starPadSDK.Inq;
using starPadSDK.Inq.MSInkCompat;
using starPadSDK.Geom;

namespace LogicPad2.TruthTable
{
    public class MSInkAnalysisCanvas : System.Windows.Controls.InkCanvas
    {
        public MSInkAnalysisCanvas()
        {
            this.StrokeCollected += OnStrokesCollected;
          
            this.StylusDown += (sender, args) =>
            {
                    this.ShowInkAnalysisFeedback = true;
            };
            
            InitInkAnalysis();
        }

        public void InitInkAnalysis()
        {
            _inkAnalyzer = new InkAnalyzer(this.Dispatcher);
                        // Add a listener to StrokesChanged event of InkAnalysis.Strokes collection.

            // Add a listener to ResultsUpdated event.
            _inkAnalyzer.ResultsUpdated += OnInkAnalyzerResultsUpdated;

            _inkAnalyzer.AnalysisModes = AnalysisModes.AutomaticReconciliationEnabled;

            this.ShowInkAnalysisFeedback = true;

         //   _inkAnalyzer.Activity += new ActivityEventHandler(_inkAnalyzer_Activity);            
        }

        void _inkAnalyzer_Activity(object sender, EventArgs e)
        {
            InkAnalyzer theInkAnalyzer = sender as InkAnalyzer;

            AnalysisRegion theAnalysisRegion = theInkAnalyzer.Abort();

            // Add the region that was being analyzed to the analyzer's dirty region.
            theInkAnalyzer.DirtyRegion.Union(theAnalysisRegion); 
        }

        public void DeconstructInkAnalysis()
        {
            _inkAnalyzer.Dispose();
            _inkAnalyzer = null;
        }


        //Check if the stroke is inside of ink region
        private bool IsWrittenInInkRegion(Rect rect, out bool isheader)
        {
            foreach (InkRegion _inkRegion in TruthTable.InkRegions)
            {
                if (_inkRegion.Rect.IntersectsWith(rect))
                {
                    AnalyzedRegion = _inkRegion;
                    if (_inkRegion.IsHeader)
                    {
                        isheader = true;
                    }
                    else {
                        isheader = false;
                    }
                    return true;
                }
                else
                {
                    continue;
                }
            }
            isheader = false;
            return false;
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
                bool removeCurrentStroke = CheckRemoveHeader(stroke);

                IEnumerable<starPadSDK.Geom.Pt> hull = stroq.ConvexHull();

                List<Point> hullTemp = new List<Point>();
                foreach (starPadSDK.Geom.Pt pt in hull)
                {
                    hullTemp.Add(new Point(pt.X, pt.Y));
                }

                StrokeCollection stks = this.Strokes.HitTest(hullTemp, 1);  
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

                if (this.Strokes.Contains(stroke) && removeCurrentStroke)
                {
                    this.Strokes.Remove(stroke);
                    //AnalyzedRegion.Strokes.Remove(stroke);
                }
                this.ShowInkAnalysisFeedback = false;
                return true;
            }
            return false;
        }

        Deg fpdangle(Pt a, Pt b, Vec v)
        {
            return (a - b).Normalized().UnsignedAngle(v.Normalized());
        }

        private bool CheckRemoveHeader(Stroke stroke)
        {
            Rect strokeBound = stroke.GetBounds();
            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement header = this.Children[i];

                if (header is InkFigure)
                {
                    InkFigure g = header as InkFigure;
                    Rect grect = g.Bound;
                    bool isOutputHeader = false;
                    if (strokeBound.IntersectsWith(grect))
                    {
                        this.Children.Remove(header);
                        
                        //Need to change later, code is not elegant here--I hate my code here BO KANG.
                        Rect inkRect = new Rect();
                        foreach(KeyValuePair<Rect, string> pair in TruthTable.TermNameDict)
                        {
                            if (g.Text.Text.Equals(pair.Value))
                            {
                                inkRect = pair.Key;
                                isOutputHeader = false;
                                break;
                            }
                        }

                        foreach(KeyValuePair<Rect, string> pair in TruthTable.OutputMap)
                        {
                            if (g.Text.Text.Equals(pair.Value))
                            {
                                inkRect = pair.Key;
                                isOutputHeader = true;
                                break;
                            }
                        }

                        if (!isOutputHeader)
                        {
                            TruthTable.TermNameDict[inkRect] = null;
                        }
                        else
                        {
                            TruthTable.OutputMap[inkRect] = null;
                        }
                        
                        return true;
                    } 
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// OnStrokesChanged - A handler for InkCanvas.Strokes.StrokesChanged event
        /// </summary>
        public void OnStrokesCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            Stroke currentStroke = e.Stroke;

            this.Strokes.Add(currentStroke);

            if (TruthTable == null)
            {
                this.Strokes.Remove(currentStroke);
                return;
            }

            Rect rect = currentStroke.GetBounds();

            bool isHeader = false;

            if (IsWrittenInInkRegion(rect, out isHeader))
            {             
                if (ScribbleDelete(currentStroke)) return;
  
                // Add the region that was being analyzed to the analyzer's dirty region.
                //_inkAnalyzer.DirtyRegion.Union(theAnalysisRegion);
                if (isHeader)
                {
                    foreach (Stroke stroke in this.Strokes)
                    {
                        if (!stroke.GetBounds().IntersectsWith(rect))
                        {
                            _inkAnalyzer.RemoveStroke(stroke);
                        }
                    }
                }
                else
                {
                    _inkAnalyzer.RemoveStrokes(this.Strokes);
                }
              
                _inkAnalyzer.AddStroke(currentStroke);
                _inkAnalyzer.SetStrokeType(currentStroke, StrokeType.Unspecified);
                _inkAnalyzer.BackgroundAnalyze();

                this.ShowInkAnalysisFeedback = true;  
            }
            else
            {
                this.Strokes.Remove(currentStroke);
            }
        }

        /// <summary>
        /// OnInkAnalyzerResultsUpdated - A handler for InkAnalyzer.ResultsUpdated event
        /// which is called when BackgroundAnalyze completes
        /// </summary>
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

        /// <summary>
        /// Set to true to display the parse structure and recognition results
        /// overlayed on the InkAnalysisCanvas
        /// </summary>
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

        #region Properties

        private InputTruthTable _truthTable;

        public InputTruthTable TruthTable
        {
            get { return _truthTable; }
            set {
                _truthTable = value;
                _feedbackAdorner.TruthTable = value;
            }
        }


        private InkRegion _analyzedRegion;

        public InkRegion AnalyzedRegion
        {
            get { return _analyzedRegion; }
            set { _analyzedRegion = value; }
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

        public InkAnalysisFeedbackAdorner FeedbackAdorner1
        {
            get { return _feedbackAdorner; }
            set { _feedbackAdorner = value; }
        }

        #endregion
    }
}