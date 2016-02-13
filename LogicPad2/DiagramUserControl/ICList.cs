using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

namespace LogicPad2.Diagram
{
    public class ICList : BindingList<UIGates.IC>
    {
        private List<UIGates.IC> backup = new List<UIGates.IC>();

        public ICList()
        {
            this.ListChanged += new ListChangedEventHandler(ICList_ListChanged);
        }

        void ICList_ListChanged(object sender, ListChangedEventArgs e)
        {
            Dictionary<Gates.AbstractGate, Gates.AbstractGate> replacements;

            if (e.ListChangedType == ListChangedType.ItemChanged ||
                e.ListChangedType == ListChangedType.ItemDeleted)
            {

                UIGates.IC original = backup[e.NewIndex];

                // must also change the template ICs
                foreach (UIGates.IC tic in this)
                {

                    replacements = ((Gates.IC)tic.AbGate).Circuit.ReplaceICs(original.AbGate.Name,
                        e.ListChangedType == ListChangedType.ItemChanged ? (Gates.IC)this[e.NewIndex].AbGate : null);
                    if (replacements.Count > 0)
                    {
                        tic.UpdateLocationHints(replacements);
                        if (ChangeIC != null)
                            ChangeIC(this, new ChangeICEventArgs(tic, tic));
                    }
                }
                if (ChangeIC != null)
                    ChangeIC(this, new ChangeICEventArgs(original,
                        e.ListChangedType == ListChangedType.ItemChanged ? this[e.NewIndex] : null));
            }


            backup.Clear();
            backup.AddRange(this);

        }

        /// <summary>
        /// Given a name, find the IC matching that name.  Returns the original.
        /// Throws an exception if the name is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIGates.IC GetIC(string name)
        {
            foreach (UIElement uie in this.Items)
            {
                if (uie is UIGates.IC)
                {
                    if (((UIGates.IC)uie).AbGate.Name == name)
                        return ((UIGates.IC)uie);
                }
            }
            throw new ArgumentException("Not found");
        }

        /// <summary>
        /// Appends -1, or -2, or -3, etc. to the basename until it finds a name
        /// which is not yet in use in this list.
        /// </summary>
        /// <param name="basename"></param>
        /// <returns></returns>
        public string GenerateAvailableName(string basename)
        {
            UIGates.IC pic = null;
            int seq = 1;
            do
            {
                try
                {
                    pic = GetIC(basename + "-" + seq.ToString());
                    seq++;
                }
                catch (ArgumentException) { pic = null; }
            } while (pic != null);

            return basename + "-" + seq.ToString();
        }


        /// <summary>
        /// Indicates a certain IC has been replaced with another IC.  Both the original
        /// and new IC are provided.
        /// </summary>
        public class ChangeICEventArgs
        {
            public UIGates.IC original;
            public UIGates.IC newic;
            public ChangeICEventArgs(UIGates.IC original, UIGates.IC newic)
            {
                this.original = original;
                this.newic = newic;
            }
        }

        public delegate void ChangeICEventHandler(object sender, ChangeICEventArgs e);

        /// <summary>
        /// Occurs when an IC is replaced with another IC
        /// </summary>
        public event ChangeICEventHandler ChangeIC;

    }
}
