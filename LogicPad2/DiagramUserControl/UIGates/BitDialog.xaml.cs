using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicPad2.Diagram.UIGates
{
    /// <summary>
    /// Interaction logic for BitDialog.xaml
    /// </summary>
    public partial class BitDialog : Window
    {
        protected int _bits;

        public BitDialog()
        {
            InitializeComponent();
            _bits = -1;
        }

        public int Bits
        {
            get {

                return _bits;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _bits = (int)bitSlider.Value;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _bits = -1;
            Close();
        }
    }
}
