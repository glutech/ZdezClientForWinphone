using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Xml;
namespace ZdezClientForWP
{
    public partial class App : Application
    {

        private static object _navigateParam;
        public static object NavigateParam
        {
            get { var p = _navigateParam; _navigateParam = null; return p; }
            set { _navigateParam = value; }
        }

        /// <summary>
        ///提供对电话应用程序的根框架的轻松访问。
        /// </summary>
        /// <returns>电话应用程序的根框架。</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Application 对象的构造函数。
        /// </summary>
        public App()
        {
            // 初始化数据库
            Model.MsgContext.InitDB();

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // 特定于电话的初始化
            InitializePhoneApplication();

            // 利用第三方的 ThemeManager 固定主题
            ThemeManager.SetCustomTheme(new Uri("/ZdezClientForWP;component/Themes/ThemeResources.xaml", UriKind.Relative), Theme.Light);

            // 调试时显示图形分析信息。
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 显示当前帧速率计数器。
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // 显示在每个帧中重绘的应用程序区域。
                //Application.Current.Host.Settings.EnableRedrawRegions = true；

                // Enable non-production analysis visualization mode, 
                // 该模式显示递交给 GPU 的包含彩色重叠区的页面区域。
                //Application.Current.Host.Settings.EnableCacheVisualization = true；

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                //  注意: 仅在调试模式下使用此设置。禁用用户空闲检测的应用程序在用户不使用电话时将继续运行
                // 并且消耗电池电量。
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
            // 关联推送接收逻辑
            Service.NotificationService.Instance.NotificationReceived += (title, content, param) =>
            {
                // 震动提示
                if (Service.SettingService.Instance.PullVibrate)
                {
                    Helper.Toolkit.Vibrate();
                }
                //  声音提示
                if (Service.SettingService.Instance.PullSound)
                {
                    Helper.Toolkit.PlayNoticeSound();
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var toast = new Coding4Fun.Toolkit.Controls.ToastPrompt()
                    {
                        Message = "新通知：" + title,
                        Foreground = (Brush)Resources["PhoneForegroundBrush"],
                        Background = (Brush)Resources["PhoneAccentBrush"],
                        FontSize = 24,
                    };

                    toast.Completed += (_s, _e) =>
                    {
                        if (_e.PopUpResult == Coding4Fun.Toolkit.Controls.PopUpResult.Ok)
                        {
                            ((PhoneApplicationFrame)Application.Current.RootVisual).Navigate(new Uri(param, UriKind.Relative));
                        }
                        else
                        {
                            Service.ShellTileService.Instance.UpdateOnSystemNotification(title);
                        }
                    };
                    toast.Show();
                });
            };
        }

        // 应用程序启动(例如，从“开始”菜单启动)时执行的代码
        // 此代码在重新激活应用程序时不执行
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            Service.ScheduledAgentService.Instance.AutoCheck();
        }

        /// <summary>
        /// 标志应用是否从休眠状态恢复并且未被逻辑删除
        /// 值在Application.Activeted事件的ActivatedEventArgs.IsApplicationInstancePreserved取得
        /// Page页使用完后应手动清空，同时，程序Deactivated时也会被清空
        /// </summary>
        public static bool? IsApplicationInstancePreserved { get; set; }

        // 激活应用程序(置于前台)时执行的代码
        // 此代码在首次启动应用程序时不执行
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            IsApplicationInstancePreserved = e.IsApplicationInstancePreserved;
        }

        // 停用应用程序(发送到后台)时执行的代码
        // 此代码在应用程序关闭时不执行
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            IsApplicationInstancePreserved = null;
        }

        // 应用程序关闭(例如，用户点击“后退”)时执行的代码
        // 此代码在停用应用程序时不执行
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // 导航失败时执行的代码
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 导航已失败；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        // 出现未处理的异常时执行的代码
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        #region 电话应用程序初始化

        // 避免双重初始化
        private bool phoneApplicationInitialized = false;

        // 请勿向此方法中添加任何其他代码
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // 创建框架但先不将它设置为 RootVisual；这允许初始
            // 屏幕保持活动状态，直到准备呈现应用程序时。
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // 处理导航故障
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // 确保我们未再次初始化
            phoneApplicationInitialized = true;
        }

        // 请勿向此方法中添加任何其他代码
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // 设置根视觉效果以允许应用程序呈现
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // 删除此处理程序，因为不再需要它
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}