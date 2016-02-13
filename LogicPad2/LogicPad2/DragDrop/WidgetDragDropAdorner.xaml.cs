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

namespace LogicPad2.DragDrop
{
    /// <summary>
    /// Interaction logic for WidgetDragDropAdorner.xaml
    /// </summary>
    public partial class WidgetDragDropAdorner : LogicPadDragDropAdornerBase
    {
        public WidgetDragDropAdorner()
        {
            InitializeComponent();
        }

        public override void StateChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WidgetDragDropAdorner myclass = (WidgetDragDropAdorner)d;
            switch ((DropState)e.NewValue)
            { 
                case DropState.CanDrop:
                    myclass.back.Stroke = Resources["canDropBrush"] as SolidColorBrush;
                    myclass.indicator.Source = Resources["dropIcon"] as DrawingImage;
                    break;
                case DropState.CannotDrop:
                    myclass.back.Stroke = Resources["solidRed"] as SolidColorBrush;
                    myclass.indicator.Source = Resources["noDropIcon"] as DrawingImage;
                    break;
            }

        }
    }
}
