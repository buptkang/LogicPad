using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace LogicPad2
{
    public enum LogicCanvasType
    {
        Expression, Diagram, TruthTable, EqualSign, Question, Main
    };

    public class SubCanvasEventArgs : EventArgs
	{
		private LogicCanvasType _logicCanvasType;

        public LogicCanvasType LogicCanvasType
        {
          get { return _logicCanvasType; }
          set { _logicCanvasType = value; }
        }

        private Rect _central;

        public Rect Central
        {
          get { return _central; }
          set { _central = value; }
        }

        private StrokeCollection _strokes;

        public StrokeCollection MyStrokes
        {
            get { return _strokes; }
            set { _strokes = value; }
        }

        public SubCanvasEventArgs(LogicCanvasType type, Rect central)
        {
            this._logicCanvasType = type;
            this._central = central;
        }

		public SubCanvasEventArgs(LogicCanvasType type, Rect central, StrokeCollection strokes)
		{
			this._logicCanvasType = type;
            this._central = central;
            this._strokes = strokes;
		}		
	}

    public delegate void GenerateSubInkCanvasEventHandler(object sender, SubCanvasEventArgs args);

    public class InkAnalysisFeedbackAdorner : Adorner
    {
        public event GenerateSubInkCanvasEventHandler _subInkCanvasGenerated;

        private InkAnalyzer _inkAnalyzer;
        /// <summary>
        /// Constructor
        /// </summary>
        public InkAnalysisFeedbackAdorner(UIElement adornedElement, InkAnalyzer inkAnalyzer)
            : base(adornedElement)
        {
            _inkAnalyzer = inkAnalyzer;
        }

        /// <summary>
        /// OnRender
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.PushOpacity(0.45d);
            //recurse through the tree of results
            DrawFeedback(drawingContext, _inkAnalyzer.RootNode);
        }

        protected virtual void OnSubCanvasGenerated(SubCanvasEventArgs e)
        {
            if (_subInkCanvasGenerated != null)
            {
                _subInkCanvasGenerated(this, e);
            }
        }

        private void CheckOutputValue(StrokeCollection myStrokes, String recognizedCharacter)
        {
            Rect rect = myStrokes.GetBounds();

            SubCanvasEventArgs args;

            switch(recognizedCharacter)
            {
                case "T":                    
                    args = new SubCanvasEventArgs(LogicCanvasType.TruthTable, rect, myStrokes);
                    OnSubCanvasGenerated(args);
                    break;
                case "e":                  
                    args = new SubCanvasEventArgs(LogicCanvasType.Expression, rect, myStrokes);
                    OnSubCanvasGenerated(args);
                    break;
                case "D":
                    args = new SubCanvasEventArgs(LogicCanvasType.Diagram, rect, myStrokes);
                    OnSubCanvasGenerated(args);           
                    break;
                case "d":
                    args = new SubCanvasEventArgs(LogicCanvasType.Diagram, rect, myStrokes);
                    OnSubCanvasGenerated(args);           
                    break;
                case "=":
                    args = new SubCanvasEventArgs(LogicCanvasType.EqualSign, rect, myStrokes);
                    OnSubCanvasGenerated(args);
                    break;
                case "?":
                    args = new SubCanvasEventArgs(LogicCanvasType.Question, rect, myStrokes);
                    OnSubCanvasGenerated(args);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// InkAnalysis results form a tree, this method is called recursively
        /// to render each node in the tree.
        /// </summary>
        private void DrawFeedback(DrawingContext drawingContext, ContextNode contextNode)
        {
            //see what type of ContextNode this is by casting it

            Rect nodeBounds = contextNode.Strokes.GetBounds();
            InkWordNode inkWordNode = contextNode as InkWordNode;
            if (inkWordNode != null)
            {
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Blue, 1.0d), nodeBounds, 1d, 1d);
                drawingContext.DrawText(new FormattedText(inkWordNode.GetRecognizedString(),
                                                            CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Verdana"),
                                                            9.0d,
                                                            Brushes.Black),
                                        nodeBounds.BottomLeft);

                //check current output value
                CheckOutputValue(contextNode.Strokes, inkWordNode.GetRecognizedString());
                goto recurse;
            }

            InkDrawingNode inkDrawingNode = contextNode as InkDrawingNode;
            if (inkDrawingNode != null)
            {
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Purple, 1.0d), nodeBounds, 1d, 1d);
                drawingContext.DrawText(new FormattedText("Drawing: " + inkDrawingNode.GetShapeName(),
                                                            CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Verdana"),
                                                            9.0d,
                                                            Brushes.Black),
                                        nodeBounds.BottomLeft);
                CheckOutputValue(contextNode.Strokes, inkDrawingNode.GetShapeName());
                goto recurse;
            }

            InkBulletNode inkBulletNode = contextNode as InkBulletNode;
            if (inkBulletNode != null)
            {
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Green, 1.0d), nodeBounds, 1d, 1d);
                drawingContext.DrawText(new FormattedText(inkBulletNode.GetRecognizedString(),
                                            CultureInfo.CurrentCulture,
                                            FlowDirection.LeftToRight,
                                            new Typeface("Verdana"),
                                            9.0d,
                                            Brushes.Black),
                                        nodeBounds.BottomLeft);
                goto recurse;
            }

            WritingRegionNode writingRegionNode = contextNode as WritingRegionNode;
            if (writingRegionNode != null)
            {
                nodeBounds.Inflate(3d, 3d);
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Black, 1.0d), nodeBounds, 1d, 1d);
                drawingContext.DrawText(new FormattedText("Writing Region",
                                            CultureInfo.CurrentCulture,
                                            FlowDirection.LeftToRight,
                                            new Typeface("Verdana"),
                                            9.0d,
                                            Brushes.Black),
                                        nodeBounds.BottomLeft + new Vector(0, 3));
        
                goto recurse;
            }

            ParagraphNode paragraphNode = contextNode as ParagraphNode;
            if (paragraphNode != null)
            {
                nodeBounds.Inflate(2d, 2d); //inflate so this will be visible outside the line node
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Red, 1.0d), nodeBounds, 1d, 1d);
                goto recurse;
            }

            LineNode lineNode = contextNode as LineNode;
            if (lineNode != null)
            {
                nodeBounds.Inflate(1d, 1d); //inflate so this will be visible outside the word node
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Orange, 1.0d), nodeBounds, 1d, 1d);
                goto recurse;
            }

        recurse:
            foreach (ContextNode subNode in contextNode.SubNodes)
            {
                DrawFeedback(drawingContext, subNode);
            }
        }
    }
}
