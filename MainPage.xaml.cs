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
using DuoDuoParserLib.Parser;
using DuoDuoPhotoGallery.ViewModels;
using Coding4Fun.Toolkit.Controls;
using System.Threading;
using QCodeKit.Utilities;

namespace DuoDuoPhotoGallery
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// MainPageViewModel
        /// </summary>
        private DDNHMainpageViewModel _mainVM = null;
        private static int ClearCount = 0;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (_mainVM == null)
            {
                _mainVM = new DDNHMainpageViewModel();
            }
            _mainVM.MainPageDataLoadCompleted += DataLoadCompleted;
            loadinUC.Visibility = System.Windows.Visibility.Visible;
            /// 启动新线程 使得数据加载不会影响页面的展现s
            ThreadPool.QueueUserWorkItem((arg) =>
                {
                    _mainVM.LoadPageData();
                });
        }
        /// <summary>
        /// 数据加载完毕
        /// </summary>
        /// <param name="args"></param>
        private void DataLoadCompleted(ParseCompletedArgs args)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                /// 数据加载成功
                if (args.CurParseState == ParseState.EParseSuccess)
                {
                    FillDataToMetroFlow();
                    FillDataToOtherList();
                }
                else
                {
                    RemindNetworkError();
                }
                loadinUC.Visibility = System.Windows.Visibility.Collapsed;
            });
        }
        // 为 ViewModel 项加载数据
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            BindC4FData();
            if (_mainVM.IsLoadFolderSize)
            {
                LoadFolderSize();
            }
        }
        /// <summary>
        /// 获取占用的存储空间
        /// </summary>
        private void LoadFolderSize()
        {
            ThreadPool.QueueUserWorkItem((state) =>
                {
                    bool isRemind = false;
                    if (_mainVM != null)
                    {
                        double folderSize = _mainVM.GetDirectorySize(ref isRemind) / 1024;
                        if (isRemind)
                        {
                            if (folderSize >= 1)
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    folderSize = Math.Round(folderSize * 100) / 100.0;
                                    ToastPrompt toast = new ToastPrompt();
                                    toast.Message = string.Format("程序占用了{0}M存储空间, 请及时清理", folderSize);
                                    toast.FontSize = 24;
                                    toast.Background = new SolidColorBrush(Color.FromArgb(255, 194, 141, 63));
                                    toast.Show();
                                });
                            }
                        }
                    }
                });
        }
        /// <summary>
        /// 数据绑定
        /// </summary>
        private void BindC4FData()
        {
            if (_mainVM != null)
            {
                MorePanel.DataContext = _mainVM;
            }
        }
        /// <summary>
        /// 向MetroFlow控件填充数据
        /// </summary>
        private void FillDataToMetroFlow()
        {
            if (_mainVM.MainRecommandData != null)
            {
                recommadUC.Init(_mainVM.MainRecommandData);
            }
        }
        /// <summary>
        /// 向推荐的其他内容列表填充数据
        /// </summary>
        private void FillDataToOtherList()
        {
            if (_mainVM.MainRecommandOtherData != null)
            {
                recommandOtherList.ItemsSource = _mainVM.MainRecommandOtherData;
            }
        }
        /// <summary>
        /// 离开页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_mainVM != null)
            {
                ThreadPool.QueueUserWorkItem((arg) =>
                {
                      _mainVM.Deactivate();
                });
            }
        }
        /// <summary>
        /// 回到页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

        }
        /// <summary>
        /// SNS设置点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SnsAccountSetting_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/SocialBindPage.xaml", UriKind.RelativeOrAbsolute));
        }
        /// <summary>
        /// 推荐的其他内容点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecommandOtherClick(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var bindData = btn.Tag as DuoDuoParserLib.Model.DDPhotoDetail;
                if (bindData != null)
                {
                    if (QCodeKit.Networking.DeviceNetworkHelper.IsDeviceNetworkAvailable())
                    {
                        if (bindData != null)
                        {
                            AppViewModel.Instance.CurDetail = bindData;
                            //AppViewModel.Instance.CurGallery = _curGallery;
                            this.NavigationService.Navigate(new Uri("/Views/DDPhotoDetailContentPage.xaml", UriKind.RelativeOrAbsolute));
                        }
                    }
                    else
                    {
                        RemindNetworkError();
                    }
                }
            }
        }
        /// <summary>
        /// 提示网络失败
        /// </summary>
        private void RemindNetworkError()
        {
            ToastPrompt toast = new ToastPrompt();
            toast.Title = "多多内涵吧";
            toast.Message = "网络不可用, 请稍后重试";
            toast.FontSize = 24;
            toast.Background = new SolidColorBrush(Color.FromArgb(255, 194, 141, 63));
            toast.Show();
        }
        /// <summary>
        /// 清理缓存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearCache_Click(object sender, EventArgs e)
        {
            if (ClearCount == 0)
            {
                Thread delThread = new Thread(() =>
                    {
                        if (_mainVM != null)
                        {
                            _mainVM.ClearCache();
                            ClearCount++;
                        }

                        this.Dispatcher.BeginInvoke(() =>
                            {
                                ToastPrompt toast = new ToastPrompt();
                                toast.Title = "多多内涵吧";
                                toast.Message = "缓存清理完毕";
                                toast.FontSize = 24;
                                toast.Background = new SolidColorBrush(Color.FromArgb(255, 194, 141, 63));
                                toast.Show();
                            });
                    });
                delThread.Start();
            }
        }
        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutBtn_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/DDAboutPage.xaml", UriKind.Relative));
        }
        /// <summary>
        /// 评论应用程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentButton_Click(object sender, EventArgs e)
        {
            QCodeKit.Extensions.Environment.Marketplace.CommentApp("09fb14af-4dfe-49d8-a1bb-8127bdb1f236");
        }
    }
}