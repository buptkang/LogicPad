using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogicPadParser
{
    public class PrimeImplicantsChartTable
    {
        public PrimeImplicantsChartTable()
        { }

        public PrimeImplicantsChartTable(int[] values,
              List<int> mini_vals,
              List<int> mini_subs,
              int one_sol_or_all_possible)
            : base()
        {
            List<int> ones_positions_in_table;

            number_of_rows = mini_vals.Count;

            table = new List<PrimeImplicantsChartRow>();

            ones_positions_in_table = onesPositionsInTable(values);

            number_of_columns = ones_positions_in_table.Count;

            for (int i = 0; i < number_of_rows; i++)
                this.addRow(
                   createPrimeChartRow(calPossibleValues(mini_vals[i], mini_subs[i]),
                               ones_positions_in_table), i);

            calculateALL(one_sol_or_all_possible);
        }

        /*
         *onesPositionsInTable return ones position in table 
         */
        private List<int> onesPositionsInTable(int[] values)
        {
            List<int> pos = new List<int>();

            for (int i = 0; i < values.Length; i++)
                if (values[i] == 1)
                    pos.Add(i);

            return pos;
        }


        /*
         *create the a row in the prime implicants table
         */

        private int[] createPrimeChartRow(List<int> possible_values, List<int> pos)
        {
            int p = pos.Count;

            int[] check = new int[p];

            for (int i = 0; i < p; i++)
                for (int j = 0; j < possible_values.Count; j++)
                    if (pos[i] == possible_values[j])
                    {
                        check[i] = 1;
                        break;
                    }
            return check;
        }

        /*
         *calPossibleValues calculate all possible values from number and sub field 
         */

        private List<int> calPossibleValues(int number, int sub)
        {
            List<int> values = new List<int>();

            List<int> pos = calNumberAndPositionsOfOnes(sub);

            int s = pos.Count;

            int pow = (int)Math.Pow(2, s);

            int sum = 0;

            for (int i = 0; i < pow; i++)
            {
                for (int j = 0; j < s; j++)
                    sum = sum + (decimalToBinary(i, j) * (int)Math.Pow(2, pos[j]));

                values.Add(number + sum);

                sum = 0;
            }
            return values;
        }

        /*
         *calNumberAndPositionsOfOnes return the position of ones in a number ex: 11(1011) the positions is 1,2,4
         */

        private List<int> calNumberAndPositionsOfOnes(int number)
        {
            int p = (int)Math.Ceiling(Math.Log(number) / Math.Log(2));

            int max = (int)Math.Pow(2, p);

            List<int> pos = new List<int>();

            int count = 0;

            while (max != 0)
            {
                if (number >= max)
                {
                    number -= max;

                    pos.Add(p - count);
                }
                count++;

                max /= 2;
            }
            return pos;
        }

        /*
         *calculateALL calculate the answer
         */

        private void calculateALL(int one_sol_or_all_possible)
        {
            createEssentialPrimesIndexs();

            if (checkNonEssentialExist())
            {
                if (one_sol_or_all_possible == 0)
                    createMinimumeCompinationOfNonEssentialTable();

                else
                    createOneMinimumeCompinationOfNonEssentialTable();
            }
        }

        /*
         *check if there is any unessential terms
         */

        private bool checkNonEssentialExist()
        {
            bool check = false;

            for (int i = 0; i < satisfied_columns_by_essintial_primes.Length; i++)
                if (satisfied_columns_by_essintial_primes[i] == 0)
                    check = true;

            return (check && (non_essential_table.Count > 0));
        }

        /*
         *get the essential terms
         */

        private void createEssentialPrimesIndexs()
        {
            int count = 0;

            int essential_prime_index = -1;

            int[] mark_essential_primes = new int[number_of_rows];

            satisfied_columns_by_essintial_primes = new int[number_of_columns];

            for (int i = 0; i < number_of_columns; i++)
            {
                for (int j = 0; j < number_of_rows; j++)
                {
                    if (table[j].getCell(i) == 1)
                    {
                        count++;

                        essential_prime_index = j;
                    }
                }
                if (count == 1)
                {
                    mark_essential_primes[essential_prime_index] = 1;

                    satisfied_columns_by_essintial_primes[i] = 1;
                }
                count = 0;
            }
            for (int i = 0; i < number_of_rows; i++)
            {
                if (mark_essential_primes[i] == 0)
                    non_essential_table.Add(table[i]);

                else
                    essential_primes_indexs.Add(i);
            }
            for (int i = 0; i < essential_primes_indexs.Count; i++)
                for (int j = 0; j < number_of_columns; j++)
                    if ((table[(essential_primes_indexs[i])]).getCell(j) == 1)
                        satisfied_columns_by_essintial_primes[j] = 1;
        }

        /*
         *createMinimumeCompinationOfNonEssentialTable is a function that calculates all possible minimume compinations of the non essential terms
         */

        private void createMinimumeCompinationOfNonEssentialTable()
        {
            int[] check = new int[number_of_columns];

            int smallest = non_essential_table.Count;

            GenerateNumbersOrderedByNumberOfOnes g = new GenerateNumbersOrderedByNumberOfOnes(non_essential_table.Count);

            List<long> temp = new List<long>();

            bool mark = false;

            List<int> temp_combine = new List<int>();

            check = satisfied_columns_by_essintial_primes;

            for (int i = 1; i < non_essential_table.Count; i++)
            {
                temp = g.getNumbers();

                for (int j = 0; j < temp.Count; j++)
                {
                    for (int k = 0; k < non_essential_table.Count; k++)
                    {
                        if (decimalToBinary(temp[j], k) == 1)
                        {
                            check = combine(non_essential_table[k].Row, check);

                            temp_combine.Add(non_essential_table[k].Row_index);
                        }
                        //TODO Check Current Thread
                        if (!Thread.CurrentThread.IsAlive)
                            return;
                    }
                    if (checkAnswer(check))
                    {
                        minimume_compination_of_non_essential_table.Add(temp_combine);

                        mark = true;
                    }
                    temp_combine = new List<int>();

                    check = satisfied_columns_by_essintial_primes;
                }
                if (mark)
                    break;
            }
        }

        /*
         * createOneMinimumeCompinationOfNonEssentialTable is a function that calculates one possible minimume compination of the non essential terms
         * and is faster than the function createMinimumeCompinationOfNonEssentialTable.
         */

        private void createOneMinimumeCompinationOfNonEssentialTable()
        {
            List<PrimeImplicantsChartRow> temp_table = non_essential_table;

            List<int> temp_combine = new List<int>();

            int max_n = 0;

            PrimeImplicantsChartRow max_r = new PrimeImplicantsChartRow();

            for (int j = 0; j < temp_table.Count; j++)
                temp_table[j].setRow(combineNotFirst(satisfied_columns_by_essintial_primes,
                    temp_table[j].Row), temp_table[j].Row_index);

            while (temp_table.Count > 0)
            {
                max_n = calculateRowWithMaximumNumberOfOnes(temp_table);

                max_r = temp_table[max_n];

                temp_combine.Add(max_r.Row_index);

                temp_table.RemoveAt(max_n);

                for (int j = 0; j < temp_table.Count; j++)
                {
                    temp_table[j].setRow(combineNotFirst(max_r.Row, temp_table[j].Row), temp_table[j].Row_index);

                    if (temp_table[j].getNumberOfOnesInRow() == 0)
                    {
                        temp_table.RemoveAt(j);
                        j--;
                    }
                }
            }
            minimume_compination_of_non_essential_table.Add(temp_combine);
        }


        /*
         *calculateRowWithMaximumNumberOfOnes returns the number of the row that has the maximum number of ones.
         */
        private int calculateRowWithMaximumNumberOfOnes(List<PrimeImplicantsChartRow> temp_table)
        {
            int max = temp_table[0].getNumberOfOnesInRow();

            int max_n = 0;

            for (int i = 1; i < temp_table.Count; i++)
            {
                if (temp_table[i].getNumberOfOnesInRow() > max)
                {
                    max = temp_table[i].getNumberOfOnesInRow();

                    max_n = i;
                }
            }
            return max_n;
        }

        /*
         *return a digit of the input number according to the position enterd
         *if the  number is 6(0110) and the position is 2 it will return 1
         */

        private int decimalToBinary(long number, int pos)
        {
            int temp = 0;

            for (int i = 0; i <= pos; i++)
            {
                temp = (int)(number % 2);

                number /= 2;
            }
            return temp;
        }

        /*
         *combine will combine two integer arrays of ones and zeros(works like an or gate)
         *x[] = 1110001, y[]=0111011
         *1110001 or  0111011 =>1111011
         */
        private int[] combine(int[] x, int[] y)
        {
            int[] temp = new int[x.Length];

            for (int i = 0; i < y.Length; i++)
                if (x[i] == 1 || y[i] == 1)
                    temp[i] = 1;
            return temp;
        }


        /*
         *combineNotFirst will combine two integer arrays of ones and zeros(works like an or gate but it reverse the first input(x) first)
         *x[] = 1110001, y[]=0111011
         *'(1110001) and  0111011 =>0111111
         */
        private int[] combineNotFirst(int[] x, int[] y)
        {
            int[] temp = new int[x.Length];

            for (int i = 0; i < y.Length; i++)
                if (x[i] == 1)
                    temp[i] = 0;
                else
                    temp[i] = y[i];
            return temp;
        }

        /*
         *check if all the inputed array digits is one 
         */
        private bool checkAnswer(int[] check)
        {
            bool ch = true;

            for (int i = 0; i < check.Length; i++)
                if (check[i] == 0)
                    ch = false;
            return ch;
        }

        void addRow(int[] row, int index)
        {
            PrimeImplicantsChartRow temp = new PrimeImplicantsChartRow();

            temp.setRow(row, index);

            table.Add(temp);
        }

        public int[] getRow(int row_number)
        {
            return table[row_number].Row;
        }

        /*******************************************************************************/

        #region variable and property list

        private List<PrimeImplicantsChartRow> table;

        private List<PrimeImplicantsChartRow> non_essential_table = new List<PrimeImplicantsChartRow>();

        private List<List<int>> minimume_compination_of_non_essential_table = new List<List<int>>();

        private List<int> essential_primes_indexs = new List<int>();

        private int number_of_rows;

        private int number_of_columns;

        private int[] satisfied_columns_by_essintial_primes;

        public List<List<int>> Minimume_compination_of_non_essential_table
        {
            get { return minimume_compination_of_non_essential_table; }
            set { minimume_compination_of_non_essential_table = value; }
        }

        public List<int> Essential_primes_indexs
        {
            get { return essential_primes_indexs; }
            set { essential_primes_indexs = value; }
        }

        public int[] Satisfied_columns_by_essintial_primes
        {
            get { return satisfied_columns_by_essintial_primes; }
            set { satisfied_columns_by_essintial_primes = value; }
        }

        public int Number_of_rows
        {
            get { return number_of_rows; }
            set { number_of_rows = value; }
        }

        public int Number_of_columns
        {
            get { return number_of_columns; }
            set { number_of_columns = value; }
        }

        #endregion
    }
}
