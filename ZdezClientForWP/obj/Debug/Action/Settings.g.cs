﻿#pragma checksum "C:\Works\ZdezClientForWP\ZdezClientForWP\Action\Settings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "37E3A5E3CDE8DFE6436B6C62C5C0923F"
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
    
    
    public partial class SettingsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Coding4Fun.Toolkit.Controls.ChatBubbleTextBox SettingFloatTips;
        
        internal Microsoft.Phone.Controls.ToggleSwitch PullSwitchToggle;
        
        internal Microsoft.Phone.Controls.ToggleSwitch UEPlanBtn;
        
        internal System.Windows.Controls.Button FeedbackBtn;
        
        internal System.Windows.Controls.Button UpdaterBtn;
        
        internal System.Windows.Controls.Button AboutUsBtn;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/ZdezClientForWP;component/Action/Settings.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.SettingFloatTips = ((Coding4Fun.Toolkit.Controls.ChatBubbleTextBox)(this.FindName("SettingFloatTips")));
            this.PullSwitchToggle = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("PullSwitchToggle")));
            this.UEPlanBtn = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("UEPlanBtn")));
            this.FeedbackBtn = ((System.Windows.Controls.Button)(this.FindName("FeedbackBtn")));
            this.UpdaterBtn = ((System.Windows.Controls.Button)(this.FindName("UpdaterBtn")));
            this.AboutUsBtn = ((System.Windows.Controls.Button)(this.FindName("AboutUsBtn")));
            this.ProgressOverlay = ((Coding4Fun.Toolkit.Controls.ProgressOverlay)(this.FindName("ProgressOverlay")));
        }
    }
}

