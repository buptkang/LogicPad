﻿#pragma checksum "..\..\..\PieMenuGateSelector.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D01B80275ADD7BDC94AB0EB1BFB365EA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LogicPad2;
using LogicPad2.DragDrop;
using LogicPad2.UIGates;
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
using System.Windows.Shell;


namespace LogicPad2 {
    
    
    /// <summary>
    /// PieMenuGateSelector
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class PieMenuGateSelector : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.PieMenu pieMenu;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.UIGates.Not tbNot;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.UIGates.And tbAnd;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.UIGates.Or tbOr;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.UIGates.Xor tbXor;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.UIGates.UserInput tbUserInput;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\PieMenuGateSelector.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LogicPad2.UIGates.UserOutput tbUserOutput;
        
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
            System.Uri resourceLocater = new System.Uri("/LogicPad2;component/piemenugateselector.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\PieMenuGateSelector.xaml"
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
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.pieMenu = ((LogicPad2.PieMenu)(target));
            return;
            case 2:
            this.tbNot = ((LogicPad2.UIGates.Not)(target));
            return;
            case 3:
            this.tbAnd = ((LogicPad2.UIGates.And)(target));
            return;
            case 4:
            this.tbOr = ((LogicPad2.UIGates.Or)(target));
            return;
            case 5:
            this.tbXor = ((LogicPad2.UIGates.Xor)(target));
            return;
            case 6:
            this.tbUserInput = ((LogicPad2.UIGates.UserInput)(target));
            return;
            case 7:
            this.tbUserOutput = ((LogicPad2.UIGates.UserOutput)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
