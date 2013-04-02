using DuoDuoParserLib.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuoDuoPhotoGallery.ViewModels
{
    /// <summary>
    /// 数据加载接口
    /// </summary>
    public interface ILoadData
    {
        void LoadPageData();
        void SavePageData();
    }
}
