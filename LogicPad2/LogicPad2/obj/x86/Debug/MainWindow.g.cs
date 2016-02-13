﻿#pragma checksum "..\..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3C4E54CE553736B0963AAFEBC1A4C265"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LogicPad2;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace LogicPad2 {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\MainWindow.xaml"
        internal LogicPad2.MainWindow logicPad2MainWindow;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.Border logicPadMenu;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.MenuItem menuExpression;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.MenuItem menuDiagram;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.MenuItem menuTruthTable;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.MenuItem practiceMenu;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.MenuItem testMenu;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.MenuItem touchOption;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.MenuItem about;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.StackPanel sketchUserStudyPanel;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.Label showingContent;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.StackPanel timer;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button StartButton;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button StopButton;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\MainWindow.xaml"
        internal LogicPad2.WidgetSelector widgetSelector;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.ScrollViewer MainGCScroller;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\MainWindow.xaml"
        internal LogicPad2.MSInkAnalysisCanvas mainInkCanvas;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.Canvas adornerLayer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LogicPad2;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.logicPad2MainWindow = ((LogicPad2.MainWindow)(target));
            return;
            case 2:
            this.logicPadMenu = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            
            #line 21 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.Window_ExitApp);
            
            #line default
            #line hidden
            return;
            case 4:
            this.menuExpression = ((System.Windows.Controls.MenuItem)(target));
            
            #line 26 "..\..\..\MainWindow.xaml"
            this.menuExpression.Click += new System.Windows.RoutedEventHandler(this.GenerateRrepresentation);
            
            #line default
            #line hidden
            return;
            case 5:
            this.menuDiagram = ((System.Windows.Controls.MenuItem)(target));
            
            #line 29 "..\..\..\MainWindow.xaml"
            this.menuDiagram.Click += new System.Windows.RoutedEventHandler(this.GenerateRrepresentation);
            
            #line default
            #line hidden
            return;
            case 6:
            this.menuTruthTable = ((System.Windows.Controls.MenuItem)(target));
            
            #line 32 "..\..\..\MainWindow.xaml"
            this.menuTruthTable.Click += new System.Windows.RoutedEventHandler(this.GenerateRrepresentation);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 39 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ClickRecognize);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 41 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ClickNext);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 43 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ClickUserId);
            
            #line default
            #line hidden
            return;
            case 10:
            this.practiceMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 47 "..\..\..\MainWindow.xaml"
            this.practiceMenu.Click += new System.Windows.RoutedEventHandler(this.ClickModelSelection);
            
            #line default
            #line hidden
            return;
            case 11:
            this.testMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 49 "..\..\..\MainWindow.xaml"
            this.testMenu.Click += new System.Windows.RoutedEventHandler(this.ClickModelSelection);
            
            #line default
            #line hidden
            return;
            case 12:
            this.touchOption = ((System.Windows.Controls.MenuItem)(target));
            
            #line 54 "..\..\..\MainWindow.xaml"
            this.touchOption.Click += new System.Windows.RoutedEventHandler(this.IsTouchEnabled);
            
            #line default
            #line hidden
            return;
            case 13:
            this.about = ((System.Windows.Controls.MenuItem)(target));
            
            #line 59 "..\..\..\MainWindow.xaml"
            this.about.Click += new System.Windows.RoutedEventHandler(this.AboutLogicPad);
            
            #line default
            #line hidden
            return;
            case 14:
            this.sketchUserStudyPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 15:
            this.showingContent = ((System.Windows.Controls.Label)(target));
            return;
            case 16:
            this.timer = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 17:
            this.StartButton = ((System.Windows.Controls.Button)(target));
            
            #line 70 "..\..\..\MainWindow.xaml"
            this.StartButton.Click += new System.Windows.RoutedEventHandler(this.StartButton_Click);
            
            #line default
            #line hidden
            return;
            case 18:
            this.StopButton = ((System.Windows.Controls.Button)(target));
            
            #line 71 "..\..\..\MainWindow.xaml"
            this.StopButton.Click += new System.Windows.RoutedEventHandler(this.StopButton_Click);
            
            #line default
            #line hidden
            return;
            case 19:
            this.widgetSelector = ((LogicPad2.WidgetSelector)(target));
            return;
            case 20:
            this.MainGCScroller = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 90 "..\..\..\MainWindow.xaml"
            this.MainGCScroller.LayoutUpdated += new System.EventHandler(this.Scroller_MainCanvasLayoutUpdated);
            
            #line default
            #line hidden
            return;
            case 21:
            this.mainInkCanvas = ((LogicPad2.MSInkAnalysisCanvas)(target));
            return;
            case 22:
            this.adornerLayer = ((System.Windows.Controls.Canvas)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

