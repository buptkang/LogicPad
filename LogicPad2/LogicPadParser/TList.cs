using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPadParser
{
    public class TList
    {
        //List size
        private int size;

        //List head node
        private Node head;

        //List tail node
        private Node tail;

        public TList()
        {
            size = 0;
            head = null;
            tail = null;
        }

        /*
         *add node to the list
         */
        public void addNode(int value, int sub, bool mark)
        {
            Node temp = new Node();

            if (size == 0)
                head = tail = temp;
            else
            {
                tail.Next = temp;
                tail = temp;
            }
            size++;

            temp.Value = value;
            temp.Sub = sub;
            temp.Mark = mark;
        }

        /*
            *add node to the list
        */

        public void addNode(int value)
        {
            addNode(value, 0, false);
        }


        /*
         * get all the value fields in each node of the list in avector
         */

        public List<int> getValues()
        {
            List<int> values = new List<int>();

            Node temp = head;

            while (temp != null)
            {
                values.Add(temp.Value);
                temp = temp.Next;
            }
            return values;
        }

        /*
         * get all the sub fields in each node of the list in avector
         */

        public List<int> getSubs()
        {
            List<int> subs = new List<int>();

            Node temp = head;

            while (temp != null)
            {
                subs.Add(temp.Sub);
                temp = temp.Next;
            }
            return subs;
        }


        #region Property List
        public Node Head
        {
            get { return head; }
            set { head = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public Node Tail
        {
            get { return tail; }
            set { tail = value; }
        }
        #endregion
    }

    public class Node
    {
        private int value;

        private int sub;

        private Node next;

        private bool mark;

        #region Property List
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public int Sub
        {
            get { return sub; }
            set { sub = value; }
        }

        public Node Next
        {
            get { return next; }
            set { next = value; }
        }

        public bool Mark
        {
            get { return mark; }
            set { mark = value; }
        }

        #endregion

        public Node()
        {
            value = -1;
            sub = 0;
            next = null;
            mark = false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Node node = obj as Node;
            if ((System.Object)node == null)
            {
                return false;
            }

            return (value == node.value) && (sub == node.sub);
        }

        public static bool operator ==(Node n1, Node n2)
        {
            if (System.Object.ReferenceEquals(n1, n2))
            {
                return true;
            }

            if ((object)n1 == null || (object)n2 == null)
            {
                return false;
            }

            return ((n1.Value == n2.Value) && (n1.Sub == n2.Sub));
        }

        public static bool operator !=(Node n1, Node n2)
        {
            return !(n1 == n2);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ Sub.GetHashCode();
        }
    }
}
