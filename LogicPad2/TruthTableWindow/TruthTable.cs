using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace TruthTableWindow
{
    public class TruthTable
    {
        public TruthTable()
        {
        }

        private double _truthTableHeight;
        public double TruthTableHeight
        {
            set { _truthTableHeight = value; }
            get { return _truthTableHeight; }
        }

        private double _truthTableWidth;
        public double TruthTableWidth
        {
            set { _truthTableWidth = value; }
            get { return _truthTableWidth; }
        }

        public void InitDrawing()
        {
            DrawTruthTableHeaders();
            DrawTruthTableValues();
            DrawGrid();
        }

        public void DrawGrid()
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
                _truthTableWidth += this.Columns * GAP /5;
                h_line.Y1 = initValue;
                h_line.Y2 = initValue;

                initValue += GAP;

                RenderCanvas.Children.Add(h_line);
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
                _truthTableHeight += this.Rows * GAP / 5;

                initValue += GAP;

                RenderCanvas.Children.Add(v_line);
            }
        }//drawGrid

        //Black brush
        public void DrawTruthTableHeaders()
        {
            Rect b;
            Point center;
            Brush color;
            InkFigure header = null;

            //For Row 0, draw headers 
            for (int i = 0; i < Number_of_terms; i++)
            {
                b = new Rect(GAP + (i + 1) * GAP, GAP, GAP, GAP);
                center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
                color = Brushes.Black;
                header = new InkFigure(Terms_names[i], b, center, color);
                header.SetHeader();
                //RenderCanvas.Children.Add(CreateInkRegion(b));
                RenderCanvas.Children.Add(header);
            }

            //Output
            b = new Rect(GAP * Columns, GAP, GAP, GAP);
            center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
            color = Brushes.Black;
            header = new InkFigure("Output", b, center, color);
            header.SetHeader();
            //RenderCanvas.Children.Add(CreateInkRegion(b));
            RenderCanvas.Children.Add(header);
        }

        public void DrawTruthTableValues()
        {
            Rect b;
            Point center;
            Brush color;
            InkFigure values = null;

            int termSquares = (int)Math.Pow(2, number_of_terms);

            for (int i = 0; i < termSquares; i++)
            {
                //Draw sequence number
                b = new Rect(GAP, GAP + (i + 1) * GAP, GAP, GAP);
                center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
                color = Brushes.Black;
                values = new InkFigure(i.ToString(), b, center, color);
                values.SetHeader();
                RenderCanvas.Children.Add(values);

                //Draw truth table row numbers
                char[] rowValues = ComputeRowValue(i, number_of_terms);
                for (int j = 0; j < Terms_names.Length; j++)
                {
                    b = new Rect((j + 1) * GAP, (i + 1) * GAP, GAP, GAP);
                    center = new Point(b.X + b.Width + b.Width / 2, b.Y + b.Height + b.Height / 2);
                    color = Brushes.Blue;
                    values = new InkFigure(rowValues[j].ToString(), b, center, color);
                    RenderCanvas.Children.Add(values);
                }

                //For output column
                //Output
                b = new Rect(GAP * Columns, (i + 2) * GAP, GAP, GAP);
                center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
                color = Brushes.Red;
                values = new InkFigure(Fvalues[i].ToString(), b, center, color);
                values.SetResult();
                RenderCanvas.Children.Add(values);
            }
        }

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


        #region Properties

        public static readonly double GAP = 55.0;

        private Canvas _renderCanvas;

        public Canvas RenderCanvas
        {
            set { _renderCanvas = value; }
            get { return _renderCanvas; }
        }

        private int number_of_terms;

        public int Number_of_terms
        {
            get { return number_of_terms; }
            set { number_of_terms = value; }
        }

        private int _rows;

        public int Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        private int _columns;

        public int Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        private String[] terms_names;

        public String[] Terms_names
        {
            get { return terms_names; }
            set { terms_names = value; }
        }

        private int[] fvalues;

        public int[] Fvalues
        {
            get { return fvalues; }
            set { fvalues = value; }
        }

        private String _outputName;

        public String OutputName
        {
            set { _outputName = value; }
            get { return _outputName; }
        }

        #endregion

    }
}
