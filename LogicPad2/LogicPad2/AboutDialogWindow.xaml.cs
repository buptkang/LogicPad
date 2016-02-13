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
using System.Windows.Shapes;

namespace LogicPad2
{
    /// <summary>
    /// Interaction logic for AboutDialogWindow.xaml
    /// </summary>
    public partial class AboutDialogWindow : Window
    {
        public AboutDialogWindow()
        {
            InitializeComponent();
        }

        private void NavigateTOISUE(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
