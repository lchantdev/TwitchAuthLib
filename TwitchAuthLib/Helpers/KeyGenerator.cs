using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAuthLib.Helpers
{
    /// <summary>
    /// Used to generate random strings for certain resources(such as invite codes)
    /// </summary>
    public abstract class KeyGenerator
    {
        private static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        /// <summary>
        /// Returns a randomised key of the specified size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetUniqueKey(int size)
        {
            var data = RandomNumberGenerator.GetBytes(4 * size);

            StringBuilder result = new(size);
            for (var i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static string GetUniqueKeyOriginal_BIASED(int size)
        {
            var data = RandomNumberGenerator.GetBytes(size);

            StringBuilder result = new(size);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}
