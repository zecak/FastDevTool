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
            var tablename = context.Request.Form.Get("tname");
            var pageindex = context.Request.Form.Get("pindex");
            var pagesize = context.Request.Form.Get("psize");
            if (string.IsNullOrWhiteSpace(tablename)|| string.IsNullOrWhiteSpace(pageindex) || string.IsNullOrWhiteSpace(pagesize))
            {
                context.Response.Write(new Msg() { code = -1, msg = "缺少参数", }.ToJson());
                context.Response.End();
            }
            paging.PageIndex = Convert.ToInt32(pageindex);
            paging.PageSize = Convert.ToInt32(pagesize);
            var table= localDbContext.GetListForPage(tablename, paging);

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