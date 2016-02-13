using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPadParser
{
    public class TruthTableSolver
    {
        public TruthTableSolver()
        {
            table = new TList();
        }

        private TruthTable _currentTruthTable;

        public TruthTable CurrentTruthTable
        {
            get { return _currentTruthTable; }
            set
            {
                _currentTruthTable = value;
                //set four input variables for solver
                this.values = _currentTruthTable.Fvalues;
                this.one_sol_or_all_possible = _currentTruthTable.One_sol_or_all_possible;
                this.sum_of_products_or_product_of_sums = _currentTruthTable.Sum_of_products_or_product_of_sums;

                if (_currentTruthTable.TermNameDict != null)
                {
                    //Copy value array without output header inside of term names
                    this.terms_names = _currentTruthTable.TermNameDict.Values.ToArray();
                }
                else {
                    this.terms_names = _currentTruthTable.Terms_names;
                }

            }
        }

        public void Solve()
        {
            TList min;

            Format output = new Format();

            bool no_ones = checkNoOnes();

            bool no_zeros = checkNoZeros();

            if (no_ones && no_zeros)
            {
                Solution = "1 or 0";
            }
            else if (no_zeros)
            {
                Solution = "1";
            }
            else if (no_ones)
            {
                Solution = "0";
            }
            else
            {
                if (LogicPadParser.IsMinimizationActive)
                {
                    fillTable();

                    min = minimize(table);

                    min = smoosh(min);

                    //Convert TruthTable Data Structure to InterTree
                    InterTree temp = null;

                    solution = output.formatOutput(
                            values,
                            min.getValues(),
                            min.getSubs(),
                            terms_names,
                            sum_of_products_or_product_of_sums,
                            one_sol_or_all_possible, ref temp
                            );

                    InterTree root = new InterTree(InterTree.LogicOperator.EQUAL);
                    root.LeftNode = new InterTree(this.CurrentTruthTable.outputName);
                    root.RightNode = temp;
                    LogicPadParser.truthTable = root;                    
                }
                else
                {
                    //TODO without minimization
                }
            }
        }

        /*
         * fill the table with it's values.
         */
        private void fillTable()
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i] != 0)
                    table.addNode(i);
        }

        /*
         * check if the truth table has no ones.
         */

        private bool checkNoOnes()
        {
            bool check = true;

            for (int i = 0; i < values.Length; i++)
                if (values[i] == 1)
                    check = false;

            return check;
        }


        /*
         * check if the truth table has no zeros.
         */

        private bool checkNoZeros()
        {
            bool check = true;

            for (int i = 0; i < values.Length; i++)
                if (values[i] == 0)
                    check = false;

            return check;
        }

        /************************************************************
         *  Quine–McCluskey algorithm
         *  Minimize is the function that minimize the truth table.
         *************************************************************
         */
        private TList minimize(TList mtable)
        {
            Node temp = new Node();

            Node ntemp = new Node();

            TList res = new TList();

            bool check_end = true;

            int subt = 0;

            temp = mtable.Head;

            while (temp != null)
            {
                ntemp = temp.Next;

                while (ntemp != null)
                {
                    subt = ntemp.Value - temp.Value;

                    if (checkVaryByOneDigit(temp, ntemp))
                    {
                        if ((res.Head == null) || (res.Tail.Value != temp.Value)
                                || (res.Tail.Sub != temp.Sub + subt))
                            res.addNode(temp.Value, temp.Sub + subt, false);

                        check_end = false;

                        temp.Mark = true;
                        ntemp.Mark = true;
                    }
                    ntemp = ntemp.Next;
                }
                if (!temp.Mark)
                    res.addNode(temp.Value, temp.Sub, false);
                temp = temp.Next;
            }
            if (check_end)
                return res;

            return minimize(res);
        }

        /*
         * checkVaryByOneDigite checks if the two values vary by one digit.
         */
        private bool checkVaryByOneDigit(Node temp, Node ntemp)
        {
            int v, nv;

            int s, ns;

            int subt;

            v = temp.Value;

            nv = ntemp.Value;

            s = temp.Sub;

            ns = ntemp.Sub;

            subt = Math.Abs(v - nv);

            if (subt == 0 || s != ns)
            {
                return false;
            }
            else if (subt == 1)
            {
                if (v % 2 == 0)
                    return true;
                else
                    return false;
            }
            else if (checkPowerOfTwo(subt))
            {
                if (((v / subt) % 2) == 0)
                    return true;
                else
                    return false;
            }
            return
                false;

        }

        /*
         * checkPowerOfTwo return true if the number is a power of two (0,1,2,4,8,.....).
         */

        private bool checkPowerOfTwo(int number)
        {
            double temp = Math.Log(number) / Math.Log(2);
            return ((Math.Ceiling(temp) == Math.Floor(temp)) && (number != 0));
        }

        /*
         * remove any identical nodes in the list by one node.
         */
        private TList smoosh(TList min)
        {
            TList fin;

            Node temp, ntemp;

            fin = new TList();

            temp = min.Head;

            bool check = true;

            while (temp != null)
            {
                ntemp = temp.Next;

                while (ntemp != null)
                {
                    if (temp == ntemp)
                        check = false;

                    ntemp = ntemp.Next;
                }
                if (check)
                    fin.addNode(temp.Value, temp.Sub, temp.Mark);

                check = true;

                temp = temp.Next;
            }
            return fin;
        }

        //Table that represent the position of the ones and don't cares in the truth table
        private TList table;

        private int[] values;

        private String[] terms_names;

        private int sum_of_products_or_product_of_sums;

        private int one_sol_or_all_possible;

        private String solution;

        public String Solution
        {
            get { return solution; }
            set { solution = value; }
        }
    }
}
