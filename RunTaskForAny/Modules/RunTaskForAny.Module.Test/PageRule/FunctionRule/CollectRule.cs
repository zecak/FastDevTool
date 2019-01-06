using RunTaskForAny.Common.Helper;
using RunTaskForAny.Module.Test.PageRule.FunctionSegment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule.FunctionRule
{
    /// <summary>
    /// 采集规则
    /// </summary>
    public class CollectRule
    {
        CollectRuleConfig RuleConfig;
        public CollectRule(CollectRuleConfig config)
        {
            RuleConfig = config;

        }

        /// <summary>
        /// 获取页数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetNextPageList()
        {
            if (string.IsNullOrWhiteSpace(RuleConfig.FirstSinglePageListRuleSegmentUrl))
            {
                return null;
            }
            var html = HttpTool.AjaxGet(RuleConfig.FirstSinglePageListRuleSegmentUrl);
            return GetList(html);
        }
        public DataTable GetList(string html)
        {                
            //确定单页=>确定列表地址:如果有则直接(处理列表),没有则进入(确定列表)

            //确定列表

            //处理列表

            if (string.IsNullOrWhiteSpace(html))
            {
                return null;
            }
            if (RuleConfig.ListRuleSegment == null)
            {
                return null;
            }

            var tablename = "";
            DataTable dataTable = new DataTable();
            foreach (var segment in RuleConfig.ListPageRuleSegments)
            {
                dataTable.Columns.Add(segment.Name);
            }
            var doc = NSoup.NSoupClient.Parse(html);
            var duan = doc.Body;
            NSoup.Select.Elements find_elements = GetElements(duan, RuleConfig.ListRuleSegment);
            if (find_elements != null && find_elements.Count > 0)
            {
                foreach (var element in find_elements)
                {
                    DataRow dataRow = dataTable.NewRow();//保存采集的数据
                    List<string> lst = new List<string>();

                    foreach (var segment in RuleConfig.ListPageRuleSegments)
                    {
                        lst.Add(GetValue(element, segment));
                    }

                    dataRow.ItemArray = lst.ToArray();
                    dataTable.Rows.Add(dataRow);
                }
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

            }

            return find_elements;
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

            }

            return val;
        }

        public Uri GetNextUrl(NSoup.Nodes.Element element)
        {
            var url = GetValue(element, RuleConfig.PagingRuleSegment);
            if(string.IsNullOrWhiteSpace(url))
            {
                return null;
            }
            Uri uri = new Uri(url);
            return uri;
        }
    }
}
