using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Ink;

using LogicPadParser;

namespace LogicPad2.TruthTable
{
    public delegate void DisplayTruthTableErrorHandler(object sender, TruthTableErrorArgs e);

    public class InputTruthTable : LogicPadParser.TruthTable
    {
        public event DisplayTruthTableErrorHandler truthTableInputErrorDisplayed;

        private System.Windows.Controls.InkCanvas inkCanvas;

        public System.Windows.Controls.InkCanvas InkCanvas
        {
            get { return inkCanvas; }
            set { inkCanvas = value; }
        }
        private List<InkRegion> inkRegions;

        public List<InkRegion> InkRegions
        {
            get { return inkRegions; }
            set { inkRegions = value; }
        }


        public InputTruthTable(System.Windows.Controls.InkCanvas inkCanvas, int number_of_terms)
            : base(number_of_terms)
        {
            //Rows 
            this.Rows = (int)Math.Pow(2, this.Number_of_terms) + 1;
            //Columns
            this.Columns = this.Number_of_terms + 2;

            this.InkCanvas = inkCanvas;

            //Default Settings for term names 
            this.terms_names = new String[number_of_terms];
            Array.Copy(default_terms_names, 0, terms_names, 0, number_of_terms);

            TermNameDict = new Dictionary<Rect, string>();
            OutputMap = new Dictionary<Rect, string>();

            this.fvalues = new int[(int)Math.Pow(2, number_of_terms)];

            InkRegions = new List<InkRegion>();

        }

        #region Drawing
        /*
         * In general, there will be (number of terms + 2) columns
         * 
         * For rows, besides header row, there will be n ^ 2 rows
         *  
         */
        public override void DrawGrid()
        {
            double initValue = GAP;

            //Draw all the horizontal lines
            Line h_line = null;

            for (int i = 0; i <= this.Rows; i++)
            {
                h_line = new Line();
                h_line.Stroke = Brushes.Black;

                h_line.X1 = GAP;
                h_line.X2 = h_line.X1 + this.Columns * GAP;
                h_line.Y1 = initValue;
                h_line.Y2 = initValue;

                initValue += GAP;

                InkCanvas.Children.Add(h_line);
            }

            initValue = GAP;

            //Draw all the vertical lines
            Line v_line = null;
            for (int j = 0; j <= this.Columns; j++)
            {
                v_line = new Line();
                v_line.Stroke = Brushes.Black;

                v_line.X1 = initValue;
                v_line.X2 = initValue;
                v_line.Y1 = GAP;
                v_line.Y2 = v_line.Y1 + this.Rows * GAP;

                initValue += GAP;

                InkCanvas.Children.Add(v_line);
            }
        }//drawGrid

        //Black brush
        public override void DrawTruthTableHeaders()
        {
            Rect b;
            Point center;
            Brush color;
            InkFigure header = null;

            InkRegion inkRegion;

            //For Row 0, draw headers 
            for (int i = 0; i < Number_of_terms; i++)
            {
                b = new Rect(GAP + (i + 1) * GAP, GAP, GAP, GAP);
                center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
                color = Brushes.Black;
                header = new InkFigure(Terms_names[i], b, center, color);
                header.SetHeader();
                InkCanvas.Children.Add(CreateInkRegion(b));
                InkCanvas.Children.Add(header);
                //Create one InkRegion
                inkRegion = new InkRegion(b);
                InkRegions.Add(inkRegion);

                TermNameDict.Add(b, Terms_names[i]);
            }

            //Output
            b = new Rect(GAP * Columns, GAP, GAP, GAP);
            center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
            color = Brushes.Black;
            header = new InkFigure("Output", b, center, color);
            header.SetHeader();
            InkCanvas.Children.Add(CreateInkRegion(b));
            InkCanvas.Children.Add(header);

            inkRegion = new InkRegion(b);
            inkRegion.IsOutputHeader = true;
            InkRegions.Add(inkRegion);

            OutputMap.Add(b, "Output");
        }



        /*
         *  V is the sequence line number 
         * 
         */
        private char[] ComputeRowValue(int v, int number_of_terms)
        {
            char[] rowValues = new char[number_of_terms];

            for (int i = 0; i < rowValues.Length; i++)
            {
                rowValues[i] = '0';
            }

            char[] convertedValues = (Convert.ToString(v, 2)).ToCharArray();

            Array.Copy(convertedValues, 0, rowValues, rowValues.Length - convertedValues.Length, convertedValues.Length);

            return rowValues;
        }

        public override void DrawTruthTableValues()
        {
            Rect b;
            Point center;
            Brush color;
            InkFigure values = null;
            InkRegion inkRegion;

            int termSquares = (int)Math.Pow(2, number_of_terms);

            for (int i = 0; i < termSquares; i++)
            {
                //Draw sequence number
                b = new Rect(GAP, GAP + (i + 1) * GAP, GAP, GAP);
                center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
                color = Brushes.Black;
                values = new InkFigure(i.ToString(), b, center, color);
                values.SetHeader();
                InkCanvas.Children.Add(values);

                //Draw truth table row numbers
                char[] rowValues = ComputeRowValue(i, number_of_terms);
                for (int j = 0; j < Terms_names.Length; j++)
                {
                    b = new Rect((j + 1) * GAP, (i + 1) * GAP, GAP, GAP);
                    center = new Point(b.X + b.Width + b.Width / 2, b.Y + b.Height + b.Height / 2);
                    color = Brushes.Blue;
                    values = new InkFigure(rowValues[j].ToString(), b, center, color);
                    InkCanvas.Children.Add(values);
                }

                //For output column
                //Output
                b = new Rect(GAP * Columns, (i + 2) * GAP, GAP, GAP);
                InkCanvas.Children.Add(CreateInkRegion(b));
                inkRegion = new InkRegion(b, i);
                InkRegions.Add(inkRegion);
            }
        }

