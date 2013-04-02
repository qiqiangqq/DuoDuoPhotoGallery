using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using DuoDuoPhotoGallery.ViewModels;
using DuoDuoParserLib.Parser;
using System.Threading;
using DuoDuoParserLib.Model;
using System.Windows.Controls.Primitives;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;

namespace DuoDuoPhotoGallery.Views
{
    public partial class DDPhotoGalleryPage : PhoneApplicationPage
    {
        /// <summary>
        /// 当前的类型信息
        /// </summary>
        private DDPhotoGallery _curGallery = null;
        private DDPhotoDetailPageViewModel _channelVM = null;
        private ScrollViewer _galleryScrollViewer = null;
        private ScrollBar _galleryScrollBar = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DDPhotoGalleryPage()
        {
            InitializeComponent();
            this.Loaded += DDPhotoGalleryPage_Loaded;
            this.Unloaded += DDPhotoGalleryPage_Unloaded;
        }
        /// <summary>
        /// 卸载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDPhotoGalleryPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_galleryScrollViewer != null)
            {
                _galleryScrollViewer = null;
            }

            if (_galleryScrollBar != null)
            {
                _galleryScrollBar.ValueChanged -= _galleryScrollBar_ValueChanged;
                _galleryScrollBar = null;
            }
        }
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDPhotoGalleryPage_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }
        /// <summary>
        /// 导航到该页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _curGallery = AppViewModel.Instance.CurGallery;
        }
        /// <summary>
        /// 离开该页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((arg) =>
                {
                    if (_channelVM != null)
                    {
                        try
                        {
                            _channelVM.SavePageData();
                        }
                        catch (Exception)
                        { }
                    }
                });
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (_channelVM == null && _curGallery != null)
            {
                _channelVM = new DDPhotoDetailPageViewModel(_curGallery.CurType);
                _channelVM.ChannelDataLoadCompleted += DataLoadCompleted;
            }
            loadinUC.Visibility = System.Windows.Visibility.Visible;
            loadinUC.ResetContent();
            ///数据后台线程加载
            ThreadPool.QueueUserWorkItem((arg) =>
                {
                    this.Dispatcher.BeginInvoke(() =>
                        {
                            _channelVM.LoadPageData();
                        });
                });
        }
        /// <summary>
        /// 初始化滚动条
        /// </summary>
        private void InitDragControls()
        {
            if (_galleryScrollViewer == null)
            {
                _galleryScrollViewer = QCodeKit.Extensions.UI.VisualTreeHelperExtensions.FindVisualChild<ScrollViewer>(this.detailList);
                if (_galleryScrollViewer != null)
                {
                    _galleryScrollBar = QCodeKit.Extensions.UI.VisualTreeHelperExtensions.FindVisualChild<ScrollBar>(_galleryScrollViewer);
                    if (_galleryScrollBar != null)
                    {
                        _galleryScrollBar.ValueChanged += _galleryScrollBar_ValueChanged;
                    }
                }
            }
        }

        void _galleryScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            object valueObj = scrollBar.GetValue(ScrollBar.ValueProperty);
            object maxObj = scrollBar.GetValue(ScrollBar.MaximumProperty);
            if (valueObj != null && maxObj != null)
            {
                double value = (double)valueObj;
                double max = (double)maxObj;
                double offset = max - value;
                if (value >= max)
                {
                    loadinUC.Visibility = System.Windows.Visibility.Visible;
                    ///数据后台线程加载
                    this.Dispatcher.BeginInvoke(() =>
                        {
                            int curPage = _channelVM.LoadNextPageData();
                            loadinUC.IndicatorContent = string.Format("第{0}页的数据正在加载中...", curPage);
                        });
                }
            }
        }
        /// <summary>
        /// 数据结束完毕
        /// </summary>
        /// <param name="args"></param>
        private void DataLoadCompleted(ParseCompletedArgs args)
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    if (args.CurParseState == ParseState.EParseSuccess)
                    {
                        if (_channelVM != null)
                        {
                            LayoutRoot.DataContext = _channelVM.ChannelGallery;
                        }
                        InitDragControls();
                    }
                    else
                    {
                        ShowNetworkErrorReminder();
                    }
                    loadinUC.Visibility = System.Windows.Visibility.Collapsed;
                });
        }

        private void DDGalleryDataBindUC_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var bindUC = sender as DDGalleryDataBindUC;
            if (bindUC != null)
            {
                if (QCodeKit.Networking.DeviceNetworkHelper.IsDeviceNetworkAvailable())
                {
                    DDPhotoDetail detail = bindUC.PhotoDetail;
                    if (detail != null)
                    {
                        AppViewModel.Instance.CurDetail = detail;
                        AppViewModel.Instance.CurGallery = _curGallery;
                        this.NavigationService.Navigate(new Uri("/Views/DDPhotoDetailContentPage.xaml", UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                    ShowNetworkErrorReminder();
                }
            }
        }
        /// <summary>
        /// 网络错误提示
        /// </summary>
        private void ShowNetworkErrorReminder()
        {
            ToastPrompt toast = new ToastPrompt();
            toast.Title = "多多内涵吧";
            toast.Message = "网络不可用, 请稍后重试";
            toast.FontSize = 24;
            toast.Background = new SolidColorBrush(Color.FromArgb(255, 194, 141, 63));
            toast.Show();
        }
    }
}