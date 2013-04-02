using Community.CsharpSqlite.SQLiteClient;
using DuoDuoParserLib.Model;
using DuoDuoParserLib.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace DuoDuoPhotoGallery.Helper
{
    /// <summary>
    /// 应用程序数据管理者类
    /// </summary>
    public class DDPhotoGalleryDataManager
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        private static DDPhotoGalleryDataManager _instance = new DDPhotoGalleryDataManager();
        public static DDPhotoGalleryDataManager Instance
        {
            get
            {
                return _instance;
            }
        }
        public static readonly string dd_dbname = "duoduo.db";
        private DDPhotoDetailDataHelper _detailDataHelper = null;
        private IsolatedStorageFile fileEngine = IsolatedStorageFile.GetUserStoreForApplication();
        private SqliteConnection _dbConn = null;
        /// <summary>
        /// 数据库信息
        /// </summary>
        public static string DBInfo
        {
            get
            {
                return string.Format("Version=3,uri=file:{0}", dd_dbname);
            }
        }
        private DDPhotoGalleryDataManager()
        {
            
        }
        ~DDPhotoGalleryDataManager()
        {
            if (_dbConn != null)
            {
                _dbConn.Close();
                _dbConn.Dispose();
            }
        }
        private void Init()
        {
            _dbConn = new SqliteConnection(DDPhotoGalleryDataManager.DBInfo);
            _dbConn.Open();
        }
        #region table operation
        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool CreateTable(string tablename, string create_sql)
        {
            bool ret = false;
            try
            {
                if (!IsTableExisted(tablename))
                {
                    using (SqliteCommand cmd = _dbConn.CreateCommand())
                    {
                        cmd.CommandText = create_sql;
                        var opRet = cmd.ExecuteNonQuery();
                        ret = opRet == 0 ? true : false;
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                return (ret = false);
            }
            return ret;
        }
        /// <summary>
        /// 数据表是否存在
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool IsTableExisted(string tablename)
        {
            Init();
            if (string.IsNullOrEmpty(tablename))
            {
                return false;
            }
            bool ret = false;
            try
            {
                using (SqliteCommand dbCmd = _dbConn.CreateCommand())
                {
                    dbCmd.CommandText = string.Format("SELECT COUNT(*) FROM sqlite_master where type='table' and name='{0}'", tablename);
                    if (0 == Convert.ToInt32(dbCmd.ExecuteScalar()))
                    {
                        ret = false;
                    }
                    else
                    {
                        ret = true;
                    }
                    dbCmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 删除一个表
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool DeleteTable(string tablename)
        {
            if (string.IsNullOrEmpty(tablename))
            {
                return false;
            }
            bool ret = false;
            SqliteTransaction st = null;
            try
            {
                st = _dbConn.BeginTransaction();
                using (SqliteCommand dbCmd = _dbConn.CreateCommand())
                {
                    dbCmd.CommandText = string.Format("DROP TABLE {0}", tablename);
                    ret = dbCmd.ExecuteNonQuery() == 1 ? true : false;
                    dbCmd.Dispose();
                }
                st.Commit();
                st = null;
            }
            catch (Exception)
            {
                if (st != null)
                {
                    st.Rollback();
                }
                ret = false;
            }
            return ret;
        }
        #endregion
        #region dd photo detial operation
        /// <summary>
        /// 向内容细节表插入数据
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="channelType"></param>
        /// <returns></returns>
        public bool InsertDataToDetailTable(DDPhotoDetail detail, DDChannelType channelType, string tableName)
        {
            if (_detailDataHelper == null)
            {
                _detailDataHelper = new DDPhotoDetailDataHelper(_dbConn);
            }
            _detailDataHelper.PhotoDetailTable = tableName;
            return _detailDataHelper.InsertData(detail, channelType);
        }
        /// <summary>
        /// 插入一个列表
        /// </summary>
        /// <param name="detailList"></param>
        /// <param name="channelType"></param>
        /// <returns></returns>
        public void InsertDataListToDetailTable(IEnumerable<DDPhotoDetail> detailList, DDChannelType channelType, string tableName)
        {
            if (_detailDataHelper == null)
            {
                _detailDataHelper = new DDPhotoDetailDataHelper(_dbConn);
            }
            _detailDataHelper.PhotoDetailTable = tableName;
            _detailDataHelper.InsertDataList(detailList, channelType);
        }
        /// <summary>
        /// 删除一条详情信息
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool DeleteDetailData( string tableName, DDPhotoDetail detail = null)
        {
            if (_detailDataHelper == null)
            {
                _detailDataHelper = new DDPhotoDetailDataHelper(_dbConn);
            }
            _detailDataHelper.PhotoDetailTable = tableName;
            if (detail != null)
            {
                return _detailDataHelper.DeleteData(detail);
            }
            else
            {
                return _detailDataHelper.DeleteAll();
            }
        }
        /// <summary>
        /// 获取细节数据
        /// </summary>
        /// <param name="channelType"></param>
        /// <returns></returns>
        public IEnumerable<DDPhotoDetail> SelectGalleryData(DDChannelType channelType, string tableName)
        {
            if (_detailDataHelper == null)
            {
                _detailDataHelper = new DDPhotoDetailDataHelper(_dbConn);
            }
            _detailDataHelper.PhotoDetailTable = tableName;
            return _detailDataHelper.SelectGalleryData(channelType);
        }
        #endregion
    }
}
