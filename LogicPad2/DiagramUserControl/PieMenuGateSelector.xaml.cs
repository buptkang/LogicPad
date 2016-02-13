using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace LogicPad2.Diagram
{
    /// <summary>
    /// Interaction logic for PieMenuGateSelector.xaml
    /// </summary>
    public partial class PieMenuGateSelector : UserControl
    {
        #region Legacy code
        
        private UndoRedo.UndoManager undoProvider;
        private bool _ro = false;
        private string _icname;
        private ICList icl;
        private const double MAX_SIZE = 100;
    
        /// <summary>
        /// If an undo manager is provided, changes to ICs will be undoable.
        /// </summary>
        public UndoRedo.UndoManager UndoProvider
        {
            set
            {
                undoProvider = value;
            }
        }

        public bool IsReadOnly
        {
            set
            {
                /*
                foreach (Gate g in spGates.Children)
                {
                    g.IsReadOnly = value;
                    g.ContextMenu.IsEnabled = !value;
                    SetInfoLine(g as UIGates.IC);
                }
                 * */
                _ro = value;
            }
        }

        public string ICName
        {
            set
            {
                _icname = value;
                /*
                foreach (UIGates.IC ic in spGates.Children)
                {
                    if (((Gates.IC)ic.AbGate).DeepIncludes(value))
                        ic.Visibility = Visibility.Collapsed;
                    else
                        ic.Visibility = Visibility.Visible;
                }
                 * */
            }
        }

        public ICList ICList
        {
            set
            {
                if (icl != null)
                {
                    icl.ListChanged -= new System.ComponentModel.ListChangedEventHandler(icl_ListChanged);
                    icl.ChangeIC -= new ICList.ChangeICEventHandler(icl_ChangeIC);
                    //spGates.Children.Clear();
                }
                icl = value;
                foreach (UIGates.IC nic in icl)
                    AddDragDropGate((UIGates.IC)nic.CreateUserInstance());

                icl.ListChanged += new System.ComponentModel.ListChangedEventHandler(icl_ListChanged);
                icl.ChangeIC += new ICList.ChangeICEventHandler(icl_ChangeIC);
            }
        }

        private void icl_ChangeIC(object sender, ICList.ChangeICEventArgs e)
        {
            int idx = -1;
          
            /*
            for (int i = 0; i < spGates.Children.Count; i++)
                if (((UIGates.IC)spGates.Children[i]).AbGate.Name == e.original.AbGate.Name)
                    idx = i;
            spGates.Children.RemoveAt(idx);
            */
            
            if (e.newic != null)
                AddDragDropGate(idx, (UIGates.IC)e.newic.CreateUserInstance());
        }

        private void icl_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            // we have to clone these because there could be multiple selectors
            // operating off of a single master ICList
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    AddDragDropGate(e.NewIndex, (UIGates.IC)icl[e.NewIndex].CreateUserInstance());
                    break;
                case ListChangedType.ItemChanged:
                    // replace the gate as needed
                    //spGates.Children.RemoveAt(e.NewIndex);
                    AddDragDropGate(e.NewIndex, (UIGates.IC)icl[e.NewIndex].CreateUserInstance());
                    break;
                case ListChangedType.Reset:
                    ICList = icl;
                    break;
            }
        }

        private void AddDragDropGate(int pos, UIGates.IC g)
        {
            g.DataContext = g.CreateUserInstance();

            DragDrop.DragDropHelper.SetIsDragSource(g, true);
            DragDrop.DragDropHelper.SetDragDropControl(g, new DragDrop.GateDragDropAdorner());
            DragDrop.DragDropHelper.SetDropTarget(g, "inkCanvas");
            DragDrop.DragDropHelper.SetAdornerLayer(g, "adornerLayer");


            g.PreviewICNameChanged += (object sender2, string newname, ref bool cancel) =>
            {
                if (newname == "")
                    cancel = true;

                foreach (Gate g2 in icl)
                {
                    if (newname == g2.AbGate.Name)
                        cancel = true;
                }
            };

            g.ICNameChanged += (sender2, newname) =>
            {
                UIGates.IC oic = icl.GetIC((g.AbGate.Name));
                UIGates.IC nic = g.CreateUserInstance(newname);
                icl[icl.IndexOf(oic)] = nic;
                if (undoProvider != null)
                    undoProvider.Add(new UndoRedo.ReplaceIC(icl, oic, nic));

            };

            ScaleTransform st = new ScaleTransform();
            st.CenterX = g.Width / 2.0;
            st.CenterY = g.Height / 2.0;
            double fac = 1.0;
            if (g.Width > MAX_SIZE)
                fac = Math.Min(MAX_SIZE / g.Width, fac);

            if (g.Height > MAX_SIZE)
                fac = Math.Min(MAX_SIZE / g.Height, fac);
            st.ScaleY = fac;
            st.ScaleX = fac;
            g.LayoutTransform = st;


            g.ContextMenu = new ContextMenu();
            MenuItem exp = new MenuItem();
            exp.Header = "Export...";
            exp.Click += (sender2, e2) =>
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".ic";
                dlg.Filter = "IC (.ic)|*.ic";
                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    CircuitXML cxml = new CircuitXML(icl);
                    try
                    {
                        cxml.Save(dlg.FileName, g);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to save IC: " + ex.ToString());
                    }
                }
            };
            g.ContextMenu.Items.Add(exp);
            MenuItem del = new MenuItem();
            del.Header = "Delete";
            del.Click += (sender2, e2) =>
            {
                if (MessageBox.Show("All instances of this IC in all circuits will be removed.  This operation cannot be undone.  Proceed?", "Danger Zone", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    UIGates.IC todel = icl.GetIC(g.AbGate.Name);

                    icl.Remove(todel);
                    if (undoProvider != null)
                        undoProvider.Clear();
                }
            };
            g.ContextMenu.Items.Add(del);
            MenuItem hid = new MenuItem();
            hid.Header = "Hide";
            hid.Click += (sender2, e2) =>
            {
                g.Visibility = Visibility.Collapsed;
            };
            //g.ContextMenu.Items.Add(hid);

            //BOKANG
            //spGates.Children.Insert(pos, g);
            //g.MouseDoubleClick += new MouseButtonEventHandler(g_MouseDoubleClick);
            //BOKANG
            //expUserGates.IsExpanded = true;
            g.BringIntoView();
            g.IsReadOnly = _ro;
            g.ContextMenu.IsEnabled = !_ro;

            if (!string.IsNullOrEmpty(_icname))
                if (((Gates.IC)g.AbGate).DeepIncludes(_icname))
                    g.Visibility = Visibility.Collapsed;

            ((Gates.IC)g.AbGate).Circuit.Start();
        }


        private void AddDragDropGate(UIGates.IC g)
        {
            //AddDragDropGate(spGates.Children.Count, g);
        }


        private void g_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //BOKANG
            /*
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s2, e2) => { System.Threading.Thread.Sleep(500); };
            bg.RunWorkerCompleted += (s2, e2) => { DragDrop.DragDropHelper.Cancel(); };

            if (_ro)
                return;

            foreach (Window w in Application.Current.Windows)
            {
                if (((Window1)w).BasedOn != null &&
                    ((Window1)w).MyEditLevel == Window1.EditLevel.EDIT &&
                    ((Window1)w).BasedOn.AbGate.Name == ((UIGates.IC)sender).AbGate.Name)
                {
                    w.Activate();
                    return;
                }
            }

            UIGates.IC template = ((UIGates.IC)sender).CreateUserInstance() as UIGates.IC;
            ((Gates.IC)template.AbGate).Circuit.Start();
            Window1 icw = new Window1(template, icl, Window1.EditLevel.EDIT);


            icw.Show();
            icw.Closing += (s2, e2) =>
            {
                try
                {

                    
                    // only replace gates if changes made
                    if (!icw.gateCanvas.UndoProvider.IsAtSavePoint)
                    {
                        UIGates.IC oic = icl.GetIC(icw.BasedOn.AbGate.Name);
                        UIGates.IC nic = icw.GetIC();

                        // check for recursion
                        // can bypass the selector if you are sneaky
                        foreach (Gates.AbstractGate ag in ((Gates.IC)nic.AbGate).Circuit)
                        {
                            if (ag is Gates.IC)
                            {
                                if (((Gates.IC)ag).DeepIncludes(((UIGates.IC)nic).AbGate.Name)) 
                                {
                                    MessageBox.Show("Recursive circuit detected");
                                    return;
                                }
                            }
                        }

                        // check for decreased inputs
                        // can't undo
                        if (oic.AbGate.NumberOfInputs > nic.AbGate.NumberOfInputs)
                        {
                            if (MessageBox.Show("Reducing the number of inputs will affect all instances of this IC in all circuits.  This operation cannot be undone. Proceed?", "Danger Zone", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                                return;


                        }

                        icl[icl.IndexOf(oic)] = nic;
                        if (undoProvider != null)
                            undoProvider.Add(new UndoRedo.ReplaceIC(icl, oic, nic));

                    }

                }
                catch (Exception) { } // can fail if this IC has been removed

                ((Gates.IC)template.AbGate).Circuit.Stop();

            };

            icl.ChangeIC += (s2, e2) =>
            {
                if (e2.original.AbGate.Name == ((UIGates.IC)sender).AbGate.Name)
                {
                    if (e2.newic == null)
                    {
                        icw.Close();
                    }
                    else
                    {
                        // find the gate being edited
                        foreach (UIGates.IC g in spGates.Children)
                            if (g.AbGate.Name == e2.newic.AbGate.Name)
                                icw.RefreshGateCanvas(g.CreateUserInstance() as UIGates.IC);
                    }
                }
            };
            
            */

        }

        #endregion

        public PieMenuGateSelector()
        {
            InitializeComponent();

            tbAnd.DataContext = new UIGates.And();
            tbNot.DataContext = new UIGates.Not();
            tbOr.DataContext = new UIGates.Or();
            tbXor.DataContext = new UIGates.Xor();
            tbUserInput.DataContext = new UIGates.UserInput();
            tbUserOutput.DataContext = new UIGates.UserOutput();
            //tbComment.DataContext = new UIGates.Comment();

            tbUserInput.IsReadOnly = false;
            tbUserOutput.IsReadOnly = false;
            //tbComment.IsReadOnly = false;
        }

        public UIGates.Not NotGate
        {
            set { this.tbNot = value; }
            get { return this.tbNot; }
        }

        public UIGates.And AndGate
        {
            set { this.tbAnd = value; }
            get { return this.tbAnd; }
        }

        public UIGates.Or OrGate
        {
            set { this.tbOr = value; }
            get { return this.tbOr; }
        }

        public UIGates.Xor XorGate
        {
            set { this.tbXor = value; }
            get { return this.tbXor; }
        }

        public UIGates.UserInput InputGate
        {
            set { this.tbUserInput = value; }
            get { return this.tbUserInput; }
        }

        public UIGates.UserOutput OutputGate
        {
            set { this.tbUserOutput = value; }
            get { return this.tbUserOutput; }
        }

        public double UserControlToolTipX
        {
            get { return this.UserControlToolTipXY.X; }
            set { this.UserControlToolTipXY.X = value; }
        }

        public double UserControlToolTipY
        {
            get { return this.UserControlToolTipXY.Y; }
            set { this.UserControlToolTipXY.Y = value; }
        }
    }
}
