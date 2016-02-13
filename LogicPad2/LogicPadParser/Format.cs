using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPadParser
{
    public class Format
    {
        /*
        * formatOutput format the answer to a readable string
        */
        public String formatOutput(
                       int[] values,
                       List<int> mini_vals,
                       List<int> mini_subs,
                       String[] terms_names,
                       int sum_of_products_or_product_of_sums,
                       int one_sol_or_all_possible, ref InterTree truthTableInterTree)
        {

            PrimeImplicantsChartTable prime_implicant_chart =
                new PrimeImplicantsChartTable(values, mini_vals, mini_subs, one_sol_or_all_possible);

            List<int> essential_primes_indexs =
                prime_implicant_chart.Essential_primes_indexs;

            List<List<int>> minimume_compination_of_non_essential_table =
                prime_implicant_chart.Minimume_compination_of_non_essential_table;

            List<String> essential_primes_st_v = new List<String>();

            String essential_primes_st = System.String.Empty;

            List<String> non_essential_st_v = new List<String>();

            List<InterTree> essential_interTree = new List<InterTree>();
            InterTree tempInterTree = null;

            for (int i = 0; i < essential_primes_indexs.Count; i++)
            {
                essential_primes_st_v.Add(
                          formatPrime(
                              mini_vals[essential_primes_indexs[i]],
                              mini_subs[essential_primes_indexs[i]],
                              terms_names,
                              sum_of_products_or_product_of_sums, ref tempInterTree));
                essential_interTree.Add(tempInterTree);
            }

            if (essential_primes_st_v.Count > 0)
            {
                essential_primes_st =
                        addFormatedPrimes(essential_primes_st_v, sum_of_products_or_product_of_sums);
                tempInterTree = addInterTreePrimes(essential_interTree, sum_of_products_or_product_of_sums);
                truthTableInterTree = tempInterTree;
            }

            for (int i = 0; i < minimume_compination_of_non_essential_table.Count; i++)
            {
                List<int> temp = minimume_compination_of_non_essential_table[i];

                List<String> temp_st = new List<String>();

                for (int j = 0; j < temp.Count; j++)
                    temp_st.Add(
                            formatPrime(
                                mini_vals[temp[j]],
                                mini_subs[temp[j]],
                                terms_names,
                                sum_of_products_or_product_of_sums));
                non_essential_st_v.Add(addFormatedPrimes(temp_st, sum_of_products_or_product_of_sums));
            }

            String final_answer = null;

            if (!non_essential_st_v.Any())
            {
                final_answer = essential_primes_st;
            }
            else
            {
                for (int i = 0; i < non_essential_st_v.Count; i++)
                    final_answer += essential_primes_st +
                        ((essential_primes_st.Length > 0) ?
                        //replaced "." with empty String
                        ((sum_of_products_or_product_of_sums == 0) ? " + " : " . ") : "");
            }
            return final_answer;
        }

        private String formatPrime(int value, int sub, String[] terms_names, int format, ref InterTree rootTree)
        {
            String answer = null;

            bool check = false;

            InterTree temp = null, gate = null;

            for (int i = (terms_names.Length - 1); i >= 0; i--)
            {
                if ((sub % 2) == 0)
                {
                    if (((value % 2) == 0 && format == 0) || ((value % 2) == 1 && format == 1))
                    {
                        if (check)
                        {
                            if (format == 0)
                            {
                                gate = new InterTree(InterTree.LogicOperator.AND);
                            }
                            else
                            {
                                gate = new InterTree(InterTree.LogicOperator.OR);
                            }
                        }

                        InterTree notGate = new InterTree(InterTree.LogicOperator.NOT);
                        notGate.LeftNode = new InterTree(terms_names[i].ToString());
                        if (check)
                        {
                            gate.LeftNode = notGate;
                            gate.RightNode = temp;
                            temp = gate;
                        }
                        else {
                            temp = notGate;
                        }
                        //replaced "." with empty String
                        answer = terms_names[i] + "'" + ((check) ? ((format == 0) ? "" : "+") : "") + answer;
                    }
                    else
                    {
                        if (!check)
                        {
                            //Only Letters
                            temp = new InterTree(terms_names[i].ToString());
                        }
                        else {
                            if (format == 0)
                            {
                                gate = new InterTree(InterTree.LogicOperator.AND);
                            }
                            else {
                                gate = new InterTree(InterTree.LogicOperator.OR);
                            }
                            gate.LeftNode = new InterTree(terms_names[i].ToString());
                            gate.RightNode = temp;
                            temp = gate;
                        }
                        answer = terms_names[i] + " " + ((check) ? ((format == 0) ? "" : "+") : "") + answer;                
                    }
                    
                    answer = " " + answer;

                    check = true;
                }
                sub /= 2;

                value /= 2;
            }
            answer = "(" + answer + ")";

            rootTree = temp;
            return answer;
        }

        /*
         * formatPrime format the prime to a string
         */

        private String formatPrime(int value, int sub, String[] terms_names, int format)
        {
            String answer = null;

            bool check = false;

            for (int i = (terms_names.Length - 1); i >= 0; i--)
            {
                if ((sub % 2) == 0)
                {
                    if (((value % 2) == 0 && format == 0) || ((value % 2) == 1 && format == 1))
                        answer = terms_names[i] + "'" + ((check) ? ((format == 0) ? "" : "+") : "") + answer;
                    else
                        answer = terms_names[i] + " " + ((check) ? ((format == 0) ? "" : "+") : "") + answer;

                    answer = " " + answer;

                    check = true;
                }
                sub /= 2;

                value /= 2;
            }
            answer = "(" + answer + ")";

            return answer;
        }

        /*
         * add the formated prime to a full answer
         */

        private String addFormatedPrimes(List<String> essential_primes_st_v, int format)
        {
            String answer = null;

            answer = essential_primes_st_v[0];

            for (int i = 1; i < essential_primes_st_v.Count; i++)
                answer += ((format == 0) ? " + " : "") + essential_primes_st_v[i];

            return answer;
        }

        private InterTree addInterTreePrimes(List<InterTree> essential_interTree_list, int format)
        {
            InterTree result = null, gate;

            result = essential_interTree_list[0];

            for (int i = 1; i < essential_interTree_list.Count; i++)
            {
                if (format == 0)
                {
                    //OR Gate
                    gate = new InterTree(InterTree.LogicOperator.OR);
                }
                else { 
                    //AND Gate
                    gate = new InterTree(InterTree.LogicOperator.AND);
                }

                gate.LeftNode = essential_interTree_list[i];
                gate.RightNode = result;
                result = gate;
            }
            return result;
        }
    }
}
