using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Alexis.WindowsPhone.Social;
using DuoDuoPhotoGallery.ViewModels;
using DuoDuoPhotoGallery.Helper;

namespace DuoDuoPhotoGallery.Views
{
    public partial class SocialSendPage : PhoneApplicationPage
    {
        public SocialSendPage()
        {
            InitializeComponent();
            this.img.Source = DDSocialShareViewModel.Instance.CurrentSharedImage;
            shareTo.Text = "分享到 " + DDSocialShareViewModel.Instance.CurrentSocialType.ToString();
            ptb_status.Text = "多多内涵吧分享该图片";
        }

        private void Send()
        {
            this.Focus();
            ApplicationBar.IsVisible = false;

            grid.Visibility = System.Windows.Visibility.Visible;
            tbk_busy.Text = "正在发送...";
            if (sb_busy != null)
            {
                sb_busy.Begin();
            }
            SocialAPI.Client = SocialConstants.GetClient(DDSocialShareViewModel.Instance.CurrentSocialType);
            if (img.Visibility == Visibility.Visible)
            {
                SocialAPI.UploadStatusWithPic(DDSocialShareViewModel.Instance.CurrentSocialType, ptb_status.Text, SocialConstants.SHARE_IMAGE, (isSuccess, err) =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(delegate
                    {
                        ApplicationBar.IsVisible = true;
                        grid.Visibility = System.Windows.Visibility.Collapsed;
                        if (isSuccess)
                        {
                            MessageBox.Show("发送成功");
                            if (NavigationService.CanGoBack)
                            {
                                NavigationService.GoBack();
                            }
                        }
                        else
                        {
                            MessageBox.Show("分享失败");
                        }
                    });
                });
            }
            else
            {
                SocialAPI.UpdateStatus(DDSocialShareViewModel.Instance.CurrentSocialType, ptb_status.Text, (isSuccess, err) =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(delegate
                    {
                        ApplicationBar.IsVisible = true;
                        grid.Visibility = System.Windows.Visibility.Collapsed;
                        if (isSuccess)
                        {
                            MessageBox.Show("发送成功");
                            if (NavigationService.CanGoBack)
                            {
                                NavigationService.GoBack();
                            }
                        }
                        else
                        {
                            MessageBox.Show("分享失败");
                        }
                    });
                });
            }
           
            
        }

        private void Appbar_Send_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.img.Source = null;
        }
    }
}