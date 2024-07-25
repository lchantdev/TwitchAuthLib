using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAuthLib.Exceptions
{
    public class TwitchTokenRefreshException : Exception
    {
        public TwitchTokenRefreshException(string message) : base(message)
        {
        }

        public TwitchTokenRefreshException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