        #endregion

        public bool IsHeaderBox(Rect rect)
        {
            foreach (InkRegion region in InkRegions)
            {
                if (region.Rect.Contains(rect))
                {
                    if (region.IsHeader)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private Path CreateInkRegion(Rect rect)
        {
            Path path = new Path();
            path.Fill = Brushes.LightBlue;

            RectangleGeometry rectangle = new RectangleGeometry();
            rectangle.Rect = rect;

            path.Data = rectangle;

            return path;
        }

        public void insertValue(Rect rect, string value)
        {
            foreach (InkRegion region in InkRegions)
            {
                if (region.Rect.Contains(rect))
                {
                    if (!region.IsHeader)
                    {
                        if (region.IsInputInvalid)
                        {
                            InkCanvas.Children.Remove(region.ErrorProneFigure);
                            region.IsInputInvalid = false;
                            region.ErrorProneFigure = null;
                        }
                        region.HasInputValue = true;
                        this.Fvalues[region.RowIndex] = Convert.ToInt32(value);
                    }
                    else
                    {
                        if (region.IsInputInvalid)
                        {
                            InkCanvas.Children.Remove(region.ErrorProneFigure);
                            region.IsInputInvalid = false;
                            region.ErrorProneFigure = null;
                        }
                        if (!region.IsOutputHeader)
                        {
                            //Change term_names
                            TermNameDict[region.Rect] = value;
                        }
                        else
                        {
                            OutputMap[region.Rect] = value;
                        }
                        region.HasInputValue = true;
                    }
                }
            }
        }

        public bool CheckInputValueCompleted()
        {
            foreach (InkRegion _region in this.inkRegions)
            {
                if (!_region.HasInputValue && !_region.IsHeader)
                {
                    TruthTableErrorArgs args = new TruthTableErrorArgs();
                    args.ErrorMsg = "Pls complete input values!!!";
                    truthTableInputErrorDisplayed(this, args);
                    return false;
                }
            }
            return true;
        }


        public void DisplayInputErrorInfo(Rect rect, StrokeCollection removedStrokes)
        {
            foreach (InkRegion inkRegion in InkRegions)
            {
                if (inkRegion.Rect.Contains(rect))
                {
                    TruthTableErrorArgs args = new TruthTableErrorArgs();
                    args.RemovedStrokes = removedStrokes;

                    if (!inkRegion.IsInputInvalid)
                    {
                        inkRegion.IsInputInvalid = true;
                    }
                    else
                    {
                        return;
                    }

                    if (!inkRegion.IsHeader)
                    {
                        args.ErrorMsg = "Warning: Input 1 or 0 in this region!";
                        truthTableInputErrorDisplayed(this, args);
                    }
                    else
                    {
                        /*
                        b = new Rect(inkRegion.Rect.X, inkRegion.Rect.Y - GAP, 2 * GAP, GAP);
                        center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
                        color = Brushes.Red;
                        values = new InkFigure("Warning: Input capitalized single letter in the below box!", b, center, color);

                        inkRegion.ErrorProneFigure = values;

                        InkCanvas.Children.Add(values);
                        */
                        args.ErrorMsg = "Warning: Input capitalized single letter in this region!";
                        truthTableInputErrorDisplayed(this, args);
                    }
                }

            }
        }


    }

    public class InkRegion
    {
        private Rect _rect;

        public Rect Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }

        private bool _isHeader;

        public bool IsHeader
        {
            get { return _isHeader; }
            set { _isHeader = value; }
        }

        private bool _isOutputHeader;

        public bool IsOutputHeader
        {
            get { return _isOutputHeader; }
            set { _isOutputHeader = value; }
        }

        private bool hasInputValue;

        public bool HasInputValue
        {
            get { return hasInputValue; }
            set { hasInputValue = value; }
        }

        private int _rowIndex;

        public int RowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }

        private bool isInputInvalid;

        public bool IsInputInvalid
        {
            get { return isInputInvalid; }
            set { isInputInvalid = value; }
        }

        private InkFigure _errorProneFigure;

        public InkFigure ErrorProneFigure
        {
            get { return _errorProneFigure; }
            set { _errorProneFigure = value; }
        }

        public InkRegion(Rect rect)
        {
            this._rect = rect;
            this._isHeader = true;
        }

        public InkRegion(Rect rect, int rowIndex)
        {
            this._rect = rect;
            this._isHeader = false;
            this._rowIndex = rowIndex;
        }
    }

    public class TruthTableErrorArgs : EventArgs
    {
        private string errorMsg;

        private StrokeCollection removedStrokes;

        public string ErrorMsg { get { return errorMsg; } set { errorMsg = value; } }

        public StrokeCollection RemovedStrokes { get { return removedStrokes; } set { removedStrokes = value; } }

        public TruthTableErrorArgs()
        {
        }
    }
}
