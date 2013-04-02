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
using System.Windows.Media.Animation;
using Alexis.WindowsPhone.Social;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using DuoDuoPhotoGallery.Helper;
using System.Windows.Controls.Primitives;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;

namespace DuoDuoPhotoGallery.Views
{
    public partial class DDPhotoDetailContentPage : PhoneApplicationPage
    {
        private DDDetailContentPageViewModel _dcVM = null;
        private bool _isShowIndicator = false;
        private bool _isDataArrange = false;
        private Storyboard _showPanelSb = null;
        private Storyboard _hidePanelSb = null;
        private ScrollViewer _imgListScrollViewer = null;
        private StackPanel _imgStackPanel = null;
        private Popup _popUp;
        /// <summary>
        ///  广告控件
        /// </summary>
        private QCodeKitAdUC _adUC = null;

        public DDPhotoDetailContentPage()
        {
            InitializeComponent();
            Init();
            this.Loaded += DDPhotoDetailContentPage_Loaded;
            this.SizeChanged += DDPhotoDetailContentPage_SizeChanged;
        }
        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDPhotoDetailContentPage_Loaded(object sender, RoutedEventArgs e)
        {
            loadinUC.Visibility = System.Windows.Visibility.Visible;
            _dcVM.LoadPageData();
            loadinUC.Visibility = System.Windows.Visibility.Collapsed;
            if (_dcVM.CurParsedContent == null && _dcVM.CurParsedContentList == null)
            {
                this.ApplicationBar.IsMenuEnabled = false;
            }
            else
            {
                EnableSnsShare();
            }
        }
        /// <summary>
        /// 设置社交分享功能
        /// </summary>
        private void EnableSnsShare()
        {
            this.ApplicationBar.IsMenuEnabled = true;
            if (this.ApplicationBar.MenuItems != null && this.ApplicationBar.MenuItems.Count > 0)
            {
                ApplicationBarMenuItem snsItem = this.ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
                if (snsItem != null)
                {
                    snsItem.IsEnabled = true;
                }
            }
        }
        /// <summary>
        /// SizeChange事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDPhotoDetailContentPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            imgUC.Width = e.NewSize.Width;
            imgUC.Height = e.NewSize.Height;
        }
        /// <summary>
        /// 数据解析完毕
        /// </summary>
        /// <param name="args"></param>
        private void DetailContentParsedCompleted(ParseCompletedArgs args)
        {
            /// 数据解析完毕
            if (args != null && args.CurParseState == ParseState.EParseSuccess)
            {
                EnableSnsShare();
                SetAdUC();
                if (_dcVM.IsSingle)
                {
                    imgUC.ImageSource = _dcVM.CurParsedContent.DetailContentImageWebUrl;
                    indicatorPanel.DataContext = _dcVM.CurPhotoDetail;
                    _isDataArrange = true;
                }
                else
                {
                    InitMultiImagePanel();
                    AssignMultiImg2Panel();
                    _isDataArrange = true;
                }
            }
            else
            {
                _isDataArrange = false;
                ShowReminder("网络不可用, 请稍后重试");
            }
        }
        /// <summary>
        /// 导航到该页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _dcVM.CurPhotoDetail = AppViewModel.Instance.CurDetail;
        }
        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            if (_isDataArrange && _dcVM != null && _dcVM.IsSingle)
            {
                if (_dcVM.CurPhotoDetail.PhotoUGC != null &&
                    _dcVM.CurPhotoDetail.PhotoUGC.IsValid)
                {
                    if (!_isShowIndicator)
                    {
                        ShowPanel();
                        EnableSnsShare();
                        _isShowIndicator = true;
                    }
                    else
                    {
                        HidePanel();
                        _isShowIndicator = false;
                    }
                }
            }
            base.OnTap(e);
        }
        /// <summary>
        /// Back键
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (_popUp != null && _popUp.IsOpen)
            {
                _popUp.IsOpen = false;
                e.Cancel = true;
                EnableSnsShare();
                return;
            }
            base.OnBackKeyPress(e);
        }
        /// <summary>
        /// 离开页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_popUp != null && _popUp.IsOpen)
            {
                _popUp.IsOpen = false;
            }
            base.OnNavigatedFrom(e);
        }
        /// <summary>
        /// 初始化多图情况
        /// </summary>
        private void InitMultiImagePanel()
        {
            if (_dcVM != null && !_dcVM.IsSingle)
            {
                ImgContentPanel.Children.Clear();
                if (_imgListScrollViewer == null)
                {
                    _imgListScrollViewer = new ScrollViewer();
                    _imgListScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    _imgListScrollViewer.Width = this.ActualWidth;
                    _imgListScrollViewer.Height = this.ActualHeight;
                }
                ImgContentPanel.Children.Add(_imgListScrollViewer);
                if (_imgStackPanel == null)
                {
                    _imgStackPanel = new StackPanel();
                    _imgStackPanel.Width = this.ActualWidth;
                    _imgStackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    _imgStackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                }
                _imgListScrollViewer.Content = _imgStackPanel;
            }
        }
        /// <summary>
        /// 将图片传入面板
        /// </summary>
        private void AssignMultiImg2Panel()
        {
            if (_dcVM != null && !_dcVM.IsSingle && _dcVM.CurParsedContentList != null 
                && _dcVM.CurParsedContentList.WebImgList != null)
            {
                foreach (var img in _dcVM.CurParsedContentList.WebImgList)
                {
                    DDPhotoFSImageUC imgUC = new DDPhotoFSImageUC();
                    imgUC.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    imgUC.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    imgUC.Margin = new Thickness(0, 0, 0, 10);
                    imgUC.ImageSource = img;
                    _imgStackPanel.Children.Add(imgUC);
                }
            }
        }
        /// <summary>
        /// 显示Panel
        /// </summary>
        private void ShowPanel()
        {
            if (_showPanelSb != null)
            {
                _showPanelSb.Stop();
                _showPanelSb.Begin();
            }
        }
        /// <summary>
        /// 隐藏Panel
        /// </summary>
        private void HidePanel()
        {
            if (_hidePanelSb != null)
            {
                _hidePanelSb.Stop();
                _hidePanelSb.Begin();
            }
        }
        /// <summary>
        /// 数据初始化
        /// </summary>
        private void Init()
        {
            if (_dcVM == null)
            {
                _dcVM = new DDDetailContentPageViewModel();
            }
            _dcVM.DetailContentParseCompleted -= DetailContentParsedCompleted;
            _dcVM.DetailContentParseCompleted += DetailContentParsedCompleted;
            _showPanelSb = this.Resources["ContentShowSb"] as Storyboard;
            _hidePanelSb = this.Resources["ContentHideSb"] as Storyboard;
        }
        /// <summary>
        /// 社交分享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarSNSShareMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ApplicationBarMenuItem;
            if (menuItem != null)
            {
                menuItem.IsEnabled = false;
                ShowShareControl();
            }
        }
        /// <summary>
        /// 图片保存到本地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePhotoToMediaLibMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUC.ImgSource != null)
            {
                var saveMenu = sender as ApplicationBarMenuItem;
                if (saveMenu != null)
                {
                    saveMenu.IsEnabled = false;
                }
                var saveResult = QCodeKit.Utilities.IsolatedStorageHelper.SaveImageToPictureLib(imgUC.ImgPathName, imgUC.ImgFileName);
                if (saveResult)
                {
                    ShowReminder("图片已经存储到本地");
                }
                if (saveMenu != null)
                {
                    saveMenu.IsEnabled = true;
                }
            }
        }
        /// <summary>
        /// 显示提示内容
        /// </summary>
        /// <param name="content"></param>
        private void ShowReminder(string content)
        {
            ToastPrompt toast = new ToastPrompt();
            toast.Message = content;
            toast.FontSize = 24;
            toast.Background = new SolidColorBrush(Color.FromArgb(255, 194, 141, 63));
            toast.Show();
        }
        /// <summary>
        /// 进行社交分享
        /// </summary>
        /// <param name="type"></param>
        private void DoSnsShare(SocialType type)
        {
            DDSocialShareViewModel.Instance.CurrentSocialType = type;
            bool isLogin = true;
            switch (type)
            {
                case SocialType.Weibo:
                    if (!(SocialAPI.WeiboAccessToken == null || SocialAPI.WeiboAccessToken.IsExpired))
                    {
                        isLogin = false; 
                    }
                    break;
                case SocialType.Tencent:
                    if (!(SocialAPI.TencentAccessToken == null || SocialAPI.TencentAccessToken.IsExpired))
                    {
                        isLogin = false;
                    }
                    break;
                case SocialType.Renren:
                    if (!(SocialAPI.RenrenAccessToken == null || SocialAPI.RenrenAccessToken.IsExpired))
                    {
                        isLogin = false;
                    }
                    break;
            }
            if (isLogin)
            {
                NavigationService.Navigate(new Uri("/Views/SocialLoginPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/SocialSendPage.xaml", UriKind.Relative));
            }
        }
        /// <summary>
        /// 截屏
        /// </summary>
        private void DoSnapShot()
        {
            if (imgUC.ImageSource != null)
            {
                try
                {
                    WriteableBitmap bitmap = new WriteableBitmap(imgUC.ImgSource);
                    bitmap.Invalidate();
                    DDSocialShareViewModel.Instance.CurrentSharedImage = bitmap;
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var stream = store.OpenFile(SocialConstants.SHARE_IMAGE, System.IO.FileMode.OpenOrCreate))
                        {
                            try
                            {
                                bitmap.SaveJpeg(stream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                catch (Exception ee)
                {

                }
            }
        }
        /// <summary>
        /// popup menu let user to choose
        /// </summary>
        private void ShowShareControl()
        {
            _popUp = new Popup
            {
                Height = 800,
                Width = 480,
            };

            ShareControl sc = new ShareControl();
            sc.Height = 800;
            sc.Width = 480;
            sc.TypeSelected = (p) =>
            {
                DoSnapShot();
                DoSnsShare(p);
            };

            _popUp.Child = sc;
            _popUp.IsOpen = true;
        }
        /// <summary>
        /// 设置广告控件
        /// </summary>
        private void SetAdUC()
        {
            if (_adUC == null)
            {
                _adUC = new QCodeKitAdUC(this);
                _adUC.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                _adUC.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _adUC.Margin = new Thickness(0);
            }
            try
            {
                if (!LayoutRoot.Children.Contains(_adUC))
                {
                    LayoutRoot.Children.Add(_adUC);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}