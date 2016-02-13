using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

/*
 * Parser for the Inter Tree
 *
 * 
 */

namespace LogicPadParser
{
    public class InterTree
    {
        private static StringBuilder builder = new StringBuilder();

        public static void TraverseTreeInOrder(InterTree root)
        {
            if (root.IsOperator)
            {
                //builder.Append("(");
                if (root.LeftNode != null) TraverseTreeInOrder(root.LeftNode);
                if (root.Operator == LogicOperator.AND)
                {
                    //replaced "." with empty string
                    builder.Append("");
                }
                else if (root.Operator == LogicOperator.OR)
                {
                    builder.Append("+");
                }
                else if(root.Operator == LogicOperator.EQUAL)
                {
                    builder.Append("=");
                }
                else if (root.Operator == LogicOperator.NOT)
                {
                    builder.Append("'");
                }
                else if (root.Operator == LogicOperator.XOR)
                {
                    builder.Append("*");
                }

                if (root.RightNode != null) TraverseTreeInOrder(root.RightNode);
                //builder.Append(")");
            }
            else
            {
                if (root.LeftNode != null) TraverseTreeInOrder(root.LeftNode);
                builder.Append(root.Head);
                if (root.RightNode != null) TraverseTreeInOrder(root.RightNode);
            }
        }

        public static void TraverseTreeBackInOrder(InterTree root)
        {
            if (root.IsOperator)
            {
                //builder.Append("(");
                if (root.RightNode != null) TraverseTreeBackInOrder(root.RightNode);
                if (root.Operator == LogicOperator.AND)
                {
                    //replaced "." with empty string
                    builder.Append("");
                }
                else if (root.Operator == LogicOperator.OR)
                {
                    builder.Append("+");
                }
                else if (root.Operator == LogicOperator.EQUAL)
                {
                    builder.Append("=");
                }
                else if (root.Operator == LogicOperator.NOT)
                {
                    builder.Append("'");
                }
                else if (root.Operator == LogicOperator.XOR)
                {
                    builder.Append("*");
                }
                if (root.LeftNode != null) TraverseTreeBackInOrder(root.LeftNode);
                //builder.Append(")");
            }
            else
            {
                if (root.RightNode != null) TraverseTreeBackInOrder(root.RightNode);
                builder.Append(root.Head);
                if (root.LeftNode != null) TraverseTreeBackInOrder(root.LeftNode);
            }
        }

        public static string ParseExpressionToString(InterTree root)
        {
            builder = new StringBuilder();
            builder.Append(root.LeftNode.Head);
            builder.Append(" = ");
            TraverseTreeInOrder(root.RightNode);
            return builder.ToString();
        }

        public static string ParseDiagramToString(InterTree root)
        {
            builder = new StringBuilder();
            builder.Append(root.LeftNode.Head);
            builder.Append(" = ");
            TraverseTreeBackInOrder(root.RightNode);
            return builder.ToString();
        }

        #region InterTree To TruthTable Conversion

        //Used for Calculating Tokens
        public static void ParseInterTreeTokens(InterTree root, ref List<String> tokens)
        {
            if (root == null)
            {
                return;
            }

            if (root.IsOperator)
            {
                ParseInterTreeTokens(root.LeftNode, ref tokens);
                ParseInterTreeTokens(root.RightNode, ref tokens);
            }else {
                if (!tokens.Contains(root.Head))
                {
                    tokens.Add(root.Head);
                }
                return;
            }
        }

        public static void ParseInterTreeToInterTreeList(ref InterTree root, ref List<InterTree> treeList)
        {
            if (root == null) return;

            if (root.IsOperator)
            {
                if (root.Operator == LogicOperator.OR)
                {
                    #region OR Gate

                    if (root.LeftNode != null)
                    {
                        if (root.LeftNode.IsOperator && root.LeftNode.Operator == LogicOperator.AND)
                        {
                            treeList.Add(root.LeftNode);
                        }
                        else if (!root.LeftNode.IsOperator)
                        {
                            treeList.Add(root.LeftNode);
                        }
                        else
                        {
                            ParseInterTreeToInterTreeList( ref root._leftNode, ref treeList);
                        }
                    }

                    if (root.RightNode != null)
                    {
                        if (root.RightNode.IsOperator && root.RightNode.Operator == LogicOperator.AND)
                        {
                            treeList.Add(root.RightNode);
                        }
                        else if (!root.RightNode.IsOperator)
                        {
                            treeList.Add(root.RightNode);
                        }
                        else
                        {
                            ParseInterTreeToInterTreeList(ref root._rightNode, ref treeList);
                        }
                    }

                    #endregion
                }
                else if(root.Operator == LogicOperator.AND)
                {
                    #region And Gate
                    treeList.Add(root);
                    #endregion
                }
                else if (root.Operator == LogicOperator.XOR)
                { 
                    #region XOR Gate
                    //TODO
                    #endregion
                }
                else if (root.Operator == LogicOperator.NOT)
                {
                    #region OR Gate
                    treeList.Add(root);
                    #endregion
                }
                return;
            }
            else {
                treeList.Add(root);
                return;
            }
        }

