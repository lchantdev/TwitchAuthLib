using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAuthLib.Models
{
    public class GenerateSignInUrlResponse
    {
        public string Url { get; set; }
        public string State { get; set; }
        public string Scopes { get; set; }
        public string RedirectUri { get; set; }
    }
}
