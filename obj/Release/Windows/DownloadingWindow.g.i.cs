﻿#pragma checksum "..\..\..\Windows\DownloadingWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "24A1CA8145FA1FA113DE318B5845D26807B53C6D1A989C1F6448B7CDA8CDE6B1"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace iMine.Launcher.Windows {
    
    
    /// <summary>
    /// DownloadingWindow
    /// </summary>
    public partial class DownloadingWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\Windows\DownloadingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SliderBase;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\Windows\DownloadingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle Slider;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\Windows\DownloadingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DownloadDesc;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\Windows\DownloadingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CloseButton;
        
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
            System.Uri resourceLocater = new System.Uri("/iMineLauncher;component/windows/downloadingwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\DownloadingWindow.xaml"
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
            this.SliderBase = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.Slider = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            this.DownloadDesc = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.CloseButton = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\..\Windows\DownloadingWindow.xaml"
            this.CloseButton.MouseEnter += new System.Windows.Input.MouseEventHandler(this.CloseButton_MouseEnter);
            
            #line default
            #line hidden
            
            #line 14 "..\..\..\Windows\DownloadingWindow.xaml"
            this.CloseButton.MouseLeave += new System.Windows.Input.MouseEventHandler(this.CloseButton_MouseLeave);
            
            #line default
            #line hidden
            
            #line 14 "..\..\..\Windows\DownloadingWindow.xaml"
            this.CloseButton.Click += new System.Windows.RoutedEventHandler(this.CloseButtonClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

