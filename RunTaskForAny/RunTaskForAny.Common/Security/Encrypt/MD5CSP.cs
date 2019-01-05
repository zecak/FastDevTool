using System;
using System.IO;
using System.Text;

namespace RunTaskForAny.Security.Encrypt
{
    /// <summary>
    /// MD5
    /// </summary>
    public class MD5CSP
    {
        public string Encrypt(string Source)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(Source);
            // This is one implementation of the abstract class MD5.
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            return  Convert.ToBase64String(result);

        }
        public byte[] Encrypt(byte[] Source)
        {          
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Source);
            return result;
        }

    }
}
