using RunTaskForAny.Common.Helper;
using RunTaskForAny.Module.Collect.PageRule.FunctionSegment;
using RunTaskForAny.Security.Encrypt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Collect.PageRule.FunctionRule
{
    /// <summary>
    /// 采集规则
    /// </summary>
    public class CollectRule
    {
        CollectRuleConfig Config;
        public CollectRule(CollectRuleConfig config)
        {
            Config = config;

        }


        /// <summary>
        /// 获取列表数据,为空是未获取到该页面
        /// </summary>
        /// <returns></returns>
        public CollectData GetPageList()
        {
            CollectData collectData = new CollectData();
            DataTable dataTable = null;

            //确定单页=>确定列表地址:如果有则直接(处理列表),没有则进入(确定列表)
            if (string.IsNullOrWhiteSpace(Config.PagingRuleSegmentUrl))
            {
                var html = HttpTool.AjaxGet(Config.Url);
                var doc = NSoup.NSoupClient.Parse(html);
                var duan = doc.Body;
                if (Config.FirstSinglePageRuleSegment != null)
                {
                    var first_find_element = GetElement(duan, Config.FirstSinglePageRuleSegment);
                    if (first_find_element != null)
                    {
                        Config.FirstSinglePageListRuleSegmentUrl = GetValue(first_find_element, Config.FirstSinglePageListRuleSegment);
                        Tool.Log.Debug(Config.FirstSinglePageListRuleSegmentUrl);

                        DataTable firstDataTable = new DataTable();
                        foreach (var segment in Config.FirstSinglePageRuleSegments)
                        {
                            firstDataTable.Columns.Add(segment.Name);
                        }
                        DataRow dataRow = firstDataTable.NewRow();//保存采集的数据
                        List<string> lst = new List<string>();

                        foreach (var segment in Config.FirstSinglePageRuleSegments)
                        {
                            lst.Add(GetValue(first_find_element, segment));
                        }

                        dataRow.ItemArray = lst.ToArray();
                        firstDataTable.Rows.Add(dataRow);

                        collectData.FirstData = firstDataTable;

                        Tool.Log.Debug(firstDataTable.ToJson());
                    }

                }

                if (!string.IsNullOrWhiteSpace(Config.FirstSinglePageListRuleSegmentUrl))//处理采集到的地址
                {
                    var html_2 = HttpTool.AjaxGet(Config.FirstSinglePageListRuleSegmentUrl);
                    var doc_2 = NSoup.NSoupClient.Parse(html_2);
                    var duan_2 = doc_2.Body;

                    dataTable = new DataTable();
                    foreach (var segment in Config.ListPageRuleSegments)
                    {
                        dataTable.Columns.Add(segment.Name);
                    }

                    NSoup.Select.Elements find_elements = GetElements(duan_2, Config.ListRuleSegment);
                    if (find_elements != null && find_elements.Count > 0)
                    {
                        foreach (var element in find_elements)
                        {
                            DataRow dataRow = dataTable.NewRow();//保存采集的数据
                            List<string> lst = new List<string>();

                            foreach (var segment in Config.ListPageRuleSegments)
                            {
                                lst.Add(GetValue(element, segment));
                            }

                            dataRow.ItemArray = lst.ToArray();
                            dataTable.Rows.Add(dataRow);
                        }
                    }

                    Config.PagingRuleSegmentUrl = GetNextUrl(duan_2);

                }
                else//确定列表
                {
                    dataTable = new DataTable();
                    foreach (var segment in Config.ListPageRuleSegments)
                    {
                        dataTable.Columns.Add(segment.Name);
                    }

                    NSoup.Select.Elements find_elements = GetElements(duan, Config.ListRuleSegment);
                    if (find_elements != null && find_elements.Count > 0)
                    {
                        foreach (var element in find_elements)
                        {
                            DataRow dataRow = dataTable.NewRow();//保存采集的数据
                            List<string> lst = new List<string>();

                            foreach (var segment in Config.ListPageRuleSegments)
                            {
                                lst.Add(GetValue(element, segment));
                            }

                            dataRow.ItemArray = lst.ToArray();
                            dataTable.Rows.Add(dataRow);
                        }
                    }

                    Config.PagingRuleSegmentUrl = GetNextUrl(duan);
                }

            }
            else
            {
                var html = HttpTool.AjaxGet(Config.PagingRuleSegmentUrl);
                var doc = NSoup.NSoupClient.Parse(html);
                var duan = doc.Body;

                dataTable = new DataTable();
                foreach (var segment in Config.ListPageRuleSegments)
                {
                    dataTable.Columns.Add(segment.Name);
                }

                NSoup.Select.Elements find_elements = GetElements(duan, Config.ListRuleSegment);
                if (find_elements != null && find_elements.Count > 0)
                {
                    foreach (var element in find_elements)
                    {
                        DataRow dataRow = dataTable.NewRow();//保存采集的数据
                        List<string> lst = new List<string>();

                        foreach (var segment in Config.ListPageRuleSegments)
                        {
                            lst.Add(GetValue(element, segment));
                        }

                        dataRow.ItemArray = lst.ToArray();
                        dataTable.Rows.Add(dataRow);
                    }
                }

                Config.PagingRuleSegmentUrl = GetNextUrl(duan);
            }
            collectData.ListData = dataTable;
            return collectData;
        }


        /// <summary>
        /// 查找列表
        /// </summary>
        /// <param name="duan"></param>
        /// <returns></returns>
        NSoup.Select.Elements GetElements(NSoup.Nodes.Element duan, FunctionRuleSegment ruleSegment)
        {
            NSoup.Select.Elements find_elements = null;
            NSoup.Nodes.Element find_element = null;
            //查找列表
            for (int i = 0; i < ruleSegment.GetFunctions().Count; i++)
            {
                var function = ruleSegment[i];

                var model_Attr = function as AttrFunction;
                if (model_Attr != null)
                {
                    if (find_element == null)
                    {
                        find_elements = duan.GetElementsByAttributeValue(model_Attr.AttrName, model_Attr.AttrValue);
                    }
                    else
                    {
                        find_elements = find_element.GetElementsByAttributeValue(model_Attr.AttrName, model_Attr.AttrValue);
                    }
                }
                var model_Tag = function as TagFunction;
                if (model_Tag != null)
                {
                    if (find_element == null)
                    {
                        find_elements = duan.GetElementsByTag(model_Tag.TagName);
                    }
                    else
                    {
                        find_elements = find_element.GetElementsByTag(model_Tag.TagName);
                    }
                }
                var model_Child = function as ChildFunction;
                if (model_Child != null)
                {
                    if (find_element == null)
                    {
                        find_elements = duan.Children;
                    }
                    else
                    {
                        find_elements = find_element.Children;
                    }
                }

                var model_Prev = function as PrevFunction;
                if (model_Prev != null)
                {
                    find_element = find_element.PreviousElementSibling;
                }
                var model_Next = function as NextFunction;
                if (model_Next != null)
                {
                    find_element = find_element.NextElementSibling;
                }
                var model_Parent = function as ParentFunction;
                if (model_Parent != null)
                {
                    find_element = find_element.Parent;
                }

                var model_RowIndex = function as RowIndexFunction;
                if (model_RowIndex != null)
                {
                    find_element = find_elements[model_RowIndex.RowIndex];
                }
                var model_Index = function as IndexFunction;
                if (model_Index != null)
                {
                    if (model_Index.Index < 0)
                    {
                        find_element = find_elements[find_elements.Count + model_Index.Index];
                    }
                    else
                    {
                        find_element = find_elements[model_Index.Index];
                    }
                }
                var model_FIndex = function as FIndexFunction;
                if (model_FIndex != null)
                {
                    find_element = find_elements.First;
                }
                var model_LIndex = function as LIndexFunction;
                if (model_LIndex != null)
                {
                    find_element = find_elements.Last;
                }

                var model_RemoveTag = function as RemoveTagFunction;
                if (model_RemoveTag != null)
                {
                    var es = find_element.GetElementsByTag(model_RemoveTag.TagName);
                    find_element.Children.RemoveAll(es);
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
            NSoup.Nodes.Element find_element = null;
            //查找列表
            for (int i = 0; i < ruleSegment.GetFunctions().Count; i++)
            {
                var function = ruleSegment[i];

                var model_Attr = function as AttrFunction;
                if (model_Attr != null)
                {
                    if (find_element == null)
                    {
                        find_elements = duan.GetElementsByAttributeValue(model_Attr.AttrName, model_Attr.AttrValue);
                    }
                    else
                    {
                        find_elements = find_element.GetElementsByAttributeValue(model_Attr.AttrName, model_Attr.AttrValue);
                    }
                }
                var model_Tag = function as TagFunction;
                if (model_Tag != null)
                {
                    if (find_element == null)
                    {
                        find_elements = duan.GetElementsByTag(model_Tag.TagName);
                    }
                    else
                    {
                        find_elements = find_element.GetElementsByTag(model_Tag.TagName);
                    }
                }
                var model_Child = function as ChildFunction;
                if (model_Child != null)
                {
                    if (find_element == null)
                    {
                        find_elements = duan.Children;
                    }
                    else
                    {
                        find_elements = find_element.Children;
                    }
                }

                var model_Prev = function as PrevFunction;
                if (model_Prev != null)
                {
                    find_element = find_element.PreviousElementSibling;
                }
                var model_Next = function as NextFunction;
                if (model_Next != null)
                {
                    find_element = find_element.NextElementSibling;
                }
                var model_Parent = function as ParentFunction;
                if (model_Parent != null)
                {
                    find_element = find_element.Parent;
                }

                var model_RowIndex = function as RowIndexFunction;
                if (model_RowIndex != null)
                {
                    find_element = find_elements[model_RowIndex.RowIndex];
                }
                var model_Index = function as IndexFunction;
                if (model_Index != null)
                {
                    if (model_Index.Index < 0)
                    {
                        find_element = find_elements[find_elements.Count + model_Index.Index];
                    }
                    else
                    {
                        find_element = find_elements[model_Index.Index];
                    }
                }
                var model_FIndex = function as FIndexFunction;
                if (model_FIndex != null)
                {
                    find_element = find_elements.First;
                }
                var model_LIndex = function as LIndexFunction;
                if (model_LIndex != null)
                {
                    find_element = find_elements.Last;
                }

                var model_RemoveTag = function as RemoveTagFunction;
                if (model_RemoveTag != null)
                {
                    var es = find_element.GetElementsByTag(model_RemoveTag.TagName);
                    find_element.Children.RemoveAll(es);
                }

            }

            return find_element;
        }

        string GetValue(NSoup.Nodes.Element element, FunctionRuleSegment ruleSegment)
        {
            if (element == null) { return ""; }
            if (ruleSegment == null) { return ""; }

            string val = "";
            NSoup.Select.Elements find_elements = null;
            NSoup.Nodes.Element find_element = null;
            for (int i = 0; i < ruleSegment.GetFunctions().Count; i++)
            {
                var function = ruleSegment[i];

                var model_Attr = function as AttrFunction;
                if (model_Attr != null)
                {
                    if (find_element == null)
                    {
                        find_elements = element.GetElementsByAttributeValue(model_Attr.AttrName, model_Attr.AttrValue);
                    }
                    else
                    {
                        find_elements = find_element.GetElementsByAttributeValue(model_Attr.AttrName, model_Attr.AttrValue);
                    }
                }
                var model_Tag = function as TagFunction;
                if (model_Tag != null)
                {
                    if (find_element == null)
                    {
                        find_elements = element.GetElementsByTag(model_Tag.TagName);
                    }
                    else
                    {
                        find_elements = find_element.GetElementsByTag(model_Tag.TagName);
                    }
                }
                var model_Child = function as ChildFunction;
                if (model_Child != null)
                {
                    if (find_element == null)
                    {
                        find_elements = element.Children;
                    }
                    else
                    {
                        find_elements = find_element.Children;
                    }
                }

                var model_Prev = function as PrevFunction;
                if (model_Prev != null)
                {
                    find_element = find_element.PreviousElementSibling;
                }
                var model_Next = function as NextFunction;
                if (model_Next != null)
                {
                    find_element = find_element.NextElementSibling;
                }
                var model_Parent = function as ParentFunction;
                if (model_Parent != null)
                {
                    find_element = find_element.Parent;
                }

                var model_RowIndex = function as RowIndexFunction;
                if (model_RowIndex != null)
                {
                    find_element = find_elements[model_RowIndex.RowIndex];
                }
                var model_Index = function as IndexFunction;
                if (model_Index != null)
                {
                    if (model_Index.Index < 0)
                    {
                        find_element = find_elements[find_elements.Count + model_Index.Index];
                    }
                    else
                    {
                        find_element = find_elements[model_Index.Index];
                    }
                }
                var model_FIndex = function as FIndexFunction;
                if (model_FIndex != null)
                {
                    find_element = find_elements.First;
                }
                var model_LIndex = function as LIndexFunction;
                if (model_LIndex != null)
                {
                    find_element = find_elements.Last;
                }


                var model_Text = function as TextFunction;
                if (model_Text != null)
                {
                    val = find_element.Text().Trim();
                }
                var model_Html = function as HtmlFunction;
                if (model_Html != null)
                {
                    val = find_element.Html().Trim();
                }
                var model_IHtml = function as IHtmlFunction;
                if (model_IHtml != null)
                {
                    val = find_element.Children.Html().Trim();
                }
                var model_Link = function as LinkFunction;
                if (model_Link != null)
                {
                    var link = find_element.Attr("href");
                    //Uri baseUri = new Uri(Config.Url);
                    //Uri absoluteUri = new Uri(baseUri, link);
                    //val = absoluteUri.AbsoluteUri;
                    val = link;
                }
                var model_PrevText = function as PrevTextFunction;
                if (model_PrevText != null)
                {
                    val = find_element.PreviousSibling.Attr("text").Trim();
                }
                var model_NextText = function as NextTextFunction;
                if (model_NextText != null)
                {
                    val = find_element.NextSibling.Attr("text").Trim();
                }
                var model_GetAttr = function as GetAttrFunction;
                if (model_GetAttr != null)
                {
                    val = find_element.Attr(model_GetAttr.AttrName).Trim();
                }
                var model_Clear = function as ClearFunction;
                if (model_Clear != null)
                {
                    var str = find_element.Attr(model_Clear.AttrName).Trim().Replace(model_Clear.AttrValue, "");
                    find_element.Attr(model_Clear.AttrName, str);
                }

                var model_RemoveTag = function as RemoveTagFunction;
                if (model_RemoveTag != null)
                {
                    var es = find_element.GetElementsByTag(model_RemoveTag.TagName);
                    find_element.Children.RemoveAll(es);
                }

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

        public string DataTableToMySql(DataTable dataTable)
        {
            string dbName = "Collect_v1";
            string tablename = Config.Name;
            string fieldKey = "ContentMD5";
            string fieldnames = "ContentMD5,CreateTime,UpdateTime,Status,";
            string fieldvalues = "'{0}',NOW(),NOW(),0,";
            string fields = "";

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if ((i + 1) == dataTable.Columns.Count)
                {
                    fields += dataTable.Columns[i].ColumnName + " VARCHAR(4000) ";
                    fieldnames += dataTable.Columns[i].ColumnName;
                }
                else
                {
                    fields += dataTable.Columns[i].ColumnName + " VARCHAR(4000),";
                    fieldnames += dataTable.Columns[i].ColumnName + ",";
                }
            }


            var sql = "";
            var mysql = @"
Create Database If Not Exists {0} Character Set utf8mb4;
Create Table If Not Exists {0}.{1}(
	ID int Primary key Auto_Increment,
	ContentMD5 VARCHAR(100),
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
                    var vals = string.Format(fieldvalues, contentMD5);
                    for (int j = 0; j < str_arr.Length; j++)
                    {
                        if ((j + 1) == str_arr.Length)
                        {
                            vals += "'" + str_arr[j].ToString().Replace("'", "''") + "'";
                        }
                        else
                        {
                            vals += "'" + str_arr[j].ToString().Replace("'", "''") + "',";
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
