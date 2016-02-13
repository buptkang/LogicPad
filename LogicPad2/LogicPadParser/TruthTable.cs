using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;


namespace LogicPadParser
{
    public class TruthTable
    {
        #region Variables

        protected int number_of_terms = 2;
        protected int _rows;
        protected int _columns;
        protected int sum_of_products_or_product_of_sums;
        protected int one_sol_or_all_possible;
        protected String[] terms_names;      
        protected int[] fvalues;
        protected IDictionary<Rect, string> _outputMap;
        protected string _outputName;

        public int Number_of_terms
        {
            get { return number_of_terms; }
            set { number_of_terms = value; }
        }
        public int Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }
        public int Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }
        public int Sum_of_products_or_product_of_sums
        {
            get { return sum_of_products_or_product_of_sums; }
            set { sum_of_products_or_product_of_sums = value; }
        }
        public int One_sol_or_all_possible
        {
            get { return one_sol_or_all_possible; }
            set { one_sol_or_all_possible = value; }
        }
        public String[] Terms_names
        {
            get { return terms_names; }
            set { terms_names = value; }
        }

        public int[] Fvalues
        {
            get { return fvalues; }
            set { fvalues = value; }
        }
        public IDictionary<Rect, string> OutputMap
        {
            get { return _outputMap; }
            set { _outputMap = value; }
        }
        public string outputName
        {
            get
            {
                if (OutputMap != null)
                {
                    foreach (String name in OutputMap.Values)
                    {
                        return name;
                    }
                    return null;
                }
                else {
                    return _outputName;     
                } 
            }
            set { }
        }
        public string OutputName
        {
            set { _outputName = value; }
            get { return _outputName; }
        }

        public static readonly double GAP = 55.0;

        public IDictionary<Rect, string> TermNameDict
        {
            get { return _termNameDict; }
            set { _termNameDict = value; }
        }

        protected IDictionary<Rect, string> _termNameDict;

        public readonly String[] default_terms_names = 
        {"A", "B", "C", "D", "E", "F", "G", "H" , "I" , "J" , "K" , 
            "L" , "M" , "N" , "O" , "P", "Q", "R", "S", "T", "U" , "V", "W", "X", "Y", "Z" };

        #endregion

        public TruthTable(int number_of_terms)
        {
            this.Number_of_terms = number_of_terms;

            one_sol_or_all_possible = 1;
            sum_of_products_or_product_of_sums = 0;
        }

        #region TruthTable Initialization

        public virtual void DrawTruthTableHeaders() { }

        public virtual void DrawTruthTableValues() { }

        public virtual void DrawGrid() { }

        public void InitDrawing()
        {
            DrawTruthTableHeaders();
            DrawTruthTableValues();
            DrawGrid();
        }

        #endregion

        public Dictionary<int, string[]> ConstructTruthTableRowList()
        {
            Dictionary<int, string[]> truthTableRowList = new Dictionary<int, string[]>();
            int row = (int)Math.Pow(2, this.Number_of_terms);
            StringBuilder builder = null;
            int[] indexBits = null;
            string[] indexChars = null;
            int seq = 0;
            for (int i = 0; i < row; i++)
            {
                builder = new StringBuilder();

                //byte bitString = i.ToString();
                indexBits = new int[number_of_terms];
                indexChars = new string[number_of_terms];
                seq = number_of_terms - 1;
                ConvertIndexFromDecimalToBinary(i, ref indexBits, ref indexChars, ref seq);
                truthTableRowList.Add(i, indexChars);
            }
            return truthTableRowList;
        }

        private void ConvertIndexFromDecimalToBinary(int index, ref int[] indexBits, ref string[] indexChars, ref int seq)
        {
            if (seq < 0)
            {
                return;
            }

            int temp  = index / 2;

            if(index % 2 == 0)
            {
                indexBits[seq] = 0;
                //NOT
                indexChars[seq] = Terms_names[seq] + "'";
                
            }else{
                indexBits[seq] = 1;
                indexChars[seq] = Terms_names[seq];
            }
            seq -= 1;

            ConvertIndexFromDecimalToBinary(temp, ref indexBits, ref indexChars,ref seq);
        }

    }
}