        public static void ParseInterTreeListToStringArray(List<InterTree> treeList, ref List<List<String>> stringArray, string[] term_names)
        {
            List<String> tokens;
            List<String> result;
            string recordElement = null;
            foreach (InterTree row in treeList)
            {
                tokens = new List<String>();
                ParseInterTreeTokens(row, ref tokens);

                result = new List<String>();
                foreach (string token in tokens)
                {
                    recordElement = InterTree.SearchForNode(row, token);
                    if (recordElement.Equals("flag"))
                        break;
                    result.Add(recordElement);
                }
                if (recordElement != null && !recordElement.Equals("flag"))
                    stringArray.Add(result);
            }
        }

        private static String SearchForNode(InterTree root, String token)
        {
            String temp = null;
            bool positive = false, negative = false;
            if (root == null)
            {
                return null;
            }
            if (root.IsOperator)
            {
                if (root.Operator == LogicOperator.NOT)
                {
                    if (root.LeftNode.Head.Equals(token))
                    {
                        negative = true;
                    }
                }
                else {
                    temp = SearchForNode(root.LeftNode, token);
                    if (temp != null)
                    {
                        if (temp.Equals(token))
                            positive = true;
                        if (temp.Equals(token + "'"))
                            negative = true;
                        if (temp.Equals("flag"))
                        {
                            positive = true;
                            negative = true;
                        }

                    }

                    temp = SearchForNode(root.RightNode, token);
                    if (temp != null)
                    {
                        if (temp.Equals(token))
                            positive = true;
                        if (temp.Equals(token + "'"))
                            negative = true;
                        if (temp.Equals("flag"))
                        {
                            positive = true;
                            negative = true;
                        }
                    }
                }
            }else
            {
                if (token.Equals(root.Head))
                {
                    positive = true;
                }
                   
            }
            if (positive && negative)
                return "flag";
            else if (positive)
                return token;
            else if (negative)
                return token + "'";
            return null;
        }

        public static void MatchInterTreeToTruthTable(List<List<string>> tree, Dictionary<int, string[]> truthTableRows, ref Dictionary<int, int> truthTableResults, int row)
        {
            for (int index = 0; index < truthTableRows.Count; index++)
            {
                if (InterTree.ContainsNode(tree, truthTableRows[index]))
                {
                    truthTableResults[index] = 1;
                }
                else {
                    truthTableResults[index] = 0;
                }
            }
        }

        private static bool ContainsNode(List<List<string>> tree, string[] row)
        {
            bool isContained = false;
            bool isPreviousContained = true;
            foreach (List<string> record in tree)
            {
                foreach (string recordLabel in record)
                {
                    if (isPreviousContained)
                    {
                        if (row.Contains(recordLabel))
                        {
                            isContained = true;
                            isPreviousContained = true;
                        }
                        else
                        {
                            isContained = false;
                            isPreviousContained = false;
                        }
                    }
                }
                if (!isPreviousContained)
                {
                    isPreviousContained = true;
                }

                if (isContained)
                {
                    return true;
                }
            }
            return false;
        }



        public static void AddParentToInterTree(ref InterTree tree)
        {
            InterTree temp;
            if (tree == null)
            {
                return;
            }
            if (!tree.IsOperator)
            {
                return;
            }
            else {
                if (tree.LeftNode != null)
                {
                    tree.LeftNode.ParentNode = tree;
                    temp = tree.LeftNode;
                    AddParentToInterTree(ref temp);
                }
                if (tree.RightNode != null)
                {
                    tree.RightNode.ParentNode = tree;
                    temp = tree.RightNode;
                    AddParentToInterTree(ref temp);
                }
            }
        }

