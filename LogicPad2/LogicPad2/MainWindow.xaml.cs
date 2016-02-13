using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using System.ComponentModel;
using LogicPad2.Diagram;
using SequenceGenerator;

namespace LogicPad2
{
    public partial class MainWindow : Window
    {
        //By default, touch-based interaction is disabled
        private bool _touchEnabled;
        public bool TouchEnabled {
            set { 
                _touchEnabled = value;
                if (_touchEnabled)
                    widgetSelector.Visibility = Visibility.Visible;
                else
                    widgetSelector.Visibility = Visibility.Collapsed;
            }
            get { return _touchEnabled; }
        }

        public bool isTestMode { set; get; }

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;

        private Diagram.UserControl1 diagramUserControl;

        public MainWindow()
        {
            InitializeComponent();

            TouchEnabled = false;
            mainInkCanvas.TouchEnabled = false;
            toutchMenu.Visibility = Visibility.Collapsed;
            
            //Notification from MainInkCanvas
            mainInkCanvas.ChangeMainMenuRepresentationOptions += new ChangeMenuRepresentationHandler(mainInkCanvas_ChangeMainMenuRepresentationOptions);

            DragDrop.LogicPadDragDropHelper.ItemDropped +=new EventHandler<DragDrop.DragDropEventArgs>(LogicPadDragDropHelper_ItemDropped);

            isTestMode = false;
            practiceMenu.IsEnabled = false;

            //Ad-hoc: LogicPad Diagram User Study Initialization
            
            Diagram.UserControl1 diagramUserControl1 = new Diagram.UserControl1();
            diagramUserControl1.UserControlX = 25;
            diagramUserControl1.UserControlY = 5;
            diagramUserControl1.UserControlScaleX = 1.4;
            diagramUserControl1.UserControlScaleY = 1.4;
            mainInkCanvas.Children.Add(diagramUserControl1);

            diagramUserControl = diagramUserControl1;

            diagramUserControl1.disableInkHandler += new DisableInkHandler(diagramUserControl1_disableInkCanvasInkHandler);

            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }


        #region stopwatcher

