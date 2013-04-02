using Community.CsharpSqlite.SQLiteClient;
using DuoDuoParserLib.Model;
using DuoDuoParserLib.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuoDuoPhotoGallery.Helper
{
    /// <summary>
    /// 详细内容的数据助手对象
    /// </summary>
    public class DDPhotoDetailDataHelper
    {
        private static readonly string IdCol = "id";
        private static readonly string GalleryTypeCol = "gallerytype";
        private static readonly string PhotoTitleCol = "phototitle";
        private static readonly string PhotoIntroCol = "photointro";
        private static readonly string PhotoParentWebUrlCol = "photoparentweburl";
        private static readonly string PhotoCoverImageWebUrlCol = "photocoverimageweburl";
        private static readonly string PhotoUploadTimeCol = "photouploadtime";
        private static readonly string PhotoClickTimesCol = "photoclicktimes";
        private static readonly string PhotoGoodCommentsCol = "photogoodcomments";

        private SqliteConnection _dbConn = null;
        //"ddphotodetail";
        public string PhotoDetailTable
        {
            get;
            set;
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <param name="conn"></param>
        public DDPhotoDetailDataHelper(SqliteConnection conn)
        {
            _dbConn = conn;
        }
        /// <summary>
        /// 获取创建表的sql语句
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetCreateDetailTableSql(string table)
        {
            string create_sql = string.Format("CREATE TABLE {0} ([{1}] INTEGER PRIMARY KEY, [{2}] TEXT, [{3}] TEXT, [{4}] TEXT, [{5}] TEXT, [{6}] TEXT, [{7}] TEXT, [{8}] TEXT, [{9}] TEXT)",
                table,
                IdCol,
                GalleryTypeCol,
                PhotoTitleCol,
                PhotoIntroCol,
                PhotoParentWebUrlCol,
                PhotoCoverImageWebUrlCol,
                PhotoUploadTimeCol,
                PhotoClickTimesCol,
                PhotoGoodCommentsCol);
            return create_sql;
        }
        /// <summary>
        /// 获取创建
        /// </summary>
        /// <returns></returns>
        public string GetCreateDetailTableSql()
        {
            //CREATE TABLE test ( [id] INTEGER PRIMARY KEY, [col] INTEGER UNIQUE, [col2] INTEGER, [col3] REAL, [col4] TEXT, [col5] BLOB)
            string create_sql = string.Format("CREATE TABLE {0} ([{1}] INTEGER PRIMARY KEY, [{2}] TEXT, [{3}] TEXT, [{4}] TEXT, [{5}] TEXT, [{6}] TEXT, [{7}] TEXT, [{8}] TEXT, [{9}] TEXT)",
                PhotoDetailTable,
                IdCol,
                GalleryTypeCol,
                PhotoTitleCol,
                PhotoIntroCol,
                PhotoParentWebUrlCol,
                PhotoCoverImageWebUrlCol,
                PhotoUploadTimeCol,
                PhotoClickTimesCol,
                PhotoGoodCommentsCol);
            return create_sql;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool InsertData(DDPhotoDetail detail, DDChannelType channelType)
        {
            bool ret = false;
            if (detail != null && channelType != DDChannelType.EDDUnknownChannel)
            {
                SqliteTransaction sqlTrans = null;
                try
                {
                    using (SqliteCommand dbCmd = _dbConn.CreateCommand())
                    {
                        sqlTrans = _dbConn.BeginTransaction();
                        string sql_insert = string.Format("INSERT INTO {0}({1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}) VALUES(@{1}, @{2}, @{3}, @{4}, @{5}, @{6}, @{7}, @{8});",
                                PhotoDetailTable,
                                IdCol,
                                GalleryTypeCol,
                                PhotoTitleCol,
                                PhotoIntroCol,
                                PhotoParentWebUrlCol,
                                PhotoCoverImageWebUrlCol,
                                PhotoUploadTimeCol,
                                PhotoClickTimesCol,
                                PhotoGoodCommentsCol);
                        dbCmd.CommandText = sql_insert;
                        //dbCmd.ExecuteNonQuery();
                        dbCmd.Parameters.Add("@" + IdCol, null);
                        dbCmd.Parameters.Add("@" + GalleryTypeCol, null);
                        dbCmd.Parameters.Add("@" + PhotoTitleCol, null);
                        dbCmd.Parameters.Add("@" + PhotoIntroCol, null);
                        dbCmd.Parameters.Add("@" + PhotoParentWebUrlCol, null);
                        dbCmd.Parameters.Add("@" + PhotoCoverImageWebUrlCol, null);
                        dbCmd.Parameters.Add("@" + PhotoUploadTimeCol, null);
                        dbCmd.Parameters.Add("@" + PhotoClickTimesCol, null);
                        dbCmd.Parameters.Add("@" + PhotoGoodCommentsCol, null);

                        dbCmd.Parameters["@" + GalleryTypeCol].Value = channelType.ToString();
                        dbCmd.Parameters["@" + PhotoTitleCol].Value = detail.PhotoTitle;
                        dbCmd.Parameters["@" + PhotoIntroCol].Value = detail.PhotoIntro;
                        dbCmd.Parameters["@" + PhotoParentWebUrlCol].Value = detail.PhotoParentWebUrl;
                        dbCmd.Parameters["@" + PhotoCoverImageWebUrlCol].Value = detail.PhotoCoverImageWebUrl;
                        dbCmd.Parameters["@" + PhotoUploadTimeCol].Value = detail.PhotoUGC.PhotoUploadTime;
                        dbCmd.Parameters["@" + PhotoClickTimesCol].Value = detail.PhotoUGC.PhotoClickTimes;
                        dbCmd.Parameters["@" + PhotoGoodCommentsCol].Value = detail.PhotoUGC.PhotoGoodComments;
                        dbCmd.ExecuteScalar();
                        dbCmd.Dispose();
                        sqlTrans.Commit();
                        sqlTrans = null;
                        ret = true;
                    }
                }
                catch (Exception)
                {
                    if (sqlTrans != null)
                    {
                        sqlTrans.Rollback();
                    }
                    ret = false;
                }
            }

            return ret;
        }
        /// <summary>
        /// 插入一个数据列表
        /// </summary>
        /// <param name="detailList"></param>
        public void InsertDataList(IEnumerable<DDPhotoDetail> detailList, DDChannelType channelType)
        {
            if (detailList != null && channelType != DDChannelType.EDDUnknownChannel)
            {
                SqliteTransaction sqlTrans = null;
                try
                {
                    using (SqliteCommand dbCmd = _dbConn.CreateCommand())
                    {
                        sqlTrans = _dbConn.BeginTransaction();
                        string sql_insert = string.Format("INSERT INTO {0}({1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}) VALUES(@{1}, @{2}, @{3}, @{4}, @{5}, @{6}, @{7}, @{8}, @{9});",
                                PhotoDetailTable,
                                IdCol,
                                GalleryTypeCol,
                                PhotoTitleCol,
                                PhotoIntroCol,
                                PhotoParentWebUrlCol,
                                PhotoCoverImageWebUrlCol,
                                PhotoUploadTimeCol,
                                PhotoClickTimesCol,
                                PhotoGoodCommentsCol);
                        dbCmd.CommandText = sql_insert;
                        //dbCmd.ExecuteNonQuery();
                        dbCmd.Parameters.Add("@" + IdCol, null);
                        dbCmd.Parameters.Add("@" + GalleryTypeCol, null);
                        dbCmd.Parameters.Add("@" + PhotoTitleCol, null);
                        dbCmd.Parameters.Add("@" + PhotoIntroCol, null);
                        dbCmd.Parameters.Add("@" + PhotoParentWebUrlCol, null);
                        dbCmd.Parameters.Add("@" + PhotoCoverImageWebUrlCol, null);
                        dbCmd.Parameters.Add("@" + PhotoUploadTimeCol, null);
                        dbCmd.Parameters.Add("@" + PhotoClickTimesCol, null);
                        dbCmd.Parameters.Add("@" + PhotoGoodCommentsCol, null);
                        foreach (var detail in detailList)
                        {
                            dbCmd.Parameters["@" + GalleryTypeCol].Value = channelType.ToString();
                            dbCmd.Parameters["@" + PhotoTitleCol].Value = detail.PhotoTitle;
                            dbCmd.Parameters["@" + PhotoIntroCol].Value = detail.PhotoIntro;
                            dbCmd.Parameters["@" + PhotoParentWebUrlCol].Value = detail.PhotoParentWebUrl;
                            dbCmd.Parameters["@" + PhotoCoverImageWebUrlCol].Value = detail.PhotoCoverImageWebUrl;
                            dbCmd.Parameters["@" + PhotoUploadTimeCol].Value = detail.PhotoUGC.PhotoUploadTime;
                            dbCmd.Parameters["@" + PhotoClickTimesCol].Value = detail.PhotoUGC.PhotoClickTimes;
                            dbCmd.Parameters["@" + PhotoGoodCommentsCol].Value = detail.PhotoUGC.PhotoGoodComments;
                            dbCmd.ExecuteScalar();
                        }
                        dbCmd.Dispose();
                        sqlTrans.Commit();
                        sqlTrans = null;
                    }
                }
                catch (Exception)
                {
                    if (sqlTrans != null)
                    {
                        sqlTrans.Rollback();
                    }
                }
            }
        }
        /// <summary>
        /// 删除一条字段
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool DeleteData(DDPhotoDetail detail)
        {
            bool ret = false;
            if (detail != null)
            {
                SqliteTransaction sqlTrans = null;
                try
                {
                    using (SqliteCommand dbCmd = _dbConn.CreateCommand())
                    {
                        sqlTrans = _dbConn.BeginTransaction();
                        string delete_sql = string.Format("delete from {0} where {1}=@{1};", PhotoDetailTable, PhotoCoverImageWebUrlCol);
                        dbCmd.CommandText = delete_sql;
                        ret = dbCmd.ExecuteNonQuery() == 0 ? true : false;
                        dbCmd.Dispose();
                        sqlTrans.Commit();
                        sqlTrans = null;
                    }
                }
                catch (Exception)
                {
                    if (sqlTrans != null)
                    {
                        sqlTrans.Rollback();
                        ret = false;
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll()
        {
            bool ret = false;
            SqliteTransaction sqlTrans = null;
            try
            {
                using (SqliteCommand dbCmd = _dbConn.CreateCommand())
                {
                    sqlTrans = _dbConn.BeginTransaction();
                    string delete_sql = string.Format("delete from {0};", PhotoDetailTable);
                    dbCmd.CommandText = delete_sql;
                    ret = dbCmd.ExecuteNonQuery() == 0 ? true : false;
                    dbCmd.Dispose();
                    sqlTrans.Commit();
                    sqlTrans = null;
                }
            }
            catch (Exception)
            {
                if (sqlTrans != null)
                {
                    sqlTrans.Rollback();
                    ret = false;
                }
            }
            return ret;
        }
        /// <summary>
        /// 获取某个分类的所有细节信息
        /// </summary>
        /// <param name="channelType"></param>
        /// <returns></returns>
        public IEnumerable<DDPhotoDetail> SelectGalleryData(DDChannelType channelType)
        {
            List<DDPhotoDetail> detailList = null;
            try
            {
                using (SqliteCommand dbCmd = _dbConn.CreateCommand())
                {
                    string select_sql = string.Empty;
                    if (channelType == DDChannelType.EDDUnknownChannel)
                    {
                        select_sql = string.Format("SELECT * FROM {0}", PhotoDetailTable);
                    }
                    else
                    {
                        select_sql = string.Format("SELECT * FROM {0} where {1}=@{1};", PhotoDetailTable, GalleryTypeCol);
                        dbCmd.Parameters.Add("@" + GalleryTypeCol, null);
                        dbCmd.Parameters["@" + GalleryTypeCol].Value = channelType.ToString();
                    }
                    dbCmd.CommandText = select_sql;
                    using (SqliteDataReader reader = dbCmd.ExecuteReader())
                    {
                        detailList = new List<DDPhotoDetail>();
                        while (reader.Read())
                        {
                            DDPhotoDetail detail = new DDPhotoDetail();
                            detail.PhotoTitle = reader.GetString(2);
                            if (!reader.IsDBNull(3))
                            {
                                detail.PhotoIntro = reader.GetString(3);   
                            }
                            detail.PhotoParentWebUrl = reader.GetString(4);
                            detail.PhotoCoverImageWebUrl = reader.GetString(5);
                            detail.PhotoUGC = new DDPhotoUGC();
                            if (!reader.IsDBNull(6))
                            {
                                detail.PhotoUGC.PhotoUploadTime = reader.GetString(6);
                            }
                            if (!reader.IsDBNull(7))
                            {
                                detail.PhotoUGC.PhotoClickTimes = reader.GetString(7);
                            }
                            if (!reader.IsDBNull(8))
                            {
                                detail.PhotoUGC.PhotoGoodComments = reader.GetString(8);
                            }
                            detailList.Add(detail);
                        }
                        reader.Close();
                        reader.Dispose();
                    }
                    dbCmd.Dispose();
                }
            }
            catch (Exception)
            {
            }

            return detailList;
        }
    }
}
