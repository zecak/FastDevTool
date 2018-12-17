using FastDev.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FastDev.Web.Ajax
{
    /// <summary>
    /// one 的摘要说明
    /// </summary>
    public class one : IHttpHandler
    {
        LocalDbContext localDbContext = new LocalDbContext();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //获取列表数据
            var paging = new Paging();
           var table= localDbContext.GetListForPage("u_o_o_j_k_j", paging);

            context.Response.Write(new Msg() { code=1, msg="操作成功",data= table, page= paging }.ToJson());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}