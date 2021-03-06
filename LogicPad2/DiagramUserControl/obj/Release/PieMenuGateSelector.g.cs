﻿#pragma checksum "..\..\PieMenuGateSelector.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "20837AA4AE4D8200B0EBAE56C0FF7FCB"
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
using LogicPad2.Diagram.DragDrop;
using LogicPad2.Diagram.UIGates;
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


namespace LogicPad2.Diagram {
    
    
    /// <summary>
    /// PieMenuGateSelector
    /// </summary>
    public partial class PieMenuGateSelector : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.TranslateTransform UserControlToolTipXY;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.PieMenu pieMenu;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.UIGates.Not tbNot;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.UIGates.And tbAnd;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.UIGates.Xor tbXor;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.UIGates.UserOutput tbUserOutput;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.UIGates.Or tbOr;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.Diagram.UIGates.UserInput tbUserInput;
        
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
            System.Uri resourceLocater = new System.Uri("/DiagramUserControl;component/piemenugateselector.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PieMenuGateSelector.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.UserControlToolTipXY = ((System.Windows.Media.TranslateTransform)(target));
            return;
            case 2:
            this.pieMenu = ((LogicPad2.Diagram.PieMenu)(target));
            return;
            case 3:
            this.tbNot = ((LogicPad2.Diagram.UIGates.Not)(target));
            return;
            case 4:
            this.tbAnd = ((LogicPad2.Diagram.UIGates.And)(target));
            return;
            case 5:
            this.tbXor = ((LogicPad2.Diagram.UIGates.Xor)(target));
            return;
            case 6:
            this.tbUserOutput = ((LogicPad2.Diagram.UIGates.UserOutput)(target));
            return;
            case 7:
            this.tbOr = ((LogicPad2.Diagram.UIGates.Or)(target));
            return;
            case 8:
            this.tbUserInput = ((LogicPad2.Diagram.UIGates.UserInput)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

