﻿#pragma checksum "C:\Works\ZdezClientForWP\ZdezClientForWP\Action\Account.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B3038DE07173CA447E744F8142D8C3E1"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18051
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Coding4Fun.Toolkit.Controls;
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
    
    
    public partial class AccountPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Grid InfoTable;
        
        internal System.Windows.Controls.Button PwdBtn;
        
        internal System.Windows.Controls.Button AuthBtn;
        
        internal Coding4Fun.Toolkit.Controls.ProgressOverlay ProgressOverlay;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/ZdezClientForWP;component/Action/Account.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.InfoTable = ((System.Windows.Controls.Grid)(this.FindName("InfoTable")));
            this.PwdBtn = ((System.Windows.Controls.Button)(this.FindName("PwdBtn")));
            this.AuthBtn = ((System.Windows.Controls.Button)(this.FindName("AuthBtn")));
            this.ProgressOverlay = ((Coding4Fun.Toolkit.Controls.ProgressOverlay)(this.FindName("ProgressOverlay")));
        }
    }
}

