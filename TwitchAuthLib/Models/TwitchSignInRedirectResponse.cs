using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAuthLib.Models
{
    public class TwitchSignInRedirectResponse
    {
        public string? Code { get; set; }
        public string? State { get; set; }
        public string? Scope { get; set; }
        public string? Error { get; set; }

        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }
    }
}
