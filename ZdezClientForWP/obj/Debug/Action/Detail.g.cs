﻿#pragma checksum "C:\Works\ZdezClientForWP\ZdezClientForWP\Action\Detail.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0EE78BE5126E74AE007C1562CD6D1752"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18051
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ZdezClientForWP.Action {
    
    
    public partial class DetailPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.ScrollViewer LayoutRoot;
        
        internal Microsoft.Phone.Controls.WebBrowser ContentBrowser;
        
        internal System.Windows.Controls.ProgressBar LoaddingBar;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/ZdezClientForWP;component/Action/Detail.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.ScrollViewer)(this.FindName("LayoutRoot")));
            this.ContentBrowser = ((Microsoft.Phone.Controls.WebBrowser)(this.FindName("ContentBrowser")));
            this.LoaddingBar = ((System.Windows.Controls.ProgressBar)(this.FindName("LoaddingBar")));
        }
    }
}

