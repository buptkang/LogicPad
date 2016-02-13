using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPad2.Diagram
{
    public class UserIOLabelList
    {
        public static List<string> inputLabelList = new List<string>();

        public static List<string> outputLabelList = new List<string>();

        private static readonly char[] inputLettersArray = "ABCDEFG".ToCharArray();

        private static readonly char[] outputLettersArray = "OPQXYZ".ToCharArray();

        static UserIOLabelList(){
            foreach (var letter in inputLettersArray)
            {
                inputLabelList.Add(letter.ToString());
            }
            foreach (var letter in outputLettersArray)
            {
                outputLabelList.Add(letter.ToString());
            }
        }
    }
}
