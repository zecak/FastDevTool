using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace RunTaskForAny.Common.Helper
{
    public class HttpTool2
    {
        CookieContainer theCC = new CookieContainer();
        public string Ajax(string Url, string postDataStr, Encoding encoding, string sMode = "POST", bool keepalive = true, string referer = "")
        {
            Encoding myEncoding = Encoding.UTF8;
            string sUrl = Url;
            string sPostData = postDataStr;
            string sContentType = "application/x-www-form-urlencoded";
            HttpWebRequest req;

            // == main ==
            try
            {
                // init
                req = HttpWebRequest.Create(sUrl) as HttpWebRequest;
                req.Method = sMode.ToUpper();
                req.Accept = "*/*";
                req.KeepAlive = keepalive;
                req.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.5 Safari/534.7";
                req.Referer = referer;
                if (keepalive)
                    req.CookieContainer = theCC;
                if (0 == string.Compare("POST", req.Method))
                {
                    byte[] bufPost = myEncoding.GetBytes(sPostData);
                    req.ContentType = sContentType;
                    req.ContentLength = bufPost.Length;
                    Stream newStream = req.GetRequestStream();
                    newStream.Write(bufPost, 0, bufPost.Length);
                    newStream.Close();
                }

                // Response
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                try
                {

                    using (Stream resStream = res.GetResponseStream())
                    {
                        using (StreamReader resStreamReader = new StreamReader(resStream, encoding))
                        {
                            return resStreamReader.ReadToEnd();
                        }
                    }

                }
                finally
                {
                    res.Close();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string AjaxGet(string Url, Encoding encoding, bool keepalive = true, string referer = "")
        {
           return Ajax(Url, "", encoding, "get", keepalive, referer);
        }

        public string AjaxPost(string Url, string postDataStr, Encoding encoding, bool keepalive = true, string referer = "")
        {
            return Ajax(Url, postDataStr, encoding, "post", keepalive, referer);
        }

        static HttpTool2 instance = null;
        public static HttpTool2 Instance 
        {
            get 
            {
                if (instance==null)
                {
                    instance = new HttpTool2();
                }
                return instance;
            }
        }
    }
}
