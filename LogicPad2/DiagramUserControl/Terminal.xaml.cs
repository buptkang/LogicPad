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

namespace LogicPad2.Diagram
{
    /// <summary>
    /// Interaction logic for Terminal.xaml
    /// </summary>
    public partial class Terminal : UserControl
    {
        private bool _high = false;

        public Terminal(bool isInput)
        {
            InitializeComponent();
            polyInput.Visibility = isInput ? Visibility.Visible : Visibility.Hidden;
            polyOutput.Visibility = (!isInput) ? Visibility.Visible : Visibility.Hidden;

        }


        /// <summary>
        /// Gets or sets the highlight state, usually used to indicate if this terminal is connectable.
        /// </summary>
        public bool Highlight
        {
            get
            {
                return _high;
            }
            set
            {
                _high = value;
                if (value)
                    elSelector.Fill = Brushes.LightGreen;
                else
                    elSelector.Fill = Brushes.White;
            }
        }

        private void setFill(bool value)
        {
            if (value)
            {
                polyInput.Fill = Brushes.Red;
                polyOutput.Fill = Brushes.Red;
            }
            else
            {
                polyInput.Fill = Brushes.DarkGray;
                polyOutput.Fill = Brushes.DarkGray;
            }
        }

        /// <summary>
        /// Sets the true/false value of this terminal.
        /// </summary>
        public bool Value
        {
            set
            {
                setFill(value);

            }
        }
    }
}