        public static void FilterNotGate(ref InterTree tree)
        {
            if(tree == null)
            {
                return;
            }

            if (!tree.IsOperator)
            {
                return;
            }
            else
            {
                if (tree.Operator == LogicOperator.NOT)
                {
                    if (tree.LeftNode != null && tree.LeftNode.IsOperator)
                    {
                        if (tree.LeftNode.Operator == LogicOperator.AND)
                        {
                            InterTree notLeftTree = new InterTree(LogicOperator.NOT);
                            InterTree notRightTree = new InterTree(LogicOperator.NOT);
                            InterTree orTree = new InterTree(LogicOperator.OR);
                            orTree.LeftNode = notLeftTree;
                            orTree.RightNode = notRightTree;
                            orTree.ParentNode = tree.ParentNode;

                            notLeftTree.LeftNode = tree.LeftNode.LeftNode;
                            notRightTree.LeftNode = tree.LeftNode.RightNode;

                            tree = orTree;
                        }
                        else if (tree.LeftNode.Operator == LogicOperator.OR)
                        {
                            InterTree notLeftTree = new InterTree(LogicOperator.NOT);
                            InterTree notRightTree = new InterTree(LogicOperator.NOT);
                            InterTree andTree = new InterTree(LogicOperator.AND);
                            andTree.LeftNode = notLeftTree;
                            andTree.RightNode = notRightTree;
                            andTree.ParentNode = tree.ParentNode;

                            notLeftTree.LeftNode = tree.LeftNode.LeftNode;
                            notRightTree.LeftNode = tree.LeftNode.RightNode;

                            tree = andTree;
                        }
                    }
                }
                FilterNotGate(ref tree._leftNode);
                FilterNotGate(ref tree._rightNode);
            }
        }

        public static void CheckDominationLaw(ref InterTree tree)
        {
            if (tree != null)
            {
                if (tree._leftNode != null) CheckDominationLaw(ref tree._leftNode);
                if (tree._rightNode != null) CheckDominationLaw(ref tree._rightNode);
            }else return;

            if (!tree.IsOperator)
            {
                return;
            }
            else
            {
                if (tree.Operator == LogicOperator.AND)
                {
                    if (tree.LeftNode == null || tree.RightNode == null)
                    {
                        tree = null;
                    }
                }
                
            }
        
        }

        public static void CheckComplement(ref InterTree tree)
        {
            if (tree == null)
            {
                return;
            }

            if (!tree.IsOperator)
            {
                return;
            }
            else
            {
                if (tree.Operator == LogicOperator.AND)
                {
                    if (tree.LeftNode != null && tree.LeftNode.IsOperator)
                    {
                        if (tree.LeftNode.Operator == LogicOperator.NOT)
                        {

                            if (tree.RightNode != null && tree.LeftNode.LeftNode != null && !tree.RightNode.IsOperator && !tree.LeftNode.LeftNode.IsOperator && tree.LeftNode.LeftNode.Head == tree.RightNode.Head)
                            {
                                tree = null;
                            }
                        }

                    }
                    else if (tree.RightNode != null && tree.RightNode.IsOperator)
                    {
                        if (tree.RightNode.Operator == LogicOperator.NOT)
                        {

                            if (tree.LeftNode != null && tree.RightNode.LeftNode != null && !tree.LeftNode.IsOperator && !tree.RightNode.LeftNode.IsOperator && tree.RightNode.LeftNode.Head == tree.LeftNode.Head)
                            {
                                tree = null;
                                //tree.LeftNode = null;
                                //tree.RightNode = null;
                            }
                        }

                    }
                }
                if (tree != null)
                {
                    CheckComplement(ref tree._leftNode);
                    CheckComplement(ref tree._rightNode);
                }
            }
        }



