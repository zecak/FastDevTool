using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GrpcLib.Security.Encrypt
{
    public class EncryptHelper
    {
        public enum EncryptType
        {
            None = 0,
            DES = 1,
            RC2 = 2,
            AES = 4,
            RSA = 8,
            RSAJS = 16,
            MD5 = 32,
            MD5Z = 64
        }

        static RSA_ rsaInstance;
        /// <summary>
        /// 非对称RSA
        /// </summary>
        public static RSA_ RSAInstance
        {
            get
            {
                if (rsaInstance == null)
                {
                    rsaInstance = new RSA_();
                }
                return rsaInstance;
            }
        }

        private static RSA_ rsaInstancejs;
        /// <summary>
        /// 非对称RSA for JS,只是生成的PubKeyExponent,PubKeyModulus值不一样
        /// </summary>
        public static RSA_ RSAInstanceJS
        {
            get
            {
                if (rsaInstancejs == null)
                {
                    rsaInstancejs = new RSA_(true);
                }
                return rsaInstancejs;
            }
        }

        static string EnDes(string content, string key, string iv)
        {
            if (key.Length != 8) { throw new Exception("des加密密钥长度只能8位"); }
            DES_ des = new DES_(key, iv);
            return des.Encrypt(content);
        }
        static string EnDes(string content, string key)
        {
            if (key.Length != 8) { throw new Exception("des加密密钥长度只能8位"); }
            DES_ des = new DES_(key);
            return des.Encrypt(content);
        }
        static string DeDes(string content, string key, string iv)
        {
            if (key.Length != 8) { throw new Exception("des加密密钥长度只能8位"); }
            DES_ des = new DES_(key, iv);
            return des.Decrypt(content);
        }
        static string DeDes(string content, string key)
        {
            if (key.Length != 8) { throw new Exception("des加密密钥长度只能8位"); }
            DES_ des = new DES_(key);
            return des.Decrypt(content);
        }
        static string EnRC2(string content, string key)
        {
            RC2_ rc2 = new RC2_(key);
            return rc2.Encrypt(content);
        }
        static string EnRC2(string content, string key, string iv)
        {
            RC2_ rc2 = new RC2_(key, iv);
            return rc2.Encrypt(content);
        }
        static string DeRC2(string content, string key)
        {
            RC2_ rc2 = new RC2_(key);
            return rc2.Decrypt(content);
        }
        static string DeRC2(string content, string key, string iv)
        {
            RC2_ rc2 = new RC2_(key, iv);
            return rc2.Decrypt(content);
        }
        static string EnAES(string content, string key)
        {
            Rijndael_ aes = new Rijndael_(key);
            return aes.Encrypt(content);
        }
        static string EnAES(string content, string key, string iv)
        {
            Rijndael_ aes = new Rijndael_(key, iv);
            return aes.Encrypt(content);
        }
        static string DeAES(string content, string key)
        {
            Rijndael_ aes = new Rijndael_(key);
            return aes.Decrypt(content);
        }
        static string DeAES(string content, string key, string iv)
        {
            Rijndael_ aes = new Rijndael_(key, iv);
            return aes.Decrypt(content);
        }
        public static string MD5Z(string content)
        {
            MD5CSP md5 = new MD5CSP();
            return md5.Encrypt(content);
        }
        public static string MD5(string content)
        {
            {
                //微软md5方法参考return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
                byte[] b = Encoding.Default.GetBytes(content);
                b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
                string ret = "";
                for (int i = 0; i < b.Length; i++)
                    ret += b[i].ToString("X").PadLeft(2, '0');
                return ret;
            }
        }


        public static string APIEncode(string content, EncryptType etype, string key = "")
        {
            string result = string.Empty;
            switch (etype)
            {
                case EncryptType.None:
                    result = content;
                    break;
                case EncryptType.DES:
                    result = EnDes(content, key, key);
                    break;
                case EncryptType.RC2:
                    result = EnRC2(content, key, key);
                    break;
                case EncryptType.AES:
                    result = EnAES(content, key, key);
                    break;
                case EncryptType.RSA:
                    result = RSAInstance.Encrypt(content, key);
                    break;
                case EncryptType.RSAJS:
                    result = RSAInstanceJS.Encrypt(content, key);
                    break;
                case EncryptType.MD5:
                    result = MD5(content + key);
                    break;
                case EncryptType.MD5Z:
                    result = MD5Z(content + key);
                    break;
                default:
                    break;
            }
            return result;
        }

        public static string APIDecode(string content, EncryptType etype, string key = "", string deMD5 = "")
        {
            try
            {

                string result = string.Empty;
                switch (etype)
                {
                    case EncryptType.None:
                        result = content;
                        break;
                    case EncryptType.DES:
                        result = DeDes(content, key, key);
                        break;
                    case EncryptType.RC2:
                        result = DeRC2(content, key, key);
                        break;
                    case EncryptType.AES:
                        result = DeAES(content, key, key);
                        break;
                    case EncryptType.RSA:
                        result = RSAInstance.Decrypt(content, key);
                        break;
                    case EncryptType.RSAJS:
                        result = RSAInstanceJS.Decrypt(content, key);
                        break;
                    case EncryptType.MD5:
                        result = (MD5(deMD5 + key) == content ? content : null);
                        break;
                    case EncryptType.MD5Z:
                        result = (MD5Z(deMD5 + key) == content ? content : null);
                        break;
                    default:
                        break;
                }
                return result;

            }
            catch { return null; }
        }

        public static string APIEncode(string text, string type, string key)
        {
            switch (type.ToUpper())
            {
                case "AES":
                    return APIEncode(text, EncryptType.AES, key);
                case "DES":
                    return APIEncode(text, EncryptType.DES, key);
                case "RC2":
                    return APIEncode(text, EncryptType.RC2, key);
                case "RSA":
                    return APIEncode(text, EncryptType.RSA, key);
                case "RSAJS":
                    return APIEncode(text, EncryptType.RSAJS, key);
                case "MD5":
                    return APIEncode(text, EncryptType.MD5, key);
                case "MD5Z":
                    return APIEncode(text, EncryptType.MD5Z, key);
                default:
                    return Security.Encrypt.EncryptHelper.MD5(text + key);
            }
        }

        public static string APIDecode(string text, string type, string key, string deMD5 = "")
        {
            switch (type.ToUpper())
            {
                case "AES":
                    return APIDecode(text, EncryptType.AES, key);
                case "DES":
                    return APIDecode(text, EncryptType.DES, key);
                case "RC2":
                    return APIDecode(text, EncryptType.RC2, key);
                case "RSA":
                    return APIDecode(text, EncryptType.RSA, key);
                case "RSAJS":
                    return APIDecode(text, EncryptType.RSAJS, key);
                case "MD5":
                    return APIDecode(text, EncryptType.MD5, key, deMD5);
                case "MD5Z":
                    return APIDecode(text, EncryptType.MD5Z, key, deMD5);
                default:
                    return APIDecode(text, EncryptType.MD5, key, deMD5);
            }
        }

        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string EnBase64(string encode)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(encode);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// base64解码
        /// </summary>
        /// <param name="decode"></param>
        /// <returns></returns>
        public static string DeBase64(string decode)
        {
            byte[] outputb = Convert.FromBase64String(decode);
            return System.Text.Encoding.Default.GetString(outputb);
        }

        public static string ComputeFileMd5(string fileFullPath)
        {
            try
            {
                using (FileStream file = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] hash_byte = md5.ComputeHash(file);
                    string str = System.BitConverter.ToString(hash_byte);
                    str = str.Replace("-", "");
                    return str;
                }  
            }
            catch (Exception e)
            {
                return "";
            }
            
        }
    }
}
