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

using System.Windows.Ink;

namespace InkCanvas
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
			
			System.Windows.Ink.DrawingAttributes inkAttribute =  new System.Windows.Ink.DrawingAttributes();
			
			inkAttribute.Height = 10; 
			inkAttribute.Width = 10;
			
			//DrawingBoard.DefaultDrawingAttributes = inkAttribute;
		}

		private void StylusMoving(object sender, System.Windows.Input.StylusEventArgs e)
		{
			// TODO: Add event handler implementation here.
			//StylusPointCollection originalPoints = e.GetStylusPoints(DrawingBoard); 
  
			//float currentPressure = originalPoints[0].PressureFactor;
		}
	}
}