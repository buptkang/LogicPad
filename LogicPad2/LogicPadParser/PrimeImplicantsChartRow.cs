using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPadParser
{
    public class PrimeImplicantsChartRow
    {
        public PrimeImplicantsChartRow()
        {
            row_index = -1;

            row_length = 0;
        }

        public int getNumberOfOnesInRow()
        {
            int count = 0;

            for (int i = 0; i < row_length; i++)
                if (row[i] == 1)
                    count++;

            return count;
        }

        public int getCell(int cell_number)
        {
            return row[cell_number];
        }

        public void setRow(int[] x_row, int index)
        {
            Row = x_row;
            Row_index = index;
            Row_length = Row.Length;
        }

        private int[] row; //prime implicants row values

        private int row_index;//prime implicants row index

        private int row_length; //prime implicant row length

        #region Property List

        public int[] Row
        {
            get { return row; }
            set
            {
                row = value;
                Row_length = row.Length;
            }
        }

        public int Row_index
        {
            get { return row_index; }
            set { row_index = value; }
        }

        public int Row_length
        {
            get { return row_length; }
            set { row_length = value; }
        }

        #endregion
    }
}
