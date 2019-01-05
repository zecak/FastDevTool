using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace RunTaskForAny.Security.Encrypt
{
    /// <summary>
    /// 非对称RSA
    /// </summary>
    public class RSA_
    {
        private CertRSA crsa;
        public CertRSA CRSA
        {
            get
            {
               return crsa;
            }
        }

        private RSACryptoServiceProvider rsa;
        public RSA_(bool forjs=false)
        {
            rsa = new RSACryptoServiceProvider();
            crsa = new CertRSA();
            crsa.PrivateKey = GetPrivateKey();
            crsa.PublicKey = GetPublicKey();
            crsa.PubKeyExponent = GetPublicKeyExponent(forjs);
            crsa.PubKeyModulus = GetPublicKeyModulus(forjs);
        }
        /// <summary>
        /// 得到公钥
        /// </summary>
        /// <returns></returns>
        public string GetPublicKey()
        {
            return rsa.ToXmlString(false);
        }
        /// <summary>
        /// 得到私钥
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey()
        {
            return rsa.ToXmlString(true);
        }
        /// <summary>
        /// setMaxDigits(129);var key = new RSAKeyPair({$PublicKeyExponent}, "", {$PublicKeyModulus});encryptedString(key, "123456");
        /// 
        /// </summary>
        /// <param name="isJS"></param>
        /// <returns></returns>
        public string GetPublicKeyExponent(bool isJS=false)
        {
            RSAParameters parameter = rsa.ExportParameters(false);
            if (!isJS) return Convert.ToBase64String(parameter.Exponent); 
            return BytesToHexString(parameter.Exponent); 
        }
        /// <summary>
        /// setMaxDigits(129);var key = new RSAKeyPair({$PublicKeyExponent}, "", {$PublicKeyModulus});encryptedString(key, "123456");
        /// </summary>
        /// <param name="isJS"></param>
        /// <returns></returns>
        public string GetPublicKeyModulus(bool isJS = false)
        {
            RSAParameters parameter = rsa.ExportParameters(true);
            if (!isJS) return Convert.ToBase64String(parameter.Modulus);
            return BytesToHexString(parameter.Modulus);
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">待加密字符串</param>
        /// <param name="PublicKey">公钥</param>
        /// <returns></returns>
        public string Encrypt_JS(string Source, string PublicKey)
        {
            var base64 = HexStringToBytes(Source);
            byte[] done = Encrypt(base64, PublicKey);
            return BytesToHexString(done);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">待解密字符串</param>
        /// <param name="PrivateKey">私钥</param>
        /// <returns></returns>
        public string Decrypt_JS(string Source, string PrivateKey)
        {
            Encoding encode = Encoding.Default;
            var base64 = HexStringToBytes(Source);
            byte[] done = Decrypt(base64, PrivateKey);
            return encode.GetString(done);
        }
        public string BytesToHexString(byte[] input)
        {
            StringBuilder hexString = new StringBuilder(64);

            for (int i = 0; i < input.Length; i++)
            {
                hexString.Append(String.Format("{0:X2}", input[i]));
            }
            return hexString.ToString();
        }
        public byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }

            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }

            byte[] result = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return result;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">待加密字符串</param>
        /// <param name="PublicKey">公钥</param>
        /// <returns></returns>
        public string Encrypt(string Source, string PublicKey)
        {
            Encoding encode = Encoding.Default;
            return Encrypt(Source, encode, PublicKey);
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">待加密字符串</param>
        /// <param name="PublicKey">公钥</param>
        /// <returns></returns>
        public string Encrypt(string Source,Encoding encode, string PublicKey)
        {
            var base64=Convert.FromBase64String(Convert.ToBase64String(encode.GetBytes(Source)));
            byte[] done = Encrypt(base64, PublicKey);
            return Convert.ToBase64String(done);
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">待加密字符数组</param>
        /// <param name="PublicKey">公钥</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] Source, string PublicKey)
        {
            rsa.FromXmlString(PublicKey);
            return rsa.Encrypt(Source, false);
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="inFileName">待加密文件路径</param>
        /// <param name="outFileName">加密后文件路径</param>
        /// <param name="PublicKey">公钥</param>
        public void Encrypt(string inFileName, string outFileName, string PublicKey)
        {
            rsa.FromXmlString(PublicKey);
            FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            byte[] bin = new byte[1000];
            long rdlen = 0;
            long totlen = fin.Length;
            int len;

            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 1000);
                byte[] bout=rsa.Encrypt(bin,false);
                fout.Write(bout, 0, bout.Length);
                rdlen = rdlen + len;
            }
          
            fout.Close();
            fin.Close();

        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">待解密字符串</param>
        /// <param name="PrivateKey">私钥</param>
        /// <returns></returns>
        public string Decrypt(string Source, string PrivateKey)
        {
            Encoding encode = Encoding.Default;
            return Decrypt(Source, encode, PrivateKey);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">待解密字符串</param>
        /// <param name="PrivateKey">私钥</param>
        /// <returns></returns>
        public string Decrypt(string Source, Encoding encode, string PrivateKey)
        {
            var base64 = Convert.FromBase64String(Source);
            byte[] done = Decrypt(base64, PrivateKey);
            return encode.GetString(done);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">待解密字符数组</param>
        /// <param name="PrivateKey">私钥</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] Source, string PrivateKey)
        {
            rsa.FromXmlString(PrivateKey);
            return rsa.Decrypt(Source, false);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="inFileName">待解密文件路径</param>
        /// <param name="outFileName">解密后文件路径</param>
        /// <param name="PrivateKey">私钥</param>
        public void Decrypt(string inFileName, string outFileName, string PrivateKey)
        {
            rsa.FromXmlString(PrivateKey);
            FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            byte[] bin = new byte[1000];
            long rdlen = 0;
            long totlen = fin.Length;
            int len;

            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 1000);
                byte[] bout = rsa.Decrypt(bin, false);
                fout.Write(bout, 0, bout.Length);
                rdlen = rdlen + len;
            }
            
            fout.Close();
            fin.Close();

        }


    }

    [Serializable]
    public class CertRSA
    {
        private string privateKey;
        private string pubKeyExponent;
        private string pubKeyModulus;
        private string publicKey;

        public string PrivateKey
        {
            get
            {
                return this.privateKey;
            }
            set
            {
                this.privateKey = value;
            }
        }

        public string PubKeyExponent
        {
            get
            {
                return this.pubKeyExponent;
            }
            set
            {
                this.pubKeyExponent = value;
            }
        }

        public string PubKeyModulus
        {
            get
            {
                return this.pubKeyModulus;
            }
            set
            {
                this.pubKeyModulus = value;
            }
        }

        public string PublicKey
        {
            get
            {
                return this.publicKey;
            }
            set
            {
                this.publicKey = value;
            }
        }
    }


}
