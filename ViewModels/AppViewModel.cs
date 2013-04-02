using DuoDuoParserLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuoDuoPhotoGallery.ViewModels
{
    /// <summary>
    /// App全局ViewModel
    /// </summary>
    public class AppViewModel
    {
        private static object _lock = new object();
        private static AppViewModel _instance = null;
        public static AppViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppViewModel();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 当前的类型信息
        /// </summary>
        public DDPhotoGallery CurGallery
        {
            get;
            set;
        }
        /// <summary>
        /// 当前的详情信息
        /// </summary>
        public DDPhotoDetail CurDetail
        {
            get;
            set;
        }
        /// <summary>
        /// 当前页面标题
        /// </summary>
        public string CurPageTitle
        {
            get;
            set;
        }
    }
}
