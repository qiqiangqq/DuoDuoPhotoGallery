using Coding4Fun.Toolkit.Controls;
using DuoDuoParserLib.Model;
using DuoDuoParserLib.Parser;
using DuoDuoPhotoGallery.Helper;
using Microsoft.Phone.Controls;
using QCodeKit.MVVM;
using QCodeKit.MVVM.Contracts;
using QCodeKit.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;

namespace DuoDuoPhotoGallery.ViewModels
{
    /// <summary>
    /// DuoDuo内涵吧
    /// </summary>
    public class DDNHMainpageViewModel : BaseViewModel, IEventSink<string>, ITombstoneFriendly, ILoadData
    {
        /// <summary>
        ///  事件
        /// </summary>
        public IActionCommand<object> TileClickCommand { get; private set; }
        /// <summary>
        /// MainPage数据加载完成
        /// </summary>
        public Action<ParseCompletedArgs> MainPageDataLoadCompleted;
        /// <summary>
        ///     For pages, the list of commands to bind to application bar buttons
        /// </summary>
        public override IEnumerable<IActionCommand<object>> ApplicationButtonBindings
        {
            get { return null; }
        }
        /// <summary>
        ///     For pages, the list of commands to bind to application bar menu items
        /// </summary>
        public override IEnumerable<IActionCommand<object>> ApplicationMenuBindings
        {
            get { return null; }
        }
        /// <summary>
        /// 是否数据加载完毕
        /// </summary>
        public bool IsDataLoaded
        {
            private set;
            get;
        }
        /// <summary>
        /// 是否加载文件夹占用空间
        /// </summary>
        public bool IsLoadFolderSize
        {
            get
            {
                if (_packageFolderSize <= 0.0)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 主页推荐内容Model
        /// </summary>
        private DDPhotoGallery _mainpageRecommandGallery = null;
        /// <summary>
        /// 首页推荐数据列表
        /// </summary>
        private ObservableCollection<DDPhotoDetail> _mainRecommandData = null;
        public ObservableCollection<DDPhotoDetail> MainRecommandData
        {
            get
            {
                return _mainRecommandData;
            }
            set
            {
                _mainRecommandData = value;
                this.RaisePropertyChanged(() => MainRecommandData);
            }
        }
        /// <summary>
        /// 主页推荐的其余数据
        /// </summary>
        private ObservableCollection<DDPhotoDetail> _mainRecommandOtherData = null;
        public ObservableCollection<DDPhotoDetail> MainRecommandOtherData
        {
            get
            {
                return _mainRecommandOtherData;
            }
            set
            {
                _mainRecommandOtherData = value;
                this.RaisePropertyChanged(() => MainRecommandOtherData);
            }
        }
        /// <summary>
        /// 相册列表
        /// </summary>
        private DDPhotoGalleryList _galleryList = null;
        public DDPhotoGalleryList GalleryList
        {
            get
            {
                return _galleryList;
            }
            private set
            {
                _galleryList = value;
                this.RaisePropertyChanged(() => GalleryList);
            }
        }
        /// <summary>
        /// MainPage数据解析引擎
        /// </summary>
        private DDMainpageParser _mainPageDataParser = null;
        private DDPhotoGalleryDataManager _dataManager = DDPhotoGalleryDataManager.Instance;
        private readonly string MainPageRecommandTable = "recommandtable";
        private double _packageFolderSize = 0.0;
        /// <summary>
        /// 加载主页推荐数据
        /// </summary>
        public void LoadPageData()
        {
            try
            {
                if (IsDataLoaded)
                {
                    if (MainPageDataLoadCompleted != null)
                    {
                        MainPageDataLoadCompleted(new ParseCompletedArgs(ParseState.EParseSuccess));
                    }
                    return;
                }

                if (QCodeKit.Networking.DeviceNetworkHelper.IsDeviceNetworkAvailable())
                {
                    LoadOnlineMainPageData();
                }
                else
                {
                    LoadOfflineMainPageData();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadOnlineMainPageData()
        {
            if (_mainPageDataParser == null)
            {
                _mainPageDataParser = new DDMainpageParser(DDChannelType.EDDMainPageChannel);
                _mainPageDataParser.ParseCompleted += MainPageDataParseCompleted;
            }
            _mainPageDataParser.ParseAsync();
        }
        /// <summary>
        /// 获取主页的离线缓存数据
        /// </summary>
        private void LoadOfflineMainPageData()
        {
            if (_dataManager.IsTableExisted(MainPageRecommandTable))
            {
                var detailList = _dataManager.SelectGalleryData(DDChannelType.EDDMainPageChannel, MainPageRecommandTable);
                DetailListArrange(detailList);
            }
            else
            {
                if (MainPageDataLoadCompleted != null)
                {
                    MainPageDataLoadCompleted(new ParseCompletedArgs(ParseState.EParseFailed, "网络不可用, 请稍后重试"));
                }
            }
        }
        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            try
            {
                var files = IsolatedStorageHelper.GetFileNames(DDPhotoImageUCHelper.OfflineFolder + "//*");
                
                foreach (var file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        var filepath = DDPhotoImageUCHelper.OfflineFolder + "//" + file;
                        IsolatedStorageHelper.DeleteFile(filepath);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 获取文件夹的空间大小
        /// </summary>
        /// <returns></returns>
        public double GetDirectorySize(ref bool isRemind)
        {
            if (_packageFolderSize <= 0.0)
            {
                isRemind = true;
                var files = IsolatedStorageHelper.GetFileNames(DDPhotoImageUCHelper.OfflineFolder + "//*");
                IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
                double folderSize = 0.0;

                try
                {
                    foreach (var file in files)
                    {
                        if (!string.IsNullOrEmpty(file))
                        {
                            var filepath = DDPhotoImageUCHelper.OfflineFolder + "//" + file;
                            using (var fileStream = store.OpenFile(filepath, System.IO.FileMode.Open))
                            {
                                if (fileStream != null)
                                {
                                    double filesize = fileStream.Length;
                                    folderSize += filesize;
                                }
                                fileStream.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    folderSize /= 1024;
                    _packageFolderSize = folderSize;
                }
            }
            else
            {
                isRemind = false;
            }

            return _packageFolderSize; ;
        }
        /// <summary>
        /// 数据分配
        /// </summary>
        /// <param name="detailList"></param>
        private void DetailListArrange(IEnumerable<DDPhotoDetail> detailList)
        {
            ParseCompletedArgs args = null;
            if (detailList != null && detailList.Count() >= 3)
            {
                MainRecommandData = new ObservableCollection<DDPhotoDetail>(detailList.Take(3));
                MainRecommandOtherData = new ObservableCollection<DDPhotoDetail>(detailList.Where((photo) =>
                {
                    if (MainRecommandData.Contains(photo, new DDPhotoDetailEqualityComparer()))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }));
                args = new ParseCompletedArgs(ParseState.EParseSuccess);
            }
            else
            {
                args = new ParseCompletedArgs(ParseState.EParseFailed);
            }
            IsDataLoaded = true;
            ///数据加载完成
            if (MainPageDataLoadCompleted != null)
            {
                MainPageDataLoadCompleted(args);
            }
        }
        /// <summary>
        /// 将最新的离线数据存储
        /// </summary>
        public void SavePageData()
        {
            try
            {
                string create_sql = DDPhotoDetailDataHelper.GetCreateDetailTableSql(MainPageRecommandTable);
                /// 创建首页推荐信息表
                /// 该表中只存储每次更新后的推荐内容列表中的内容
                /// 不重复提交
                var ret = _dataManager.CreateTable(MainPageRecommandTable, create_sql);
                if (_mainpageRecommandGallery != null)
                {
                    /// 删除之前存储的所有数据
                    _dataManager.DeleteDetailData(MainPageRecommandTable);
                    var detailList = _mainpageRecommandGallery.DetailList;
                    _dataManager.InsertDataListToDetailTable(detailList, DDChannelType.EDDMainPageChannel, MainPageRecommandTable);
                }
            }
            catch (Exception)
            { }
        }
        /// <summary>
        /// 数据解析完毕
        /// </summary>
        /// <param name="args"></param>
        private void MainPageDataParseCompleted(ParseCompletedArgs args)
        {
            /// 解析完成
            if (args != null && args.CurParseState == ParseState.EParseSuccess)
            {
                _mainpageRecommandGallery = _mainPageDataParser.ParsedObj;
                if (_mainpageRecommandGallery != null && _mainpageRecommandGallery.DetailList != null)
                {
                    var detailList = _mainpageRecommandGallery.DetailList;
                    DetailListArrange(detailList);
                    /// 这个还是放到页面
                    /// SaveFreshOnlineMainPageData();
                }
            }
            else
            {
                IsDataLoaded = true;
                ///数据加载完成
                if (MainPageDataLoadCompleted != null)
                {
                    MainPageDataLoadCompleted(new ParseCompletedArgs(ParseState.EParseFailed));
                }
            }
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DDNHMainpageViewModel()
        {
            IsDataLoaded = false;
            UltraLightLocator.EventAggregator.SubscribeOnDispatcher(this);
            InitGalleryList();
            InitEventHandler();
        }
        /// <summary>
        /// 初始化事件处理器
        /// </summary>
        private void InitEventHandler()
        {
            TileClickCommand = new ActionCommand<object>(
                o => TileClick(o));
        }
        /// <summary>
        /// 初始化GalleryList
        /// </summary>
        private void InitGalleryList()
        {
            if (GalleryList == null)
            {
                GalleryList = new DDPhotoGalleryList();
            }
            /// 
            var nhtGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDNHTChannel);
            GalleryList.Append(nhtGallery);
            ///
            var bzGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDBZMHChannel);
            GalleryList.Append(bzGallery);
            ///
            var sxzGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDSXZChannel);
            GalleryList.Append(sxzGallery);
            ///
            //var nhdzGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDNHDZChannel);
            //GalleryList.Append(nhdzGallery);
            ///
            var wjcListGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDWJCChannel);
            GalleryList.Append(wjcListGallery);
            ///
            var yyxjGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDYYXJChannel);
            GalleryList.Append(yyxjGallery);
            ///
            var rankListGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDRANKLISTChannel);
            GalleryList.Append(rankListGallery);
            ///
            var xcjListGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDXCJChannel);
            GalleryList.Append(xcjListGallery);
            //
            var jhxdListGallery = DDCategoryParserFactory.CreateParseChannel(DDChannelType.EDDJHXDChannel);
            GalleryList.Append(jhxdListGallery);
        }
        /// <summary>
        /// Tile对象点击处理
        /// </summary>
        private void TileClick(object param)
        {
            var pg = param as DDPhotoGallery;
            AppViewModel.Instance.CurGallery = pg;
            if (pg != null)
            {
                InnerNavigate("/Views/DDPhotoGalleryPage.xaml");
            }
        }
        /// <summary>
        /// 内部跳转方法
        /// </summary>
        /// <param name="navigateUri"></param>
        private void InnerNavigate(string navigateUri)
        {
            var rootFrame = App.Current.RootVisual as PhoneApplicationFrame;
            if (rootFrame != null)
            {
                var curPage = rootFrame.Content as PhoneApplicationPage;
                if (curPage != null)
                {
                    curPage.NavigationService.Navigate(new Uri(navigateUri, UriKind.RelativeOrAbsolute));
                }
            }
        }
        /// <summary>
        /// 取消回退请求
        /// </summary>
        /// <returns></returns>
        public override bool CancelBackRequest()
        {
            return !Dialog.ShowMessage("关闭多多内涵吧", "确定要离开多多内涵吧吗?", true);
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
            SavePageData();
        }
        /// <summary>
        ///     Returned from tombstone
        /// </summary>
        public void Activate()
        {
            LoadPageData();
        }
    }
}
