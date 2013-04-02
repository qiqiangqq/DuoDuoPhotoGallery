using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Threading;

namespace DuoDuoPhotoGallery
{
    public partial class DDPhotoLoadingUC : UserControl
    {
        #region ColorFrom
        public static readonly DependencyProperty ColorFromProperty =
            DependencyProperty.Register("ColorFrom",
            typeof(Color),
            typeof(DDPhotoLoadingUC),
            new PropertyMetadata(Colors.White));
        /// <summary>
        /// 图片源地址
        /// </summary>
        public Color ColorFrom
        {
            set { SetValue(ColorFromProperty, value); }
            get { return (Color)GetValue(ColorFromProperty); }
        }
        #endregion
        #region ColorEnd
        public static readonly DependencyProperty ColorEndProperty =
            DependencyProperty.Register("ColorEnd",
            typeof(Color),
            typeof(DDPhotoLoadingUC),
            new PropertyMetadata(Colors.White));
        /// <summary>
        /// 图片源地址
        /// </summary>
        public Color ColorEnd
        {
            set { SetValue(ColorEndProperty, value); }
            get { return (Color)GetValue(ColorEndProperty); }
        }
        #endregion
        #region IndicatorContent
        public static readonly DependencyProperty IndicatorContentProperty =
            DependencyProperty.Register("IndicatorContent",
            typeof(string),
            typeof(DDPhotoLoadingUC),
            new PropertyMetadata("内容正在努力加载中...", IndicatorContentChanged));
        /// <summary> 
        /// 图片源地址
        /// </summary>
        public string IndicatorContent
        {
            set { SetValue(IndicatorContentProperty, value); }
            get { return (string)GetValue(IndicatorContentProperty); }
        }
        #endregion
        private GradientStop _fromChanger = null;
        private GradientStop _toChanger = null;
        private DispatcherTimer _gradientChaner = null;
        /// <summary>
        /// 准备渐变色
        /// </summary>
        private void InitGradientColor()
        {
            var linerGB = new LinearGradientBrush();
            linerGB.StartPoint = new Point(0, 0);
            linerGB.EndPoint = new Point(1, 1);
            linerGB.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
            linerGB.GradientStops = new GradientStopCollection();
            _fromChanger = new GradientStop();
            _fromChanger.Color = ColorFrom;
            _fromChanger.Offset = 0;
            linerGB.GradientStops.Add(_fromChanger);

            _toChanger = new GradientStop();
            _toChanger.Color = ColorEnd;
            _toChanger.Offset = 1;
            linerGB.GradientStops.Add(_toChanger);
            txtShow.Foreground = linerGB;
        }
        /// <summary>
        /// 构造
        /// </summary>
        public DDPhotoLoadingUC()
        {
            InitializeComponent();
            this.Loaded += DDPhotoLoadingUC_Loaded;
            this.Unloaded += DDPhotoLoadingUC_Unloaded;
        }
        /// <summary>
        /// 重置提示的内容为初始值
        /// </summary>
        public void ResetContent()
        {
            IndicatorContent = "内容正在努力加载中...";
        }
        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDPhotoLoadingUC_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_gradientChaner != null)
            {
                _gradientChaner.Stop();
                _gradientChaner = null;
            }
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDPhotoLoadingUC_Loaded(object sender, RoutedEventArgs e)
        {
            txtShow.Text = IndicatorContent;
            InitGradientColor();
            InitColorChanger();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitColorChanger()
        {
            if (_gradientChaner == null)
            {
                _gradientChaner = new DispatcherTimer();  //创建定时器
                _gradientChaner.Interval = TimeSpan.FromMilliseconds(100);    //设定间隔为0.2秒
                _gradientChaner.Tick +=  Chanager_Tick;
            }
            _gradientChaner.Start();     //开启定时器
        }
        /// <summary>
        /// 事件轮询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Chanager_Tick(object sender, object e)
        {
            if (_fromChanger.Offset >= 1.0)
            {
                _fromChanger.Offset = 0.0;
            }

            if (_fromChanger != null)
            {
                _fromChanger.Offset += 0.1;
            }
        }
        /// <summary>
        /// 提示内容改变事件
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        static void IndicatorContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DDPhotoLoadingUC uc = d as DDPhotoLoadingUC;
            if (uc != null)
            {
                uc.SetIndicatorContent(e.NewValue);
            }
        }
        /// <summary>
        /// 设置提示内容
        /// </summary>
        /// <param name="content"></param>
        void SetIndicatorContent(object content)
        {
            string strContent = content as string;
            if (!string.IsNullOrEmpty(strContent))
            {
                txtShow.Text = strContent;
            }
        }
    }
}
