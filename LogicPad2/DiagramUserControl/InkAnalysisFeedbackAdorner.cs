﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;

using starPadSDK.WPFHelp;

namespace LogicPad2.Diagram
{
    public class InkAnalysisFeedbackAdorner : Adorner
    {
        private InkAnalyzer _inkAnalyzer;

        public InkAnalysisFeedbackAdorner(UIElement adornedElement, InkAnalyzer inkAnalyzer)
            : base(adornedElement)
        {
            _inkAnalyzer = inkAnalyzer;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.PushOpacity(0.45d);
            //recurse through the tree of results
            DrawFeedback(drawingContext, _inkAnalyzer.RootNode);

        }

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
