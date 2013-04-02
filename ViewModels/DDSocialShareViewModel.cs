using Alexis.WindowsPhone.Social;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace DuoDuoPhotoGallery.ViewModels
{
    /// <summary>
    /// 社交账号页面ViewModel
    /// </summary>
    public class DDSocialShareViewModel
    {
        private static object _lockObj = new object();
        private static DDSocialShareViewModel _instance = null;
        public static DDSocialShareViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new DDSocialShareViewModel();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 当前的分享类型
        /// </summary>
        public SocialType CurrentSocialType
        {
            get;
            set;
        }
        /// <summary>
        /// 是否绑定账号时回退
        /// </summary>
        public bool IsLoginGoBack
        {
            get;
            set;
        }
        /// <summary>
        /// 分享的文字内容
        /// </summary>
        public string Status
        {
            get;
            set;
        }
        /// <summary>
        /// 当前分享的图片
        /// </summary>
        public WriteableBitmap CurrentSharedImage
        {
            get;
            set;
        }
        private DDSocialShareViewModel()
        {
        }
    }
}
