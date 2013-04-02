using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Alexis.WindowsPhone.Social;
using DuoDuoPhotoGallery.ViewModels;
using DuoDuoPhotoGallery.Helper;

namespace DuoDuoPhotoGallery.Views
{
    public partial class SocialBindPage : PhoneApplicationPage
    {
        /// <summary>
        /// 是否清理页面栈
        /// </summary>
        private bool _isClearBackStack = false;
        public SocialBindPage()
        {
            InitializeComponent();
            snsAccountControl.BindAction = ((p) =>
            {
                DDSocialShareViewModel.Instance.IsLoginGoBack = true;
                DDSocialShareViewModel.Instance.CurrentSocialType = p;
                NavigationService.Navigate(new Uri("/Views/SocialLoginPage.xaml", UriKind.Relative));
            });
        }
    }
}