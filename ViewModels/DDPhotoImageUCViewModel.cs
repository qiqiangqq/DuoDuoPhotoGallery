using QCodeKit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace DuoDuoPhotoGallery.ViewModels
{
    /// <summary>
    /// 自定义图片ViewModel
    /// </summary>
    public class DDPhotoImageUCHelper
    {
        public Action<string> ImgDownloadProgressChanged = null;
        public Action ImgDownloadCompleted = null;
        /// <summary>
        /// 图片WebUrl
        /// </summary>
        public string ImgUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 图片本地存储全路径
        /// </summary>
        public string ImgFilePath
        {
            get
            {
                string imgname = GetPhotoName(ImgUrl);
                return string.Format("{0}/{1}", OfflineFolder, imgname);
            }
        }
        /// <summary>
        /// 获取图像本地名称
        /// </summary>
        public string ImageFileName
        {
            get
            {
                return GetPhotoName(ImgUrl);
            }
        }
        /// <summary>
        /// 是否加载网络数据
        /// </summary>
        public bool IsDataLoadFromWeb
        {
            get;
            set;
        }
        private BitmapImage innerBitmap = null;
        public static readonly string OfflineFolder = "ImgOffline";
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="imgurl"></param>
        public DDPhotoImageUCHelper()
        {
        }
        /// <summary>
        /// 获取图片名称
        /// </summary>
        /// <param name="imageWebUrl"></param>
        /// <returns></returns>
        protected string GetPhotoName(string imageWebUrl)
        {
            if (!string.IsNullOrEmpty(imageWebUrl))
            {
                int nidx = imageWebUrl.LastIndexOf("/");
                if (nidx > 0)
                {
                    return imageWebUrl.Substring(nidx + 1, imageWebUrl.Length - nidx - 1);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 检查是否已经为离线缓存文件
        /// </summary>
        /// <returns></returns>
        private bool CheckHasOfflined()
        {
            string filePath = ImgFilePath;
            return IsolatedStorageHelper.IsFileExisted(filePath);
        }
        /// <summary>
        /// 获取图片的源
        /// 如果已经缓存， 则获取缓存数据
        /// </summary>
        /// <returns></returns>
        public BitmapImage GetImgSource()
        {
            /// 已经缓存
            innerBitmap = null;
            if (CheckHasOfflined())
            {
                innerBitmap = IsolatedStorageHelper.GetBitmapImage(ImgFilePath);
                if (!CheckImgValid())
                {
                    FillLoadFromWeb();
                    if (CheckImgValid())
                    {
                        IsDataLoadFromWeb = false;
                        if (ImgDownloadCompleted != null)
                        {
                            ImgDownloadCompleted();
                        }
                    }
                }
                else
                {
                    IsDataLoadFromWeb = false;
                }
            }
            else
            {
                FillLoadFromWeb();
            }

            return innerBitmap;
        }
        /// <summary>
        /// 检查图片数据是否为有效数据
        /// </summary>
        /// <returns></returns>
        private bool CheckImgValid()
        {
            if (innerBitmap == null || innerBitmap != null && innerBitmap.PixelHeight == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void FillLoadFromWeb()
        {
            IsDataLoadFromWeb = true;
            innerBitmap = new BitmapImage(new Uri(ImgUrl, UriKind.RelativeOrAbsolute));
            innerBitmap.CreateOptions = BitmapCreateOptions.None;
            innerBitmap.DownloadProgress += bi_DownloadProgress;
        }
        /// <summary>
        /// 下载进度改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bi_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            if (e.Progress < 100)
            {
                string result = string.Format("%{0}", e.Progress);
                if (ImgDownloadProgressChanged != null)
                {
                    ImgDownloadProgressChanged(result);
                }
            }
            else
            {
                if (innerBitmap != null)
                {
                    innerBitmap.DownloadProgress -= bi_DownloadProgress;
                }
                if (ImgDownloadCompleted != null)
                {
                    ImgDownloadCompleted();
                }
                SaveBitmapImage(innerBitmap);
            }
        }
        /// <summary>
        /// 存储
        /// </summary>
        /// <param name="imgSource"></param>
        public void SaveBitmapImage(BitmapImage imgSource)
        {
            if (!CheckHasOfflined() && imgSource != null)
            {
                IsolatedStorageHelper.CreateDirectory(OfflineFolder);
                IsolatedStorageHelper.SaveBitmap(imgSource, ImgFilePath, 80);
            }
        }
    }
}
