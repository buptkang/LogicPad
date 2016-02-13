using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;

using System.Xml.Linq;
using LogicPadParser;

namespace LogicPad2
{
    public class LogicParser1
    {
        private static LogicParser1 _instance;
        public static LogicParser1 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogicParser1();
                }
                return _instance;
            }
        }

        public void ParseDiagramUserControl(LogicPad2.Diagram.UserControl1 userControl, 
            out string generatedExpr, out TruthTableWindow.TruthTable generatedTruthTable)
        { 
            userControl.Parse(out generatedExpr);

            generatedTruthTable = new TruthTableWindow.TruthTable();

            int numberOfTerm;
            String[] terms_names;
            int[] fvalues;
            String outputName;
            LogicPadParser.LogicPadParser.Instance.ParseInterTreeToTruthTable(
                LogicPadParser.LogicPadParser.diagram,
                out numberOfTerm,
                out terms_names,
                out fvalues,
                out outputName
             );

            LogicPadParser.TruthTable interTruthTable = new LogicPadParser.TruthTable(numberOfTerm);
            interTruthTable.Terms_names = terms_names;
            interTruthTable.Fvalues = fvalues;
            interTruthTable.OutputName = outputName;

            //Diagram To TruthTable Minimization
            LogicPadParser.TruthTableSolver solver = new LogicPadParser.TruthTableSolver();
            solver.CurrentTruthTable = interTruthTable;
            solver.Solve();

            LogicPadParser.LogicPadParser.Instance.ParseInterTreeToTruthTable(
                LogicPadParser.LogicPadParser.truthTable,
                out numberOfTerm,
                out terms_names,
                out fvalues,
                out outputName
             );

            generatedTruthTable.Number_of_terms = numberOfTerm;
            generatedTruthTable.Terms_names = terms_names;
            generatedTruthTable.Fvalues = fvalues;
            generatedTruthTable.Rows = (int)Math.Pow(2, generatedTruthTable.Number_of_terms) + 1;
            generatedTruthTable.Columns = generatedTruthTable.Number_of_terms + 2;
            generatedTruthTable.OutputName = outputName;


            generatedExpr = InterTree.ParseExpressionToString(LogicPadParser.LogicPadParser.truthTable);
        }

        public int ParseTruthTableUserControl(LogicPad2.TruthTable.UserControl1 userControl, 
            out string generatedExpr, out XElement generatedXElement)
        {
            if (userControl.TruthTableInkCanvas.TruthTable == null)
            {
                generatedExpr = null;
                generatedXElement = null;
                return 2;
            }
            else {
                generatedXElement = userControl.Parse(out generatedExpr);
                //changed generatedXElement to generatedExpr 
                if (generatedExpr.Equals("1"))
                {
                    return 1;
                }
                //changed generatedXElement to generatedExpr
                else if (generatedExpr.Equals("0"))
                {
                    return 0;
                }
                else 
                {
                    return 2;
                }
            }
        }

        public int ParseExpressionUserControl(LogicPad2.Expression.UserControl1 userControl,
            out TruthTableWindow.TruthTable generatedTruthTable, out XElement generatedXElement)
        {
            int output = 2;
            generatedXElement = userControl.Parse(ref output);
            if(output != 2)
            {
                generatedTruthTable = null;
                return output;
            }
            LogicPadParser.LogicPadParser.Instance.ParseExpressionXElement(generatedXElement);
           
            generatedTruthTable = new TruthTableWindow.TruthTable();

            int numberOfTerm;
            String[] terms_names;
            int[] fvalues;
            string outputName;

            LogicPadParser.LogicPadParser.Instance.ParseInterTreeToTruthTable(
                LogicPadParser.LogicPadParser.expression,
                out numberOfTerm,
                out terms_names,
                out fvalues,
                out outputName
             );

            if (numberOfTerm == 0)
            {
                generatedTruthTable = null;
                return 0;
            }
            else if (numberOfTerm == -1)
            {
                generatedTruthTable = null;
                return 1;
            }

            LogicPadParser.TruthTable interTruthTable = new LogicPadParser.TruthTable(numberOfTerm);
            interTruthTable.Terms_names = terms_names;
            interTruthTable.Fvalues = fvalues;
            interTruthTable.OutputName = outputName;
            
            //Expression Minimization
            LogicPadParser.TruthTableSolver solver = new LogicPadParser.TruthTableSolver();
            solver.CurrentTruthTable = interTruthTable;
            solver.Solve();

            LogicPadParser.LogicPadParser.Instance.ParseInterTreeToTruthTable(
                LogicPadParser.LogicPadParser.truthTable,
                out numberOfTerm,
                out terms_names,
                out fvalues,
                out outputName
             );

            if (numberOfTerm == 0)
            {
                generatedTruthTable = null;
                return 0;
            }
            else if (numberOfTerm == -1)
            {
                generatedTruthTable = null;
                return 1;
            }

            generatedTruthTable.Number_of_terms = numberOfTerm;
            generatedTruthTable.Terms_names = terms_names;
            generatedTruthTable.Fvalues = fvalues;
            generatedTruthTable.Rows = (int)Math.Pow(2, generatedTruthTable.Number_of_terms) + 1;
            generatedTruthTable.Columns = generatedTruthTable.Number_of_terms + 2;

            return 2;
        }

        public bool MatchTwoUserControl(UserControl leftUserControl, UserControl rightUserControl)
        {
            if (leftUserControl is LogicPad2.Expression.UserControl1 && rightUserControl is LogicPad2.Expression.UserControl1)
            {
                // Two Expressions
                LogicPad2.Expression.UserControl1 leftExprUserControl = 
                    leftUserControl as LogicPad2.Expression.UserControl1;

                LogicPad2.Expression.UserControl1 rightExprUserControl = 
                    rightUserControl as LogicPad2.Expression.UserControl1;

                XElement generatedXElement = null;
                TruthTableWindow.TruthTable generatedTruthTable = null;

                int leftResult = LogicParser1.Instance.ParseExpressionUserControl(leftExprUserControl, out generatedTruthTable, out generatedXElement);          
                int[] leftValueArray = null;
                if(leftResult == 2 )
                {
                    leftValueArray = generatedTruthTable.Fvalues;
                }
                
                int rightResult = LogicParser1.Instance.ParseExpressionUserControl(rightExprUserControl, out generatedTruthTable, out generatedXElement);
                int[] rightValueArray = null;
                if (rightResult == 2)
                {
                    rightValueArray = generatedTruthTable.Fvalues;
                }

                if (leftResult == 2 && rightResult == 2)
                {
                    return LogicPadParser.LogicPadParser.MatchTwoTruthTableValueArray(leftValueArray, rightValueArray);
                }
                else if (leftResult == rightResult && (leftResult == 1 || leftResult == 0))
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            else if (leftUserControl is LogicPad2.Expression.UserControl1 && rightUserControl is LogicPad2.Diagram.UserControl1)
            {
                LogicPad2.Expression.UserControl1 leftExprUserControl =
                        leftUserControl as LogicPad2.Expression.UserControl1;

                LogicPad2.Diagram.UserControl1 rightDiagramUserControl =
                        rightUserControl as LogicPad2.Diagram.UserControl1;

                XElement generatedXElement = null;
                TruthTableWindow.TruthTable generatedTruthTable = null;
                String generatedExpr = null;
                int result = LogicParser1.Instance.ParseExpressionUserControl(leftExprUserControl, out generatedTruthTable, out generatedXElement);
                if (result == 0 || result == 1)
                {
                    return false;
                }
                int[] leftValueArray = generatedTruthTable.Fvalues;

                LogicParser1.Instance.ParseDiagramUserControl(rightDiagramUserControl, out generatedExpr, out generatedTruthTable);
                int[] rightValueArray = generatedTruthTable.Fvalues;

                return LogicPadParser.LogicPadParser.MatchTwoTruthTableValueArray(leftValueArray, rightValueArray);
            }
            else if (leftUserControl is LogicPad2.Expression.UserControl1 && rightUserControl is LogicPad2.TruthTable.UserControl1)
            {
                LogicPad2.Expression.UserControl1 leftExprUserControl =
                       leftUserControl as LogicPad2.Expression.UserControl1;
                LogicPad2.TruthTable.UserControl1 rightTruthTableUserControl =
                        rightUserControl as LogicPad2.TruthTable.UserControl1;

                XElement generatedXElement = null;
                TruthTableWindow.TruthTable generatedTruthTable = null;
                int result = LogicParser1.Instance.ParseExpressionUserControl(leftExprUserControl, out generatedTruthTable, out generatedXElement);
                if (result == 0 || result == 1)
                {
                    return false;
                }
                int[] leftValueArray = generatedTruthTable.Fvalues;
 
                int[] rightValueArray = rightTruthTableUserControl.TruthTableInkCanvas.TruthTable.Fvalues;

                return LogicPadParser.LogicPadParser.MatchTwoTruthTableValueArray(leftValueArray, rightValueArray);
            }
            else if (leftUserControl is LogicPad2.Diagram.UserControl1 && rightUserControl is LogicPad2.Expression.UserControl1)
            {
                return MatchTwoUserControl(rightUserControl, leftUserControl);

            }else if(leftUserControl is LogicPad2.Diagram.UserControl1 && rightUserControl is LogicPad2.Diagram.UserControl1)
            {
                // Two Diagrams
                LogicPad2.Diagram.UserControl1 leftDiagramUserControl =
                    leftUserControl as LogicPad2.Diagram.UserControl1;

                LogicPad2.Diagram.UserControl1 rightDiagramUserControl =
                    rightUserControl as LogicPad2.Diagram.UserControl1;

                string generatedExpr = null;
                TruthTableWindow.TruthTable generatedTruthTable = null;

                LogicParser1.Instance.ParseDiagramUserControl(leftDiagramUserControl, out generatedExpr, out generatedTruthTable);
                int[] leftValueArray = generatedTruthTable.Fvalues;
                LogicParser1.Instance.ParseDiagramUserControl(rightDiagramUserControl, out generatedExpr, out generatedTruthTable);
                int[] rightValueArray = generatedTruthTable.Fvalues;

                return LogicPadParser.LogicPadParser.MatchTwoTruthTableValueArray(leftValueArray, rightValueArray);
            }else if(leftUserControl is LogicPad2.Diagram.UserControl1 && rightUserControl is LogicPad2.TruthTable.UserControl1)
            {
                LogicPad2.Diagram.UserControl1 leftDiagramUserControl =
                      leftUserControl as LogicPad2.Diagram.UserControl1;
                LogicPad2.TruthTable.UserControl1 rightTruthTableUserControl =
                        rightUserControl as LogicPad2.TruthTable.UserControl1;

                TruthTableWindow.TruthTable generatedTruthTable = null;
                string generatedExpr = null;
                LogicParser1.Instance.ParseDiagramUserControl(leftDiagramUserControl, out generatedExpr, out generatedTruthTable);
                int[] leftValueArray = generatedTruthTable.Fvalues;

                int[] rightValueArray = rightTruthTableUserControl.TruthTableInkCanvas.TruthTable.Fvalues;

                return LogicPadParser.LogicPadParser.MatchTwoTruthTableValueArray(leftValueArray, rightValueArray);
            }
            else if(leftUserControl is LogicPad2.TruthTable.UserControl1 && rightUserControl is LogicPad2.Expression.UserControl1)
            {
                return MatchTwoUserControl(rightUserControl, leftUserControl);
            }
            else if(leftUserControl is LogicPad2.TruthTable.UserControl1 && rightUserControl is LogicPad2.Diagram.UserControl1)
            {
                return MatchTwoUserControl(rightUserControl, leftUserControl);
            }
            else if (leftUserControl is LogicPad2.TruthTable.UserControl1 && rightUserControl is LogicPad2.TruthTable.UserControl1)
            {
                //Two TruthTable
                LogicPad2.TruthTable.UserControl1 leftTruthTableUserControl =
                 leftUserControl as LogicPad2.TruthTable.UserControl1;

                LogicPad2.TruthTable.UserControl1 rightTruthTableUserControl =
                    rightUserControl as LogicPad2.TruthTable.UserControl1;

                int[] leftValueArray = leftTruthTableUserControl.TruthTableInkCanvas.TruthTable.Fvalues;
                int[] rightValueArray = rightTruthTableUserControl.TruthTableInkCanvas.TruthTable.Fvalues;

                return LogicPadParser.LogicPadParser.MatchTwoTruthTableValueArray(leftValueArray, rightValueArray);
            }
            return false; 
        }
    }
}
