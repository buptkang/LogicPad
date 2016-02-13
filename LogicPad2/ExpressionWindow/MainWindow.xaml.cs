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

namespace ExpressionWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly double GAP = 55.0;

        public MainWindow(String Output)
        {
            InitializeComponent();

            Rect b = new Rect(GAP * 3, GAP, GAP, GAP);
            Point center = new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
            Brush color = Brushes.Black;
            InkFigure expression = new InkFigure(Output, b, center, color);
            
            //RenderCanvas.Children.Add(CreateInkRegion(b));
            RenderCanvas.Children.Add(expression);
        }
        
        public Canvas RenderCanvas
        {
            get { return expressionRenderer; }
            set { expressionRenderer = value; }
        }
    }
}
