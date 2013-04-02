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
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace DuoDuoPhotoGallery
{
    public partial class DDPhotoImageUC : UserControl
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource",
            typeof(string),
            typeof(DDPhotoImageUC),
            new PropertyMetadata(string.Empty, OnImageSourceChanged));
        /// <summary>
        /// 图片源地址
        /// </summary>
        public string ImageSource
        {
            set { SetValue(ImageSourceProperty, value); }
            get { return (string)GetValue(ImageSourceProperty); }
        }
        /// <summary>
        /// 图片源地址改变
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as DDPhotoImageUC;
            if (uc != null)
            {
                uc.InitData();
            }
        }
        /// <summary>
        /// 是否显示加载进度提示
        /// </summary>
        private bool _isShowImgLoadingIndicator = true;
        public bool IsShowImgLoadingIndicator
        {
            get
            {
                return _isShowImgLoadingIndicator;
            }
            set
            {
                _isShowImgLoadingIndicator = value;
            }
        }
        /// <summary>
        /// 成员
        /// </summary>
        private DDPhotoImageUCHelper _imgHelper = null;
        private BitmapImage _imgSource = null;
        /// <summary>
        /// 图片原始对象
        /// </summary>
        public BitmapImage ImgSource
        {
            get
            {
                return _imgSource;
            }
        }
        private Storyboard _indicatorSb = null;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DDPhotoImageUC()
        {
            InitializeComponent();
            this.Loaded += DDPhotoImageUC_Loaded;
        }
        /// <summary>
        /// 图像加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDPhotoImageUC_Loaded(object sender, RoutedEventArgs e)
        {
            if (_imgSource == null)
            {
                InitData();
            }
            rootImgBrush.ImageSource = _imgSource;
            if (_imgSource != null)
            {
                coverRect.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            if (_imgHelper == null)
            {
                _imgHelper = new DDPhotoImageUCHelper();
            }
            if (IsShowImgLoadingIndicator)
            {
                _imgHelper.ImgDownloadProgressChanged += ImageSourceDownloadProgressChanged;
                _imgHelper.ImgDownloadCompleted += ImageSourceDownloadCompleted;
            }
            _imgHelper.ImgUrl = ImageSource;
            _imgSource = _imgHelper.GetImgSource();
        }
        /// <summary>
        /// 显示图片下载进度
        /// </summary>
        private void ShowImgLoadingIndicator()
        {
            if (IsShowImgLoadingIndicator)
            {
                if (_indicatorSb == null)
                {
                    _indicatorSb = this.Resources["IndicatorSb"] as Storyboard;
                }
                indicatorPanel.Visibility = System.Windows.Visibility.Visible;
                _indicatorSb.Begin();
            }
        }
        /// <summary>
        /// 图片下载进度变化
        /// </summary>
        /// <param name="progress"></param>
        private void ImageSourceDownloadProgressChanged(string progress)
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    indicatorTxt.Text = string.Format("正在加载:{0}",progress);
                });
        }
        /// <summary>
        /// 图片下载完毕
        /// </summary>
        private void ImageSourceDownloadCompleted()
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    if (IsShowImgLoadingIndicator)
                    {
                        if (_indicatorSb != null)
                        {
                            _indicatorSb.Stop();
                            indicatorPanel.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                });
        }
    }
}
