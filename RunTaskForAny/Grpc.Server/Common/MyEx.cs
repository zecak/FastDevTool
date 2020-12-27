using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Common
{
    public static class MyEx
    {
        public static string ToJson(this object obj)
        {
            try
            {
                var datetimeFormat = "yyyy-MM-dd HH:mm:ss";
                //日期和间都管用  
                JsonSerializerSettings jsSettings = new JsonSerializerSettings();
                jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jsSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                jsSettings.NullValueHandling = NullValueHandling.Ignore;
                jsSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter
                {
                    DateTimeFormat = datetimeFormat
                });
                return JsonConvert.SerializeObject(obj, jsSettings);
                //return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
                return "";
            }

        }

        public static T JsonTo<T>(this string json)
        {
            try
            {
                var datetimeFormat = "yyyy-MM-dd HH:mm:ss";
                //日期和间都管用  
                JsonSerializerSettings jsSettings = new JsonSerializerSettings();
                jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jsSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                jsSettings.NullValueHandling = NullValueHandling.Ignore;
                jsSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter
                {
                    DateTimeFormat = datetimeFormat
                });
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, jsSettings);
            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex);
                return default(T);
            }

        }

    }
}
