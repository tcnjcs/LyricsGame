using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using LyricsGame.Models;

namespace LyricsGame
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            OAuthWebSecurity.RegisterFacebookClient(
                appId: "485105974853614",
                appSecret: "796f2aa3f9091c5c133e466e7db00416");

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
