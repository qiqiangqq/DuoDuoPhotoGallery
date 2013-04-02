using DuoDuoParserLib.Model;
using DuoDuoParserLib.Parser;
using QCodeKit.MVVM;
using QCodeKit.MVVM.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuoDuoPhotoGallery.ViewModels
{
    public class DDDetailContentPageViewModel : BaseViewModel, IEventSink<string>, ITombstoneFriendly, ILoadData
    {
        public Action<ParseCompletedArgs> DetailContentParseCompleted;
        /// <summary>
        /// 数据解析器
        /// </summary>
        private DDPhotoDetailContentParser _detailContentParser = null;
        /// <summary>
        /// 照片内容对象
        /// </summary>
        public DDPhotoDetail CurPhotoDetail
        {
            get;
            set;
        }
        /// <summary>
        /// 解析后的对象是否为单一内容
        /// </summary>
        public bool IsSingle
        {
            get;
            private set;
        }
        /// <summary>
        /// 解析后的对象
        /// </summary>
        public DDPhotoDetailContent CurParsedContent
        {
            get;
            private set;
        }
        /// <summary>
        /// 当前解析的详情列表
        /// </summary>
        public DDPhotoDetailContentList CurParsedContentList
        {
            get;
            private set;
        }
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
        /// 构造函数
        /// </summary>
        public DDDetailContentPageViewModel()
        {
            UltraLightLocator.EventAggregator.SubscribeOnDispatcher(this);
        }
        /// <summary>
        /// 取消回退请求
        /// </summary>
        /// <returns></returns>
        public override bool CancelBackRequest()
        {
            return !Dialog.ShowMessage("Close Application", "Are you sure you wish to leave?", true);
        }
        /// <summary>
        ///     Handle the published string event
        /// </summary>
        /// <param name="publishedEvent">The text that was sent</param>
        public void HandleEvent(string publishedEvent)
        {
            Dialog.ShowMessage("Received Text", publishedEvent, false);
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
        /// <summary>
        /// 加载页面数据
        /// </summary>
        public void LoadPageData()
        {
            if (QCodeKit.Networking.DeviceNetworkHelper.IsDeviceNetworkAvailable())
            {
                LoadOnlineData();
            }
        }
        /// <summary>
        /// 加载在线数据
        /// </summary>
        private void LoadOnlineData()
        {
            if (CurPhotoDetail != null)
            {
                if (_detailContentParser == null)
                {
                    _detailContentParser = new DDPhotoDetailContentParser(CurPhotoDetail);
                }
                _detailContentParser.ParseCompleted += DataLoadCompleted;
                _detailContentParser.ParseAsync();
            }
        }
        /// <summary>
        /// 保存页面数据
        /// </summary>
        public void SavePageData()
        {
        }
        /// <summary>
        /// 数据加载完毕
        /// </summary>
        private void DataLoadCompleted(ParseCompletedArgs args)
        {
            if (args != null && args.CurParseState == ParseState.EParseSuccess)
            {
                IsSingle = _detailContentParser.IsSingle;
                if (IsSingle)
                {
                    CurParsedContent = _detailContentParser.CurSingleDetailContent;
                }
                else
                {
                    CurParsedContentList = _detailContentParser.ParsedObj;
                }
                if (DetailContentParseCompleted != null)
                {
                    DetailContentParseCompleted(args);
                }
            }
        }
    }
}
