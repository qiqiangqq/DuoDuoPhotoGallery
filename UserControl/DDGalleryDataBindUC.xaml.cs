using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using DuoDuoParserLib.Model;

namespace DuoDuoPhotoGallery
{
    public partial class DDGalleryDataBindUC : UserControl
    {
        #region DDPhotoDetail
        public static readonly DependencyProperty PhotoDetailProperty =
            DependencyProperty.Register("PhotoDetail",
            typeof(DDPhotoDetail),
            typeof(DDGalleryDataBindUC),
            new PropertyMetadata(null, PhotoDetailChanged));
        /// <summary> 
        /// 图片源地址
        /// </summary>
        public DDPhotoDetail PhotoDetail
        {
            set { SetValue(PhotoDetailProperty, value); }
            get { return (DDPhotoDetail)GetValue(PhotoDetailProperty); }
        }

        private static void PhotoDetailChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DDGalleryDataBindUC uc = o as DDGalleryDataBindUC;
            if (uc != null)
            {
                uc.BindChangedData(e.NewValue as DDPhotoDetail);
            }
        }
        /// <summary>
        /// 数据绑定
        /// </summary>
        private void BindData()
        {
            if (LayoutRoot != null)
            {
                LayoutRoot.DataContext = null;
                LayoutRoot.DataContext = PhotoDetail;
            }
        }
        /// <summary>
        /// 绑定改变了的新数据
        /// </summary>
        /// <param name="newDetail"></param>
        private void BindChangedData(DDPhotoDetail newDetail)
        {
            if (LayoutRoot != null && newDetail != null)
            {
                LayoutRoot.DataContext = null;
                LayoutRoot.DataContext = newDetail;
            }
        }
        #endregion

        public DDGalleryDataBindUC()
        {
            InitializeComponent();
            this.Loaded += DDGalleryDataBindUC_Loaded;
        }

        void DDGalleryDataBindUC_Loaded(object sender, RoutedEventArgs e)
        {
            //PhotoDetail = this.DataContext as DDPhotoDetail;
            if (LayoutRoot.DataContext == null)
            {
                LayoutRoot.DataContext = PhotoDetail;
            }
        }
    }
}
