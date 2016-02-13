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

using SequenceGenerator;

namespace LogicPad2
{
    /// <summary>
    /// Interaction logic for UserInputWindow.xaml
    /// </summary>
    public partial class UserInputWindow : Window
    {
        public UserInputWindow()
        {
            InitializeComponent();

            this.Closed += new EventHandler(UserInputWindow_Closed);
        }

        void UserInputWindow_Closed(object sender, EventArgs e)
        {
            MainWindow window = this.Owner as MainWindow;
            window.LoadFirstDiagram();
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32(textBox.Text);
            string text = SequenceGenerator.SequenceGenerator.Instance.GetUserSequence(index);
            //MessageBox.Show(text);

            MainWindow window = this.Owner as MainWindow;

            window.UserId = index;
            window.SequenceTask = SequenceGenerator.SequenceGenerator.Instance.GetTaskSequence(index, UserInterface.hybrid);

            this.Close();
        }
    }
}
