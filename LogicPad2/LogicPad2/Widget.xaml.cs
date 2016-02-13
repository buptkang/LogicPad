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
    /// Interaction logic for Widget.xaml
    /// </summary>
    public partial class Widget : UserControl
    {
        private DropShadowEffect glow;

        public Widget(LogicCanvasType type)
        {
            InitializeComponent();
            ToolTip = type.ToString();

            glow = new DropShadowEffect();
            glow.ShadowDepth = 0;
            glow.Color = Colors.Blue;
            glow.BlurRadius = 5;
        }
    }
}
