using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAuthLib.Exceptions
{
    public class TwitchAppTokenException :Exception
    {
        public TwitchAppTokenException(string message) : base(message)
        {
        }
    }
}
