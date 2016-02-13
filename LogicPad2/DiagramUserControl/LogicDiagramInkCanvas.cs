using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Ink;

namespace LogicPad2.Diagram
{
    public class LogicDiagramInkCanvas : InkCanvas
    {
        private bool _showInkAnalysisFeedback = true;
        /// <summary>
        /// The private AdornerDecorator InkCanvas uses to render selection feedback.
        /// We use it to display feedback for InkAnalysis
        /// </summary>
        private AdornerDecorator _adornerDecorator = null;
        private InkAnalysisFeedbackAdorner _feedbackAdorner = null;

        private InkAnalyzer _inkAnalyzer;


        public LogicDiagramInkCanvas()
        {
            _inkAnalyzer = new InkAnalyzer(this.Dispatcher);

            this.ShowInkAnalysisFeedback = true;
        }

        public InkAnalyzer InkAnalyzer
        {
            get { return _inkAnalyzer; }
        }

        public AdornerDecorator AdornerDecorator
        {
            get { return _adornerDecorator; }
        }

        public InkAnalysisFeedbackAdorner InkFeedbackAdorner
        {
            get { return _feedbackAdorner; }
        }

        public bool ShowInkAnalysisFeedback
        {
            get { return _showInkAnalysisFeedback; }
            set
            {
                _showInkAnalysisFeedback = value;

                if (_adornerDecorator == null)
                {

                    Console.Write(this.VisualChildrenCount);
                    //We want to adorn the InkCanvas's inner canvas with an adorner 
                    //that we use to display the parse and recognition results
                    _adornerDecorator = (AdornerDecorator)GetVisualChild(0);

                    DependencyObject inkPresenter = VisualTreeHelper.GetChild(_adornerDecorator, 0);
                    DependencyObject innerCanvas = VisualTreeHelper.GetChild(inkPresenter, 0);

                    _feedbackAdorner = new InkAnalysisFeedbackAdorner((UIElement)innerCanvas, _inkAnalyzer);
                    _adornerDecorator.AdornerLayer.Add(_feedbackAdorner);
                }

                //if (_showInkAnalysisFeedback)
                //{
                //    _feedbackAdorner.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    _feedbackAdorner.Visibility = Visibility.Collapsed;
                //}
                _feedbackAdorner.Visibility = Visibility.Collapsed;
            }
        }
       
    }
}
