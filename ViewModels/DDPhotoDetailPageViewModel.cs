using QCodeKit.MVVM;
using QCodeKit.MVVM.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuoDuoParserLib.Parser;
using DuoDuoPhotoGallery.Helper;
using DuoDuoParserLib.Model;

namespace DuoDuoPhotoGallery.ViewModels
{
    /// <summary>
    /// DetailPage VM
    /// </summary>
    public class DDPhotoDetailPageViewModel : BaseViewModel, IEventSink<string>, ITombstoneFriendly, ILoadData
    {
        public Action<ParseCompletedArgs> ChannelDataLoadCompleted;
        /// <summary>
        /// 频道实体对象
        /// </summary>
        private DDPhotoGallery _channelGallery = null;
        public DDPhotoGallery ChannelGallery
        {
            get
            {
                return _channelGallery;
            }
            private set
            {
                _channelGallery = value;
                this.RaisePropertyChanged(() => ChannelGallery);
            }
        }
        /// <summary>
        /// 数据解析引擎
        /// </summary>
        private DDChannelParser _channelDataParser = null;
        private DDPhotoGalleryDataManager _dataManager = DDPhotoGalleryDataManager.Instance;
        /// <summary>
        /// 当前的频道类型
        /// </summary>
        public DDChannelType CurType
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
        /// 频道类型
        /// </summary>
        /// <param name="type"></param>
        public DDPhotoDetailPageViewModel(DDChannelType type)
        {
            CurType = type;
            UltraLightLocator.EventAggregator.SubscribeOnDispatcher(this);
        }
        /// <summary>
        /// 加载页面数据
        /// </summary>
        public void LoadPageData()
        {
            if (QCodeKit.Networking.DeviceNetworkHelper.IsDeviceNetworkAvailable())
            {
                LoadChannelPageOnlineData();
            }
            else
            {
                LoadChannelPageOfflineData();
            }
        }
        /// <summary>
        /// 获取下一个分页的内容
        /// </summary>
        public int LoadNextPageData()
        {
            if (QCodeKit.Networking.DeviceNetworkHelper.IsDeviceNetworkAvailable())
            {
                if (ChannelGallery != null && _channelDataParser != null && !string.IsNullOrEmpty(_channelDataParser.ParseDataBaseUrl) && ChannelGallery.HasNextPage())
                {
                    string nextPageUrl = string.Format("{0}{1}{2}", _channelDataParser.ParseDataBaseUrl, ++ChannelGallery.CurPage, _channelDataParser.ParseUrlPostFix);
                    if (_channelDataParser == null)
                    {
                        _channelDataParser = new DDChannelParser(CurType);
                        _channelDataParser.ParseCompleted += OnlineDataParseCompleted;
                    }
                    _channelDataParser.ParseAsync(nextPageUrl);
                }
            }
            else
            {
                var args = new ParseCompletedArgs(ParseState.EParseFailed);
                if (ChannelDataLoadCompleted != null)
                {
                    ChannelDataLoadCompleted(args);
                }
            }

            return _channelGallery.CurPage;
        }
        /// <summary>
        /// 加载离线数据
        /// </summary>
        private void LoadChannelPageOfflineData()
        {
            ParseCompletedArgs args = null;
            string tablename = DDCategoryParserFactory.GetChannelTableName(CurType);
            if (_dataManager.IsTableExisted(tablename))
            {
                var detailList = _dataManager.SelectGalleryData(CurType, tablename);
                if (detailList != null && detailList.Count() > 0)
                {
                    ChannelGallery = DDCategoryParserFactory.CreateParseChannel(CurType);
                    ChannelGallery.DetailList = new System.Collections.ObjectModel.ObservableCollection<DDPhotoDetail>(detailList);
                    args = new ParseCompletedArgs(ParseState.EParseSuccess);
                }
                else
                {
                    args = new ParseCompletedArgs(ParseState.EParseFailed);
                }
            }
            else
            {
                args = new ParseCompletedArgs(ParseState.EParseFailed);
            }
            if (ChannelDataLoadCompleted != null)
            {
                ChannelDataLoadCompleted(args);
            }
        }
        /// <summary>
        /// 加载在线数据
        /// </summary>
        private void LoadChannelPageOnlineData()
        {
            if (_channelDataParser == null)
            {
                _channelDataParser = new DDChannelParser(CurType);
                _channelDataParser.ParseCompleted += OnlineDataParseCompleted;
            }
            _channelDataParser.ParseAsync();
        }
        /// <summary>
        /// 存储数据
        /// </summary>
        public void SavePageData()
        {
            try
            {
                /// 创建首页推荐信息表
                /// 该表中只存储每次更新后的推荐内容列表中的内容
                /// 不重复提交
                string tablename = DDCategoryParserFactory.GetChannelTableName(CurType);
                string create_sql = DDPhotoDetailDataHelper.GetCreateDetailTableSql(tablename);
                var ret = _dataManager.CreateTable(tablename, create_sql);
                if (_channelGallery != null)
                {
                    /// 删除之前存储的所有数据
                    _dataManager.DeleteDetailData(tablename);
                    var detailList = _channelGallery.DetailList;
                    _dataManager.InsertDataListToDetailTable(detailList, CurType, tablename);
                }
            }
            catch (Exception)
            { }
        }
        /// <summary>
        /// 数据解析完毕
        /// </summary>
        /// <param name="args"></param>
        private void OnlineDataParseCompleted(ParseCompletedArgs args)
        {
            if (args != null && args.CurParseState == ParseState.EParseSuccess)
            {
                ChannelGallery = _channelDataParser.ParsedObj;
            }
            if (ChannelDataLoadCompleted != null)
            {
                ChannelDataLoadCompleted(args);
            }
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
    }
}
