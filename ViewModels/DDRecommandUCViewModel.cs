using DuoDuoParserLib.Model;
using Microsoft.Phone.Controls;
using QCodeKit.MVVM;
using QCodeKit.MVVM.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DuoDuoPhotoGallery.ViewModels
{
    /// <summary>
    /// DDRecommandUC ViewModel
    /// </summary>
    public class DDRecommandUCViewModel: BaseViewModel, IEventSink<string>, ITombstoneFriendly
    {
        public Action CurSelectedChanged = null;
        /// <summary>
        /// 左键按下
        /// </summary>
        public IActionCommand<object> TapLeftCommand { get; private set; }
        public IActionCommand<object> TapRightCommand { get; private set; }
        public IActionCommand<object> TapNavigateCommand { get; private set;}
        /// <summary>
        /// 当前推荐的内容
        /// </summary>
        private DDPhotoDetail _curRecommandDetail = null;
        public DDPhotoDetail CurRecommandDetail
        {
            get
            {
                return _curRecommandDetail;
            }
            set
            {
                _curRecommandDetail = value;
                this.RaisePropertyChanged(() => CurRecommandDetail);
                if (CurSelectedChanged != null)
                {
                    CurSelectedChanged();
                }
            }
        }
        /// <summary>
        /// 推荐数据列表
        /// </summary>
        private ObservableCollection<DDPhotoDetail> _recommandList = null;
        public ObservableCollection<DDPhotoDetail> RecommandList
        {
            get
            {
                return _recommandList;
            }
            set
            {
                _recommandList = value;
                this.RaisePropertyChanged(() => RecommandList);
            }
        }
        private DDPhotoImageUCHelper _imgHelper = null;
        private long _innerTickStart = 0;
        private long _innerTickEnd = 0;
        /// <summary>
        /// 上一个选中的对象
        /// </summary>
        public int PreIdx
        {
            get;
            private set;
        }
        /// <summary>
        /// 当前的索引
        /// </summary>
        public int CurIdx
        {
            get;
            private set;
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DDRecommandUCViewModel()
        {
            CurIdx = 0;
            PreIdx = 0;
            _imgHelper = new DDPhotoImageUCHelper();
            InitEventHandler();
        }
        /// <summary>
        /// 初始化数据列表
        /// </summary>
        /// <param name="recommandList"></param>
        public void InitDataList(IEnumerable<DDPhotoDetail> recommandList)
        {
            RecommandList = new ObservableCollection<DDPhotoDetail>(recommandList);
            if (RecommandList != null && RecommandList.Count > 0)
            {
                PreIdx = RecommandList.Count - 1;
                CurRecommandDetail = RecommandList.ElementAt(CurIdx);
                _imgHelper.ImgUrl = CurRecommandDetail.PhotoCoverImageWebUrl;
                CurRecommandDetail.ImgSource = _imgHelper.GetImgSource();
            }
        }
        /// <summary>
        /// 初始化事件处理器
        /// </summary>
        private void InitEventHandler()
        {
            TapLeftCommand = new ActionCommand<object>(
                o => MoveToLeft(o));
            TapRightCommand = new ActionCommand<object>(
                o => MoveToRight());
            TapNavigateCommand = new ActionCommand<object>(
                o => InnerNavigate());
        }
        /// <summary>
        /// 向左移动
        /// </summary>
        private void MoveToLeft(object o)
        {
            if (RecommandList != null)
            {
                PreIdx = CurIdx;
                if (CurIdx == 0)
                {
                    CurIdx = RecommandList.Count - 1;
                }
                else
                {
                    --CurIdx;
                }
                CurRecommandDetail = RecommandList.ElementAt(CurIdx);
                _imgHelper.ImgUrl = CurRecommandDetail.PhotoCoverImageWebUrl;
                CurRecommandDetail.ImgSource = _imgHelper.GetImgSource();
            }
            _innerTickStart = Stopwatch.GetTimestamp();
        }
        /// <summary>
        /// 向右移动
        /// </summary>
        public void MoveToRight()
        {
            if (RecommandList != null)
            {
                PreIdx = CurIdx;
                CurIdx = (CurIdx + 1) % RecommandList.Count;
                CurRecommandDetail = RecommandList.ElementAt(CurIdx);
                _imgHelper.ImgUrl = CurRecommandDetail.PhotoCoverImageWebUrl;
                CurRecommandDetail.ImgSource = _imgHelper.GetImgSource();
            }
            _innerTickStart = Stopwatch.GetTimestamp();
        }
        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="uri"></param>
        private void InnerNavigate()
        {
            _innerTickEnd = Stopwatch.GetTimestamp();
            if (!WhetherNavigate())
            {
                return;
            }
            // 页面跳转
            AppViewModel.Instance.CurDetail = CurRecommandDetail;
            //AppViewModel.Instance.CurGallery = ;
            string uri = "/Views/DDPhotoDetailContentPage.xaml";
            var rootFrame = App.Current.RootVisual as PhoneApplicationFrame;
            if (rootFrame != null)
            {
                var curPage = rootFrame.Content as PhoneApplicationPage;
                if (curPage != null)
                {
                    curPage.NavigationService.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
                }
            }
        }
        private bool WhetherNavigate()
        {
            var offset = _innerTickEnd - _innerTickStart;
            Debug.WriteLine(offset);
            if (offset <= 6000000)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// For pages, the list of commands to bind to application bar buttons
        /// </summary>
        public override IEnumerable<IActionCommand<object>> ApplicationButtonBindings
        {
            get { return null; }
        }
        /// <summary>
        /// For pages, the list of commands to bind to application bar menu items
        /// </summary>
        public override IEnumerable<IActionCommand<object>> ApplicationMenuBindings
        {
            get { return null; }
        }
        /// <summary>
        /// 取消回退请求
        /// </summary>
        /// <returns></returns>
        public override bool CancelBackRequest()
        {
            return false;
        }
        /// <summary>
        ///     Handle the published string event
        /// </summary>
        /// <param name="publishedEvent">The text that was sent</param>
        public void HandleEvent(string publishedEvent)
        {
            
        }
        /// <summary>
        ///     Tombstone
        /// </summary>
        public void Deactivate()
        {

        }
        /// <summary>
        ///     Returned from tombstone
        /// </summary>
        public void Activate()
        {

        }
    }
}
