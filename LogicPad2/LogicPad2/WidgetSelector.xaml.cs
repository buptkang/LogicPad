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
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicPad2
{
    /// <summary>
    /// Interaction logic for WidgetSelector.xaml
    /// </summary>
    public partial class WidgetSelector : UserControl
    {
        public WidgetSelector()
        {
            InitializeComponent();

            diagramWidget.DataContext = new DiagramRep();
            expressionWidget.DataContext = new ExpressionRep();
            truthTableWidget.DataContext = new TruthTableRep();
        }
    }
}
