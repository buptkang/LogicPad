using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPadParser
{
    public class GenerateNumbersOrderedByNumberOfOnes
    {
        public GenerateNumbersOrderedByNumberOfOnes(int n_o_t)
        {
            number_of_terms = n_o_t;

            number_of_ones = 0;

            previous = generateNumbers_1();
        }


        /*
         * generateNumbers_1 generate numbers that has only one (1) in the binary form like(1,2,4,8).
         */
        private List<long> generateNumbers_1()
        {
            List<long> sol = new List<long>();

            for (int i = 0; i < number_of_terms; i++)
                sol.Add((long)Math.Pow(2, i));
            return sol;
        }

        /*
         * generateNumbers_V generate numbers that has number of ones in the binary from equal to the variable "number_of_ones".
         */

        private List<long> generateNumbers_V()
        {
            List<long> sol = new List<long>();

            sol.Add(((long)Math.Pow(2, number_of_ones)) - 1);

            for (int i = (number_of_ones - 1); i >= 0; i--)
                sol.Add(sol[(sol.Count - 1)] + (long)Math.Pow(2, i));


            for (int i = (number_of_ones + 1); i < number_of_terms; i++)
            {
                for (int j = 0; j < previous.Count; j++)
                {
                    if (previous[j] >= (long)Math.Pow(2, i))
                        break;

                    else
                        sol.Add(previous[j] + (long)Math.Pow(2, i));
                }
            }
            previous = sol;

            return sol;
        }

        /*
         *getNumbers cals the generating functions and return the generated numbers depending on the variable "number_of_ones".
         */

        public List<long> getNumbers()
        {
            number_of_ones++;

            if (number_of_ones == 1)
                return previous;

            else if (number_of_ones == number_of_terms)
            {
                List<long> temp = new List<long>();

                temp.Add(((long)Math.Pow(2, number_of_terms)) - 1);

                return temp;
            }
            else
                return generateNumbers_V();
        }

        private List<long> previous;

        private int number_of_terms;

        private int number_of_ones;
    }
}
