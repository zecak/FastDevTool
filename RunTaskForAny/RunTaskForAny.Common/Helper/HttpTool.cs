using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RunTaskForAny.Common.Helper
{
    public class HttpTool
    {
        public static string AjaxGet(string url, SortedList<string, string> keys)
        {
            try
            {
                var urlParams = "";
                if (keys != null && keys.Count > 0)
                {
                    urlParams = keys.Aggregate("", (current, key) => current + (key.Key + "=" + key.Value + "&"));
                    if (urlParams.Length > 0) { urlParams = urlParams.Substring(0, urlParams.Length - 1); }
                }
                if (!string.IsNullOrWhiteSpace(urlParams)) { urlParams = "?" + urlParams; }
                var http = new HttpClient();
                http.Url = url + urlParams;
                http.Verb = HttpVerb.GET;
                return http.GetString();
            }
            catch (System.Exception ex)
            {
                Tool.Log.Warn(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return ex.Message;
            }
        }

        public static string AjaxGet(string url)
        {
            return AjaxGet(url, null);
        }

        public static string AjaxPost(string url, SortedList<string, string> keys)
        {
            try
            {
                var http = new HttpClient();
                http.Url = url;
                http.Verb = HttpVerb.POST;
                foreach (var k in keys)
                {
                    http.PostingData.Add(k.Key, k.Value);
                }
                return http.GetString();
            }
            catch (System.Exception ex)
            {
                Tool.Log.Warn(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
                return ex.Message;
            }
        }



    }
}
