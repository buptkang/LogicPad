using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Globalization;

using Gates;

namespace LogicPad2.Diagram.UIGates
{
    public class Comment : Gate
    {
        private TextBox nm;
        private void ResizeDueToName()
        {
            Gates.IOGates.Comment cmt = _gate as Gates.IOGates.Comment;

            FormattedText ft = new FormattedText(cmt.Value, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
               new Typeface(nm.FontFamily, nm.FontStyle, nm.FontWeight, nm.FontStretch),
               nm.FontSize, nm.Foreground);

            // I would assume 40 b/c that is the total below
            // for diff between label width and gate width
            // but it is not enough
            // hacking it
            this.Width = Math.Max(64, ft.Width + 50);

            string path = "M0,0 ";
            for (int i = 20; i < this.Width - 20; i += 9)
                path += "a 5,5 45 1 1 9,0 ";
            path += "a 5,5 45 1 1 9,0 ";

            for (int i = 20; i < this.Height - 20; i += 9)
                path += "a 5,5 45 1 1 0,9 ";
            path += "a 5,5 45 1 1 0,9 ";

            for (int i = 20; i < this.Width - 20; i += 9)
                path += "a 5,5 45 1 1 -9,0 ";
            path += "a 5,5 45 1 1 -9,0 ";

            for (int i = 20; i < this.Height - 20; i += 9)
                path += "a 5,5 45 1 1 0,-9 ";
            path += "a 5,5 45 1 1 0,-9 ";

            ph.Data = StreamGeometry.Parse(path);

            nm.Text = cmt.Value;
            nm.Width = this.Width - 40;
            nm.Height = this.Height - 40;
        }

        public Comment()
            : this(new Gates.IOGates.Comment())
        {

        }

        /// <summary>
        /// If an undo manager is provided, changes to the content
        /// of this comment will be undo-able.
        /// </summary>
        public UndoRedo.UndoManager UndoProvider { get; set; }

        private Path ph;
        private string prevcmt = "";

        public Comment(Gates.IOGates.Comment cmt)
            : base(cmt, new TerminalID[0])
        {

            ph = new Path();

            ph.Stroke = Brushes.Black;
            ph.StrokeThickness = 2;
            ph.Fill = Brushes.White;
            ph.Margin = new System.Windows.Thickness(15);
            myCanvas.Children.Add(ph);


            nm = new TextBox();
            nm.Text = cmt.Value;
            nm.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            nm.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            nm.Margin = new System.Windows.Thickness(20);
            nm.Width = this.Width - 40;
            nm.Height = this.Height - 40;
            nm.BorderThickness = new Thickness(0);

            myCanvas.Children.Add(nm);

            nm.TextChanged += (sender, e) =>
            {
                cmt.Value = nm.Text;
            };
            nm.LostFocus += (sender, e) =>
            {
                // because I don't want to create an undo event
                // for every single keypress!
                if (UndoProvider != null)
                    UndoProvider.Add(new UndoRedo.ChangeUserText(this, prevcmt, nm.Text, txt => { cmt.Value = txt; }));

                prevcmt = nm.Text;
            };
            cmt.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Value")
                    ResizeDueToName();

            };
            ResizeDueToName();


        }


        public override bool IsReadOnly
        {
            get
            {
                return nm.IsReadOnly;
            }
            set
            {
                base.IsReadOnly = value;
                if (nm != null)
                    nm.IsReadOnly = value;
            }
        }

        public override Gate CreateUserInstance()
        {
            Gates.IOGates.Comment cmt = _gate as Gates.IOGates.Comment;

            Comment c = new Comment();
            ((Gates.IOGates.Comment)c.AbGate).Value = cmt.Value;

            return c;
        }

    }
}