        public static void ReformulateInterTree(ref InterTree root)
        {
            if (root == null)
            {
                return;
            }
            if (!root.IsOperator)
            {
                return;
            }
            else {
               
                if (root.Operator == LogicOperator.OR)
                {
                   
                }
                else if(root.Operator == LogicOperator.AND){

                    if (root.LeftNode != null && root.LeftNode.IsOperator && root.LeftNode.Operator == LogicOperator.OR)
                    {
                        InterTree orTree = new InterTree(LogicOperator.OR);

                        InterTree duplicateOR = new InterTree(LogicOperator.AND);
                        InterTree rightNode = root.RightNode.MemberwiseClone() as InterTree;
                        duplicateOR.RightNode = rightNode;
                        rightNode.ParentNode = duplicateOR;

                        duplicateOR.LeftNode = root.LeftNode.RightNode;
                        root.LeftNode.RightNode.ParentNode = duplicateOR;

                        orTree.RightNode = duplicateOR;
                        
                        InterTree duplicateOR2 = new InterTree(LogicOperator.AND);
                        duplicateOR2.RightNode = root.RightNode;
                        root.RightNode.ParentNode = duplicateOR2;
                        duplicateOR2.LeftNode = root.LeftNode.LeftNode;
                        root.LeftNode.LeftNode.ParentNode = duplicateOR2;

                        orTree.LeftNode = duplicateOR2;

                        root = orTree;
                    }

                    if (root.RightNode != null && root.RightNode.IsOperator && root.RightNode.Operator == LogicOperator.OR)
                    {
                        InterTree orTree = new InterTree(LogicOperator.OR);

                        InterTree duplicateOR = new InterTree(LogicOperator.AND);
                        InterTree leftNode = root.LeftNode.MemberwiseClone() as InterTree;
                        duplicateOR.LeftNode = leftNode;
                        leftNode.ParentNode = duplicateOR;

                        duplicateOR.RightNode = root.RightNode.LeftNode;
                        root.RightNode.LeftNode.ParentNode = duplicateOR;

                        orTree.LeftNode = duplicateOR;

                        InterTree duplicateOR2 = new InterTree(LogicOperator.AND);
                        duplicateOR2.LeftNode = root.LeftNode;
                        root.LeftNode.ParentNode = duplicateOR2;

                        duplicateOR2.RightNode = root.RightNode.RightNode;
                        root.RightNode.RightNode.ParentNode = duplicateOR2;

                        orTree.RightNode = duplicateOR2;

                        root = orTree;
                    }
                }
                else if (root.Operator == LogicOperator.XOR)
                {     
                    InterTree OrTree = new InterTree(LogicOperator.OR);
                    OrTree.LeftNode = new InterTree(LogicOperator.AND);
                    OrTree.RightNode = new InterTree(LogicOperator.AND);

                    OrTree.LeftNode.LeftNode = new InterTree(root.LeftNode);
                    OrTree.LeftNode.RightNode = new InterTree(LogicOperator.NOT);
                    OrTree.LeftNode.RightNode.LeftNode = new InterTree(root.RightNode);

                    OrTree.RightNode.LeftNode = new InterTree(LogicOperator.NOT);
                    OrTree.RightNode.LeftNode.LeftNode = new InterTree(root.LeftNode);
                    OrTree.RightNode.RightNode = new InterTree(root.RightNode);

                    root = OrTree;
                }

                if (root.LeftNode != null) { ReformulateInterTree(ref root._leftNode); }
                if (root.RightNode != null) { ReformulateInterTree(ref root._rightNode); }
            }
        }


        #endregion

        #region Constructor
        public InterTree(String head)
        {
            _head = head;
            _isOperator = false;
        }

        public InterTree(LogicOperator op)
        {
            _op = op;
            _isOperator = true;
        }

        public InterTree(InterTree tree)
        {
            if (tree.IsOperator)
            {
                this.IsOperator = true;
                this.Operator = tree.Operator;
                this.LeftNode = tree.LeftNode;
                this.RightNode = tree.RightNode;
                this.ParentNode = tree.ParentNode;
            }
            else {
                this.Head = tree.Head;
                this.IsOperator = false;
                this.ParentNode = tree.ParentNode;
            }
        }
        #endregion

        #region Property List

        public enum LogicOperator
        {
            AND, OR, NOT, XOR, EQUAL, OTHER
        };

        //If it is operator, _op is set; else _head is set.
        private bool _isOperator;
        private string _head;
        private LogicOperator _op;

        public InterTree _leftNode;
        public InterTree _rightNode;
        public InterTree _parentNode;

        public string Head
        {
            set { _head = value; }
            get { return _head; }
        }

        public bool IsOperator
        {
            set { _isOperator = value; }
            get { return _isOperator; }
        }

        public LogicOperator Operator
        {
            set { _op = value; }
            get { return _op; }
        }

        public InterTree LeftNode
        {
            set { _leftNode = value; }
            get { return _leftNode; }
        }

        public InterTree RightNode
        {
            set { _rightNode = value; }
            get { return _rightNode; }
        }

        public InterTree ParentNode
        {
            set { _parentNode = value; }
            get { return _parentNode; }
        }


        #endregion

    }

    public class XElementTree
    {
        private XElement _head;

        public XElementTree LeftNode;

        public XElementTree RightNode;

        public XElementTree(XElement head)
        {
            _head = head;
        }

        public XElement Head {

            get { return _head; }
            set { _head = value; }
        }
    }
}
