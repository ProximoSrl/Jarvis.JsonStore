using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonObjectService.Core.Support
{
    public static class HashUtils
    {
        public static String GetHash(String str)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}
