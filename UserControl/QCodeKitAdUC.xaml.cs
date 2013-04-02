using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Com.MSN.Ads.WindowsPhone7.SDK.View;

namespace DuoDuoPhotoGallery
{
    public partial class QCodeKitAdUC : UserControl
    {
        private bool _isSmartMadShowLastTime = true;
        private readonly string AdShowKey = "AdShowKey";
        private PhoneApplicationPage _curParentPage = null;

        public QCodeKitAdUC(PhoneApplicationPage parentPage = null)
        {
            _curParentPage = parentPage;
            InitializeComponent();
            this.Loaded += QCodeKitAdUC_Loaded;
            this.Unloaded += QCodeKitAdUC_Unloaded;
        }
        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void QCodeKitAdUC_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLastAdState();
            _isSmartMadShowLastTime = !_isSmartMadShowLastTime;
            if (!_isSmartMadShowLastTime)
            {
                //smartMadAdView.Visibility = System.Windows.Visibility.Collapsed;
                //LoadMSNAdControl();
            }
        }
        /// <summary>
        /// 卸载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void QCodeKitAdUC_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveCurAdState();
        }
        /// <summary>
        /// 获取上次显示的广告控件信息
        /// </summary>
        private void LoadLastAdState()
        {
            try
            {
                _isSmartMadShowLastTime = QCodeKit.Utilities.ApplicationSettings.GetSetting<bool>(AdShowKey);
            }
            catch (Exception ex)
            {
                _isSmartMadShowLastTime = false;
            }
        }
        /// <summary>
        /// 存储本地显示的广告控件状态信息
        /// </summary>
        private void SaveCurAdState()
        {
            try
            {
                QCodeKit.Utilities.ApplicationSettings.SetSetting(AdShowKey, _isSmartMadShowLastTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 加载MSN广告控件
        /// </summary>
        private void LoadMSNAdControl()
        {
            if (_curParentPage != null)
            {
                string adSpaceId = "MNDAyfDYwMnwxOTA0fDE=";
                Thickness adPosition = new Thickness();//定义广告banner的位置。
                adPosition.Left = 0;
                adPosition.Top = 0;
                adPosition.Right = 0;
                adPosition.Bottom = 0;
                AdControl adControl = new AdControl(_curParentPage);
                adControl.InitAd(adPosition, adSpaceId);//初始化广告banner
                Grid.SetRow(adControl, 0);
                LayoutRoot.Children.Add(adControl);//注意：不要忘了将广告banner加到应用程序中。
                adControl.LoadAd();//开始请求广告
            }
        }
        /// <summary>
        /// Tap事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
    }
}
