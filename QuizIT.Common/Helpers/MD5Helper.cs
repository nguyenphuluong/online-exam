using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace QuizIT.Common.Helpers
{
    public class MD5Helper
    {
        public static string Encode(string str)
        {
            using MD5 md5hash = MD5.Create();
            byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
