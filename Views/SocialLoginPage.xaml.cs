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
    public partial class SocialLoginPage : PhoneApplicationPage
    {
        private bool _isClearBackStack = false;

        public SocialLoginPage()
        {
            InitializeComponent();
            LoadLoginControl();
        }

        private void LoadLoginControl()
        {
            AuthControl control = new AuthControl();
            var type = DDSocialShareViewModel.Instance.CurrentSocialType;
            control.SetData(type, SocialConstants.GetClient(type));
            control.action += (p) =>
            {
                if (DDSocialShareViewModel.Instance.IsLoginGoBack)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(delegate
                    {
                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.GoBack();
                        }
                    });
                }
                else
                {
                    _isClearBackStack = true;
                    Deployment.Current.Dispatcher.BeginInvoke(delegate
                    {
                        NavigationService.Navigate(new Uri("/Views/SocialSendPage.xaml", UriKind.Relative));
                    });
                }
            };
            LayoutRoot.Children.Add(control);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_isClearBackStack)
            {
                if (NavigationService.CanGoBack)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(delegate
                    {
                        NavigationService.RemoveBackEntry();
                    });
                }
            }
            base.OnNavigatedFrom(e);
        }
    }
}