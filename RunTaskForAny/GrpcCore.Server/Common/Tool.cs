using GrpcCore.Common;
using GrpcCore.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace GrpcCore.Server.Common
{
    public class Tool
    {
        static log4net.ILog log = null;
        public static log4net.ILog Log
        {
            get
            {
                if (log == null)
                {
                    var logCfg = new System.IO.FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(logCfg);
                    log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                }
                return log;
            }
        }

        const string settingFileName = "setting.json";
        static SettingInfo settingjson = null;

        public static SettingInfo Setting
        {
            get
            {
                if (settingjson == null)
                {
                    var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingFileName);
                    if (!System.IO.File.Exists(settingPath))
                    {
                        var temp = new SettingInfo() { Name = "GrpcCore Server Service", ServiceName = "GrpcCore Server Service", Description = "GrpcCore Server Service", ServerIP = "127.0.0.1", ServerKey = "12345678", ServerPort = "8090" };
                        File.WriteAllText(settingPath, temp.ToJson());
                    }
                    settingjson = System.IO.File.ReadAllText(settingPath).JsonTo<SettingInfo>();
                }
                return settingjson;
            }
            set
            {
                settingjson = value;
            }
        }


        public static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }

        /// <summary>
        /// 检查[客户端时间]在几秒内是否在[服务端时间]范围上
        /// </summary>
        /// <param name="clientTime">客户端时间</param>
        /// <param name="serverTime">服务端时间</param>
        /// <param name="timeLagSeconds">秒数,默认60秒</param>
        /// <returns></returns>
        public static bool IsTimeout(DateTime clientTime, DateTime serverTime, double timeLagSeconds = 60)
        {
            var mintime = serverTime.AddSeconds(-timeLagSeconds);
            var maxtime = serverTime.AddSeconds(timeLagSeconds);
            var rz = clientTime.CompareTo(mintime) != -1;// 1或0 为真
            var rz2 = clientTime.CompareTo(maxtime) != 1;// -1或0 为真
            return rz && rz2;
        }


        /// <summary>本地时间转换成UTC时间</summary>
        /// <param name="vDate">待转换的时间</param>
        /// <param name="Milliseconds">是否精确到毫秒</param>
        /// <returns>UTC时间</returns>
        public static long DateTimeToUTC(DateTime vDate, bool Milliseconds)
        {
            vDate = vDate.ToUniversalTime();
            var dtZone = new DateTime(1970, 1, 1, 0, 0, 0);
            if (Milliseconds)
            {
                return (long)vDate.Subtract(dtZone).TotalMilliseconds;
            }
            return (long)vDate.Subtract(dtZone).TotalSeconds;
        }

        /// <summary>
        /// 对应java中的des加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptJAVADESString(string str, string key)
        {
            MemoryStream ms = null;
            CryptoStream cs = null;
            string outstr = "";
            try
            {
                //在.Net中，利用DESCryptoServiceProvider進行加密。加密的Mode為ECB，Padding為None、Zeros與PKCS7。 
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;
                byte[] inputByteArray = UTF8Encoding.UTF8.GetBytes(str);// Encoding.ASCII.GetBytes(pToEncrypt);
                des.Key = UTF8Encoding.UTF8.GetBytes(key);
                des.IV = UTF8Encoding.UTF8.GetBytes(key);
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                outstr = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                outstr = str;
            }
            finally
            {
                ms.Close();
                cs.Close();
            }
            return outstr;
        }

        /// <summary>
        ///  对应java中默认的des解密
        /// </summary>
        /// <param name="str">已经加密的字符串(Base64格式的)</param>
        /// <param name="sKey">密钥</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptJAVADESString(string str, string sKey)
        {
            MemoryStream ms = null;
            CryptoStream cs = null;
            string outstr = "";
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;

                byte[] inputByteArray = Convert.FromBase64String(str);
                des.Key = UTF8Encoding.UTF8.GetBytes(sKey);
                des.IV = UTF8Encoding.UTF8.GetBytes(sKey);
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                outstr = Encoding.UTF8.GetString(ms.ToArray());

            }
            catch (Exception ex)
            {
                outstr = "";
            }
            finally
            {
                cs.Close();
                ms.Close();
            }
            return outstr;
        }


        public static string GetInternalIP()
        {
            string ip = "";
            var cur_ip_infos = NetworkInterface.GetAllNetworkInterfaces().Where(m => m.OperationalStatus == OperationalStatus.Up).ToArray();
            if (cur_ip_infos == null)
            {
                return "";
            }
            foreach (var cur_ip_info in cur_ip_infos)
            {
                var ipp = cur_ip_info.GetIPProperties();
                var gateway = ipp.GatewayAddresses;
                if (gateway.Count > 0)
                {
                    var add = ipp.UnicastAddresses.FirstOrDefault(m => m.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if (add != null)
                    {
                        ip = add.Address.ToString();
                    }
                }

            }
            return ip;
        }

        ///   <summary> 
        ///   获取网卡硬件地址 
        ///   </summary> 
        ///   <returns> string </returns> 
        public static string GetMacAddress()
        {
            try
            {
                string mac = "";
                var cur_ip_infos = NetworkInterface.GetAllNetworkInterfaces().Where(m => m.OperationalStatus == OperationalStatus.Up).ToArray();
                if (cur_ip_infos == null)
                {
                    return "";
                }
                foreach (var cur_ip_info in cur_ip_infos)
                {
                    var ipp = cur_ip_info.GetIPProperties();
                    var gateway = ipp.GatewayAddresses;
                    if (gateway.Count > 0)
                    {
                        if (gateway.FirstOrDefault(m => m.Address.ToString() != "::") != null)
                        {
                            mac = cur_ip_info.GetPhysicalAddress().ToString();
                        }

                    }

                }
                return mac;
            }
            catch
            {
                return "";
            }
        }

        private static string _computerName = "";
        public static string GetComputerName()
        {
            try
            {
                if (string.IsNullOrEmpty(_computerName))
                    _computerName = System.Environment.GetEnvironmentVariable("ComputerName");
                return _computerName;
            }
            catch
            {
                return "";
            }
        }

        public static string DownFile(Uri uri, string filepath)
        {
            var path = filepath;
            var name = uri.Segments[uri.Segments.Length - 1];

            for (int i = 0; i < uri.Segments.Length - 1; i++)
            {
                var str = uri.Segments[i].Replace("/", "");
                if (!string.IsNullOrWhiteSpace(str))
                {
                    path = System.IO.Path.Combine(path, str);
                }
            }
            System.IO.Directory.CreateDirectory(path);

            var filename = System.IO.Path.Combine(path, name);
            if (!System.IO.File.Exists(filename))
            {
                try
                {
                    System.Net.WebClient myWebClient = new System.Net.WebClient();
                    myWebClient.DownloadFile(uri, filename);
                }
                catch (Exception ex)
                {
                    Tool.Log.Error(ex);
                    return null;
                }
            }
            return filename;
        }

        public static List<string> DownFiles(List<Uri> uris, string filepath)
        {
            List<string> list = new List<string>();
            System.IO.Directory.CreateDirectory(filepath);
            System.Net.WebClient myWebClient = new System.Net.WebClient();
            foreach (var uri in uris)
            {
                var name = uri.Segments[uri.Segments.Length - 1];
                var filename = System.IO.Path.Combine(filepath, name);
                if (!System.IO.File.Exists(filename))
                {
                    try
                    {
                        myWebClient.DownloadFile(uri, filename);
                        list.Add(filename);
                    }
                    catch (Exception ex)
                    {
                        list.Add(null);
                        Tool.Log.Error(ex);
                    }
                }

            }
            return list;
        }

    }
}
