using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

using Gates;

namespace LogicPad2.Diagram.UIGates
{
    public class UserIO: Gate
    {
        protected Gates.IOGates.UserIO _uio;
        protected TextBox txtName;
        protected Shape _r;

        /// <summary>
        /// If an undo manager is provided, renames will be undoable.
        /// </summary>
        public UndoRedo.UndoManager UndoProvider { get; set; }

        public UserIO(Gates.IOGates.UserIO gate, TerminalID[] termids)
            : base(gate, termids)
        {
            _uio = gate;

            _gate.PropertyChanged += EventDispatcher.CreateBatchDispatchedHandler(_gate, _ui_PropertyChanged);

            txtName = new TextBox();
            txtName.Margin = new System.Windows.Thickness(5, 22, 5, 22);
            txtName.Visibility = System.Windows.Visibility.Hidden;
            txtName.Width = 54;
            txtName.Height = 20;
            txtName.LostFocus += new System.Windows.RoutedEventHandler(txtName_LostFocus);
            txtName.KeyDown += new System.Windows.Input.KeyEventHandler(txtName_KeyDown);

            //ContextMenu = new System.Windows.Controls.ContextMenu();
            //MenuItem rename = new MenuItem();
            //rename.Header = "Rename...";
            //rename.Click += new System.Windows.RoutedEventHandler(rename_Click);
            //ContextMenu.Items.Add(rename);
        }

        public override bool IsReadOnly
        {
            get
            {
                return base.IsReadOnly;
            }
            set
            {
                base.IsReadOnly = value;
                if (ContextMenu != null)
                    ContextMenu.IsEnabled = !value;
            }
        }

        void txtName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                this.Focus();
            }
        }

        void txtName_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            string oldname = _uio.Name;
            _uio.SetName(txtName.Text);
            txtName.Visibility = System.Windows.Visibility.Hidden;
            if (UndoProvider != null)
                UndoProvider.Add(new UndoRedo.ChangeUserText(this, oldname, txtName.Text, _uio.SetName));
        }

        void rename_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtName.Text = _uio.Name;
            txtName.Visibility = System.Windows.Visibility.Visible;
            txtName.Focus();
            txtName.SelectAll();
        }

        void _ui_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetFill();
        }

        protected void SetFill()
        {
            if (_uio.Value && _r.IsMouseOver)
                _r.Fill = Brushes.Pink;
            if (_uio.Value && !_r.IsMouseOver)
                _r.Fill = Brushes.Red;

            if (!_uio.Value && _r.IsMouseOver)
                _r.Fill = Brushes.Blue;
            if (!_uio.Value && !_r.IsMouseOver)
                _r.Fill = Brushes.Beige;

        }

        public override Gate CreateUserInstance()
        {
            UserIO g = base.CreateUserInstance() as UserIO;
            ((Gates.IOGates.UserIO)g.AbGate).SetName(_uio.Name);
            return g;
        }

        public string UserIOLabel
        {
            set {
                txtName.Text = value;
                _uio.SetName(txtName.Text);
            }
            get { return txtName.Text; }
        }
    }

    public class UserInput : UserIO
    {
        public UserInput() : this(new Gates.IOGates.UserInput()) { }

        public UserInput(Gates.IOGates.UserInput gate)
            : base(gate,new TerminalID[] { new TerminalID(false, 0 , Position.RIGHT)})
        {
            Rectangle r = new Rectangle();
            r.Margin = new System.Windows.Thickness(15);
            r.Width = 34;
            r.Height = 34;
            r.Stroke = Brushes.Black;
            r.StrokeThickness = 2;
            r.Fill = Brushes.White;
            myCanvas.Children.Add(r);

            _r = new Rectangle();
            _r.Margin = new System.Windows.Thickness(20);
            _r.Width = 24;
            _r.Height = 24;
            _r.Stroke = Brushes.Black;
            _r.StrokeThickness = 2;
            SetFill();

            _r.MouseEnter += new System.Windows.Input.MouseEventHandler(r_MouseEnter);
            _r.MouseLeave += new System.Windows.Input.MouseEventHandler(r_MouseLeave);
            _r.MouseDown += new System.Windows.Input.MouseButtonEventHandler(r_MouseDown);

            myCanvas.Children.Add(_r);
            myCanvas.Children.Add(txtName);
        }

        public void r_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsReadOnly)
                _uio.Value = !_uio.Value;
        }

        void r_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsReadOnly)
                SetFill();
        }

        void r_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsReadOnly)
                SetFill();
        }
    }

    public class UserOutput : UserIO
    {
        public UserOutput() : this(new Gates.IOGates.UserOutput()) { }

        public UserOutput(Gates.IOGates.UserOutput gate)
            : base(gate,
           new TerminalID[] { 
                new TerminalID(true, 0 , Position.LEFT)})
        {
            Ellipse r = new Ellipse();
            r.Margin = new System.Windows.Thickness(15);
            r.Width = 34;
            r.Height = 34;
            r.Stroke = Brushes.Black;
            r.StrokeThickness = 2;
            r.Fill = Brushes.White;
            myCanvas.Children.Add(r);

            _r = new Ellipse();
            _r.Margin = new System.Windows.Thickness(20);
            _r.Width = 24;
            _r.Height = 24;
            _r.Stroke = Brushes.Black;
            _r.StrokeThickness = 2;
            SetFill();

            myCanvas.Children.Add(_r);
            myCanvas.Children.Add(txtName);

        }
    }
}
