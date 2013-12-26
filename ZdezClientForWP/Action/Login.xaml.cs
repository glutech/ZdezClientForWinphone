using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

using ZdezClientForWP.Service;
using ZdezClientForWP.Helper;

namespace ZdezClientForWP.Action
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            ApplicationBar.BackgroundColor = Themes.ThemeAddition.OrangeColor;
            SetInputBoxStyle();
            SetInputBoxAction();
            // 防止提交过程中被退出
            BackKeyPress += (s, e) =>
            {
                if (ProgressOverlay.Visibility == System.Windows.Visibility.Visible)
                {
                    e.Cancel = true;
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            if (AuthService.Instance.IsAuth)
            {
                NavigationService.Navigate(new Uri("/Action/Main.xaml", UriKind.Relative));
            }
            // TEST
            base.OnNavigatedTo(e);
        }

        #region 输入框样式切换

        private void SetInputBoxStyle()
        {
            UserBox.GotFocus += InputBox_GotFocus;
            PwdBox.GotFocus += InputBox_GotFocus;
            UserBox.LostFocus += InputBox_LostFocus;
            PwdBox.LostFocus += InputBox_LostFocus;
        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((Control)sender).Background = (Brush)Resources["PhoneBackgroundBrush"];
        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((Control)sender).Background = (Brush)Resources["PhoneChromeBrush"];
        }

        #endregion

        #region 输入框输入状态交互

        private void SetInputBoxAction()
        {
            UserBox.KeyUp += (s, se) =>
            {
                if (se.Key == System.Windows.Input.Key.Enter) PwdBox.Focus();
            };
            PwdBox.KeyUp += (s, se) =>
            {
                if (se.Key == System.Windows.Input.Key.Enter) DoLogin();
            };
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).Click += (s, se) =>
            {
                DoLogin();
            };
        }

        #endregion

        private void DoLogin()
        {
            if (UserBox.Text == "")
            {
                Helper.Toolkit.ToastMessage("请输入账号");
                UserBox.Focus();
            }
            else if (PwdBox.Password == "")
            {
                Helper.Toolkit.ToastMessage("请输入密码");
                PwdBox.Focus();
            }
            else
            {
                Focus();
                ApplicationBar.IsVisible = false;
                ProgressOverlay.Show();
                System.Threading.ThreadPool.QueueUserWorkItem(o =>
                    {
                        var param = (IDictionary<string, object>)o;
                        var result = RemoteService.Login((string)param["Username"], (string)param["Password"]);
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            if (result.Status)
                            {
                                var r = (AuthUserStruct)result.Data;
                                AuthService.Instance.SetLogin(r);
                                NavigationService.Navigate(new Uri("/Action/Main.xaml", UriKind.Relative));
                            }
                            else
                            {
                                ProgressOverlay.Hide();
                                ApplicationBar.IsVisible = true;
                                MessageBox.Show(result.Message as string == "fail" ? "账号或密码错误" : "网络异常");
                            }
                        });
                    },new Toolkit.DictWrapper().OR("Username", UserBox.Text).OR("Password", PwdBox.Password));
            }
        }

    }
}