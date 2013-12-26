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
    public partial class FeedbackPage : PhoneApplicationPage
    {
        public FeedbackPage()
        {
            InitializeComponent();

            BackKeyPress += (s, e) =>
                {
                    if (ProgressOverlay.Visibility == System.Windows.Visibility.Visible)
                    {
                        e.Cancel = true;
                    }
                };

            ContentBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            PostBtn.Click += (_s, _e) =>
                {
                    DoPost();
                };
        }

        private void DoPost()
        {
            if (string.IsNullOrWhiteSpace(ContentBox.Text))
            {
                Helper.Toolkit.ToastMessage("请填写反馈内容");
                ContentBox.Focus();
                return;
            }
            this.Focus();
            ProgressOverlay.Visibility = System.Windows.Visibility.Visible;
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(param =>
                {
                    System.Threading.Thread.Sleep(500);
                    var r = Service.RemoteService.PostFeedback(param as string);
                    Dispatcher.BeginInvoke(() =>
                        {
                            ProgressOverlay.Visibility = System.Windows.Visibility.Collapsed;
                            if (r.Status)
                            {
                                MessageBox.Show("感谢您对找得着的支持！", "反馈内容已提交", MessageBoxButton.OK);
                                NavigationService.GoBack();
                            }
                            else
                            {
                                MessageBox.Show("网络异常");
                            }
                        });
                }), ContentBox.Text);
        }
    }
}