        void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            stopWatch.Start();
            dt.Start();

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                SaveTimeInformation(UserId, CurrentTask, currentTime);
                stopWatch.Reset();

                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
            }
        }

        private void SaveTimeInformation(int _userid, SequenceTask.Task _currentTask, string _currentTime)
        {
            var sb = new StringBuilder();
            sb.Append("User-").Append(_userid.ToString()).Append("-").Append("Timer.txt");
            string timefileName = sb.ToString();

            sb = new StringBuilder();
            sb.Append("UI: ").Append(_currentTask.UserInterface.ToString()).Append(", Difficulty: ").Append(
                _currentTask.DiagramComplexity.ToString()).Append(", Case Nmbr: ").Append(
                    _currentTask.CaseNmbr.ToString()).Append(", Time: ").Append(_currentTime).AppendLine();
            string record = sb.ToString();

            File.AppendAllText(timefileName, record);
        }

        #endregion

        void diagramUserControl1_disableInkCanvasInkHandler(object sender, DisableInkEventArgs e)
        {
            if (e.IsDisabled)
                mainInkCanvas.EditingMode = InkCanvasEditingMode.None;
            else
                mainInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void LogicPadDragDropHelper_ItemDropped(object sender, DragDrop.DragDropEventArgs e)
        {
            SubCanvasEventArgs args;
            Rect rect = new Rect(e.position.X, e.position.Y, 1, 1);
            if (e.Content is TruthTableRep)
            {
                args = new SubCanvasEventArgs(LogicCanvasType.TruthTable, rect);
                this.mainInkCanvas.GenerateSubInkCanvas(sender, args);
            }
            else if (e.Content is ExpressionRep)
            { 
                args = new SubCanvasEventArgs(LogicCanvasType.Expression, rect);
                this.mainInkCanvas.GenerateSubInkCanvas(sender, args);
            }
            else if (e.Content is DiagramRep)
            {
                args = new SubCanvasEventArgs(LogicCanvasType.Diagram, rect);
                this.mainInkCanvas.GenerateSubInkCanvas(sender, args);
            }
            else {
                
            }
            
        }

        void mainInkCanvas_ChangeMainMenuRepresentationOptions(object sender, MenuEventArgs args)
        {
            switch (args.CurrentEditingCanvas)
            {
                case LogicCanvasType.Diagram:
                    LogicPad2.Diagram.UserControl1 userControl = args.CurrentEditingUserControl as LogicPad2.Diagram.UserControl1;
                    if (userControl.IsExpressionRepreVisible)
                    {
                        menuExpression.IsEnabled = false;
                    }
                    else {
                        menuExpression.IsEnabled = true;
                    } 
                    menuDiagram.IsEnabled = false;
                    if (userControl.IsTruthTableRepreVisible)
                    {
                        menuTruthTable.IsEnabled = false;
                    }
                    else {
                        menuTruthTable.IsEnabled = true;    
                    }
                    break;
                case LogicCanvasType.Expression:
                    LogicPad2.Expression.UserControl1 userControl2 = args.CurrentEditingUserControl as LogicPad2.Expression.UserControl1; 
                    menuExpression.IsEnabled = false;
                    if (userControl2.IsDiagramRepreVisible)
                    {
                        menuDiagram.IsEnabled = false;
                    }
                    else {
                        menuDiagram.IsEnabled = true;
                    }
                    if (userControl2.IsTruthTableRepreVisible)
                    {
                        menuTruthTable.IsEnabled = false;
                    }
                    else {
                        menuTruthTable.IsEnabled = true;
                    }
                    break;
                case LogicCanvasType.TruthTable:
                    LogicPad2.TruthTable.UserControl1 userControl3 = args.CurrentEditingUserControl as LogicPad2.TruthTable.UserControl1;
                    if (userControl3.IsExpressionRepreVisible)
                    {
                        menuExpression.IsEnabled = false;
                    }
                    else{
                        menuExpression.IsEnabled = true;
                    }
                    if (userControl3.IsDiagramRepreVisible)
                    {
                        menuDiagram.IsEnabled = false;
                    }
                    else {
                        menuDiagram.IsEnabled = true;
                    }                    
                    menuTruthTable.IsEnabled = false;
                    break;
                case LogicCanvasType.Main:
                    menuExpression.IsEnabled = false;
                    menuDiagram.IsEnabled = false;
                    menuTruthTable.IsEnabled = false;
                    break;
                default:
                    menuExpression.IsEnabled = false;
                    menuDiagram.IsEnabled = false;
                    menuTruthTable.IsEnabled = false;
                    break;
            }
        }

        private void Window_ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GenerateRrepresentation(object sender, RoutedEventArgs e)
        {
            MenuItem clickedItem;
            clickedItem = e.Source as MenuItem;

            if(this.mainInkCanvas.CurrentDrawingControl == LogicCanvasType.Diagram)
            {
                #region Diagram
                string generatedExpr = null;
                TruthTableWindow.TruthTable truthTable = null;
                LogicParser1.Instance.ParseDiagramUserControl(mainInkCanvas.CurrentEditingUserControl as LogicPad2.Diagram.UserControl1,
                    out generatedExpr, out truthTable);

                LogicPad2.Diagram.UserControl1 diagramUserControl = mainInkCanvas.CurrentEditingUserControl as LogicPad2.Diagram.UserControl1;

                Rect userControlRect = new Rect(diagramUserControl.UserControlX,
                                                diagramUserControl.UserControlY,
                                                diagramUserControl.UserControlWidth,
                                                diagramUserControl.UserControlHeight);
               
                if (("menuExpression").Equals(clickedItem.Name))
                {
                    if (generatedExpr != null)
                    {
                        ExpressionWindow.ExpressionRepresentation expressionRepresentation = new ExpressionWindow.ExpressionRepresentation(generatedExpr);
                        expressionRepresentation.UserControlX = userControlRect.TopRight.X + 50;
                        expressionRepresentation.UserControlY = userControlRect.TopRight.Y;
                        this.mainInkCanvas.Children.Add(expressionRepresentation);
                        diagramUserControl.IsExpressionRepreVisible = true;
                        menuExpression.IsEnabled = false;
                        diagramUserControl.ExpressionRepr = expressionRepresentation;
                        expressionRepresentation.Owner = diagramUserControl;
                    }
                    else {
                        MessageBox.Show("Parsing Error!!!");
                    }
 
                }else if(("menuTruthTable").Equals(clickedItem.Name))
                {
                    TruthTableWindow.TruthTableRepresentation truthTableRepresentation = new TruthTableWindow.TruthTableRepresentation(truthTable);
                    truthTableRepresentation.UserControlX = userControlRect.BottomRight.X + 50;
                    truthTableRepresentation.UserControlY = userControlRect.BottomRight.Y;
                    this.mainInkCanvas.Children.Add(truthTableRepresentation);
                    diagramUserControl.IsTruthTableRepreVisible = true;
                    menuTruthTable.IsEnabled = false;
                    diagramUserControl.TruthTablePepr = truthTableRepresentation;
                    truthTableRepresentation.Owner = diagramUserControl;
                }
                #endregion
            }
            else if(this.mainInkCanvas.CurrentDrawingControl == LogicCanvasType.Expression)
            {
                #region Expression

                TruthTableWindow.TruthTable truthTable = null;
                XElement _expressionXElementTemp = null;
                int output = LogicParser1.Instance.ParseExpressionUserControl(mainInkCanvas.CurrentEditingUserControl as LogicPad2.Expression.UserControl1,
                    out truthTable, out _expressionXElementTemp);

                if (output != 2)
                {
                    MessageBox.Show("Special output: " + output);
                    return;
                }
                
                LogicPad2.Expression.UserControl1 expressionUserControl = mainInkCanvas.CurrentEditingUserControl as LogicPad2.Expression.UserControl1;

                Rect userControlRect = new Rect(expressionUserControl.UserControlX,
                                                expressionUserControl.UserControlY,
                                                expressionUserControl.UserControlWidth,
                                                expressionUserControl.UserControlHeight);

                if (_expressionXElementTemp!=null)
                    _expressionXElementTemp.Save("Temp.gcg");

                if(("menuDiagram").Equals(clickedItem.Name))
                {
                    GatesWpf.DiagramRepresentation diagramRepresentation = new GatesWpf.DiagramRepresentation();
                    diagramRepresentation.loadXMLElement("Temp.gcg");

                    diagramRepresentation.UserControlX = userControlRect.BottomRight.X + 50;
                    diagramRepresentation.UserControlY = userControlRect.BottomRight.Y;
                    this.mainInkCanvas.Children.Add(diagramRepresentation);
                    expressionUserControl.IsDiagramRepreVisible = true;
                    menuDiagram.IsEnabled = false;
                    expressionUserControl.DiagramRepr = diagramRepresentation;
                    diagramRepresentation.Owner = expressionUserControl;

                    diagramRepresentation.Loaded += delegate
                    {
                        diagramRepresentation.btnFitToScreen_Click(diagramRepresentation, new RoutedEventArgs());
                    };
                    
                }else if(("menuTruthTable").Equals(clickedItem.Name))
                {
                    TruthTableWindow.TruthTableRepresentation truthTableRepresentation = new TruthTableWindow.TruthTableRepresentation(truthTable);

                    truthTableRepresentation.UserControlX = userControlRect.TopRight.X + 50;
                    truthTableRepresentation.UserControlY = userControlRect.TopRight.Y;
                    this.mainInkCanvas.Children.Add(truthTableRepresentation);
                    expressionUserControl.IsTruthTableRepreVisible = true;
                    menuTruthTable.IsEnabled = false;
                    expressionUserControl.TruthTablePepr = truthTableRepresentation;
                    truthTableRepresentation.Owner = expressionUserControl;
                }
                #endregion
            }
            else if(this.mainInkCanvas.CurrentDrawingControl == LogicCanvasType.TruthTable)
            {
                #region TruthTable
                string generatedExpr = null;
                XElement _truthTableXElementTemp = null;
                int output = LogicParser1.Instance.ParseTruthTableUserControl(mainInkCanvas.CurrentEditingUserControl as LogicPad2.TruthTable.UserControl1,
                    out generatedExpr, out _truthTableXElementTemp);

                if (output != 2)
                {
                    MessageBox.Show("Special output: " + output);
                    return;
                }

                LogicPad2.TruthTable.UserControl1 truthTableUserControl = mainInkCanvas.CurrentEditingUserControl as LogicPad2.TruthTable.UserControl1;

                Rect userControlRect = new Rect(truthTableUserControl.UserControlX,
                                                truthTableUserControl.UserControlY,
                                                truthTableUserControl.UserControlWidth, 
                                                truthTableUserControl.UserControlHeight);
   
                if(_truthTableXElementTemp!=null)
                    _truthTableXElementTemp.Save("Temp.gcg");
                
                if(("menuDiagram").Equals(clickedItem.Name))
                {
                    if (_truthTableXElementTemp != null)
                    {
                        GatesWpf.DiagramRepresentation diagramRepresentation = new GatesWpf.DiagramRepresentation();                 
                        diagramRepresentation.loadXMLElement("Temp.gcg");

                        diagramRepresentation.UserControlX = userControlRect.BottomRight.X + 50;
                        diagramRepresentation.UserControlY = userControlRect.BottomRight.Y;
                        this.mainInkCanvas.Children.Add(diagramRepresentation);
                        truthTableUserControl.IsDiagramRepreVisible = true;
                        menuDiagram.IsEnabled = false;
                        truthTableUserControl.DiagramRepr = diagramRepresentation;
                        diagramRepresentation.Owner = truthTableUserControl;

                        diagramRepresentation.Loaded += delegate
                        {
                            diagramRepresentation.btnFitToScreen_Click(diagramRepresentation, new RoutedEventArgs());
                        };
                        
                    }
                    else {
                        MessageBox.Show("Parsing Error");
                    }
                }else if(("menuExpression").Equals(clickedItem.Name))
                {
                    if (generatedExpr != null)
                    {
                        ExpressionWindow.ExpressionRepresentation expressionRepresentation = new ExpressionWindow.ExpressionRepresentation(generatedExpr);
                        
                        expressionRepresentation.UserControlX = userControlRect.TopRight.X + 50;
                        expressionRepresentation.UserControlY = userControlRect.TopRight.Y;
                        this.mainInkCanvas.Children.Add(expressionRepresentation);
                        truthTableUserControl.IsExpressionRepreVisible = true;
                        menuExpression.IsEnabled = false;
                        truthTableUserControl.ExpressionRepr = expressionRepresentation;
                        expressionRepresentation.Owner = truthTableUserControl;
                    }else{
                        MessageBox.Show("Parsing Error");
                    }
                }
                #endregion
            }
        }

        private void AboutLogicPad(object sender, RoutedEventArgs e)
        {
            AboutDialogWindow aboutWindow = new AboutDialogWindow();
            aboutWindow.ShowDialog();
        }

        private void IsTouchEnabled(object sender, RoutedEventArgs e)
        {
            if (touchOption.IsChecked)
            {
                touchOption.IsChecked = false;
                TouchEnabled = false;
                mainInkCanvas.TouchEnabled = false;
            }
            else {
                touchOption.IsChecked = true;
                TouchEnabled = true;
                mainInkCanvas.TouchEnabled = true;
            }
        }

        private void StylusEnterWidget(object sender, StylusEventArgs e)
        {
            widgetSelector.Height = 140;
        }

        private void StylusLeaveWidget(object sender, StylusEventArgs e)
        {            
            this.Dispatcher.BeginInvoke(new ThreadStart(() => {
                 System.Threading.Thread.Sleep(100);
                 widgetSelector.Height = 20;
            })); 
        }

        #region Scrolling Control

        private void Scroller_MainCanvasLayoutUpdated(object sender, EventArgs e)
        {
            SizeCanvas();
        }

        private void SizeCanvas()
        {
            double max1 = MainGCScroller.ViewportWidth, max2 = MainGCScroller.ViewportHeight;
            double maxx = mainInkCanvas.ActualWidth, maxy = mainInkCanvas.ActualHeight;
            foreach (UserControl userControl in mainInkCanvas.Children)
            {
                if (userControl is Diagram.UserControl1)
                {
                    Diagram.UserControl1 diagramUserControl = userControl as Diagram.UserControl1;
                    maxx = Math.Max(maxx, diagramUserControl.UserControlX + diagramUserControl.UserControlWidth + 200);
                    maxy = Math.Max(maxy, diagramUserControl.UserControlY + diagramUserControl.UserControlHeight + 200);
                }
                else if (userControl is Expression.UserControl1)
                {
                    Expression.UserControl1 expressionUserControl = userControl as Expression.UserControl1;
                    maxx = Math.Max(maxx, expressionUserControl.UserControlX + expressionUserControl.UserControlWidth + 200);
                    maxy = Math.Max(maxy, expressionUserControl.UserControlY + expressionUserControl.UserControlHeight + 200);
                }
                else if (userControl is TruthTable.UserControl1)
                {
                    TruthTable.UserControl1 truthTableUserControl = userControl as TruthTable.UserControl1;
                    maxx = Math.Max(maxx, truthTableUserControl.UserControlX + truthTableUserControl.UserControlWidth + 200);
                    maxy = Math.Max(maxy, truthTableUserControl.UserControlY + truthTableUserControl.UserControlHeight + 200);
                }
                else if (userControl is ExpressionWindow.ExpressionRepresentation)
                {
                    ExpressionWindow.ExpressionRepresentation expressionRep = userControl as ExpressionWindow.ExpressionRepresentation;
                    maxx = Math.Max(maxx, expressionRep.UserControlX + expressionRep.UserControlWidth + 200);
                    maxy = Math.Max(maxy, expressionRep.UserControlY + expressionRep.UserControlHeight + 200);
                }
                else if (userControl is TruthTableWindow.TruthTableRepresentation)
                {
                    TruthTableWindow.TruthTableRepresentation truthTableRep = userControl as TruthTableWindow.TruthTableRepresentation;
                    maxx = Math.Max(maxx, truthTableRep.UserControlX + truthTableRep.UserControlWidth + 200);
                    maxy = Math.Max(maxy, truthTableRep.UserControlY + truthTableRep.UserControlHeight + 200);
                }
                else if (userControl is GatesWpf.DiagramRepresentation)
                {
                    GatesWpf.DiagramRepresentation diagramRep = userControl as GatesWpf.DiagramRepresentation;
                    maxx = Math.Max(maxx, diagramRep.UserControlX + diagramRep.UserControlWidth + 200);
                    maxy = Math.Max(maxy, diagramRep.UserControlY + diagramRep.UserControlHeight + 200);
                }
            }

            mainInkCanvas.Width = maxx;
            mainInkCanvas.Height = maxy;
            
        }

        #endregion

        #region User Study

        public void LoadFirstDiagram()
        {
            SequenceGenerator.SequenceTask.Task task = SequenceTask.First();
            CurrentTask = task;
            showingContent.Content = "Diagram Complexity: " + task.DiagramComplexity.ToString() + "       Case" + task.CaseNmbr;
            sketchUserStudyPanel.Visibility = Visibility.Visible;
            SequenceTask.Remove(task);
            RemoveTimerFile();
        }

        private void RemoveTimerFile()
        {
            var sb = new StringBuilder();
            sb.Append("User-").Append(UserId.ToString()).Append("-").Append("Timer.txt");
            string timefileName = sb.ToString();

            if (File.Exists(timefileName))
            {
                File.Delete(timefileName);
            }
        }


        private void ClickRecognize(object sender, RoutedEventArgs e)
        {

            if(isTestMode)
            {
                MessageBox.Show("Recognition Result: " + CurrentTask.RealEquation);
            }
            else
            {
                MessageBox.Show("Recognition Result: " + "F = (x+y)(xy)'");
            }
            /*
            if (CurrentTask.CaseMatched)
                MessageBox.Show("Drawing logic gate diagram match with the above equation!!!");
            else
            {
                MessageBox.Show("Drawing logic gate diagram does not match with the above equation!!!");
            }
             * */
            
        }

        private void ClickNext(object sender, RoutedEventArgs e)
        {
            if (SequenceTask.Count != 0)
            {
                SequenceGenerator.SequenceTask.Task task = SequenceTask.First();
                CurrentTask = task;
                showingContent.Content = "Diagram Complexity: " + task.DiagramComplexity.ToString() + "       Case" + task.CaseNmbr; ;
                SequenceTask.Remove(task);
               
                //Delete ink Analysis strokes
                this.mainInkCanvas.InkAnalyzer.RemoveStrokes(this.mainInkCanvas.Strokes);
                this.mainInkCanvas.Strokes.Clear();

                //Clear diagram canvas
                diagramUserControl.DiagramInkCanvas.Children.Clear();
                diagramUserControl.DiagramInkCanvas.Strokes.Clear();
            }
            else
            {
                MessageBox.Show("Done for tasks");
            }
        }

        private void ClickUserId(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.UserId.ToString());
        }

        public int UserId { set; get; }
        public List<SequenceTask.Task> SequenceTask { set; get; }

        public SequenceTask.Task CurrentTask { set; get; }

        #endregion

        private void ClickModelSelection(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            if (mi.Name.Equals("practiceMenu"))
            {
                this.mainInkCanvas.Strokes.Clear();

                practiceMenu.IsEnabled = false;
                testMenu.IsEnabled = true;

                showingContent.Content = null;
                sketchUserStudyPanel.Visibility = Visibility.Collapsed;

                isTestMode = false;

                UserId = 0;
            }
            else
            {
                this.mainInkCanvas.Strokes.Clear();

                practiceMenu.IsEnabled = true;
                testMenu.IsEnabled = false;

                isTestMode = true;

                UserInputWindow userInput = new UserInputWindow();
                userInput.Owner = this;
                userInput.ShowDialog();
            }
        }
    }    
}
