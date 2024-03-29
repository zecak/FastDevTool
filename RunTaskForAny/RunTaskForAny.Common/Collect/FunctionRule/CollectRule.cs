﻿using RunTaskForAny.Common.Helper;
using RunTaskForAny.Common.Collect.FunctionSegment;
using RunTaskForAny.Security.Encrypt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionRule
{
    /// <summary>
    /// 采集规则
    /// </summary>
    public class CollectRule
    {
        int SleepSeconds = 3600;//获取页面时,最多暂停多少秒

        CollectRuleConfig Config;
        public CollectRule(CollectRuleConfig config)
        {
            Config = config;

        }

        string GetUrl(string url)
        {
            var html = "";
            var stoptime = 5;
            while (true)
            {
                html = HttpTool.AjaxGet(url);
                if (html.Contains("内容状态异常") || html.Contains("未能解析此远程名称:") || html.Contains("无法连接到远程服务器") || html.Contains("远程服务器返回错误") || html.Contains("请求被中止") || html.Contains("操作已超时"))
                {
                    Tool.Log.Error("获取页面内容:" + html);
                    Tool.Log.Warn("获取页面内容失败," + stoptime + "秒后继续");
                    System.Threading.Thread.Sleep(stoptime * 1000);

                    if (stoptime < SleepSeconds * 1000)
                    {
                        stoptime++;
                    }
                    continue;
                }
                else
                {
                    break;
                }
            }
            return html;
        }

        public DataTable GetFirstData()
        {
            var firstData = new DataTable();
            Tool.Log.Debug("采集地址:" + Config.Url);

            var html = GetUrl(Config.Url);

            var doc = NSoup.NSoupClient.Parse(html);
            var duan = doc;
            if (Config.FirstSinglePageRuleSegment != null)
            {
                var first_find_element = GetElement(duan, Config.FirstSinglePageRuleSegment);
                if (first_find_element != null)
                {
                    Config.FirstSinglePageListRuleSegmentUrl = GetValue(first_find_element, Config.FirstSinglePageListRuleSegment);

                    Tool.Log.Debug("单页地址:" + Config.FirstSinglePageListRuleSegmentUrl);

                    if (firstData.Columns.Count <= 0)
                    {
                        foreach (var segment in Config.FirstSinglePageRuleSegments)
                        {
                            firstData.Columns.Add(segment.Name);
                        }
                    }

                    DataRow dataRow = firstData.NewRow();//保存采集的数据
                    List<string> lst = new List<string>();

                    foreach (var segment in Config.FirstSinglePageRuleSegments)
                    {
                        lst.Add(GetValue(first_find_element, segment));
                    }

                    dataRow.ItemArray = lst.ToArray();
                    firstData.Rows.Add(dataRow);

                }
                else
                {
                    Tool.Log.Warn("FirstSinglePageRuleSegment:找不到该元素,该规则似乎已失效");
                }

            }
            return firstData;
        }

        public DataTable GetPageListData(string url)
        {
            var listData = new DataTable();

            Tool.Log.Debug("列表地址:" + url);

            var html = GetUrl(url);

            var doc = NSoup.NSoupClient.Parse(html);
            var duan = doc;

            if (listData.Columns.Count <= 0)
            {
                foreach (var segment in Config.ListPageRuleSegments)
                {
                    listData.Columns.Add(segment.Name);
                }

                listData.Columns.Add("页数");
                listData.Columns.Add("页码");

                if (Config.ListContentPageRuleSegment != null)
                {
                    listData.Columns.Add(Config.ListContentPageRuleSegment.Name);
                }

            }

            NSoup.Select.Elements find_elements = GetElements(duan, Config.ListRuleSegment);
            if (find_elements != null && find_elements.Count > 0)
            {
                var number = 1;
                foreach (var element in find_elements)
                {
                    DataRow dataRow = listData.NewRow();//保存采集的数据
                    List<string> lst = new List<string>();

                    foreach (var segment in Config.ListPageRuleSegments)
                    {
                        lst.Add(GetValue(element, segment));
                    }

                    lst.Add(find_elements.Count.ToString());
                    lst.Add(number.ToString());

                    if (Config.ListContentPageRuleSegment != null)
                    {
                        lst.Add(GetValue(element, Config.ListContentPageRuleSegment));
                    }

                    dataRow.ItemArray = lst.ToArray();
                    listData.Rows.Add(dataRow);

                    number++;
                }
            }
            else
            {
                Tool.Log.Warn("ListRuleSegment:找不到该元素,该规则似乎已失效");
            }

            Config.PagingRuleSegmentUrl = GetNextUrl(duan);

            return listData;
        }

        public DataTable GetPageContentData(string url,int pageNumber=-1,int pageIndex=-1)
        {
            var contentData = new DataTable();

            if (string.IsNullOrWhiteSpace(url)) { Tool.Log.Warn("采集地址为空"); return contentData; }
            
            Tool.Log.Debug("[总页" + pageNumber + "]" + "[第" + pageIndex + "页] " + "内容地址:" + url);

            var html = GetUrl(url);
            var doc = NSoup.NSoupClient.Parse(html);
            var duan = doc;

            if (contentData.Columns.Count <= 0)
            {
                foreach (var segment in Config.ContentPageRuleSegments)
                {
                    contentData.Columns.Add(segment.Name);
                }
            }

            DataRow dataRow_content = contentData.NewRow();//保存采集的数据
            List<string> lst_content = new List<string>();

            foreach (var segment in Config.ContentPageRuleSegments)
            {
                lst_content.Add(GetValue(duan, segment));
            }

            dataRow_content.ItemArray = lst_content.ToArray();
            contentData.Rows.Add(dataRow_content);

            return contentData;
        }

        ///// <summary>
        ///// 获取列表数据,为空是未获取到该页面
        ///// </summary>
        ///// <returns></returns>
        //public CollectData GetAllPageList(string page_url = "")
        //{
        //    CollectData collectData = new CollectData();
        //    collectData.ListData = new DataTable();
        //    collectData.ContentData = new DataTable();
        //    collectData.NextPageData = new DataTable();
        //    collectData.NextPageData.Columns.Add("PageUrl");

        //    //确定单页=>确定列表地址:如果有则直接(处理列表),没有则进入(确定列表)
        //    if (string.IsNullOrWhiteSpace(page_url) && string.IsNullOrWhiteSpace(Config.PagingRuleSegmentUrl))
        //    {
        //        collectData.FirstData = GetFirstData();

        //        if (!string.IsNullOrWhiteSpace(Config.FirstSinglePageListRuleSegmentUrl))//处理采集到的地址
        //        {
        //            Tool.Log.Debug("列表地址:" + Config.FirstSinglePageListRuleSegmentUrl);

        //            {
        //                var row = collectData.NextPageData.NewRow();
        //                row.ItemArray = new string[] { Config.FirstSinglePageListRuleSegmentUrl };
        //                collectData.NextPageData.Rows.Add(row);
        //            }

        //            var html_2 = GetUrl(Config.FirstSinglePageListRuleSegmentUrl);

        //            var doc_2 = NSoup.NSoupClient.Parse(html_2);
        //            var duan_2 = doc_2;

        //            if (collectData.ListData.Columns.Count <= 0)
        //            {
        //                foreach (var segment in Config.ListPageRuleSegments)
        //                {
        //                    collectData.ListData.Columns.Add(segment.Name);
        //                }
        //            }

        //            NSoup.Select.Elements find_elements = GetElements(duan_2, Config.ListRuleSegment);
        //            if (find_elements != null && find_elements.Count > 0)
        //            {
        //                var number = 1;
        //                foreach (var element in find_elements)
        //                {
        //                    DataRow dataRow = collectData.ListData.NewRow();//保存采集的数据
        //                    List<string> lst = new List<string>();

        //                    foreach (var segment in Config.ListPageRuleSegments)
        //                    {
        //                        lst.Add(GetValue(element, segment));
        //                    }

        //                    dataRow.ItemArray = lst.ToArray();
        //                    collectData.ListData.Rows.Add(dataRow);


        //                    //处理内容
        //                    if (Config.ListContentPageRuleSegment != null)
        //                    {
        //                        var contentUrl = GetValue(element, Config.ListContentPageRuleSegment);
        //                        if (!string.IsNullOrWhiteSpace(contentUrl))
        //                        {
        //                            Tool.Log.Debug("[总页" + find_elements.Count + "]" + "[第" + number + "页] " + "内容地址:" + contentUrl);
        //                            number++;

        //                            var html_content = GetUrl(contentUrl);

        //                            var doc_content = NSoup.NSoupClient.Parse(html_content);
        //                            var duan_content = doc_content;

        //                            if (collectData.ContentData.Columns.Count <= 0)
        //                            {
        //                                foreach (var segment in Config.ContentPageRuleSegments)
        //                                {
        //                                    collectData.ContentData.Columns.Add(segment.Name);
        //                                }
        //                            }

        //                            DataRow dataRow_content = collectData.ContentData.NewRow();//保存采集的数据
        //                            List<string> lst_content = new List<string>();

        //                            foreach (var segment in Config.ContentPageRuleSegments)
        //                            {
        //                                lst_content.Add(GetValue(duan_content, segment));
        //                            }

        //                            dataRow_content.ItemArray = lst_content.ToArray();
        //                            collectData.ContentData.Rows.Add(dataRow_content);

        //                        }
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                Tool.Log.Warn("ListRuleSegment:找不到该元素,该规则似乎已失效");
        //            }

        //            Config.PagingRuleSegmentUrl = GetNextUrl(duan_2);

        //        }
        //        else//确定列表
        //        {
        //            {
        //                var row = collectData.NextPageData.NewRow();
        //                row.ItemArray = new string[] { Config.Url };
        //                collectData.NextPageData.Rows.Add(row);
        //            }

        //            if (collectData.ListData.Columns.Count <= 0)
        //            {
        //                foreach (var segment in Config.ListPageRuleSegments)
        //                {
        //                    collectData.ListData.Columns.Add(segment.Name);
        //                }
        //            }

        //            NSoup.Select.Elements find_elements = GetElements(duan, Config.ListRuleSegment);
        //            if (find_elements != null && find_elements.Count > 0)
        //            {
        //                var number = 1;
        //                foreach (var element in find_elements)
        //                {
        //                    DataRow dataRow = collectData.ListData.NewRow();//保存采集的数据
        //                    List<string> lst = new List<string>();

        //                    foreach (var segment in Config.ListPageRuleSegments)
        //                    {
        //                        lst.Add(GetValue(element, segment));
        //                    }

        //                    dataRow.ItemArray = lst.ToArray();
        //                    collectData.ListData.Rows.Add(dataRow);


        //                    //处理内容
        //                    if (Config.ListContentPageRuleSegment != null)
        //                    {
        //                        var contentUrl = GetValue(element, Config.ListContentPageRuleSegment);
        //                        if (!string.IsNullOrWhiteSpace(contentUrl))
        //                        {
        //                            Tool.Log.Debug("[总页" + find_elements.Count + "]" + "[第" + number + "页] " + "内容地址:" + contentUrl);
        //                            number++;

        //                            var html_content = GetUrl(contentUrl);

        //                            var doc_content = NSoup.NSoupClient.Parse(html_content);
        //                            var duan_content = doc_content;

        //                            if (collectData.ContentData.Columns.Count <= 0)
        //                            {
        //                                foreach (var segment in Config.ContentPageRuleSegments)
        //                                {
        //                                    collectData.ContentData.Columns.Add(segment.Name);
        //                                }
        //                            }

        //                            DataRow dataRow_content = collectData.ContentData.NewRow();//保存采集的数据
        //                            List<string> lst_content = new List<string>();

        //                            foreach (var segment in Config.ContentPageRuleSegments)
        //                            {
        //                                lst_content.Add(GetValue(duan_content, segment));
        //                            }

        //                            dataRow_content.ItemArray = lst_content.ToArray();
        //                            collectData.ContentData.Rows.Add(dataRow_content);

        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Tool.Log.Warn("ListRuleSegment:找不到该元素,该规则似乎已失效");
        //            }

        //            Config.PagingRuleSegmentUrl = GetNextUrl(duan);

        //        }

        //    }
        //    else
        //    {
        //        if (!string.IsNullOrWhiteSpace(page_url))
        //        {
        //            Config.PagingRuleSegmentUrl = page_url;//重新继续处理之前的URL
        //        }
        //        Tool.Log.Debug("列表下一页地址:" + Config.PagingRuleSegmentUrl);

        //        {
        //            var row = collectData.NextPageData.NewRow();
        //            row.ItemArray = new string[] { Config.PagingRuleSegmentUrl };
        //            collectData.NextPageData.Rows.Add(row);
        //        }

        //        var html = GetUrl(Config.PagingRuleSegmentUrl);

        //        var doc = NSoup.NSoupClient.Parse(html);
        //        var duan = doc;

        //        if (collectData.ListData.Columns.Count <= 0)
        //        {
        //            foreach (var segment in Config.ListPageRuleSegments)
        //            {
        //                collectData.ListData.Columns.Add(segment.Name);
        //            }
        //        }

        //        NSoup.Select.Elements find_elements = GetElements(duan, Config.ListRuleSegment);
        //        if (find_elements != null && find_elements.Count > 0)
        //        {
        //            var number = 1;
        //            foreach (var element in find_elements)
        //            {
        //                DataRow dataRow = collectData.ListData.NewRow();//保存采集的数据
        //                List<string> lst = new List<string>();

        //                foreach (var segment in Config.ListPageRuleSegments)
        //                {
        //                    lst.Add(GetValue(element, segment));
        //                }

        //                dataRow.ItemArray = lst.ToArray();
        //                collectData.ListData.Rows.Add(dataRow);


        //                //处理内容
        //                if (Config.ListContentPageRuleSegment != null)
        //                {
        //                    var contentUrl = GetValue(element, Config.ListContentPageRuleSegment);
        //                    if (!string.IsNullOrWhiteSpace(contentUrl))
        //                    {
        //                        Tool.Log.Debug("[总页" + find_elements.Count + "]" + "[第" + number + "页] " + "内容地址:" + contentUrl);
        //                        number++;

        //                        var html_content = GetUrl(contentUrl);

        //                        var doc_content = NSoup.NSoupClient.Parse(html_content);
        //                        var duan_content = doc_content;

        //                        if (collectData.ContentData.Columns.Count <= 0)
        //                        {
        //                            foreach (var segment in Config.ContentPageRuleSegments)
        //                            {
        //                                collectData.ContentData.Columns.Add(segment.Name);
        //                            }
        //                        }

        //                        DataRow dataRow_content = collectData.ContentData.NewRow();//保存采集的数据
        //                        List<string> lst_content = new List<string>();

        //                        foreach (var segment in Config.ContentPageRuleSegments)
        //                        {
        //                            lst_content.Add(GetValue(duan_content, segment));
        //                        }

        //                        dataRow_content.ItemArray = lst_content.ToArray();
        //                        collectData.ContentData.Rows.Add(dataRow_content);

        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Tool.Log.Warn("ListRuleSegment:找不到该元素,该规则似乎已失效");
        //        }

        //        Config.PagingRuleSegmentUrl = GetNextUrl(duan);
        //    }

        //    return collectData;
        //}

        public DataTable GetPageContent(string contentUrl)
        {
            var dataTable = new DataTable();

            if (!string.IsNullOrWhiteSpace(contentUrl))
            {
                Tool.Log.Debug("[内容地址]:" + contentUrl);

                var html_content = GetUrl(contentUrl);

                var doc_content = NSoup.NSoupClient.Parse(html_content);
                var duan_content = doc_content;

                if (dataTable.Columns.Count <= 0)
                {
                    foreach (var segment in Config.ContentPageRuleSegments)
                    {
                        dataTable.Columns.Add(segment.Name);
                    }
                }

                DataRow dataRow_content = dataTable.NewRow();//保存采集的数据
                List<string> lst_content = new List<string>();

                foreach (var segment in Config.ContentPageRuleSegments)
                {
                    lst_content.Add(GetValue(duan_content, segment));
                }

                dataRow_content.ItemArray = lst_content.ToArray();
                dataTable.Rows.Add(dataRow_content);

            }

            return dataTable;
        }

        /// <summary>
        /// 查找列表
        /// </summary>
        /// <param name="duan"></param>
        /// <returns></returns>
        NSoup.Select.Elements GetElements(NSoup.Nodes.Element duan, FunctionRuleSegment ruleSegment)
        {
            NSoup.Select.Elements find_elements = null;
            NSoup.Nodes.Element find_element = duan;
            //查找列表
            for (int i = 0; i < ruleSegment.GetFunctions().Count; i++)
            {
                var function = ruleSegment[i];

                var i_findElements = function as IFindElements;
                if (i_findElements != null)
                {
                    find_elements = i_findElements.FindElements(find_element);
                }

                var i_findElementForList = function as IFindElementByList;
                if (i_findElementForList != null)
                {
                    i_findElementForList.FindElement(find_elements);
                }

                var i_findElement = function as IFindElement;
                if (i_findElement != null)
                {
                    i_findElement.FindElement(find_element);
                }

                var i_filterElement = function as IFilterElement;
                if (i_filterElement != null)
                {
                    i_filterElement.Filter(find_element);
                }

            }

            return find_elements;
        }
        /// <summary>
        /// 查找单页
        /// </summary>
        /// <param name="duan"></param>
        /// <param name="ruleSegment"></param>
        /// <returns></returns>
        NSoup.Nodes.Element GetElement(NSoup.Nodes.Element duan, FunctionRuleSegment ruleSegment)
        {
            NSoup.Select.Elements find_elements = null;
            NSoup.Nodes.Element find_element = duan;
            //查找列表
            for (int i = 0; i < ruleSegment.GetFunctions().Count; i++)
            {
                var function = ruleSegment[i];

                var i_findElements = function as IFindElements;
                if (i_findElements != null)
                {
                    find_elements = i_findElements.FindElements(find_element);
                }

                var i_findElementForList = function as IFindElementByList;
                if (i_findElementForList != null)
                {
                    i_findElementForList.FindElement(find_elements);
                }

                var i_findElement = function as IFindElement;
                if (i_findElement != null)
                {
                    i_findElement.FindElement(find_element);
                }

                var i_filterElement = function as IFilterElement;
                if (i_filterElement != null)
                {
                    i_filterElement.Filter(find_element);
                }

            }

            return find_element;
        }

        string GetValue(NSoup.Nodes.Element element, FunctionRuleSegment ruleSegment)
        {
            if (element == null) { return ""; }
            if (ruleSegment == null) { return ""; }

            string val = ""; string[] strs = null;
            NSoup.Select.Elements find_elements = null;
            NSoup.Nodes.Element find_element = element;
            for (int i = 0; i < ruleSegment.GetFunctions().Count; i++)
            {
                if (find_element == null) { return null; }

                var function = ruleSegment[i];

                //查找元素
                var i_findElements = function as IFindElements;
                if (i_findElements != null)
                {
                    find_elements = i_findElements.FindElements(find_element);
                }

                var i_findElementForList = function as IFindElementByList;
                if (i_findElementForList != null)
                {
                    find_element = i_findElementForList.FindElement(find_elements);
                }

                var i_findElement = function as IFindElement;
                if (i_findElement != null)
                {
                    find_element = i_findElement.FindElement(find_element);
                }

                //过滤元素
                var i_filterElement = function as IFilterElement;
                if (i_filterElement != null)
                {
                    i_filterElement.Filter(find_element);
                }


                //根据元素获取值
                var i_getValueByElement = function as IGetValueByElement;
                if (i_getValueByElement != null)
                {
                    val = i_getValueByElement.GetValue(find_element);
                }

                //将值转为数组
                var i_valueToValues = function as IValueToValues;
                if (i_valueToValues != null)
                {
                    strs = i_valueToValues.GetValues(val);
                }

                //根据数组获取值
                var i_valuesToValue = function as IValuesToValue;
                if (i_valuesToValue != null)
                {
                    val = i_valuesToValue.GetValue(strs);
                }

                //处理值
                var i_doValue = function as IDoValue;
                if (i_doValue != null)
                {
                    val = i_doValue.DoValue(val);
                }


                //处理内容列表
                var model_List = function as ListFunction;
                if (model_List != null)
                {
                    DataTable data = new DataTable();
                    foreach (var item in ruleSegment.ListSegments)
                    {
                        data.Columns.Add(item.Name);
                    }
                    foreach (var eles in find_elements)
                    {
                        DataRow dataRow = data.NewRow();//保存采集的数据
                        List<string> lst = new List<string>();
                        foreach (var item in ruleSegment.ListSegments)
                        {
                            lst.Add(GetValue(eles, item));
                        }
                        dataRow.ItemArray = lst.ToArray();
                        data.Rows.Add(dataRow);
                    }
                    val = data.ToJson();

                }


            }

            return val;
        }

        string GetNextUrl(NSoup.Nodes.Element element)
        {
            var url = GetValue(element, Config.PagingRuleSegment);
            return url;
        }

        public string DataTableToMySql(string tablename, DataTable dataTable, int listPageNumber = 0)
        {
            string fieldKey = "ContentMD5";
            string fieldnames = "ContentMD5,CollectListPageNumber,CollectPageTotal,CollectPageNumber,CreateTime,UpdateTime,Status,";
            string fieldvalues = "'{0}','{3}','{1}','{2}',NOW(),NOW(),0,";
            string fields = "";

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if ((i + 1) == dataTable.Columns.Count)
                {
                    fields += dataTable.Columns[i].ColumnName + " text ";
                    fieldnames += dataTable.Columns[i].ColumnName;
                }
                else
                {
                    fields += dataTable.Columns[i].ColumnName + " text,";
                    fieldnames += dataTable.Columns[i].ColumnName + ",";
                }
            }


            var sql = "";
            var mysql = @"
Create Table If Not Exists {1}(
	ID int Primary key Auto_Increment,
	ContentMD5 VARCHAR(100),
    CollectListPageNumber int,
    CollectPageTotal int,
    CollectPageNumber int,
    CreateTime datetime,
    UpdateTime datetime,
    Status int,
	{2}
);
{3}
";

            var mysql_insert_into = @"
INSERT INTO {1}({4}) 
SELECT {5}  
FROM DUAL WHERE NOT EXISTS (
    SELECT 1 FROM {1} WHERE {2}='{3}' LIMIT 1
); 
";
            var sqlAll = "";

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var str_arr = dataTable.Rows[i].ItemArray;
                if (str_arr != null)
                {
                    var contentMD5 = EncryptHelper.MD5(str_arr.ToJson());
                    var vals = string.Format(fieldvalues, contentMD5, dataTable.Rows.Count, (i + 1), listPageNumber);
                    for (int j = 0; j < str_arr.Length; j++)
                    {
                        if ((j + 1) == str_arr.Length)
                        {
                            vals += "'" + str_arr[j].ToString().Replace("\\", "\\\\").Replace("'", "\\'").Replace("[", "\\[").Replace("]", "\\]") + "'";
                        }
                        else
                        {
                            vals += "'" + str_arr[j].ToString().Replace("\\", "\\\\").Replace("'", "\\'").Replace("[", "\\[").Replace("]", "\\]") + "',";
                        }
                    }

                    var sqlrow = string.Format(mysql_insert_into, "", tablename, fieldKey, contentMD5, fieldnames, vals);

                    sqlAll += sqlrow;
                }
            }

            sql = string.Format(mysql, "", tablename, fields, sqlAll);

            return sql;
        }

        public string DataTableToMySql(string dbName, string tablename, DataTable dataTable, int listPageNumber = 0)
        {
            if (string.IsNullOrWhiteSpace(dbName))
            {
                dbName = Config.Name;
            }

            string fieldKey = "ContentMD5";
            string fieldnames = "ContentMD5,CollectListPageNumber,CollectPageTotal,CollectPageNumber,CreateTime,UpdateTime,Status,";
            string fieldvalues = "'{0}','{3}','{1}','{2}',NOW(),NOW(),0,";
            string fields = "";

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if ((i + 1) == dataTable.Columns.Count)
                {
                    fields += dataTable.Columns[i].ColumnName + " text ";
                    fieldnames += dataTable.Columns[i].ColumnName;
                }
                else
                {
                    fields += dataTable.Columns[i].ColumnName + " text,";
                    fieldnames += dataTable.Columns[i].ColumnName + ",";
                }
            }


            var sql = "";
            var mysql = @"
