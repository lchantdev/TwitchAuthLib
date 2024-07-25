using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAuthLib.Models
{
    public enum TwitchTokenType
    {
        User,
        App
    }

    public class TwitchToken
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public TwitchTokenType TokenType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int ExpiresIn { get; set; }
    }
}
