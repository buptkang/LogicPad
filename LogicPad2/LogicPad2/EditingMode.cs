using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPad2
{
    public class EditingMode
    {
        public static EditingModeType currentEditingMode = EditingModeType.None;
        
        public enum EditingModeType
        {
            SketchLogicGate,
            AnnotateLogicGate,
            SketchMathExp,
            None
        }
    }
}
