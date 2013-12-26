using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using ZdezClientForWP.Helper;

namespace ZdezClientForWP.Action
{
    public partial class AccountPwd : PhoneApplicationPage
    {
        public AccountPwd()
        {
            InitializeComponent();

            BackKeyPress += (s, e) =>
            {
                if (ProgressOverlay.Visibility == System.Windows.Visibility.Visible)
                {
                    e.Cancel = true;
                }
            };

            SourcePwd.KeyUp += (_s, _e) =>
                {
                    if (_e.Key == System.Windows.Input.Key.Enter)
                    {
                        NewPwd1.Focus();
                    }
                };
            NewPwd1.KeyUp += (_s, _e) =>
                {
                    if (_e.Key == System.Windows.Input.Key.Enter)
                    {
                        NewPwd2.Focus();
                    }
                };
            NewPwd2.KeyUp += (_s, _e) =>
                {
                    if (_e.Key == System.Windows.Input.Key.Enter)
                    {
                        DoPost();
                    }
                };
            PostBtn.Click += (_s, _e) =>
                {
                    DoPost();
                };
        }

        private void DoPost()
        {
            if (string.IsNullOrWhiteSpace(SourcePwd.Password))
            {
                Helper.Toolkit.ToastMessage("请输入原密码");
                SourcePwd.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPwd1.Password))
            {
                Helper.Toolkit.ToastMessage("请输入新密码");
                NewPwd1.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPwd2.Password))
            {
                Helper.Toolkit.ToastMessage("请再次输入新密码");
                return;
            }
            if (NewPwd1.Password != NewPwd2.Password)
            {
                Helper.Toolkit.ToastMessage("两次新密码不相同，请检查");
                NewPwd1.Focus();
                return;
            }
            if (SourcePwd.Password == NewPwd1.Password)
            {
                Helper.Toolkit.ToastMessage("新密码不能与原密码相同，请检查");
                NewPwd1.Focus();
                return;
            }
            this.Focus();
            ProgressOverlay.Visibility = System.Windows.Visibility.Visible;
            var callbackParam = new Dictionary<string, object>();
            callbackParam.Add("sourcePassword", SourcePwd.Password);
            callbackParam.Add("newPassword", NewPwd1.Password);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((param) =>
                {
                    var sourcePassword = (param as IDictionary<string, object>)["sourcePassword"] as string;
                    var newPassword = (param as IDictionary<string, object>)["newPassword"] as string;
                    var r = Service.RemoteService.ModifyPassword(sourcePassword, newPassword);
                    Dispatcher.BeginInvoke(() =>
                        {
                            ProgressOverlay.Visibility = System.Windows.Visibility.Collapsed;
                            if (r.Status)
                            {
                                if ((bool)r.Data)
                                {
                                    MessageBox.Show("修改成功");
                                    NavigationService.GoBack();
                                }
                                else
                                {
                                    MessageBox.Show("原始密码错误");
                                }
                            }
                            else
                            {
                                MessageBox.Show("网络异常");
                            }
                        });
                }), callbackParam);
        }

    }
}