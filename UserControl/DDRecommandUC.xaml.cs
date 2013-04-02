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
using DuoDuoParserLib.Model;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace DuoDuoPhotoGallery
{
    /// <summary>
    /// 推荐内容控件
    /// </summary>
    public partial class DDRecommandUC : UserControl
    {
        private DDRecommandUCViewModel _ddVM = null;
        private DispatcherTimer _animationDriver = null;
        private readonly int DRIVEINTERNAL = 2200;
        private SolidColorBrush selectBrush; 
        private SolidColorBrush noneBrush;
        public DDRecommandUC()
        {
            InitializeComponent();
            selectBrush = new SolidColorBrush(Colors.Orange);
            noneBrush = new SolidColorBrush(Colors.LightGray);
            this.Loaded += DDRecommandUC_Loaded;
            this.Unloaded += DDRecommandUC_Unloaded;
        }
        /// <summary>
        /// 控件卸载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDRecommandUC_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_animationDriver != null)
            {
                _animationDriver.Tick -= _animationDriver_Tick;
                _animationDriver.Stop();
                _animationDriver = null;
            }
        }
        /// <summary>
        /// Load事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DDRecommandUC_Loaded(object sender, RoutedEventArgs e)
        {
        }
        /// <summary>
        /// 按下事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            var ret = VisualStateManager.GoToState(this, "Pressed", true);
        }
        /// <summary>
        /// 抬起事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            var ret = VisualStateManager.GoToState(this, "Released", true);
        }
        /// <summary>
        /// 鼠标指针离开事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                var ret = VisualStateManager.GoToState(this, "Released", true);
            }
            catch (Exception)
            { }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="detailList"></param>
        public void Init(IEnumerable<DDPhotoDetail> detailList)
        {
            if (detailList != null)
            {
                if (_ddVM == null)
                {
                    _ddVM = new DDRecommandUCViewModel();
                    _ddVM.CurSelectedChanged += DetailSelectChanged;
                    _ddVM.InitDataList(detailList);
                }
                backRect.Visibility = System.Windows.Visibility.Collapsed;
                LayoutRoot.DataContext = _ddVM;
                InitAnimation();
            }
        }
        /// <summary>
        /// 当前选择项改变
        /// </summary>
        private void DetailSelectChanged()
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    _animationDriver.Stop();
                    ChangeRectSelectState(false, _ddVM.PreIdx);
                    ChangeRectSelectState(true, _ddVM.CurIdx);
                    _animationDriver.Start();
                });
        }
        /// <summary>
        /// 改变矩形的选中颜色状态
        /// </summary>
        /// <param name="isCur"></param>
        /// <param name="idx"></param>
        private void ChangeRectSelectState(bool isCur, int idx)
        {
            switch (idx)
            {
                case 0:
                    {
                        if (isCur)
                        {
                            firstRect.Fill = selectBrush;
                        }
                        else
                        {
                            firstRect.Fill = noneBrush;
                        }
                        break;
                    }
                case 1:
                    {
                        if (isCur)
                        {
                            secondRect.Fill = selectBrush;
                        }
                        else
                        {
                            secondRect.Fill = noneBrush;
                        }
                        break;
                    }
                case 2:
                    {
                        if (isCur)
                        {
                            thirdRect.Fill = selectBrush;
                        }
                        else
                        {
                            thirdRect.Fill = noneBrush;
                        }
                        break;
                    }
            }
        }
        /// <summary>
        /// 初始化动画
        /// </summary>
        private void InitAnimation()
        {
            if (_animationDriver == null)
            {
                _animationDriver = new DispatcherTimer();
                _animationDriver.Interval = TimeSpan.FromMilliseconds(DRIVEINTERNAL);
                _animationDriver.Tick += _animationDriver_Tick;
            }
            _animationDriver.Stop();
            _animationDriver.Start();
        }
        /// <summary>
        /// Timer事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _animationDriver_Tick(object sender, EventArgs e)
        {
            if (_ddVM != null && _ddVM.RecommandList != null)
            {
                _ddVM.MoveToRight();
            }
        }
    }
}
