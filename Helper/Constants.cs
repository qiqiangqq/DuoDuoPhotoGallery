using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Alexis.WindowsPhone.Social;
using System.Windows.Media.Imaging;

namespace DuoDuoPhotoGallery.Helper
{
    /// <summary>
    /// 社交相关常量数据
    /// </summary>
    public class SocialConstants
    {
        public const string SHARE_IMAGE = "DDSocialShare.jpg";
        /// <summary>
        /// current socail type
        /// </summary>
        public static SocialType CurrentSocialType { get; set; }
        /// <summary>
        /// if login from account page, then we should goback
        /// </summary>
        public static bool IsLoginGoBack { get; set; }
        /// <summary>
        /// shared text
        /// </summary>
        public static string Status { get; set; }
        /// <summary>
        /// shared image
        /// </summary>
        public static WriteableBitmap ShareImage { get; set; }
        /// <summary>
        /// 获取客户端社交属性信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ClientInfo GetClient(SocialType type)
        {
            ClientInfo client = new ClientInfo();
            switch (type)
            {
                case SocialType.Weibo:
                    client.ClientId = "943422421";
                    client.ClientSecret = "1562b38947e4c8e1bee082ea9c130425";
                    client.RedirectUri = "https://api.weibo.com/oauth2/default.html";
                    break;
                case SocialType.Tencent:
                    client.ClientId = "801317241";
                    client.ClientSecret = "7c00fece2e444ca7bba1a48ff635c8a0";
                    break;
                case SocialType.Renren:
                    client.ClientId = "227650";
                    client.ClientApiKey = "67c6a7fcf1754d20991d42299378751f";
                    client.ClientSecret = "6d0d97c1ba9b417fb2beadb9cd55a5bc ";
                    break;
                default:
                    break;
            }
            return client;
        }
    }
}
