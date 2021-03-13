
using GrpcCore.Common;
using GrpcCore.Common.Security;
using GrpcCore.DB.Database;
using GrpcCore.DB.DBCache;
using System;
using System.Linq;

namespace GrpcCore.DB
{
    public class Helper
    {
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
                throw new Exception("未找到配置文件" + settingFileName);
            }
            settingjson = System.IO.File.ReadAllText(settingPath).JsonTo<SettingInfo>();
            if (settingjson == null)
            {
                throw new Exception("配置文件" + settingFileName + "加载错误"); throw new Exception("配置文件" + settingFileName + "加载错误");
            }
        }

        public void Redis()
        {
            //var redisHelper = new Redis("127.0.0.1:6379,password=123456");
            //bool r1 = redisHelper.Insert("key", "Hello Word!");
            //if (r1)
            //{
            //    Console.WriteLine(redisHelper.Get("key"));
            //    redisHelper.Remove("key");
            //}
        }

        public static void UpdateForWanjuOfflineMerchantWallet(decimal price)
        {
            using (var db = new WanjuOfflineContext())
            {
                var m = db.Merchants.FirstOrDefault(a => a.Uuid == Setting.MerchantUUID);
                if (m == null) { throw new Exception("商家不存在:" + Setting.MerchantUUID); }
                var m_wallet = db.MerchantWallets.FirstOrDefault(a => a.Uuid == Setting.MerchantWalletUUID);
                if (m_wallet == null) { throw new Exception("商家钱包不存在:" + Setting.MerchantWalletUUID); }
                m_wallet.Balance += price;
                db.SaveChanges();
            }
        }

        public static decimal GetForWanjuOfflineMerchantWallet()
        {
            using (var db = new WanjuOfflineContext())
            {
                var m = db.Merchants.FirstOrDefault(a => a.Uuid == Setting.MerchantUUID);
                if (m == null) { throw new Exception("商家不存在:" + Setting.MerchantUUID); }
                var m_wallet = db.MerchantWallets.FirstOrDefault(a => a.Uuid == Setting.MerchantWalletUUID);
                if (m_wallet == null) { throw new Exception("商家钱包不存在:" + Setting.MerchantWalletUUID); }
                return m_wallet.Balance??0;
            }
        }

        public static void UpdateForWanjuOfflineEmployeeWallet(string uuid, decimal price)
        {
            using (var db = new WanjuOfflineContext())
            {
                var user = db.Employees.FirstOrDefault(u => u.Uuid == uuid);
                if (user == null) { throw new Exception("用户不存在:" + uuid); }
                var user_wallet = db.EmployeeWallets.FirstOrDefault(m => m.EmployeeId == user.Id);
                if (user_wallet == null) { throw new Exception("用户钱包不存在:" + uuid); }
                user_wallet.Balance += price;
                db.SaveChanges();
            }
        }
        public static decimal GetForWanjuOfflineEmployeeWallet(string uuid)
        {
            using (var db = new WanjuOfflineContext())
            {
                var user = db.Employees.FirstOrDefault(u => u.Uuid == uuid);
                if (user == null) { throw new Exception("用户不存在:" + uuid); }
                var user_wallet = db.EmployeeWallets.FirstOrDefault(m => m.EmployeeId == user.Id);
                if (user_wallet == null) { throw new Exception("用户钱包不存在:" + uuid); }
                return user_wallet.Balance??0;
            }
        }
    }
}
