using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using starPadSDK.MathExpr;
using starPadSDK.UnicodeNs;

namespace LogicPadParser
{
    public class LogicPadParser
    {
        public static bool IsMinimizationActive = true;

        public static InterTree expression;
        public static InterTree diagram;
        public static InterTree truthTable;

        #region Constructor Singleton
        private static LogicPadParser _instance;
        private LogicPadParser()
        {
        
        }
        public static LogicPadParser Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogicPadParser();
                }
                return _instance;
            }
        }
        #endregion

        //Expression
        public XElement ParseStarPadExpr(starPadSDK.MathExpr.Expr expr)
        {
            Reset();

            XElement root = new XElement("CircuitGroup");
            root.SetAttributeValue("Version", "1.2");
            XElement circuit = new XElement("Circuit");

            XElement gates = new XElement("Gates");
            XElement wires = new XElement("Wires");

            if (expr is starPadSDK.MathExpr.CompositeExpr)
            {
                #region composite handling
                starPadSDK.MathExpr.CompositeExpr compositeExpr = expr as starPadSDK.MathExpr.CompositeExpr;
                if (compositeExpr.Head != starPadSDK.MathExpr.WellKnownSym.equals)
                {
                    #region

                    //Create UserOutput and relative comment    

                    XElement gt = new XElement("Gate");
                    gt.SetAttributeValue("Type", "Comment");
                    gt.SetAttributeValue("Name", "Y");
                    gt.SetAttributeValue("ID", cid);
                    gt.Add(new XElement("Point"));
                    gt.Element("Point").SetAttributeValue("X", dPoint.X);
                    gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                    gt.Element("Point").SetAttributeValue("Angle", 0.0f);
                    gt.Add(new XElement("Comment", "Y"));

                    gates.Add(gt);
                    cid++;
                    //iniY += 80;

                    gt = new XElement("Gate");
                    gt.SetAttributeValue("Type", "UserOutput");
                    gt.SetAttributeValue("Name", "Y");
                    gt.SetAttributeValue("ID", cid);
                    gt.Add(new XElement("Point"));
                    gt.Element("Point").SetAttributeValue("X", dPoint.X - deltaX);
                    gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                    gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                    gates.Add(gt);
                    cid++;
                    //iniX -= 80;

                    XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                    wire.Element("From").SetAttributeValue("ID", 3);
                    wire.Element("From").SetAttributeValue("Port", 0);
                    wire.Element("To").SetAttributeValue("ID", 2);
                    wire.Element("To").SetAttributeValue("Port", 0);

                    wires.Add(wire);

                    ConvertStarPadExprToXElement(ref gates, ref wires, compositeExpr, new DPoint(dPoint.X- deltaX, dPoint.Y));

                    #endregion
                }
                else {
                    if (compositeExpr.Args[0] is starPadSDK.MathExpr.LetterSym)
                    {
                        #region
                        string outputLabel = (compositeExpr.Args[0] as starPadSDK.MathExpr.LetterSym).Letter.ToString();

                        //Create UserOutput and relative comment    

                        XElement gt = new XElement("Gate");
                        gt.SetAttributeValue("Type", "Comment");
                        gt.SetAttributeValue("Name", outputLabel);
                        gt.SetAttributeValue("ID", cid);
                        gt.Add(new XElement("Point"));
                        gt.Element("Point").SetAttributeValue("X", dPoint.X);
                        gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                        gt.Element("Point").SetAttributeValue("Angle", 0.0f);
                        gt.Add(new XElement("Comment", outputLabel));

                        gates.Add(gt);
                        cid++;
                        //iniX -= 80;

                        gt = new XElement("Gate");
                        gt.SetAttributeValue("Type", "UserOutput");
                        gt.SetAttributeValue("Name", outputLabel);
                        gt.SetAttributeValue("ID", cid);
                        gt.Add(new XElement("Point"));
                        gt.Element("Point").SetAttributeValue("X", dPoint.X - deltaX);
                        gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                        gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                        gates.Add(gt);
                        cid++;
                        //iniX -= 80;

                        XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                        wire.Element("From").SetAttributeValue("ID", 3);
                        wire.Element("From").SetAttributeValue("Port", 0);
                        wire.Element("To").SetAttributeValue("ID", 2);
                        wire.Element("To").SetAttributeValue("Port", 0);

                        wires.Add(wire);

                        ConvertStarPadExprToXElement(ref gates, ref wires, compositeExpr.Args[1], new DPoint(dPoint.X - deltaX, dPoint.Y));
                        #endregion
                    }
                    else
                    {
                        #region
                        string outputLabel = (compositeExpr.Args[1] as starPadSDK.MathExpr.LetterSym).Letter.ToString();

                        //Create UserOutput and relative comment    

                        XElement gt = new XElement("Gate");
                        gt.SetAttributeValue("Type", "Comment");
                        gt.SetAttributeValue("Name", outputLabel);
                        gt.SetAttributeValue("ID", cid);
                        gt.Add(new XElement("Point"));
                        gt.Element("Point").SetAttributeValue("X", dPoint.X);
                        gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                        gt.Element("Point").SetAttributeValue("Angle", 0.0f);
                        gt.Add(new XElement("Comment", outputLabel));

                        gates.Add(gt);
                        cid++;
                        //iniX -= 80;

                        gt = new XElement("Gate");
                        gt.SetAttributeValue("Type", "UserOutput");
                        gt.SetAttributeValue("Name", outputLabel);
                        gt.SetAttributeValue("ID", cid);
                        gt.Add(new XElement("Point"));
                        gt.Element("Point").SetAttributeValue("X", dPoint.X - deltaX);
                        gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                        gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                        gates.Add(gt);
                        cid++;
                        //iniX -= 80;

                        XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                        wire.Element("From").SetAttributeValue("ID", 3);
                        wire.Element("From").SetAttributeValue("Port", 0);
                        wire.Element("To").SetAttributeValue("ID", 2);
                        wire.Element("To").SetAttributeValue("Port", 0);

                        wires.Add(wire);

                        ConvertStarPadExprToXElement(ref gates, ref wires, compositeExpr.Args[0], new DPoint(dPoint.X - deltaX, dPoint.Y));
                        #endregion
                    }
                }
                #endregion
            }
            else if (expr is starPadSDK.MathExpr.LetterSym)
            {
                #region simple Symbol input
                //Create one UserOutput with Comment
                XElement gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "Comment");
                gt.SetAttributeValue("Name", "Y");
                gt.SetAttributeValue("ID", cid);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", dPoint.X );
                gt.Element("Point").SetAttributeValue("Y", dPoint.Y );
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);
                gt.Add(new XElement("Comment", "Y"));
                

                gates.Add(gt);
                cid++;
                //dPoint.Y += 80;

                 gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "UserOutput");
                gt.SetAttributeValue("Name", "Y");
                gt.SetAttributeValue("ID", cid);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", dPoint.X - deltaX);
                gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                //dPoint.X -= 80;

                //Create one UserInput with Comment
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type","UserInput");
                gt.SetAttributeValue("Name", ((starPadSDK.MathExpr.LetterSym)expr).Letter.ToString());
                gt.SetAttributeValue("ID", cid);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", dPoint.X - 2 * deltaX);
                gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                //dPoint.X -= 80;

                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "Comment");
                gt.SetAttributeValue("Name", ((starPadSDK.MathExpr.LetterSym)expr).Letter.ToString());
                gt.SetAttributeValue("ID", cid);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", dPoint.X - 3 * deltaX);
                gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);
                gt.Add(new XElement("Comment", ((starPadSDK.MathExpr.LetterSym)expr).Letter.ToString()));

                gates.Add(gt);
                cid++;
                //dPoint.Y += 80;

                //Add one wire
                XElement wire = new XElement("Wire",new XElement("From"), new XElement("To"));
                wire.Element("From").SetAttributeValue("ID", 3);
                wire.Element("From").SetAttributeValue("Port", 0);
                wire.Element("To").SetAttributeValue("ID", 2);
                wire.Element("To").SetAttributeValue("Port", 0);
                
                wires.Add(wire);

                #endregion
            }
            else
            {
                return null;
            }

            circuit.Add(gates);
            circuit.Add(wires);

            root.Add(circuit);
            return root;
 
        }

        //TruthTable
        public XElement ParseTruthTable(InterTree rootTree)
        {
            Reset();

            XElement root = new XElement("CircuitGroup");
            root.SetAttributeValue("Version", "1.2");
            XElement circuit = new XElement("Circuit");

            XElement gates = new XElement("Gates");
            XElement wires = new XElement("Wires");


            XElement gt = new XElement("Gate");
            gt.SetAttributeValue("Type", "Comment");
            gt.SetAttributeValue("Name", rootTree.LeftNode.Head);
            gt.SetAttributeValue("ID", cid);
            gt.Add(new XElement("Point"));
            gt.Element("Point").SetAttributeValue("X", dPoint.X);
            gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
            gt.Element("Point").SetAttributeValue("Angle", 0.0f);
            gt.Add(new XElement("Comment", rootTree.LeftNode.Head));

            gates.Add(gt);
            cid++;
            //iniX -= 80;

            gt = new XElement("Gate");
            gt.SetAttributeValue("Type", "UserOutput");
            gt.SetAttributeValue("Name", rootTree.LeftNode.Head);
            gt.SetAttributeValue("ID", cid);
            gt.Add(new XElement("Point"));
            gt.Element("Point").SetAttributeValue("X", dPoint.X - deltaX);
            gt.Element("Point").SetAttributeValue("Y", dPoint.Y);
            gt.Element("Point").SetAttributeValue("Angle", 0.0f);

            gates.Add(gt);
            cid++;
            //iniX -= 80;

            XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
            wire.Element("From").SetAttributeValue("ID", 3);
            wire.Element("From").SetAttributeValue("Port", 0);
            wire.Element("To").SetAttributeValue("ID", 2);
            wire.Element("To").SetAttributeValue("Port", 0);

            wires.Add(wire);

            ConvertInterTreeToXElement(ref gates, ref wires, rootTree.RightNode, new DPoint(dPoint.X - deltaX, dPoint.Y));


            circuit.Add(gates);
            circuit.Add(wires);

            root.Add(circuit);
            return root;
        }

        //TruthTable
        public void ParseInterTreeToTruthTable(InterTree tree, 
            out int number_of_term, out string[] term_names, 
            out int[] fvalue, out string outputName)
        {
            List<String> termList = new List<String>();

            if (tree == null)
            {
                number_of_term = -1;
                term_names = null;
                fvalue = null;
                outputName = null;
                return;
            } 

            outputName = tree.LeftNode.Head;
            InterTree.ParseInterTreeTokens(tree.RightNode, ref termList);
            term_names = termList.ToArray();
            Array.Sort(term_names);
            number_of_term = termList.Count;

            //Add Parent pointer to each node
            InterTree.AddParentToInterTree(ref tree._rightNode);
            //Change Not Gate
            InterTree.FilterNotGate(ref tree._rightNode);
            //Change XOR and change Product to Summation
            InterTree.ReformulateInterTree(ref tree._rightNode);
            //Check Complement Law
            //InterTree.CheckComplement(ref tree._rightNode);
            //Check Domination Law
            //InterTree.CheckDominationLaw(ref tree._rightNode);
            /*
            if (tree._rightNode == null)
            {
                number_of_term = 0;
                term_names = null;
                fvalue = null;
                outputName = null;
                return;
            }
            */
            TruthTable truthTable = new TruthTable(number_of_term);
            truthTable.Terms_names = term_names;
            truthTable.OutputName = outputName;

            Dictionary<int, string[]> truthTableRows = truthTable.ConstructTruthTableRowList();
            //Initialization
            int row = (int)Math.Pow(2, number_of_term);
            Dictionary<int, int> truthTableResults = new Dictionary<int, int>(row);
            for (int index = 0; index < row; index++)
            {
                truthTableResults.Add(index, 0);
            }
            //Segement InterTree to InterTreeList
            List<InterTree> interTreeList = new List<InterTree>();

            InterTree.ParseInterTreeToInterTreeList(ref tree._rightNode, ref interTreeList);

            List<List<string>> stringArray = new List<List<string>>();
            InterTree.ParseInterTreeListToStringArray(interTreeList, ref stringArray, term_names);

            if (stringArray.Count == 0)
            {
                number_of_term = 0;
                term_names = null;
                fvalue = null;
                outputName = null;
                return;
            }

            InterTree.MatchInterTreeToTruthTable(stringArray, truthTableRows, ref truthTableResults, row);

            fvalue = truthTableResults.Values.ToArray();
        }

        //Diagram
        public string ParseDiagramXElement(XElement root)
        {
            LogicPadParser.diagram = ParseXElement(root);

            if (LogicPadParser.diagram != null)
            {
                return InterTree.ParseDiagramToString(LogicPadParser.diagram);
            }
            else
            {
                return null;
            }
        }

        //Expression
        public string ParseExpressionXElement(XElement root)
        {
            LogicPadParser.expression = ParseXElement(root);

            if (LogicPadParser.expression != null)
            {
                return InterTree.ParseExpressionToString(LogicPadParser.expression);
            }
            else
            {
                return null;
            }
        }


        #region Private Properties and Methods

        private static int cid = 1;
        private Dictionary<int, Gates.AbstractGate> gid = new Dictionary<int, Gates.AbstractGate>();
        private static int deltaX = 80;
        private static int deltaY = 80;
        private static DPoint dPoint = new DPoint(850, 600);
        private Dictionary<string, XElement> addedInputGate = new Dictionary<String, XElement>();

        private void Reset()
        {
            cid = 1;
            gid.Clear();
            //iniX = 850;
            //iniY = 300;
            dPoint = new DPoint(850, 600);
            addedInputGate.Clear();
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private int Rand()
        {
            return RandomNumber(40, 120);
        }

        private XElement ConvertInterTreeToXElement(ref XElement gates, ref XElement wires, InterTree tree, DPoint parentPos)
        {
            if (tree.IsOperator)
            {
                #region
                XElement gate = CheckInterTreeHead(ref gates, tree.Operator, new DPoint(parentPos.X - Rand(), parentPos.Y));

                int toGateID = int.Parse(gate.Attribute("ID").Value);

                if (tree.Operator == InterTree.LogicOperator.NOT)
                {
                    //Not gate 
                    XElement fromGate = ConvertInterTreeToXElement(ref gates, ref wires, tree.LeftNode, new DPoint(parentPos.X - Rand(), parentPos.Y));
                    int fromGateID = int.Parse(fromGate.Attribute("ID").Value);

                    XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                    wire.Element("From").SetAttributeValue("ID", fromGateID);
                    wire.Element("From").SetAttributeValue("Port", 0);
                    wire.Element("To").SetAttributeValue("ID", toGateID);
                    wire.Element("To").SetAttributeValue("Port", 0);
                    wires.Add(wire);
                }
                else {
                    //Left Child
                    XElement fromGate = ConvertInterTreeToXElement(ref gates, ref wires, tree.LeftNode, new DPoint(parentPos.X - Rand(), parentPos.Y - Rand()));
                    int fromGateID = int.Parse(fromGate.Attribute("ID").Value);

                    XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                    wire.Element("From").SetAttributeValue("ID", fromGateID);
                    wire.Element("From").SetAttributeValue("Port", 0);
                    wire.Element("To").SetAttributeValue("ID", toGateID);
                    wire.Element("To").SetAttributeValue("Port", 1);
                    wires.Add(wire);

                    //Right Child
                    fromGate = ConvertInterTreeToXElement(ref gates, ref wires, tree.RightNode, new DPoint(parentPos.X - Rand(), parentPos.Y + Rand()));
                    fromGateID = int.Parse(fromGate.Attribute("ID").Value);

                    wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                    wire.Element("From").SetAttributeValue("ID", fromGateID);
                    wire.Element("From").SetAttributeValue("Port", 0);
                    wire.Element("To").SetAttributeValue("ID", toGateID);
                    wire.Element("To").SetAttributeValue("Port", 0);
                    wires.Add(wire);
                } 
                return gate;
                #endregion
            }
            else
            {
                #region
                //Header So it is the input, add inputGate and relative Comment
                string letter = tree.Head;
                if (!addedInputGate.Keys.Contains(letter))
                {
                    //Create one UserInput with Comment
                    XElement gt = new XElement("Gate");
                    gt.SetAttributeValue("Type", "UserInput");
                    gt.SetAttributeValue("Name", letter);
                    gt.SetAttributeValue("ID", cid);
                    gt.Add(new XElement("Point"));
                    gt.Element("Point").SetAttributeValue("X", parentPos.X - Rand());
                    gt.Element("Point").SetAttributeValue("Y", parentPos.Y);
                    gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                    gates.Add(gt);
                    addedInputGate.Add(letter, gt);
                    cid++;
                    //iniX -= 80;

                    gt = new XElement("Gate");
                    gt.SetAttributeValue("Type", "Comment");
                    gt.SetAttributeValue("Name", letter);
                    gt.SetAttributeValue("ID", cid);
                    gt.Add(new XElement("Point"));
                    gt.Element("Point").SetAttributeValue("X", parentPos.X - 2 * Rand());
                    gt.Element("Point").SetAttributeValue("Y", parentPos.Y);
                    gt.Element("Point").SetAttributeValue("Angle", 0.0f);
                    gt.Add(new XElement("Comment", letter));

                    gates.Add(gt);
                    cid++;
                    //iniY += 80;
                }
                return addedInputGate[letter];
                #endregion

            }
        }

        private XElement ConvertStarPadExprToXElement(ref XElement gates, ref XElement wires, starPadSDK.MathExpr.Expr expr, DPoint parentPos)
        {
            if (expr is starPadSDK.MathExpr.LetterSym)
            {
                #region
                //So it is the input, add inputGate and relative Comment
                string letter = (expr as starPadSDK.MathExpr.LetterSym).Letter.ToString();
                if (!addedInputGate.Keys.Contains(letter)) {
                   
                    //Create one UserInput with Comment
                    XElement gt = new XElement("Gate");
                    gt.SetAttributeValue("Type", "UserInput");
                    gt.SetAttributeValue("Name", letter);
                    gt.SetAttributeValue("ID", cid);
                    gt.Add(new XElement("Point"));
                    gt.Element("Point").SetAttributeValue("X", parentPos.X - Rand());
                    gt.Element("Point").SetAttributeValue("Y", parentPos.Y);
                    gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                    gates.Add(gt);
                    addedInputGate.Add(letter, gt);
                    cid++;
                    //iniX -= 80;

                    gt = new XElement("Gate");
                    gt.SetAttributeValue("Type", "Comment");
                    gt.SetAttributeValue("Name", letter);
                    gt.SetAttributeValue("ID", cid);
                    gt.Add(new XElement("Point"));
                    gt.Element("Point").SetAttributeValue("X", parentPos.X - 2 * Rand());
                    gt.Element("Point").SetAttributeValue("Y", parentPos.Y);
                    gt.Element("Point").SetAttributeValue("Angle", 0.0f);
                    gt.Add(new XElement("Comment", letter));

                    gates.Add(gt);
                    cid++;
                    //iniY += 80;
                }
                return addedInputGate[letter];
                #endregion
            }
            else if (expr is starPadSDK.MathExpr.CompositeExpr)
            {
                starPadSDK.MathExpr.CompositeExpr compositeExpr = expr as starPadSDK.MathExpr.CompositeExpr;

                bool isNotGate = false;

                XElement gate = CheckExprHead(ref gates, compositeExpr.Head, new DPoint(parentPos.X - Rand(), parentPos.Y), out isNotGate);

                int toGateID = int.Parse(gate.Attribute("ID").Value);
              
                if (compositeExpr.Args.Length == 2)
                {
                    if (isNotGate)
                    {
                        //Not gate 
                        XElement fromGate = ConvertStarPadExprToXElement(ref gates, ref wires, compositeExpr.Args[0], new DPoint(parentPos.X - Rand(), parentPos.Y));
                        int fromGateID = int.Parse(fromGate.Attribute("ID").Value);

                        XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                        wire.Element("From").SetAttributeValue("ID", fromGateID);
                        wire.Element("From").SetAttributeValue("Port", 0);
                        wire.Element("To").SetAttributeValue("ID", toGateID);
                        wire.Element("To").SetAttributeValue("Port", 0);
                        wires.Add(wire);
                    }
                    else
                    {
                        //Left Child
                        XElement fromGate = ConvertStarPadExprToXElement(ref gates, ref wires, compositeExpr.Args[0], new DPoint(parentPos.X - Rand(), parentPos.Y - Rand()));
                        int fromGateID = int.Parse(fromGate.Attribute("ID").Value);

                        XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                        wire.Element("From").SetAttributeValue("ID", fromGateID);
                        wire.Element("From").SetAttributeValue("Port", 0);
                        wire.Element("To").SetAttributeValue("ID", toGateID);
                        wire.Element("To").SetAttributeValue("Port", 1);
                        wires.Add(wire);

                        //Right Child
                        fromGate = ConvertStarPadExprToXElement(ref gates, ref wires, compositeExpr.Args[1], new DPoint(parentPos.X - Rand(), parentPos.Y + Rand()));
                        fromGateID = int.Parse(fromGate.Attribute("ID").Value);

                        wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                        wire.Element("From").SetAttributeValue("ID", fromGateID);
                        wire.Element("From").SetAttributeValue("Port", 0);
                        wire.Element("To").SetAttributeValue("ID", toGateID);
                        wire.Element("To").SetAttributeValue("Port", 0);
                        wires.Add(wire);
                    }
                }
                else if (compositeExpr.Args.Length > 2)
                {
                    //There are above 2 argumennts inside of list, so construct one new binary tree to parse. 

                    List<XElement> duplicatedGates = new List<XElement>();
                    DPoint tempPoint = new DPoint(parentPos.X -deltaX, parentPos.Y - deltaY);;
                    for(int i = 0; i < compositeExpr.Args.Length - 2; i++)
                    {
                        XElement gt = DuplicateGate(ref gates, gate, ref tempPoint);
                        duplicatedGates.Add(gt);
                    }

                    List<starPadSDK.MathExpr.Expr> compositeExpressions = new List<starPadSDK.MathExpr.Expr>();

                    foreach (starPadSDK.MathExpr.Expr ce in compositeExpr.Args)
                    {
                        compositeExpressions.Add(ce);
                    }

                    XElementTree argTree = new XElementTree(gate);

                    duplicateWires(ref gates, ref wires, ref duplicatedGates, ref compositeExpressions, ref argTree);

                }
                else if (compositeExpr.Args.Length == 1)
                { 
                    //Not gate 
                    XElement fromGate = ConvertStarPadExprToXElement(ref gates, ref wires, compositeExpr.Args[0], new DPoint(parentPos.X - Rand(), parentPos.Y));
                    int fromGateID = int.Parse(fromGate.Attribute("ID").Value);

                    XElement wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                    wire.Element("From").SetAttributeValue("ID", fromGateID);
                    wire.Element("From").SetAttributeValue("Port", 0);
                    wire.Element("To").SetAttributeValue("ID", toGateID);
                    wire.Element("To").SetAttributeValue("Port", 0);
                    wires.Add(wire);
                }
                return gate;
            }
            else {
                return null;
            }
        }

        private void duplicateWires(ref XElement gates, ref XElement wires, ref List<XElement> duplicatedGates, ref List<starPadSDK.MathExpr.Expr> compositeExpressions, ref XElementTree toGate)
        {
            int toGateID = int.Parse(toGate.Head.Attribute("ID").Value);

            XElement fromGate = null;
            int fromGateID;
            XElement wire = null;

            XElement fromGate2 = null;
            int fromGateID2;

            if (duplicatedGates.Count != 0)
            {
                fromGate = duplicatedGates[0];
                fromGateID = int.Parse(fromGate.Attribute("ID").Value);
                duplicatedGates.Remove(fromGate);

                wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                wire.Element("From").SetAttributeValue("ID", fromGateID);
                wire.Element("From").SetAttributeValue("Port", 0);
                wire.Element("To").SetAttributeValue("ID", toGateID);
                wire.Element("To").SetAttributeValue("Port", 1);
                wires.Add(wire);
                
                if (toGate.LeftNode == null)
                {
                    toGate.LeftNode = new XElementTree(fromGate);
                    duplicateWires(ref gates, ref wires, ref duplicatedGates, ref compositeExpressions, ref toGate.LeftNode);
                }
                else {
                    toGate.RightNode = new XElementTree(fromGate);
                    duplicateWires(ref gates, ref wires, ref duplicatedGates, ref compositeExpressions, ref toGate.RightNode);
                }
            }
            else {
                starPadSDK.MathExpr.Expr expr = compositeExpressions[0];
                compositeExpressions.Remove(expr);

                double x = double.Parse(toGate.Head.Element("Point").Attribute("X").Value);
                double y = double.Parse(toGate.Head.Element("Point").Attribute("Y").Value);

                DPoint toGatePoint;

                if (toGate.LeftNode == null)
                {
                     toGatePoint = new DPoint(x, y - deltaY);
                     fromGate2 = ConvertStarPadExprToXElement(ref gates, ref wires, expr, toGatePoint);
                     toGate.LeftNode = new XElementTree(fromGate2);
                }
                else {
                     toGatePoint = new DPoint(x, y + deltaY);
                     fromGate2 = ConvertStarPadExprToXElement(ref gates, ref wires, expr, toGatePoint);
                     toGate.RightNode = new XElementTree(fromGate2);
                }

                fromGateID2 = int.Parse(fromGate2.Attribute("ID").Value);

                wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                wire.Element("From").SetAttributeValue("ID", fromGateID2);
                wire.Element("From").SetAttributeValue("Port", 0);
                wire.Element("To").SetAttributeValue("ID", toGateID);
                wire.Element("To").SetAttributeValue("Port", 1);
                wires.Add(wire);        
            }

            if (compositeExpressions.Count != 0)
            {
                starPadSDK.MathExpr.Expr expr = compositeExpressions[0];
                compositeExpressions.Remove(expr);

                double x = double.Parse(toGate.Head.Element("Point").Attribute("X").Value);
                double y = double.Parse(toGate.Head.Element("Point").Attribute("Y").Value);

                DPoint toGatePoint;

                if (toGate.LeftNode == null)
                {
                    toGatePoint = new DPoint(x, y - deltaY);
                    fromGate2 = ConvertStarPadExprToXElement(ref gates, ref wires, expr, toGatePoint);
                    toGate.LeftNode = new XElementTree(fromGate2);
                }
                else
                {
                    toGatePoint = new DPoint(x, y + deltaY);
                    fromGate2 = ConvertStarPadExprToXElement(ref gates, ref wires, expr, toGatePoint);
                    toGate.RightNode = new XElementTree(fromGate2);
                }

                fromGateID2 = int.Parse(fromGate2.Attribute("ID").Value);

                wire = new XElement("Wire", new XElement("From"), new XElement("To"));
                wire.Element("From").SetAttributeValue("ID", fromGateID2);
                wire.Element("From").SetAttributeValue("Port", 0);
                wire.Element("To").SetAttributeValue("ID", toGateID);
                wire.Element("To").SetAttributeValue("Port", 0);
                wires.Add(wire);
            }
        }

        private XElement DuplicateGate(ref XElement gates, XElement gate, ref DPoint point)
        {
            point.X -= deltaX;
            point.Y -= deltaY;
            
            XElement gt = new XElement("Gate");
            gt.SetAttributeValue("Type", gate.Attribute("Type").Value);
            gt.SetAttributeValue("Name", gate.Attribute("Name").Value);
            gt.SetAttributeValue("ID", cid);
            gt.SetAttributeValue("NumInputs", 2);
            gt.Add(new XElement("Point"));
            gt.Element("Point").SetAttributeValue("X", point.X);
            gt.Element("Point").SetAttributeValue("Y", point.Y);
            gt.Element("Point").SetAttributeValue("Angle", 0.0f);
            
            gates.Add(gt);
            cid++;
            //iniX -= 80;
            //iniY += 80;

            return gt;
        }


        private XElement CheckInterTreeHead(ref XElement gates, InterTree.LogicOperator oper, DPoint point)
        {
            XElement gt = null;

            //TODO: XOR Gate and Not Gate
            if (oper == InterTree.LogicOperator.OR)
            {
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "Or");
                gt.SetAttributeValue("Name", "Or");
                gt.SetAttributeValue("ID", cid);
                gt.SetAttributeValue("NumInputs", 2);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", point.X);
                gt.Element("Point").SetAttributeValue("Y", point.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                //iniX -= 80;
                //iniY += 80;
            }
            else if (oper == InterTree.LogicOperator.AND)
            {
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "And");
                gt.SetAttributeValue("Name", "And");
                gt.SetAttributeValue("ID", cid);
                gt.SetAttributeValue("NumInputs", 2);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", point.X);
                gt.Element("Point").SetAttributeValue("Y", point.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                //iniX -= 80;
                //iniY += 80;
            }
            else if (oper == InterTree.LogicOperator.NOT)
            {
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "Not");
                gt.SetAttributeValue("Name", "Not");
                gt.SetAttributeValue("ID", cid);
                gt.SetAttributeValue("NumInputs", 2);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", point.X);
                gt.Element("Point").SetAttributeValue("Y", point.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
            } 
           
            return gt;
        }

        private XElement CheckExprHead(ref XElement gates, starPadSDK.MathExpr.Expr expr, DPoint point, out bool isNotGate)
        {
            XElement gt = null;

            if (expr == starPadSDK.MathExpr.WellKnownSym.plus)
            {
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "Or");
                gt.SetAttributeValue("Name", "Or");
                gt.SetAttributeValue("ID", cid);
                gt.SetAttributeValue("NumInputs", 2);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", point.X);
                gt.Element("Point").SetAttributeValue("Y", point.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                //iniX -= 80;
                //iniY += 80;
                isNotGate = false;
            }
            else if (expr == starPadSDK.MathExpr.WellKnownSym.times)
            {
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "And");
                gt.SetAttributeValue("Name", "And");
                gt.SetAttributeValue("ID", cid);
                gt.SetAttributeValue("NumInputs", 2);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", point.X);
                gt.Element("Point").SetAttributeValue("Y", point.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                //iniX -= 80;
                //iniY += 80;
                isNotGate = false;
            }
            else if( expr == starPadSDK.MathExpr.WellKnownSym.lognot){ 
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "Not");
                gt.SetAttributeValue("Name", "Not");
                gt.SetAttributeValue("ID", cid);
                gt.SetAttributeValue("NumInputs", 2);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", point.X);
                gt.Element("Point").SetAttributeValue("Y", point.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                isNotGate = true;
            }
            else if (expr == starPadSDK.MathExpr.WellKnownSym.xorGate)
            {
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "Xor");
                gt.SetAttributeValue("Name", "Xor");
                gt.SetAttributeValue("ID", cid);
                gt.SetAttributeValue("NumInputs", 2);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", point.X);
                gt.Element("Point").SetAttributeValue("Y", point.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                isNotGate = false;
            }
            else if (expr == starPadSDK.MathExpr.WellKnownSym.power)
            {
                //Not Gate
                gt = new XElement("Gate");
                gt.SetAttributeValue("Type", "Not");
                gt.SetAttributeValue("Name", "Not");
                gt.SetAttributeValue("ID", cid);
                gt.SetAttributeValue("NumInputs", 1);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", point.X);
                gt.Element("Point").SetAttributeValue("Y", point.Y);
                gt.Element("Point").SetAttributeValue("Angle", 0.0f);

                gates.Add(gt);
                cid++;
                isNotGate = true;
            }
            else {
                isNotGate = false;    
            }

            return gt;
        }

        private static List<XElement> parsedWires = new List<XElement>();

        private static Dictionary<int, Gates.AbstractGate> diagramGid = new Dictionary<int, Gates.AbstractGate>();

        public InterTree ParseXElement(XElement root)
        {
            parsedWires.Clear();
            diagramGid.Clear();

            XElement circuit = root.Element("Circuit");

            //Like load XElement
            if (root.Attribute("Version") != null && root.Attribute("Version").Value != "1.2")
                throw new Exception("Unsupport version " + root.Attribute("Version").Value);

            Gates.Circuit c = new Gates.Circuit();

                XElement outputGate = null;

                foreach (XElement gate in circuit.Element("Gates").Elements())
                {
                    Gates.AbstractGate abgate = this.CreateGate(gate);
                    if (gate.Attribute("Type").Value == "UserOutput")
                    {
                        outputGate = gate;
                    }
                    c.Add(abgate);
                    int gateIDTest = int.Parse(gate.Attribute("ID").Value);

                    diagramGid.Add(int.Parse(gate.Attribute("ID").Value), abgate);
                }
                if(outputGate == null)
                    return null;

                char letter = outputGate.Attribute("Name").Value[0];

                InterTree rootInterTree = new InterTree(InterTree.LogicOperator.EQUAL);
                rootInterTree.LeftNode = new InterTree(letter.ToString());

                int gateID = int.Parse(outputGate.Attribute("ID").Value);

                ConvertXElementToInterTree(gateID, ref rootInterTree, circuit);

                return rootInterTree;
        
        }

        //Modified method from CircuitXML.CreateGate Method
        private Gates.AbstractGate CreateGate(XElement gate)
        {
            int numInputs = 2; // default for variable input gates
            if (gate.Attribute("NumInputs") != null)
                numInputs = int.Parse(gate.Attribute("NumInputs").Value);

            switch (gate.Attribute("Type").Value)
            {
                case "And":
                    return new Gates.BasicGates.And(numInputs);
                case "Not":
                    return new Gates.BasicGates.Not();
                case "Or":
                    return new Gates.BasicGates.Or(numInputs);
                case "Nand":
                    return new Gates.BasicGates.Nand(numInputs);
                case "Nor":
                    return new Gates.BasicGates.Nor(numInputs);
                case "Xor":
                    return new Gates.BasicGates.Xor();
                case "Xnor":
                    return new Gates.BasicGates.Xnor();
                case "Buffer":
                    return new Gates.BasicGates.Buffer();
                case "UserInput":
                    Gates.IOGates.UserInput ui = new Gates.IOGates.UserInput();
                    ui.SetName(gate.Attribute("Name").Value);
                    return ui;
                case "UserOutput":
                    Gates.IOGates.UserOutput uo = new Gates.IOGates.UserOutput();
                    uo.SetName(gate.Attribute("Name").Value);
                    return uo;
                case "NumericInput":
                    Gates.IOGates.NumericInput ni = new Gates.IOGates.NumericInput(int.Parse(gate.Attribute("Bits").Value));
                    ni.SelectedRepresentation = (Gates.IOGates.AbstractNumeric.Representation)int.Parse(gate.Attribute("SelRep").Value);
                    ni.Value = gate.Attribute("Value").Value;
                    return ni;
                case "NumericOutput":
                    Gates.IOGates.NumericOutput no = new Gates.IOGates.NumericOutput(int.Parse(gate.Attribute("Bits").Value));
                    no.SelectedRepresentation = (Gates.IOGates.AbstractNumeric.Representation)int.Parse(gate.Attribute("SelRep").Value);
                    return no;
                case "Clock":
                    Gates.IOGates.Clock clk = new Gates.IOGates.Clock(int.Parse(gate.Attribute("Milliseconds").Value));
                    return clk;
                case "Comment":
                    Gates.IOGates.Comment cmt = new Gates.IOGates.Comment();
                    cmt.Value = gate.Element("Comment").Value;
                    return cmt;
            }
            throw new ArgumentException("unknown gate");
        }

        private void ConvertXElementToInterTree(int ID, ref InterTree root,  XElement circuit)
        {
            InterTree tree = null;

            foreach (XElement wire in circuit.Element("Wires").Elements())
            {
                int wireToGateID = int.Parse(wire.Element("To").Attribute("ID").Value);
              
                if (wireToGateID == ID && !parsedWires.Contains(wire))
                {
                    parsedWires.Add(wire);

                    //Retrieve the fromGateID
                    int wireFromGateID = int.Parse(wire.Element("From").Attribute("ID").Value);

                    Gates.AbstractGate fromGate = diagramGid[wireFromGateID];
                    
                    if (fromGate is Gates.IOGates.UserInput)
                    {
                        string inputGateName = (fromGate as Gates.IOGates.UserInput).Name;
                        tree = new InterTree(inputGateName);
                        tree.LeftNode = null;
                        tree.RightNode = null;
                    }
                    else if (fromGate is Gates.BasicGates.And)
                    {
                        tree = new InterTree(InterTree.LogicOperator.AND);
                    }
                    else if (fromGate is Gates.BasicGates.Or)
                    {
                        tree = new InterTree(InterTree.LogicOperator.OR);
                    }
                    else if (fromGate is Gates.BasicGates.Xor)
                    {
                        tree = new InterTree(InterTree.LogicOperator.XOR);
                    }
                    else if (fromGate is Gates.BasicGates.Not)
                    {
                        tree = new InterTree(InterTree.LogicOperator.NOT);
                    }
                    else {
                        tree = new InterTree(InterTree.LogicOperator.OTHER);
                    }
                   
                    if (root.LeftNode == null)
                    {
                        root.LeftNode = tree;
                    }
                    else
                    {
                        root.RightNode = tree;
                    }
                    ConvertXElementToInterTree(wireFromGateID, ref tree, circuit);
                }
            }
        }

        private starPadSDK.MathExpr.Expr RetrieveStarPadExpr(string s)
        {
               return starPadSDK.MathExpr.Text.Convert(s);
        }

        #endregion


        #region Match

        public static bool MatchTwoTruthTableValueArray(int[] fvalues1, int[] fvalues2)
        {
            if (fvalues1 == null || fvalues2 == null || fvalues1.Length != fvalues2.Length)
            {
                return false;
            }

            for (int i = 0; i < fvalues1.Length; i++)
            {
                if (!Object.Equals(fvalues1[i], fvalues2[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool MatchTwoInterTree(InterTree tree1, InterTree tree2)
        {
            return MatchInternalTwoInterTree(tree1.RightNode, tree2.RightNode);
        }

        private static bool MatchInternalTwoInterTree(InterTree tree1, InterTree tree2)
        {
            if (tree1.IsOperator && tree2.IsOperator)
            {
                //Compare the head operator
                if (tree1.Operator != tree2.Operator)
                {
                    return false;
                }

                if (tree1.Operator != InterTree.LogicOperator.NOT)
                {
                    //Compare two left node
                    if (tree1.LeftNode != null && tree2.RightNode != null)
                    {
                        if (!MatchInternalTwoInterTree(tree1.LeftNode, tree2.RightNode))
                        {
                            return false;
                        }
                    }
                    else if (tree1.LeftNode == null && tree2.RightNode == null)
                    {
                        //doing nothing,continue to comparee
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    //Compare two right node
                    if (tree1.RightNode != null && tree2.LeftNode != null)
                    {
                        if (!MatchInternalTwoInterTree(tree1.RightNode, tree2.LeftNode))
                        {
                            return false;
                        }
                    }
                    else if (tree1.RightNode == null && tree2.LeftNode == null)
                    {
                        //doing nothing, continue to compare
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    return true;

                }
                else
                {
                    //Not Operator
                    //Compare two left node
                    if (tree1.LeftNode != null && tree2.LeftNode != null)
                    {
                        if (!MatchInternalTwoInterTree(tree1.LeftNode, tree2.LeftNode))
                        {
                            return false;
                        }
                    }
                    else if (tree1.LeftNode == null && tree2.LeftNode == null)
                    {
                        //doing nothing,continue to comparee
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
            }
            else if (!tree1.IsOperator && !tree2.IsOperator)
            {
                //Two Letters
                return true;
            }
            else
            {
                //one Letter, one operator
                return false;
            }

        }

        #endregion
    }
}
