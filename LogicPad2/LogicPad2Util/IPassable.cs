using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Input;
using System.Windows.Controls;

namespace LogicPad2Util
{
    /*
     * This interface is used for passing parent inkCanavs event toward children inkcanvas
     * 
     */
    public interface IPassable
    {
        void StylusDown(object sender, StylusDownEventArgs e);
        void StylusMove(object sender, StylusEventArgs e);
        void StylusUp(object sender, StylusEventArgs e);
        void StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e);
        void PreviewStylusDown(object sender, StylusDownEventArgs e);
        void PreviewStylusMove(object sender, StylusEventArgs e);
        void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e);
        void PreviewMouseMove(object sender, MouseEventArgs e);
        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e);
        void PreviewMouseDown(object sender, MouseButtonEventArgs e);
    }
}
