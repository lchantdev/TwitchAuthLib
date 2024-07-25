using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAuthLib
{
    internal class Constants
    {
        public const string TwitchAuthBaseUrl = "https://id.twitch.tv/oauth2";

        public const string UserGrantType = "code";
        public const string UserGrantTypeImplicit = "token";
        public const string UserReturnGrantType = "authorization_code";
        public const string AppGrantType = "client_credentials";
        public const string RefreshGrantType = "refresh_token";
    }
}
