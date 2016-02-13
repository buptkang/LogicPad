using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LogicPad2.Diagram.PieMenuUserControl
{
    public class GateTest : INotifyPropertyChanged
    {
        private string myClass;

        public string Class
        {
            get { return myClass; }
            set
            {
                myClass = value;
                RaisePropertyChangeEvent("Class");
            }
        }

        private double image;

        public double Image
        {
            get { return image; }
            set
            {
                image = value;
                RaisePropertyChangeEvent("Image");
            }
        }

        public static List<GateTest> ConstructTestData()
        {
            List<GateTest> assetClasses = new List<GateTest>();

            assetClasses.Add(new GateTest() { Class = "AND", Image = 1.0 });
            assetClasses.Add(new GateTest() { Class = "OR", Image = 1.0 });
            assetClasses.Add(new GateTest() { Class = "NOT", Image = 1.0 });
            assetClasses.Add(new GateTest() { Class = "XOR", Image = 1.0 });
            assetClasses.Add(new GateTest() { Class = "Input", Image = 1.0 });
            assetClasses.Add(new GateTest() { Class = "Output", Image = 1.0 });

            return assetClasses;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangeEvent(String propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
