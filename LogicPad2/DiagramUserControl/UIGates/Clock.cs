using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;

using Gates;

namespace LogicPad2.Diagram.UIGates
{
    public class Clock : Gate
    {
        private Gates.IOGates.Clock _clock;
        private TextBox nval;

        public Clock() : this(new Gates.IOGates.Clock(0)) { }

        public Clock(Gates.IOGates.Clock gate)
            : base(gate, new TerminalID[] { new TerminalID(false, 0, Position.TOP) })
        {

            _clock = gate;



            Rectangle r = new Rectangle();
            r.Margin = new System.Windows.Thickness(5, 17, 5, 17);
            r.Width = this.Width - 10;
            r.Height = this.Height - 34;
            r.Stroke = Brushes.Black;
            r.StrokeThickness = 2;
            r.Fill = Brushes.White;
            myCanvas.Children.Add(r);

            Path ph = new Path();
            ph.Data = StreamGeometry.Parse("M 10,22 h 5 v 5 h -5 v 5 h 5 v 5 h -5 v 5 h 5");
            ph.Stroke = Brushes.Black;
            ph.StrokeThickness = 2;
            ph.Fill = Brushes.White;
            myCanvas.Children.Add(ph);

            nval = new TextBox();
            nval.Margin = new System.Windows.Thickness(20, 23, 10, 23);
            nval.FontFamily = new FontFamily("Courier New");
            nval.FontSize = 12;
            nval.TextAlignment = TextAlignment.Center;
            nval.Width = 34;
            nval.Height = 18;
            nval.Background = Brushes.AntiqueWhite;


            Binding bind = new Binding("Milliseconds");
            bind.Source = _clock;
            bind.FallbackValue = "0";
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.ValidatesOnExceptions = true;
            nval.SetBinding(TextBox.TextProperty, bind);


            Binding bindve = new Binding("(Validation.Errors)[0].Exception.InnerException.Message");
            bindve.Source = nval;
            bindve.Mode = BindingMode.OneWay;
            bindve.FallbackValue = "Clock period in milliseconds";
            nval.SetBinding(TextBox.ToolTipProperty, bindve);


            myCanvas.Children.Add(nval);


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
                if (nval != null)
                    nval.IsReadOnly = value;
            }
        }

        public override Gate CreateUserInstance()
        {
            return new Clock(new Gates.IOGates.Clock(_clock.Milliseconds));
        }
    }
}