Create Database If Not Exists {0} Character Set utf8mb4;
Create Table If Not Exists {0}.{1}(
	ID int Primary key Auto_Increment,
	ContentMD5 VARCHAR(100),
    CollectListPageNumber int,
    CollectPageTotal int,
    CollectPageNumber int,
    CreateTime datetime,
    UpdateTime datetime,
    Status int,
	{2}
);
{3}
";

            var mysql_insert_into = @"
INSERT INTO {0}.{1}({4}) 
SELECT {5}  
FROM DUAL WHERE NOT EXISTS (
    SELECT 1 FROM {0}.{1} WHERE {2}='{3}' LIMIT 1
); 
";
            var sqlAll = "";

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var str_arr = dataTable.Rows[i].ItemArray;
                if (str_arr != null)
                {
                    var contentMD5 = EncryptHelper.MD5(str_arr.ToJson());
                    var vals = string.Format(fieldvalues, contentMD5, dataTable.Rows.Count, (i + 1), listPageNumber);
                    for (int j = 0; j < str_arr.Length; j++)
                    {
                        if ((j + 1) == str_arr.Length)
                        {
                            vals += "'" + str_arr[j].ToString().Replace("\\", "\\\\").Replace("'", "\\'").Replace("[", "\\[").Replace("]", "\\]") + "'";
                        }
                        else
                        {
                            vals += "'" + str_arr[j].ToString().Replace("\\", "\\\\").Replace("'", "\\'").Replace("[", "\\[").Replace("]", "\\]") + "',";
                        }
                    }

                    var sqlrow = string.Format(mysql_insert_into, dbName, tablename, fieldKey, contentMD5, fieldnames, vals);

                    sqlAll += sqlrow;
                }
            }

            sql = string.Format(mysql, dbName, tablename, fields, sqlAll);

            return sql;
        }


    }
}
