using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPad2.Diagram.UndoRedo
{
    public interface IUndoable
    {
        void Undo();

        void Redo();

        bool CanUndo();

        bool CanRedo();

        string Name { get; }
    }
}