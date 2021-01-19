using GrpcLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Agent.Common
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

        static DirectoryMonitor monitor2 = null;

        const string settingFileName = "setting.json";
        static SettingInfo settingjson = null;

        public static SettingInfo Setting
        {
            get
            {
                if (settingjson == null)
                {
                    Monitor_Change2("");

                    monitor2 = new DirectoryMonitor(AppDomain.CurrentDomain.BaseDirectory, settingFileName);
                    monitor2.Change += Monitor_Change2;
                    monitor2.Start();

                    //var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingFileName);
                    //if (!System.IO.File.Exists(settingPath))
                    //{
                    //    var temp = new SettingInfo() { Name = "Grpc Agent Service Center", ServiceName = "Grpc Agent Service Center", Description = "Grpc Agent Service Center", AgentIP = "127.0.0.1", AgentKey = "123456", AgentPort = "8080", ServerRun="1", ServerList=new List<ServerInfo>() { new ServerInfo() { IP = "127.0.0.1", Key = "12345678", Port = "8090" } } };
                    //    File.WriteAllText(settingPath, temp.ToJson());
                    //}
                    //settingjson = System.IO.File.ReadAllText(settingPath).JsonTo<SettingInfo>();
                }
                return settingjson;
            }
            set
            {
                if (value != null)
                {
                    settingjson = value;
                    var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingFileName);
                    System.IO.File.WriteAllText(settingPath, settingjson.ToJson());
                }
            }
        }

        private static void Monitor_Change2(string _path)
        {
            var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingFileName);
            if (!System.IO.File.Exists(settingPath)) 
            { 
                Log.Error("未找到配置文件" + settingFileName); 
                throw new Exception("未找到配置文件" + settingFileName);
            }
            settingjson = System.IO.File.ReadAllText(settingPath).JsonTo<SettingInfo>();
            if (settingjson == null)
            {
                Log.Error("配置文件" + settingFileName + "加载错误"); throw new Exception("配置文件" + settingFileName + "加载错误");
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
        ///   获取cpu序列号     
        ///   </summary> 
        ///   <returns> string </returns> 
        public static string GetCpuInfo()
        {
            try
            {
                string cpuInfo = string.Empty;
                using (ManagementClass cimobject = new ManagementClass("Win32_Processor"))
                {
                    ManagementObjectCollection moc = cimobject.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        cpuInfo = mo.Properties["Name"].Value.ToString();
                        cpuInfo += "|" + mo.Properties["DeviceID"].Value.ToString();
                        cpuInfo += "|" + mo.Properties["Caption"].Value.ToString();
                        break;
                    }
                }
                return cpuInfo ?? "";
            }
            catch
            {
                return "";
            }

        }

        ///   <summary> 
        ///   获取硬盘ID     
        ///   </summary> 
        ///   <returns> string </returns> 
        public static string GetHDid()
        {
            try
            {
                string HDid = "";
                using (ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive"))
                {
                    ManagementObjectCollection moc1 = cimobject1.GetInstances();
                    foreach (ManagementObject mo in moc1)
                    {
                        HDid = mo.Properties["SerialNumber"].Value.ToString();
                        break;
                    }
                }
                return HDid.Trim().ToString();
            }
            catch
            {
                return "";
            }
        }

        public static string GetCaption()
        {
            try
            {
                string HDid = "";
                using (ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive"))
                {
                    ManagementObjectCollection moc1 = cimobject1.GetInstances();
                    foreach (ManagementObject mo in moc1)
                    {
                        HDid = mo.Properties["Caption"].Value.ToString();
                        break;
                    }
                }
                return HDid.Trim().ToString();
            }
            catch
            {
                return "";
            }
        }

        //获取硬盘编号
        public static String GetHDSN()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_PhysicalMedia");
                //网上有提到，用Win32_DiskDrive，但是用Win32_DiskDrive获得的硬盘信息中并不包含SerialNumber属性。  
                ManagementObjectCollection moc = mc.GetInstances();
                string strID = "";
                foreach (ManagementObject mo in moc)
                {
                    strID += mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
                return strID.Trim();
            }
            catch
            {
                return "";
            }
        }//End

        public static String GetMemorySN()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc = mc.GetInstances();
                string strID = "";
                foreach (ManagementObject mo in moc)
                {
                    strID = mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
                return strID ?? "";
            }
            catch
            {
                return "";
            }
        }//End

        public static string GetBaseBoard()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_baseboard");
                string biosNumber = string.Empty;
                foreach (ManagementObject mgt in searcher.Get())
                {
                    biosNumber = mgt["Product"].ToString();
                    break;
                }
                return biosNumber ?? "";
            }
            catch
            {
                return "";
            }
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
            
            for (int i = 0; i < uri.Segments.Length-1; i++)
            {
                var str = uri.Segments[i].Replace("/","");
                if(!string.IsNullOrWhiteSpace(str))
                {
                    path = System.IO.Path.Combine(path,str);
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
