﻿#pragma checksum "..\..\GateInkCanvas.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "40748F10D30947911F572ACCEAABE90F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LogicPad2.Diagram;
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
using starPadSDK.WPFHelp;


namespace LogicPad2.Diagram {
    
    
    /// <summary>
    /// GateInkCanvas
    /// </summary>
    public partial class GateInkCanvas : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\GateInkCanvas.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer GCScroller;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\GateInkCanvas.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.LogicDiagramInkCanvas circuitInkCanvas;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\GateInkCanvas.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle dragSelect;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\GateInkCanvas.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.Wire dragWire;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DiagramUserControl;component/gateinkcanvas.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\GateInkCanvas.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.GCScroller = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 8 "..\..\GateInkCanvas.xaml"
            this.GCScroller.SizeChanged += new System.Windows.SizeChangedEventHandler(this.GCScroller_SizeChanged);
            
            #line default
            #line hidden
            
            #line 9 "..\..\GateInkCanvas.xaml"
            this.GCScroller.LayoutUpdated += new System.EventHandler(this.GCScroller_LayoutUpdated);
            
            #line default
            #line hidden
            
            #line 10 "..\..\GateInkCanvas.xaml"
            this.GCScroller.ScrollChanged += new System.Windows.Controls.ScrollChangedEventHandler(this.GCScroller_ScrollChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.circuitInkCanvas = ((LogicPad2.Diagram.LogicDiagramInkCanvas)(target));
            return;
            case 3:
            this.dragSelect = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 4:
            this.dragWire = ((LogicPad2.Diagram.Wire)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

