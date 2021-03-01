using Grpc.Client.DataManage.Models;
using GrpcLib;
using GrpcLib.Common;
using GrpcLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Client.DataManage.Common
{
    public class Helper
    {
        public static AgentModel Agent { get; set; }

        public static GrpcClient GrpcClientAgent { get; set; }

        public static GrpcClient GrpcClientClient { get; set; }


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
                        var temp = new SettingInfo() { ServerIP = "127.0.0.1", ServerKey = "123456", ServerPort = "8080" };
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

    }
}
