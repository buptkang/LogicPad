using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SequenceGenerator
{
    public class TestClass
    {
        public static void Main(string[] args)
        { 
           Console.WriteLine(SequenceGenerator.Instance.GetUserSequence(12));
          
           Console.Read();
        }
    }
}
