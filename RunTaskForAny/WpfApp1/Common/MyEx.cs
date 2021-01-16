using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common
{
    public static class MyEx
    {
        public static string ToJson(this object obj)
        {
            try
            {
                var datetimeFormat = "yyyy-MM-dd HH:mm:ss";
                var dtc = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                dtc.DateTimeFormat = datetimeFormat;
                //日期和间都管用  
                JsonSerializerSettings jsSettings = new JsonSerializerSettings();
                jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jsSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                jsSettings.NullValueHandling = NullValueHandling.Ignore;
                jsSettings.Formatting = Formatting.Indented;
                jsSettings.Converters.Add(dtc);
                return JsonConvert.SerializeObject(obj, jsSettings);
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public static T JsonTo<T>(this string json)
        {
            try
            {
                var datetimeFormat = "yyyy-MM-dd HH:mm:ss";
                var dtc = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                dtc.DateTimeFormat = datetimeFormat;


                //日期和间都管用  
                JsonSerializerSettings jsSettings = new JsonSerializerSettings();
                jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jsSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                jsSettings.NullValueHandling = NullValueHandling.Ignore;
                jsSettings.Formatting = Formatting.Indented;
                jsSettings.Converters.Add(dtc);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, jsSettings);
            }
            catch (Exception ex)
            {
                return default(T);
            }

        }

        /// <summary>本地时间转换成UTC时间</summary>
        /// <param name="vDate">待转换的时间</param>
        /// <param name="Milliseconds">是否精确到毫秒</param>
        /// <returns>UTC时间</returns>
        public static long DateTimeToUTC(this DateTime vDate, bool Milliseconds=false)
        {
            vDate = vDate.ToUniversalTime();
            var dtZone = new DateTime(1970, 1, 1, 0, 0, 0);
            if (Milliseconds)
            {
                return (long)vDate.Subtract(dtZone).TotalMilliseconds;
            }
            return (long)vDate.Subtract(dtZone).TotalSeconds;
        }

        public static string ToMd5(this string s)
        {
            Encoding encoding = Encoding.UTF8;
            var bytes = encoding.GetBytes(s);
            var md5Bytes = Encrypt(bytes);
            var md5 = "";
            foreach (var b in md5Bytes)
            {
                md5 += b.ToString("X2");
            }

            return md5;
        }
        static byte[] Encrypt(byte[] Source)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Source);
            return result;
        }
    }
}
