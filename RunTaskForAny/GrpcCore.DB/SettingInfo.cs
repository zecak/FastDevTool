using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcCore.DB
{
    public class SettingInfo
    {
        public string MerchantName { get; set; }
        public string MerchantUUID { get; set; }
        public string MerchantWalletUUID { get; set; }
        public string ConnectionString_WanjuOffline { get; set; }
        public string ConnectionString_WanjuOnline { get; set; }

        public string ConnectionString_Redis { get; set; }
    }
}
