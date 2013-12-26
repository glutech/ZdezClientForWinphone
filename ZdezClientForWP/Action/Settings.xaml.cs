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
    public partial class SettingsPage : PhoneApplicationPage
    {

        public SettingsPage()
        {
            InitializeComponent();

            SettingFloatTips.Text = "为及时可靠的接收校园通知：\n请确保推送开关打开、后台任务可用\n并在桌面固定磁贴！";

            // 手动维护ToggleSwitch和SettingService的值映射关系
            PullSwitchToggle.Click += (_s, _e) =>
                {
                    if (!PullSwitchToggle.IsChecked.Value)
                    {
                        if (MessageBox.Show("关闭后将接收不到学校发布的通知", "确定关闭推送功能？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            
                            Service.SettingService.Instance.PullSwitch = false;
                        }
                        else
                        {
                            PullSwitchToggle.IsChecked = true;
                        }
                    }
                    else
                    {
                        Service.SettingService.Instance.PullSwitch = true;
                    }
                };

            UEPlanBtn.Click += (_s, _e) =>
                {
                    if (UEPlanBtn.IsChecked.Value)
                    {
                        MessageBox.Show("感谢您对找得着的支持，我们会持续改进做得更好！");
                    }
                };
            UpdaterBtn.Click += (_s, _e) =>
                {
                    ProgressOverlay.Visibility = System.Windows.Visibility.Visible;
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((param) =>
                        {
                            try
                            {
                                var xmlRequest = Helper.HttpHelper.RequestOnSync(new Uri(Service.SettingService.HOST + "client/wpVersion.xml"), Helper.HttpHelper.RequestTypeEnum.GET, null);
                                var doc = System.Xml.Linq.XDocument.Load(new System.IO.StringReader(xmlRequest.Data as string), System.Xml.Linq.LoadOptions.None);
                                var root = doc.Element("AppVersion").Element("ZdezClientForWP");
                                var version = new System.Version(root.Attribute("version").Value);
                                var target = new Uri(root.Element("Target").Value);
                                var message = root.Element("Message").Value;
                                Dispatcher.BeginInvoke(() =>
                                    {
                                        ProgressOverlay.Visibility = System.Windows.Visibility.Collapsed;
                                        if (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName).Version < version)
                                        {
                                            if (MessageBox.Show(message, "发现新版本", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                            {
                                                new Microsoft.Phone.Tasks.WebBrowserTask()
                                                {
                                                    Uri = target
                                                }.Show();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("程序已是最新版本");
                                        }
                                    });
                            }
                            catch
                            {
                                Dispatcher.BeginInvoke(() =>
                                    {
                                        ProgressOverlay.Visibility = System.Windows.Visibility.Collapsed;
                                        MessageBox.Show("网络异常");
                                    });
                            }
                        }), null);
                };
            FeedbackBtn.Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/Feedback.xaml", UriKind.Relative));
                };
            AboutUsBtn.Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/AboutUs.xaml", UriKind.Relative));
                };
        }
    }
}