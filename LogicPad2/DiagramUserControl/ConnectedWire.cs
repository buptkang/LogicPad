using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LogicPad2.Diagram
{
    public class ConnectedWire : Wire
    {
        private Gate.TerminalID origin, dest;
        private Gates.AbstractGate originGate, destGate;

        public Gate.TerminalID OriginTerminalID
        {
            get
            {
                return origin;
            }
        }

        public Gate.TerminalID DestTerminalID
        {
            get
            {
                return dest;
            }
        }

        public Gates.AbstractGate OriginGate
        {
            get
            {
                return originGate;
            }
        }

        public Gates.AbstractGate DestinationGate
        {
            get
            {
                return destGate;
            }
        }

        public ConnectedWire(Gates.AbstractGate originGate, Gate.TerminalID origin, Gates.AbstractGate destGate, Gate.TerminalID dest)
            :base()
        {
            if (origin.isInput || !dest.isInput)
            {
                throw new ArgumentException("Can only connect output (origin) to input (dest)");
            }

            Value = false;
            this.originGate = originGate;
            this.destGate = destGate;
            this.origin = origin;
            this.dest = dest;
            //originGate.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(originGate_PropertyChanged);
            originGate.PropertyChanged += EventDispatcher.CreateBatchDispatchedHandler(originGate, originGate_PropertyChanged);

            Connect();
        }

        private void originGate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        { 
            Value = originGate.Output[origin.ID];
        }

        public void Connect()
        {
            Origin = origin.t.TranslatePoint(new Point(5, 5), this);
            Destination = dest.t.TranslatePoint(new Point(5, 5), this);
        }
    }
}
