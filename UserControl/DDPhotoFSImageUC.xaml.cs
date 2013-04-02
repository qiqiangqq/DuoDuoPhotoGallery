using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Media;
using DuoDuoPhotoGallery.ViewModels;
using QCodeKit.Utilities;

namespace DuoDuoPhotoGallery
{
    /// <summary>
    /// 大图浏览
    /// </summary>
    public partial class DDPhotoFSImageUC : UserControl
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource",
            typeof(string),
            typeof(DDPhotoFSImageUC),
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
            var uc = d as DDPhotoFSImageUC;
            if (uc != null)
            {
                uc.InitData();
            }
        }
        /// <summary>
        /// 设置及获取图片下载的进度
        /// </summary>
        private string _imgDownloadProgress
        {
            set
            {
                string progress = string.Format("内容已加载:{0}",value);
                indicatorTxt.Text = progress;
            }
        }
        /// <summary>
        /// 是否显示加载进度提示
        /// </summary>
        public bool IsShowImgLoadingIndicator
        {
            get
            {
                return (bool)GetValue(IsShowImgLoadingIndicatorProperty);
            }
            set
            {
                SetValue(IsShowImgLoadingIndicatorProperty, value);
            }
        }
        public static readonly DependencyProperty IsShowImgLoadingIndicatorProperty =
            DependencyProperty.Register("IsShowImgLoadingIndicator",
            typeof(bool),
            typeof(DDPhotoFSImageUC),
            new PropertyMetadata(false, OnIsShowImgLoadingIndicatorChanged));
        private DDPhotoImageUCHelper _imgHelper = null;
        /// <summary>
        /// 图片源对象
        /// </summary>
        private BitmapImage _imgSource = null;
        public BitmapImage ImgSource
        {
            get
            {
                return _imgSource;
            }
        }
        /// <summary>
        /// 图片本地文件名
        /// </summary>
        public string ImgFileName
        {
            get
            {
                if (_imgHelper != null)
                {
                    return _imgHelper.ImageFileName;
                }
                return null;
            }
        }
        /// <summary>
        /// 图片路径名
        /// </summary>
        public string ImgPathName
        {
            get
            {
                if (_imgHelper != null)
                {
                    return _imgHelper.ImgFilePath;
                }
                return null;
            }
        }
        private CompositeTransform _imgTrans = null;
        private double _initialScale = 1.0;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DDPhotoFSImageUC()
        {
            InitializeComponent();
            InitTransform();
        }
        /// <summary>
        /// 设置图片的变形对象
        /// </summary>
        private void InitTransform()
        {
            if (_imgTrans == null)
            {
                _imgTrans = new CompositeTransform();
            }
            innerImg.RenderTransform = _imgTrans;
            innerImg.RenderTransformOrigin = new Point(0.5, 1.0);
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

            _imgHelper.ImgUrl = ImageSource;
            _imgSource = _imgHelper.GetImgSource();
            if (!_imgHelper.IsDataLoadFromWeb)
            {
                ImageSourceDownloadCompleted();
            }
            else
            {
                if (IsShowImgLoadingIndicator)
                {
                    _imgHelper.ImgDownloadProgressChanged += ImageSourceDownloadProgressChanged;
                    _imgHelper.ImgDownloadCompleted += ImageSourceDownloadCompleted;
                }
            }
        }
        /// <summary>
        /// 图片下载进度变化
        /// </summary>
        /// <param name="progress"></param>
        private void ImageSourceDownloadProgressChanged(string progress)
        {
            _imgDownloadProgress = progress;
        }
        /// <summary>
        /// 加载进度变化
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsShowImgLoadingIndicatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as DDPhotoFSImageUC;
            if (uc != null)
            {
                uc.SwitchLoading();
            }
        }
        /// <summary>
        /// 设置加载进度
        /// </summary>
        private void SwitchLoading()
        {
            if (IsShowImgLoadingIndicator)
            {
                loadingPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                loadingPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
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
                    CheckImgSize();
                    indicatorTxt.Text = "内容加载完成";
                    ShowLoading(false);
                }
            });
        }
        /// <summary>
        /// 显示加载进度
        /// </summary>
        private void ShowLoading(bool bShow)
        {
            if (bShow)
            {
                loadingPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                loadingPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 图片下载失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _innerBi_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            indicatorTxt.Text = "内容加载失败, 请检查网络后重试";
            ShowLoading(false);
        }
        /// <summary>
        /// 图片成功下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _innerBi_ImageOpened(object sender, RoutedEventArgs e)
        {
            CheckImgSize();
            indicatorTxt.Text = "内容加载完成";
            ShowLoading(false);
        }
        /// <summary>
        /// 确定图片空间的显示尺寸
        /// </summary>
        private void CheckImgSize()
        {
            if (_imgSource != null)
            {
                double imgWidth = _imgSource.PixelWidth;
                double imgHeight = _imgSource.PixelHeight;
                innerImg.Source = _imgSource;
            }
        }
        /// <summary>
        /// 手势操作开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _initialScale = _imgTrans.ScaleX;
        }
        /// <summary>
        /// 手势操作过程中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            var point = e.GetPosition(this);
            _imgTrans.ScaleX = _initialScale * e.DistanceRatio;
            _imgTrans.ScaleY = _initialScale * e.DistanceRatio;
        }
        /// <summary>
        /// 拖拽事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDrag_ManipulationDelta(object sender, DragDeltaGestureEventArgs e)
        {
            //moving along X axis
            _imgTrans.TranslateX += e.HorizontalChange;
            //moving along Y axis
            _imgTrans.TranslateY += e.VerticalChange;
        }
        /// <summary>
        /// 双点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDoubleTap(object sender, GestureEventArgs e)
        {
            _initialScale = 1.0;
            _imgTrans.ScaleX = _initialScale;
            _imgTrans.ScaleY = _initialScale;
            _imgTrans.TranslateX = 0;
            _imgTrans.TranslateY = 0;
        }
    }
}
