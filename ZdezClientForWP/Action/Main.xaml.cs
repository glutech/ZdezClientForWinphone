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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using System.Windows.Threading;

using ZdezClientForWP.Model;
using ZdezClientForWP.Service;
using ZdezClientForWP.Helper;
using Microsoft.Phone.Net.NetworkInformation;

namespace ZdezClientForWP.Action
{

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            // 初始化框架
            InitializeComponent();
            // 关联基础事件
            BackKeyPress += (s, e) =>
                {
                    if (Service.SettingService.Instance.PullSwitch)
                    {
                        if (MessageBox.Show("退出程序后将继续在后台接收校园通知", "确定要退出程序？", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("您关闭了\"后台推送\"，离开程序界面后将接收不到校园通知！", "确定要退出程序？", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            e.Cancel = true;
                        }
                    }
                };
            // 关联操作事件
            PivotControl.SelectionChanged += PivotControl_SelectionChanged;
            NewsListBox.SelectionChanged += PageListBox_SelectionChanged;
            NoticeListBox.SelectionChanged += PageListBox_SelectionChanged;
            ZdezListBox.SelectionChanged += PageListBox_SelectionChanged;
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += ApplicationBarRefreashBtn_Click;
            // 关联操作逻辑
            ZdezHrBtn.Click += (_s, _e) =>
                {
                    var task = new WebBrowserTask();
                    task.Uri = new Uri("http://www.zdez.cn");
                    task.Show();
                };
            AccountSettingBtn.Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/Account.xaml", UriKind.Relative));
                };
            SystemSettingBtn.Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/Settings.xaml", UriKind.Relative));
                };
            FeedbackBtn.Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/Feedback.xaml", UriKind.Relative));
                };
            AboutUsBtn.Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/AboutUs.xaml", UriKind.Relative));
                };
            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/Account.xaml", UriKind.Relative));
                };
            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Click += (_s, _e) =>
                {
                    NavigationService.Navigate(new Uri("/Action/Settings.xaml", UriKind.Relative));
                };
            // 初始化容器
            NewsListBox.ItemsSource = new ObservableCollection<MsgItem>();
            NoticeListBox.ItemsSource = new ObservableCollection<MsgItem>();
            ZdezListBox.ItemsSource = new ObservableCollection<MsgItem>();
            // 初始化 BackgroundWorkder
            {
                ContentBgWorker_News = new BackgroundWorker();
                ContentBgWorker_News.WorkerReportsProgress = false;
                ContentBgWorker_News.WorkerSupportsCancellation = false;
                ContentBgWorker_News.DoWork += ContentBgWorker_DoWork;
                ContentBgWorker_News.RunWorkerCompleted += ContentBgworker_RunWorkerCompleted;
                ContentBgWorker_Notice = new BackgroundWorker();
                ContentBgWorker_Notice.WorkerReportsProgress = false;
                ContentBgWorker_Notice.WorkerSupportsCancellation = false;
                ContentBgWorker_Notice.DoWork += ContentBgWorker_DoWork;
                ContentBgWorker_Notice.RunWorkerCompleted += ContentBgworker_RunWorkerCompleted;
                ContentBgWorker_Zdez = new BackgroundWorker();
                ContentBgWorker_Zdez.WorkerReportsProgress = false;
                ContentBgWorker_Zdez.WorkerSupportsCancellation = false;
                ContentBgWorker_Zdez.DoWork += ContentBgWorker_DoWork;
                ContentBgWorker_Zdez.RunWorkerCompleted += ContentBgworker_RunWorkerCompleted;
            }
            // 初始化内容模型
            {
                var newsPRS = new PullRefreashSupportor(NewsListBox, NewsInfoBox, CategoryHolder.ModelEnum.News);
                var noticePRS = new PullRefreashSupportor(NoticeListBox, NoticeInfoBox, CategoryHolder.ModelEnum.Notice);
                var zdezPRS = new PullRefreashSupportor(ZdezListBox, ZdezInfoBox, CategoryHolder.ModelEnum.Zdez);
                newsPRS.PullRefreashTriggeredEvent += PullRefreashSupportor_PullRefreashTriggeredEvent;
                noticePRS.PullRefreashTriggeredEvent += PullRefreashSupportor_PullRefreashTriggeredEvent;
                zdezPRS.PullRefreashTriggeredEvent += PullRefreashSupportor_PullRefreashTriggeredEvent;
                categoryHolder = new CategoryHolder(
                    NewsListBox, NewsProgressBar, newsPRS, ContentBgWorker_News,
                    NoticeListBox, NoticeProgressBar, noticePRS, ContentBgWorker_Notice,
                    ZdezListBox, ZdezProgressBar, zdezPRS, ContentBgWorker_Zdez
                    );
                flushManager = new FlushManager((model, status) =>
                    {
                        ContentBgWorker_BeforeWork(categoryHolder.Get(model), status, flushManager, Dispatcher);
                    });
            }
        }

        private bool IsPageNewInstance = true;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // 每次加载时检查推送通道是否已同步
            NotificationService.Instance.SyncMngr.CheckSynchronized();
            // 手动设置Bar的背景色为主题橙色
            ApplicationBar.BackgroundColor = Themes.ThemeAddition.OrangeColor;
            // 某些情况下设置失败，延时处理
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((param) =>
                {
                    System.Threading.Thread.Sleep(500);
                    ((Dispatcher)(((object[])param)[2])).BeginInvoke(() =>
                        {
                            ((IApplicationBar)(((object[])param)[0])).BackgroundColor = (Color)(((object[])param)[1]);
                        });
                }), new object[] { ApplicationBar,Themes.ThemeAddition.OrangeColor, Dispatcher });
            // 请除前置页面（如登录页）
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            // 判断页面和内容交互状态
            if (this.IsPageNewInstance)
            {
                // 首次启动时应加载所有内容
                flushManager.MarkChanged(CategoryHolder.ModelEnum.News, FlushManager.StatusEnum.NewAndOld);
                flushManager.MarkChanged(CategoryHolder.ModelEnum.Notice, FlushManager.StatusEnum.NewAndOld);
                flushManager.MarkChanged(CategoryHolder.ModelEnum.Zdez, FlushManager.StatusEnum.NewAndOld);
            }
            else if (App.IsApplicationInstancePreserved.HasValue)
            {
                if (App.IsApplicationInstancePreserved.Value)
                {
                    // 从休眠状态恢复启动时加载最新内容
                    flushManager.MarkChanged(CategoryHolder.ModelEnum.News, FlushManager.StatusEnum.New);
                    flushManager.MarkChanged(CategoryHolder.ModelEnum.Notice, FlushManager.StatusEnum.New);
                    flushManager.MarkChanged(CategoryHolder.ModelEnum.Zdez, FlushManager.StatusEnum.New);
                }
            }
            // 设置App和Page状态
            this.IsPageNewInstance = false;
            App.IsApplicationInstancePreserved = null;
            // 推送判断
            if (NavigationContext.QueryString.ContainsKey("mod") && NavigationContext.QueryString["mod"] == "newnotice")
            {
                flushManager.MarkChanged(CategoryHolder.ModelEnum.Notice, FlushManager.StatusEnum.New);
                PivotControl.SelectedItem = NoticePivotItem;
                flushManager.CheckFlush(CategoryHolder.ModelEnum.Notice);
            }
            else
            {
                PivotControl.SelectedItem = NoticePivotItem;
            }
            // 刷新磁贴
            Service.ShellTileService.Instance.UpdateOnPullRefreash();
            // 回调父方法
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 获取当前pivotitem
            var selectedPivotItem = ((sender as Pivot).SelectedItem as PivotItem);
            // 手动设置Bar的背景色为主题橙色
            ApplicationBar.BackgroundColor = Themes.ThemeAddition.OrangeColor;
            // 动态设置ApplicationoBar的可见性
            ApplicationBar.IsVisible = selectedPivotItem != OptionPivotItem;
            // pivotitem可见时检查是否需要进行内容操作
            if (selectedPivotItem == NewsPivotItem)
            {
                flushManager.CheckFlush(CategoryHolder.ModelEnum.News);
            }
            else if (selectedPivotItem == NoticePivotItem)
            {
                flushManager.CheckFlush(CategoryHolder.ModelEnum.Notice);
            }
            else if (selectedPivotItem == ZdezPivotItem)
            {
                flushManager.CheckFlush(CategoryHolder.ModelEnum.Zdez);
            }
        }

        private void PageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = sender as ListBox;
            if (box.SelectedItem != null)
            {
                var item = box.SelectedItem as MsgItem;
                Service.LocalService.MarkMsgReaded(item);
                Service.ShellTileService.Instance.UpdateOnReadStatusChanged();
                App.NavigateParam = item;
                NavigationService.Navigate(new Uri("/Action/Detail.xaml", UriKind.Relative));
                box.SelectedIndex = -1;
            }
        }

        private void PullRefreashSupportor_PullRefreashTriggeredEvent(PullRefreashSupportor sender, PullRefreashSupportor.ActionEnum action)
        {
            flushManager.MarkChanged(sender.Model, action == PullRefreashSupportor.ActionEnum.Top ? FlushManager.StatusEnum.New : FlushManager.StatusEnum.Old);
            flushManager.CheckFlush(sender.Model);
        }

        private void ApplicationBarRefreashBtn_Click(object sender, EventArgs e)
        {
            var pivot = PivotControl.SelectedItem as PivotItem;
            if (pivot != null && pivot != OptionPivotItem)
            {
                if (pivot == NewsPivotItem && !ContentBgWorker_News.IsBusy)
                {
                    NewsListBox.ItemsSource = new ObservableCollection<MsgItem>();
                    flushManager.MarkChanged(CategoryHolder.ModelEnum.News, FlushManager.StatusEnum.NewAndOld);
                    flushManager.CheckFlush(CategoryHolder.ModelEnum.News);
                }
                else if (pivot == NoticePivotItem && !ContentBgWorker_Notice.IsBusy)
                {
                    NoticeListBox.ItemsSource = new ObservableCollection<MsgItem>();
                    flushManager.MarkChanged(CategoryHolder.ModelEnum.Notice, FlushManager.StatusEnum.NewAndOld);
                    flushManager.CheckFlush(CategoryHolder.ModelEnum.Notice);
                }
                else if (pivot == ZdezPivotItem && !ContentBgWorker_Zdez.IsBusy)
                {
                    ZdezListBox.ItemsSource = new ObservableCollection<MsgItem>();
                    flushManager.MarkChanged(CategoryHolder.ModelEnum.Zdez, FlushManager.StatusEnum.NewAndOld);
                    flushManager.CheckFlush(CategoryHolder.ModelEnum.Zdez);
                }
            }
        }

        #region ContentBgWorker

        private BackgroundWorker ContentBgWorker_News;
        private BackgroundWorker ContentBgWorker_Notice;
        private BackgroundWorker ContentBgWorker_Zdez;

        private static void ContentBgWorker_BeforeWork(CategoryHolder.Items category, FlushManager.StatusEnum status, FlushManager flushManager, Dispatcher dispatcher)
        {
            // 在page间跳转和app状态变化时，之前的工作状态将被保留
            // TODO: 实现跳转时的后台任务取消和恢复，以确保程序可控
            if (!category.BackgroundWorker.IsBusy)
            {
                System.Diagnostics.Debug.WriteLine("start content work");
                // UI切换
                category.PullRefreashSupportor.IsBusy = true;
                category.ProgressBar.Visibility = Visibility.Visible;
                // 读取当前列表数据
                var currentSource = category.ListBox.ItemsSource;
                // 启动任务
                category.BackgroundWorker.RunWorkerAsync(new Toolkit.DictWrapper().OR("Category", category).OR("Status", status).OR("Source", currentSource).OR("FlushManager", flushManager).OR("Dispatcher", dispatcher));
            }
        }

        private static object ContentBgWorker_Locker = new object();

        private static void ContentBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 利用线程锁排队，同一时间只有一个 worker 处于重负载工作状态
            // TODO: 不使用多个worker，仅使用一个，不同的任务由任务队列实现
            lock (ContentBgWorker_Locker)
            {
                // 同步数据对象
                var argument = (IDictionary<string, object>)e.Argument;
                var category = (CategoryHolder.Items)argument["Category"];
                var source = (ObservableCollection<MsgItem>)argument["Source"];
                var status = (FlushManager.StatusEnum)argument["Status"];
                var flushManager = (FlushManager)argument["FlushManager"];
                var dispatcher = (Dispatcher)argument["Dispatcher"];
                var isError = false;
                // 加载历史信息：读取数据库、生成result
                object oldMsg = null;
                if ((status & FlushManager.StatusEnum.Old) != 0)
                {
                    try
                    {
                        if (category.ModelEnum == CategoryHolder.ModelEnum.News)
                        {
                            oldMsg = LocalService.GetNewsMsg((IEnumerable<MsgItem>)source);
                        }
                        else if (category.ModelEnum == CategoryHolder.ModelEnum.Notice)
                        {
                            oldMsg = LocalService.GetNoticeMsg((IEnumerable<MsgItem>)source);
                        }
                        else if (category.ModelEnum == CategoryHolder.ModelEnum.Zdez)
                        {
                            oldMsg = LocalService.GetZdezMsg((IEnumerable<MsgItem>)source);
                        }
                    }
                    catch (Exception e___)
                    {
                        flushManager.MarkChanged(category.ModelEnum, FlushManager.StatusEnum.Old);
                        System.Diagnostics.Debug.WriteLine("inter process or backgroundworder ERROR");
                        System.Diagnostics.Debug.WriteLine(e___.Message);
                        isError = true;
                    }
                }
                // 加载新信息：下载内容、写入数据库、生成Result
                object newMsg = null;
                if (DeviceNetworkInformation.IsNetworkAvailable &&  (status & FlushManager.StatusEnum.New) != 0)
                {
                    try
                    {
                        if (category.ModelEnum == CategoryHolder.ModelEnum.News)
                        {
                            var r = RemoteService.GetNewsMsg();
                            if (r.Status && r.Data != null)
                            {
                                newMsg = LocalService.FlushNewsMsg((IEnumerable<MsgItem>)r.Data);
                                RemoteService.MarkNewsMsgReceived(((IEnumerable<MsgItem>)r.Data).Map<MsgItem, string>(item => (item.Id + "")));
                            }
                        }
                        else if (category.ModelEnum == CategoryHolder.ModelEnum.Notice)
                        {
                            var r = RemoteService.GetSchoolMsg();
                            if (r.Status && r.Data != null)
                            {
                                newMsg = LocalService.FlushNoticeMsg((IEnumerable<MsgItem>)r.Data);
                                RemoteService.MarkSchoolMsgReceived(((IEnumerable<MsgItem>)r.Data).Map<MsgItem, string>(item => (item.Id + "")));
                            }
                        }
                        else if (category.ModelEnum == CategoryHolder.ModelEnum.Zdez)
                        {
                            var r = RemoteService.GetZdezMsg();
                            if (r.Status && r.Data != null)
                            {
                                newMsg = LocalService.FlushZdezMsg((IEnumerable<MsgItem>)r.Data);
                                RemoteService.MarkZdezMsgReceived(((IEnumerable<MsgItem>)r.Data).Map<MsgItem, string>(item => (item.Id + "")));
                            }
                        }
                    }
                    catch (Exception e___)
                    {
                        flushManager.MarkChanged(category.ModelEnum, FlushManager.StatusEnum.New);
                        System.Diagnostics.Debug.WriteLine("inter process or backgroundworder ERROR");
                        System.Diagnostics.Debug.WriteLine(e___.Message);
                        isError = true;
                    }
                }
                e.Result = new Toolkit.DictWrapper().OR("ISERROR", isError).OR("Category", category).OR("OldMsg", oldMsg).OR("NewMsg", newMsg).OR("FlushManager", flushManager).OR("Dispatcher", dispatcher);
            }
        }

        private static void ContentBgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 同步数据对象
            var result = (IDictionary<string, object>)e.Result;
            var isError = (bool)result["ISERROR"];
            var category = (CategoryHolder.Items)result["Category"];
            var oldMsg = (IEnumerable<MsgItem>)result["OldMsg"];
            var newMsg = (IEnumerable<MsgItem>)result["NewMsg"];
            var flushManager = (FlushManager)result["FlushManager"];
            var dispatcher = (Dispatcher)result["Dispatcher"];
            // 更新ListBox
            var source = category.ListBox.ItemsSource as ObservableCollection<MsgItem>;
            if (oldMsg != null)
            {
                if (oldMsg.IsEmpty())
                {
                    oldMsg = null;
                }
                else
                {
                    if (category.ModelEnum == CategoryHolder.ModelEnum.News)
                    {
                        foreach (var item in (IEnumerable<MsgItem>)oldMsg)
                            source.Add(item);
                    }
                    else if (category.ModelEnum == CategoryHolder.ModelEnum.Notice)
                    {
                        foreach (var item in (IEnumerable<MsgItem>)oldMsg)
                            source.Add(item);
                    }
                    else if (category.ModelEnum == CategoryHolder.ModelEnum.Zdez)
                    {
                        foreach (var item in (IEnumerable<MsgItem>)oldMsg)
                            source.Add(item);
                    }
                }
            }
            if (newMsg != null)
            {
                if (newMsg.IsEmpty())
                {
                    newMsg = null;
                }
                else
                {
                    if (category.ModelEnum == CategoryHolder.ModelEnum.News)
                    {
                        foreach (var item in (IEnumerable<MsgItem>)newMsg)
                            source.Insert(0, item);
                    }
                    else if (category.ModelEnum == CategoryHolder.ModelEnum.Notice)
                    {
                        foreach (var item in (IEnumerable<MsgItem>)newMsg)
                            source.Insert(0, item);
                    }
                    else if (category.ModelEnum == CategoryHolder.ModelEnum.Zdez)
                    {
                        foreach (var item in (IEnumerable<MsgItem>)newMsg)
                            source.Insert(0, item);
                    }
                    // 发现新信息，继续加载新信息
                    if (isError)
                    {
                        flushManager.MarkChanged(category.ModelEnum, FlushManager.StatusEnum.New);
                    }
                    else
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(param =>
                            {
                                System.Threading.Thread.Sleep(200);
                                ((Dispatcher)(((object[])param)[2])).BeginInvoke(() =>
                                    {
                                        var fm = (FlushManager)((object[])param)[0];
                                        var mode = (CategoryHolder.ModelEnum)((object[])param)[1];
                                        fm.MarkChanged(mode, FlushManager.StatusEnum.New);
                                        fm.CheckFlush(mode);
                                    });
                            }), new object[] { flushManager, category.ModelEnum, dispatcher });
                    }
                }
            }
            // 过程中发生错误
            if (isError)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(param =>
                {
                    System.Threading.Thread.Sleep(200);
                    ((Dispatcher)(((object[])param)[2])).BeginInvoke(() =>
                    {
                        var fm = (FlushManager)((object[])param)[0];
                        var mode = (CategoryHolder.ModelEnum)((object[])param)[1];
                        fm.MarkChanged(mode, FlushManager.StatusEnum.NewAndOld);
                        fm.CheckFlush(mode);
                    });
                }), new object[] { flushManager, category.ModelEnum, dispatcher });
            }
            // 内容发生变化
            if (oldMsg != null || newMsg != null)
            {
                // 通知频道内容变化
                if (category.ModelEnum == CategoryHolder.ModelEnum.Notice)
                {
                    // 更新磁贴通知
                    Service.ShellTileService.Instance.UpdateOnPullRefreash();
                }
                // 资讯频道内容变化
                else if (category.ModelEnum == CategoryHolder.ModelEnum.News)
                {
                    // TODO: 处理置顶功能
                }
            }
            // UI切换
            category.ProgressBar.Visibility = Visibility.Collapsed;
            category.PullRefreashSupportor.IsBusy = false;
        }

        #endregion

        private CategoryHolder categoryHolder;
        private FlushManager flushManager;

        class CategoryHolder
        {

            public enum ModelEnum
            {
                News,
                Notice,
                Zdez
            }

            public class Items
            {
                public ModelEnum ModelEnum { get; private set; }

                public ListBox ListBox { get; private set; }
                public ProgressBar ProgressBar { get; private set; }

                public PullRefreashSupportor PullRefreashSupportor { get; private set; }

                public BackgroundWorker BackgroundWorker { get; private set; }

                public Items(ModelEnum model, ListBox listbox, ProgressBar progressbar, PullRefreashSupportor pr, BackgroundWorker backgroundworker)
                {
                    this.ModelEnum = model;
                    this.ListBox = listbox;
                    this.ProgressBar = progressbar;
                    this.PullRefreashSupportor = pr;
                    this.BackgroundWorker = backgroundworker;
                }
            }

            private IDictionary<ModelEnum, Items> data;

            public CategoryHolder(
                ListBox newsListBox, ProgressBar newsProgressBar, PullRefreashSupportor newsPullRefreash, BackgroundWorker newsBackgroundWorker,
                ListBox noticeListBox, ProgressBar noticeProgressBar, PullRefreashSupportor noticePullRefreash, BackgroundWorker noticeBackgroundWorker,
                ListBox zdezListBox, ProgressBar zdezProgressBar, PullRefreashSupportor zdezPullRefreash, BackgroundWorker zdezBackgroundWorker

                )
            {
                data = new Dictionary<ModelEnum, Items>(3);
                data.Add(ModelEnum.News, new Items(ModelEnum.News, newsListBox, newsProgressBar, newsPullRefreash, newsBackgroundWorker));
                data.Add(ModelEnum.Notice, new Items(ModelEnum.Notice, noticeListBox, noticeProgressBar, noticePullRefreash, noticeBackgroundWorker));
                data.Add(ModelEnum.Zdez, new Items(ModelEnum.Zdez, zdezListBox, zdezProgressBar, zdezPullRefreash, zdezBackgroundWorker));
            }

            public Items Get(ModelEnum model)
            {
                return data[model];
            }

        }

        class FlushManager
        {

            [Flags]
            public enum StatusEnum
            {
                OK = 0x0,
                New = 0x1,
                Old = 0x2,
                NewAndOld = 0x1 | 0x2
            }

            private IDictionary<CategoryHolder.ModelEnum, StatusEnum> data;
            private Action<CategoryHolder.ModelEnum, StatusEnum> flushAction;

            public FlushManager(Action<CategoryHolder.ModelEnum, StatusEnum> flushAction)
            {

                this.flushAction = flushAction;
                data = new Dictionary<CategoryHolder.ModelEnum, StatusEnum>(3);
                data[CategoryHolder.ModelEnum.News] = StatusEnum.OK;
                data[CategoryHolder.ModelEnum.Notice] = StatusEnum.OK;
                data[CategoryHolder.ModelEnum.Zdez] = StatusEnum.OK;
            }

            public void MarkChanged(CategoryHolder.ModelEnum modelEnum, StatusEnum status)
            {
                var source = data[modelEnum];
                if (source != status)
                {
                    data[modelEnum] = status | source;
                }
            }

            public void CheckFlush(CategoryHolder.ModelEnum modelEnum)
            {
                var status = data[modelEnum];
                if (status != StatusEnum.OK)
                {
                    data[modelEnum] = StatusEnum.OK;
                    flushAction(modelEnum, status);
                }
            }

        }

        class PullRefreashSupportor
        {

            public enum ActionEnum { Top, Bottom }

            public ListBox Listbox { get; private set; }
            public TextBlock Infobox { get; private set; }
            public CategoryHolder.ModelEnum Model { get; private set; }

            public bool IsBusy { get; set; }

            private ScrollViewer scrollviewer;


            public PullRefreashSupportor(ListBox listbox, TextBlock infobox, CategoryHolder.ModelEnum modelenum)
            {
                this.Listbox = listbox;
                this.Infobox = infobox;
                this.Model = modelenum;

                listbox.MouseMove += listbox_MouseMove;
                listbox.ManipulationStarted += listbox_ManipulationStarted;
                listbox.ManipulationCompleted += listbox_ManipulationCompleted;
            }

            private double _invokePostition;
            private double _lastPostion;
            private ActionEnum? _action;

            private void listbox_MouseMove(object sender, MouseEventArgs e)
            {
                if (!IsBusy)
                {
                    _lastPostion = e.GetPosition(scrollviewer).Y;
                    if (_action == ActionEnum.Top)
                    {
                        var offset = _lastPostion - _invokePostition;
                        if (offset < 50)
                        {
                            Infobox.Text = "";
                        }
                        else if (offset >= 50 && offset < 100)
                        {
                            Infobox.Text = "下拉刷新";
                        }
                        else if (offset >= 100)
                        {
                            Infobox.Text = "释放后开始刷新";
                        }
                    }
                    else if (_action == ActionEnum.Bottom)
                    {
                        var offset = _invokePostition - _lastPostion;
                        if (offset < 50)
                        {
                            Infobox.Text = "";
                        }
                        else if (offset >= 50 && offset < 100)
                        {
                            Infobox.Text = "上滑加载历史信息";
                        }
                        else if (offset >= 100)
                        {
                            Infobox.Text = "释放后加载历史信息";
                        }
                    }
                }
            }

            private void listbox_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
            {
                if (!IsBusy)
                {
                    if (scrollviewer == null)
                    {
                        // 只有在控件渲染完后才能获取到
                        scrollviewer = Toolkit.GetChildUIElement<ScrollViewer>(Listbox);
                    }
                    _invokePostition = e.ManipulationOrigin.Y;
                    if (IsListOnTop)
                    {
                        _action = ActionEnum.Top;
                        Infobox.Text = "";
                        Infobox.VerticalAlignment = VerticalAlignment.Top;
                        Infobox.Visibility = Visibility.Visible;
                    }
                    else if (IsListOnBottom)
                    {
                        _action = ActionEnum.Bottom;
                        Infobox.Text = "";
                        Infobox.VerticalAlignment = VerticalAlignment.Bottom;
                        Infobox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _action = null;
                    }
                }
                e.Handled = true;
            }

            private void listbox_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
            {
                if (!IsBusy)
                {
                    if (_action != null)
                    {
                        Infobox.Visibility = Visibility.Collapsed;
                        if (_action == ActionEnum.Top)
                        {
                            var offset = _lastPostion - _invokePostition;
                            if (offset >= 100)
                            {
                                IsBusy = true;
                                if (PullRefreashTriggeredEvent != null)
                                    PullRefreashTriggeredEvent(this, _action.Value);
                            }
                        }
                        else if (_action == ActionEnum.Bottom)
                        {
                            var offset = _invokePostition - _lastPostion;
                            if (offset >= 100)
                            {
                                IsBusy = true;
                                if (PullRefreashTriggeredEvent != null)
                                    PullRefreashTriggeredEvent(this, _action.Value);
                            }
                        }
                        _action = null;
                    }
                }
            }

            private bool IsListOnTop
            {
                get { return scrollviewer.VerticalOffset < 0.2; }
            }

            private bool IsListOnBottom
            {
                get { return Math.Abs(scrollviewer.ScrollableHeight - scrollviewer.VerticalOffset) < 0.2; }
            }

            public delegate void PullRefreashTriggeredHandler(PullRefreashSupportor sender, ActionEnum action);
            public event PullRefreashTriggeredHandler PullRefreashTriggeredEvent;

        }

    }
}