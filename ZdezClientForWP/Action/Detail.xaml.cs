using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.Text.RegularExpressions;

using ZdezClientForWP.Model;

namespace ZdezClientForWP.Action
{
    public partial class DetailPage : PhoneApplicationPage
    {
        public DetailPage()
        {
            InitializeComponent();

            // 设置bar的单击事件
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += (_s, _e) =>
                {
                    NavigationService.GoBack();
                };
            // 设置内容
            ContentBrowser.IsScriptEnabled = true;
            ContentBrowser.ScriptNotify += ContentBrowser_InvokedByJS;
            // loadding状态维护
            ContentBrowser.NavigationFailed += ContentBrowser_NavigationFailed;
            ContentBrowser.Navigated += ContentBrowser_Navigated;
        }

        void ContentBrowser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            MessageBox.Show("发生错误");
            NavigationService.GoBack();
        }

        void ContentBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            LoaddingBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 手动设置Bar的背景色为主题橙色
            ApplicationBar.BackgroundColor = Themes.ThemeAddition.OrangeColor;
            // 获取传入导航参数
            var param = App.NavigateParam as MsgItem;
            if (param == null)
            {
                NavigationService.Navigate(new Uri("/Action/Main.xaml", UriKind.Relative));
                return;
            }
            // 开始加载内容
            LoaddingBar.Visibility = System.Windows.Visibility.Visible;
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(p =>
                {
                    var pp = p as MsgItem;
                    string category = "";
                    if (pp is NewsMsgItem)
                    {
                        category = "资讯频道";
                    }
                    else if (pp is NoticeMsgItem)
                    {
                        category = "校园通知";
                    }
                    else if (pp is ZdezMsgItem)
                    {
                        category = "找得着";
                    }
                    var html = HtmlBuilder(pp.Title, category, pp.Time, pp.Content);
                    html = Helper.Toolkit.ConvertToUnicode(html);
                    Dispatcher.BeginInvoke(() =>
                        {
                            ContentBrowser.NavigateToString(html);
                        });
                }), param);
            base.OnNavigatedTo(e);
        }

        private void ContentBrowser_InvokedByJS(object sender, NotifyEventArgs e)
        {
            // js回调
            var href = e.Value;
            if (!string.IsNullOrWhiteSpace(href))
            {
                try
                {
                    new Microsoft.Phone.Tasks.WebBrowserTask()
                        {
                            Uri = new Uri((href.StartsWith("http://") || href.StartsWith("https://")) ? href : Service.SettingService.HOST_NAME + href)
                        }.Show();
                }
                catch { }
            }
        }

        private static string HtmlBuilder(string title, string category, DateTime time, string content)
        {
            string result = content;
            // 修正图片链接，a链接由外部事件处理
            {
                Regex httpRegex = new Regex(@"^http://", RegexOptions.IgnoreCase);
                result = new Regex(@"(<\s*img[^</>]*?src\s*=\s*['""]?\s*)([\w\\\-\?:_/.]+)(\s*['""]?)", RegexOptions.IgnoreCase).Replace(result, m =>
                    {
                        if (m.Groups.Count == 4 && !httpRegex.IsMatch(m.Groups[2].Value))
                        {
                            return m.Groups[1] + Service.SettingService.HOST_NAME + m.Groups[2] + m.Groups[3];
                        }
                        else
                        {
                            return m.Value;
                        }
                    });
            }
            // 包装HTML
            result = @"
<!DOCTYPE html>
<html lang=""zh-cn"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0"" />
    <style>
        * { word-wrap: break-word !important; word-break:break-all !important; max-width: 100% !important; overflow-hidden !important; }
        html,body { margin: 0; padding: 0; background-color: white; }        
        img { border: none; }
        .__WP_TITLE_VIEW__, .__WP_TITLE_VIEW__ * { margin: 0; padding: 0; background-color: #F09609; color: white; font-weight: normal; }
        .__WP_TITLE_VIEW__ { padding: 1em; }
        .__WP_CONTENT_VIEW__ { padding: 1em; }
    </style>
</head>
<body>
    <div class=""__WP_TITLE_VIEW__"">
        <h2 id=""TEST"">" + title + @"</h2>
        <h4>[" + category + "]&nbsp;&nbsp;" + time.ToLongDateString() + " " + time.ToShortTimeString() + @"</h4>
    </div>
    <div class=""__WP_CONTENT_VIEW__"">
"
               + result +
@"
    </div>
    <script type=""text/javascript"">
        new function() {
            var onLinkClickFunc = function(e) {
                var evt = e || event
                  , src = evt.currentTarget || evt.target || evt.srcElement;
                if(src != null) {
                    while(src.nodeName != 'A') {
                        src = src.parentNode;
                    }
                    if(src.nodeName == 'A') {
                        var href = src.getAttribute('href', 2) || src.getAttribute('href') || src.href;
                        if(href && (typeof href == 'string')) {
                            href.replace(/^about\:+/i, '');
                            window.external.notify(href);
                        }
                    }
                }
                return false;
            };
            var linkElements = document.getElementsByTagName('a');
            for(var i = 0; i < linkElements.length; i++) {
                linkElements[i].attachEvent('onclick', onLinkClickFunc);
            }
        } ();
    </script>
</body>
</html>
";
            return result;
        }

    }

}