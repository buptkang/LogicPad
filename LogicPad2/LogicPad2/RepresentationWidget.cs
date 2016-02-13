using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace LogicPad2
{
    public class RepresentationWidget : Widget
    {
        public RepresentationWidget(LogicCanvasType type)
            : base(type)
        {
            Bitmap image = null;
            if (LogicCanvasType.Expression.Equals(type))
            {
                image = LogicPad2.Properties.Resources.Expression;
            }
            else if (LogicCanvasType.Diagram.Equals(type))
            { 
                image = LogicPad2.Properties.Resources.Diagram;
            }
            else if (LogicCanvasType.TruthTable.Equals(type))
            {
                image = LogicPad2.Properties.Resources.TruthTable;
            }
            else { 
            }

            //Funny Story to convert GDI to new Media Interface
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
            imageControl.Source = bi;
            
            widgetCanvas.Children.Add(imageControl);
        }
    }

    public class TruthTableRep : RepresentationWidget
    {
        public TruthTableRep() : this(LogicCanvasType.TruthTable) { }
        public TruthTableRep(LogicCanvasType type)
            : base(LogicCanvasType.TruthTable)
        { }
    }

    public class DiagramRep : RepresentationWidget
    {
        public DiagramRep() : this(LogicCanvasType.Diagram) { }
        public DiagramRep(LogicCanvasType type)
            : base(LogicCanvasType.Diagram)
        { }
    }

    public class ExpressionRep : RepresentationWidget
    { 
        public ExpressionRep() : this(LogicCanvasType.Expression) {}
        public ExpressionRep(LogicCanvasType type)
            : base(LogicCanvasType.Expression)
        { }
    }
}
