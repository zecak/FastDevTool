using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunTaskForAny.Security
{
    /// <summary>
    /// 通用API加密协议
    /// </summary>
    public class API
    {
        /// <summary>
        /// 检查[客户端时间]在几秒内是否在[服务端时间]范围上
        /// </summary>
        /// <param name="clientTime">客户端时间</param>
        /// <param name="serverTime">服务端时间</param>
        /// <param name="timeLagSeconds">秒数,默认60秒</param>
        /// <returns></returns>
        public static bool IsInServerTime(DateTime clientTime, DateTime serverTime, double timeLagSeconds = 60)
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

        public static DateTime TimestampToDateTime(long timestamp, bool milliseconds)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
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
