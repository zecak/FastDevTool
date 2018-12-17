using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FastDev.Web.Common
{
    public static class WebEx
    {
        public static string ToJson(this object obj)
        {
            try
            {
                var datetimeFormat = "yyyy-MM-dd HH:mm:ss";
                //日期和间都管用  
                JsonSerializerSettings jsSettings = new JsonSerializerSettings();
                jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jsSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter
                {
                    DateTimeFormat = datetimeFormat
                });
                return JsonConvert.SerializeObject(obj, jsSettings);
                //return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            }
            catch (Exception)
            {
                return "";
            }

        }
    }
}