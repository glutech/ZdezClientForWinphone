using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Net.NetworkInformation;

namespace ZdezClientForWP.Service
{
    class NotificationService : IDisposable
    {

        #region 单例模式

        private static NotificationService _instance;

        /// <summary>
        /// 获取单例模式下的当前实例
        /// </summary>
        public static NotificationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationService();
                }
                return _instance;
            }
        }

        #endregion



        public AllowableToggle Toggle
        {
            get;
            private set;
        }

        public HttpNotificationChannel Channel
        {
            get;
            private set;
        }

        public ChannelUriSyncManager SyncMngr
        {
            get;
            private set;
        }



        private const string CHANNEL_NAME = "ZdezClientForWPSchoolNoticeChannel";


        protected NotificationService()
        {
            this.SyncMngr = new ChannelUriSyncManager();
            this.Toggle = new AllowableToggle(AuthService.Instance.IsAuth, SettingService.Instance.PullSwitch, ToggleChanged);
            // 监听网络变化，确保推送通道可用
            DeviceNetworkInformation.NetworkAvailabilityChanged += DeviceNetworkInformation_NetworkAvailabilityChanged;
        }

        private void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            // 网络已连接
            if (e.NotificationType == NetworkNotificationType.InterfaceConnected)
            {
                // 强制检查通道
                Toggle.CheckToggle();
            }
        }

        private void ToggleChanged(bool toggleStatus)
        {
            if (toggleStatus)
            {
                Channel = HttpNotificationChannel.Find(CHANNEL_NAME);
                if (Channel == null)
                {
                    Channel = new HttpNotificationChannel(CHANNEL_NAME);
                    BindChannelEvents();
                    Channel.Open();
                    Channel.BindToShellToast();
                }
                else
                {
                    BindChannelEvents();
                    // 手动提交
                    // 存在可能：之前注册了通道，但通知时APP已退出
                    ChannelUriUpdated(Channel.ChannelUri);
                }
            }
            else
            {
                if (Channel != null)
                {
                    SyncMngr.IsSynchronized = true;
                    Channel.Close();
                    Channel = null;
                }
            }
        }

        private void ChannelUriUpdated(Uri uri)
        {
            SyncMngr.IsSynchronized = false;
            SyncMngr.SynchronizedTarget = uri.ToString();
            SyncMngr.CheckSynchronized();
        }

        private void BindChannelEvents()
        {
            Channel.ErrorOccurred += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("注册Microsoft推送通道错误");
                    System.Windows.MessageBox.Show("注册Microsoft推送通道时发生了错误");
                };
            Channel.ChannelUriUpdated += (s, e) =>
                {
                    ChannelUriUpdated(e.ChannelUri);   
                };
            Channel.ShellToastNotificationReceived += (s, e) =>
                {
                    if (NotificationReceived != null)
                    {
                        var response = e.Collection;
                        string title = null;
                        string content = null;
                        string param = null;
                        response.TryGetValue("wp:Text1", out title);
                        response.TryGetValue("wp:Text2", out content);
                        response.TryGetValue("wp:Param", out param);
                        NotificationReceived(title, content, param);
                    }
                };
        }


        /// <summary>
        /// 推送通道接收到消息时的跟踪委托
        /// </summary>
        public delegate void NotificationReceivedHandler(string title, string content, string param);
        /// <summary>
        /// 推送通道接收到消息时的跟踪事件
        /// </summary>
        public event NotificationReceivedHandler NotificationReceived;


        void IDisposable.Dispose()
        {
            if (Channel != null)
            {
                Channel.Dispose();
            }
        }


        #region URI同步实现

        public class ChannelUriSyncManager
        {
            const string SYNC_FLAG_KEY = "IsChannelUriSynchronized";
            const string SYNC_URI_KEY = "ChannelUriTarget";

            public bool IsSynchronized
            {
                get
                {
                    return Helper.AppSettingHelper.GetValueOrDefault<bool>(SYNC_FLAG_KEY, true);
                }
                set
                {
                    Helper.AppSettingHelper.AddOrUpdateValue(SYNC_FLAG_KEY, value);
                }
            }

            public string SynchronizedTarget
            {
                get
                {
                    return Helper.AppSettingHelper.GetValueOrDefault<string>(SYNC_URI_KEY, null);
                }
                set
                {
                    Helper.AppSettingHelper.AddOrUpdateValue(SYNC_URI_KEY, value);
                }
            }

            private static object locker = new object();

            public void CheckSynchronized()
            {
                if (!IsSynchronized)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((param) =>
                        {
                            lock (locker)
                            {
                                if (!IsSynchronized)
                                {
                                    var r = RemoteService.UpdateNotificationChannelUri((Uri)param);
                                    if (!r.Status)
                                    {
                                        IsSynchronized = false;
                                        System.Diagnostics.Debug.WriteLine("ChannelUri Sync faild. " + (Uri)param);
                                    }
                                    else
                                    {
                                        IsSynchronized = true;
                                        System.Diagnostics.Debug.WriteLine("ChannelUri Sync Ok. " + (Uri)param);
                                    }
                                }
                            }
                        }), new Uri(SynchronizedTarget));
                }
            }

            public void CheckSynchronizedOnSync()
            {
                if (!IsSynchronized)
                {
                    var target = new Uri(SynchronizedTarget);
                    var r = RemoteService.UpdateNotificationChannelUri(target);
                    if (!r.Status)
                    {
                        IsSynchronized = false;
                        System.Diagnostics.Debug.WriteLine("ChannelUri Sync faild. " + target);
                    }
                    else
                    {
                        IsSynchronized = true;
                        System.Diagnostics.Debug.WriteLine("ChannelUri Sync Ok. " + target);
                    }
                }
            }
        }

        #endregion


        #region 推送开关实现

        /// <summary>
        /// 推送开关实现类
        /// </summary>
        public class AllowableToggle
        {

            /// <summary>
            /// 标识设备设置是否允许接收推送通知
            /// </summary>
            public bool DeviceAllowable
            {
                get
                {
                    return _deviceAllowable;
                }
                set
                {
                    if (_deviceAllowable != value)
                    {
                        _deviceAllowable = value;
                        AllowableChanged();
                    }
                }
            }
            private bool _deviceAllowable = false;

            /// <summary>
            /// 标志账号是否允许接收推送通知
            /// </summary>
            public bool AuthAllowable
            {
                get
                {
                    return _authAllowable;
                }
                set
                {
                    if (_authAllowable != value)
                    {
                        _authAllowable = value;
                        AllowableChanged();
                    }
                }
            }
            private bool _authAllowable = false;

            private void AllowableChanged()
            {
                if (DeviceAllowable && AuthAllowable)
                {
                    this._allowableChangedCallback(true);
                }
                else if (!DeviceAllowable || !AuthAllowable)
                {
                    this._allowableChangedCallback(false);
                }
            }

            /// <summary>
            /// 开关状态回调
            /// </summary>
            private Action<bool> _allowableChangedCallback;

            public AllowableToggle(bool authAllow, bool deviceAllow, Action<bool> allowableChangedCallback)
            {
                this._authAllowable = authAllow;
                this._deviceAllowable = deviceAllow;
                this._allowableChangedCallback = allowableChangedCallback;
                this.AllowableChanged();
            }

            public void CheckToggle()
            {
                AllowableChanged();
            }
        }

        #endregion


    }
}