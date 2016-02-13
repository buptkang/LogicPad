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

namespace LogicPad2.TruthTable
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

        }

        #region properties

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

        public MSInkAnalysisCanvas TruthTableInkCanvas
        {
            get
            {
                return this.truthTableInkCanvas;
            }
        }

        public Button IncreaseTermButton
        {
            get { return this.AddTerm; }

        }

        public Button DecreaseTermButton
        {
            get { return this.DeleteTerm; }
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

        private bool numberOfTermSelected = false;

        private int number_of_terms;

        public int Terms
        {
            get { return number_of_terms; }
            set { number_of_terms = value; }
        }

        private string _generatedExpr;

        public string GeneratedExpr
        {
            get { return _generatedExpr; }
            set { _generatedExpr = value; }
        }
        #endregion

        #region Solve Button Trigger Event 
        public delegate void MiniBoolExprDelegate(InputTruthTable currentTruthTable);

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            if (!numberOfTermSelected)
            {
                MessageBox.Show("Select number of terms firstly");
                return;
            }
            
            MiniBoolExprDelegate miniExpr = new MiniBoolExprDelegate(MinimizeBooleanExpression);

            //this.Dispatcher.BeginInvoke(miniExpr, this.truthTableInkCanvas.TruthTable);   
        }

        public void MinimizeBooleanExpression(InputTruthTable currentTruthTable)
        {
            //TruthTableSolver solver = TruthTableSolver.Instance;
            LogicPadParser.TruthTableSolver solver = new LogicPadParser.TruthTableSolver();
            solver.CurrentTruthTable = currentTruthTable;
            solver.Solve();
            this.GeneratedExpr = currentTruthTable.outputName + " = " + solver.Solution;
            MessageBox.Show(this.GeneratedExpr);
        }
        #endregion

        #region Parameter Selection

        public void IncreaseTerm()
        {
            if (numberOfTermSelected)
            {
                ClearCanvas();
                ++Terms;
            }
            else
            {
                //Do not selected before
                //Term initialization
                Terms = 2;
                numberOfTermSelected = true;
            }

            this.myTerm.Content = Terms.ToString();

            this.truthTableInkCanvas.ShowInkAnalysisFeedback = false;

            //Initialize one TruthTable
            InputTruthTable truthTable = new InputTruthTable(this.truthTableInkCanvas, Terms);

            truthTable.truthTableInputErrorDisplayed += new DisplayTruthTableErrorHandler(truthTable_truthTableInputErrorDisplayed);

            truthTable.InitDrawing();

            this.truthTableInkCanvas.TruthTable = truthTable;

        }

        void truthTable_truthTableInputErrorDisplayed(object sender, TruthTableErrorArgs e)
        {
            //MessageBox.Show("Test");
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                MessageBox.Show(e.ErrorMsg);
                if (e.RemovedStrokes != null)
                {
                    this.truthTableInkCanvas.Strokes.Remove(e.RemovedStrokes);
                    this.truthTableInkCanvas.InkAnalyzer.RemoveStrokes(e.RemovedStrokes);
                    this.truthTableInkCanvas.ShowInkAnalysisFeedback = false;
                }
            }));
        }

        public void DecreaseTerm()
        {
            if (numberOfTermSelected)
            {
                ClearCanvas();
                --Terms;
            }
            else
            {
                //Do not selected before
                //Term initialization
                Terms = 2;
                numberOfTermSelected = true;
            }

            this.myTerm.Content = Terms.ToString();

            this.truthTableInkCanvas.ShowInkAnalysisFeedback = false;

            //Initialize one TruthTable
            InputTruthTable truthTable = new InputTruthTable(this.truthTableInkCanvas, Terms);

            truthTable.truthTableInputErrorDisplayed += new DisplayTruthTableErrorHandler(truthTable_truthTableInputErrorDisplayed);

            truthTable.InitDrawing();

            this.truthTableInkCanvas.TruthTable = truthTable;

        }

        private void ClearCanvas()
        {
            this.truthTableInkCanvas.Children.Clear();
            this.truthTableInkCanvas.InkAnalyzer.RemoveStrokes(this.truthTableInkCanvas.Strokes);
            this.truthTableInkCanvas.Strokes.Clear();

        }
		
        #endregion 

        #region Event Handler

        public new void StylusDown(object sender, StylusDownEventArgs e)
        {
            this.TruthTableInkCanvas.ShowInkAnalysisFeedback = true;
        }

        public void StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            Matrix m = new Matrix();
            m.Translate(-UserControlX, -UserControlY);
            m.Scale(1 / UserControlScaleX, 1 / UserControlScaleY);
            e.Stroke.Transform(m, false);

            this.TruthTableInkCanvas.OnStrokesCollected(this, e);
        }

        #region un-used event handler

            public new void PreviewStylusMove(object sender, StylusEventArgs e)
            {
            }

            public new void PreviewMouseDown(object sender, MouseButtonEventArgs e)
            {
            }

            public new void StylusMove(object sender, StylusEventArgs e)
            {

            }

            public new void StylusUp(object sender, StylusEventArgs e)
            {

            }

            public new void PreviewStylusDown(object sender, StylusDownEventArgs e)
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
        #endregion
        #endregion


        public XElement Parse(out string outString)
        {
            LogicPadParser.TruthTableSolver solver = new LogicPadParser.TruthTableSolver();
            solver.CurrentTruthTable = this.TruthTableInkCanvas.TruthTable;
            solver.Solve();

            if (solver.Solution.Equals("1"))
            {
                outString = "1";
                return null;
            }
            else if (solver.Solution.Equals("0"))
            {
                outString = "0";
                return null;
            }
            else
            {
                outString = LogicPadParser.InterTree.ParseExpressionToString(LogicPadParser.LogicPadParser.truthTable);
                return LogicPadParser.LogicPadParser.Instance.ParseTruthTable(LogicPadParser.LogicPadParser.truthTable);
            }
        }

    }
}
