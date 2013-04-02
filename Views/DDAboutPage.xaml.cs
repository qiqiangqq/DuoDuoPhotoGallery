using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DuoDuoPhotoGallery.Views
{
    public partial class DDAboutPage : PhoneApplicationPage
    {
        public DDAboutPage()
        {
            InitializeComponent();
        }

        private void EmailMe_Click(object sender, EventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask ect = new Microsoft.Phone.Tasks.EmailComposeTask();
            ect.Subject = "多多内涵吧";
            ect.To = "qcodekit@163.com";
            ect.Body = "请留下您的宝贵意见";
            ect.Show();
        }
    }
}