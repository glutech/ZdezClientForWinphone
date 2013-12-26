using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;

namespace ZdezClientForWP_ScheduledTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent 构造函数，初始化 UnhandledException 处理程序
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // 订阅托管异常处理程序
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// 出现未处理的异常时执行的代码
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// 运行计划任务的代理
        /// </summary>
        /// <param name="task">
        /// 调用的任务
        /// </param>
        /// <remarks>
        /// 调用定期或资源密集型任务时调用此方法
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            if (task is PeriodicTask)
            {
                if (DeviceNetworkInformation.IsNetworkAvailable)
                {
                    try
                    {
                        CheckNotificationChannelUrlSynchronized();
                        System.Diagnostics.Debug.WriteLine("CheckNotificationChannelUrlSynchronized Completed");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("ScheduledTaskAgent ERROR: [CheckNotificationChannelUrlSynchronized]");
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                    try
                    {
                        CheckShellTileRefreash();
                        System.Diagnostics.Debug.WriteLine("CheckShellTileRefreash Completed");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("ScheduledTaskAgent ERROR: [CheckShellTileRefreash]");
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("ScheduledTaskAgent NotifyComplete");
            NotifyComplete();
        }

        private const string HOST = "http://www.zdez.com.cn:9080/zdezServer/";

        private static void CheckNotificationChannelUrlSynchronized()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            bool isAuth, isSync;
            if (!settings.TryGetValue<bool>("IS_AUTH", out isAuth) || !isAuth || !settings.TryGetValue<bool>("IsChannelUriSynchronized", out isSync) || isSync)
            {
                return;
            }
            string syncUsername, syncUrl;
            if (!settings.TryGetValue<string>("AUTH_USERNAME", out syncUsername) || string.IsNullOrEmpty(syncUsername) || !settings.TryGetValue<string>("ChannelUriTarget", out syncUrl) || string.IsNullOrEmpty(syncUrl))
            {
                return;
            }
            var requestParam = new Dictionary<string, object>(2);
            requestParam.Add("stu_username", syncUsername);
            requestParam.Add("stu_staus", syncUrl);
            string response;
            if (HttpHelper.Request(new Uri(HOST + "WPClient_ModifyStaus"), requestParam, out response))
            {
                bool boolResponse;
                if (response != null && bool.TryParse(response, out boolResponse) && boolResponse)
                {
                    settings["IsChannelUriSynchronized"] = true;
                    settings.Save();
                }
            }
        }

        private void CheckShellTileRefreash()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            bool isAuth, isPullable;
            if (!settings.TryGetValue<bool>("IS_AUTH", out isAuth) || !isAuth || !settings.TryGetValue<bool>("PullSwitch", out isPullable) || !isPullable)
            {
                return;
            }
            int authId;
            if (!settings.TryGetValue<int>("AUTH_ID", out authId) || authId == 0)
            {
                return;
            }
            var requestParam = new Dictionary<string, object>(1);
            requestParam.Add("user_id", authId);
            string response;
            if (HttpHelper.Request(new Uri(HOST + "WPClient_GetPollingResult"), requestParam, out response) && !string.IsNullOrEmpty(response))
            {
                var responseStruct = RemoteNoticePoolQueryHelper.Parse(response);
                System.Diagnostics.Debug.WriteLine(responseStruct.ToReceiveNum);
                System.Diagnostics.Debug.WriteLine(responseStruct.LastItemTitle);
                string response_title = responseStruct.LastItemTitle;
                int response_count = responseStruct.ToReceiveNum;
                int settings_count;
                if (settings.TryGetValue<int>("ShellTileLastCount", out settings_count))
                {
                    response_count += settings_count;
                }
                ShellTile.ActiveTiles.First().Update(new StandardTileData()
                {
                    Title = "新通知",
                    BackgroundImage = new Uri("Background.png", UriKind.Relative),
                    Count = response_count > 99 ? 99 : response_count,
                    BackContent = response_title,
                    BackTitle = "找得着校园通知系统"
                });
            }
            else
            {
                string settings_title;
                int setings_count;
                if (settings.TryGetValue<string>("ShellTileLastTitle", out settings_title) && !string.IsNullOrEmpty(settings_title) && settings.TryGetValue<int>("ShellTileLastCount", out setings_count) && setings_count != 0)
                {
                    ShellTile.ActiveTiles.First().Update(new StandardTileData()
                    {
                        Title = "新通知",
                        BackgroundImage = new Uri("Background.png", UriKind.Relative),
                        Count = setings_count > 99 ? 99 : setings_count,
                        BackContent = settings_title,
                        BackTitle = "找得着校园通知系统"
                    });
                }
                else
                {
                    ShellTile.ActiveTiles.First().Update(new StandardTileData()
                    {
                        Title = "找得着",
                        BackgroundImage = new Uri("Background.png", UriKind.Relative)
                    });
                }
            }
        }

    }
}