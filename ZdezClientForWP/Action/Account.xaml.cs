using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ZdezClientForWP.Action
{
    public partial class AccountPage : PhoneApplicationPage
    {
        public AccountPage()
        {
            InitializeComponent();
            PwdBtn.Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/AccountPwd.xaml", UriKind.Relative));
                };
            AuthBtn.Click += (_s, _e) =>
                {
                    if (MessageBox.Show("退出后将无法接收到学校通知", "确定退出登录？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        ProgressOverlay.Visibility = System.Windows.Visibility.Visible;
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(param =>
                            {
                                var r = Service.RemoteService.Logout();
                                Dispatcher.BeginInvoke(() =>
                                    {
                                        if (!r.Status)
                                        {
                                            ProgressOverlay.Visibility = System.Windows.Visibility.Collapsed;
                                            MessageBox.Show("网络异常");
                                        }
                                        else
                                        {
                                            Service.AuthService.Instance.SetLogout();
                                            NavigationService.Navigate(new Uri("/Action/Login.xaml", UriKind.Relative));
                                        }
                                    });
                            }), null);
                    }
                };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InfoTable.DataContext = Service.AuthService.Instance.GetUserStruct();
            base.OnNavigatedTo(e);
        }
    }
}