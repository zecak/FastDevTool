using Newtonsoft.Json;
using System;

namespace GrpcCore.Common
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
        public static long ToTimestamp(this DateTime vDate, bool Milliseconds = true)
        {
            vDate = vDate.ToUniversalTime();
            var dtZone = new DateTime(1970, 1, 1, 0, 0, 0);
            if (Milliseconds)
            {
                return (long)vDate.Subtract(dtZone).TotalMilliseconds;
            }
            return (long)vDate.Subtract(dtZone).TotalSeconds;
        }

        public static DateTime ToDateTime(this long timestamp, bool milliseconds = true)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0)); // 当地时区
            if (milliseconds)
            {
                return startTime.AddMilliseconds(timestamp);
            }
            else
            {
                return startTime.AddSeconds(timestamp);
            }
        }
    }
}
