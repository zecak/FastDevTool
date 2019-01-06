using RunTaskForAny.Common.Domain;
using RunTaskForAny.Common.Helper;
using RunTaskForAny.Module.Test.PageRule;
using RunTaskForAny.Module.Test.PageRule.FunctionRule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test
{
    public class Action : ModuleBase
    {
        public Action(InfoModel infomodel) : base(infomodel) { }
        //static int num = 0;
        //public ResInfoModel Init(string data)
        //{
        //    try
        //    {
        //        num++;
        //        return new ResInfoModel() { Code = 1, Msg = "xx调用成功"+ num };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResInfoModel() { Code = 1000, Msg = ex.Message+System.Environment.NewLine+ex.StackTrace + System.Environment.NewLine };
        //    }

        //}

        public void Start()
        {
            //Task.Factory.StartNew(() =>
            //{
            //    //获取配置
            //    var filepath = System.IO.Path.Combine
            //       (AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\def.json");
            //    if (!System.IO.File.Exists(filepath))
            //    {
            //        System.IO.File.WriteAllText(filepath, new AcquisitionRule()
            //        {
            //            ListType = false,
            //            DataPath = "Data",
            //            RulesPath = "Rules",
            //            DownPath = "Down",
            //            Name = "默认配置",
            //            Url = "https://qqjj18.com",
            //            ParagraphRules = new KeyValue() { Key = "id", Value = "PoShow_Box" },
            //            RowRules = new List<KeyValue>()
            //            {
            //                new KeyValue() { Key = "列表标题", Value = "[属性名值=class&Po_topic]|[行索引=0]|[属性名值=class&Po_topic_title]|[首索引]|[标签名=a]|[首索引]|[内容]" },
            //                new KeyValue() { Key = "列表链接", Value = "[属性名值=class&Po_topic]|[行索引=0]|[属性名值=class&Po_topic_title]|[首索引]|[标签名=a]|[首索引]|[链接]" },
            //                //https://ww2464.com/content_censored/203350.htm
            //                //[清除=src&http://img.xxxx.com]
            //                new KeyValue() { Key = "列表编号", Value = "[属性名值=class&Po_topic]|[行索引=0]|[属性名值=class&Po_topic_title]|[首索引]|[标签名=a]|[首索引]|[清除=href&https://ww2464.com/content_censored/]|[清除=href&.htm]|[链接]" },
            //                new KeyValue() { Key = "列表小图", Value = "[属性名值=class&Po_topic]|[行索引=0]|[属性名值=class&Po_topicCG]|[首索引]|[标签名=img]|[首索引]|[下载图片]" },
            //                 new KeyValue() { Key = "列表大图", Value = "[属性名值=class&Po_topic]|[行索引=0]|[属性名值=class&Po_topicCG]|[首索引]|[标签名=img]|[首索引]|[清除=onmouseover&showtrail('/]|[清除=onmouseover&','',10,10)]|[取属性值=onmouseover]" },
            //                new KeyValue() { Key = "{$内容链接}", Value = "" },
            //                new KeyValue() { Key = "标题", Value = "[属性名值=class&list]|[索引=1]|[标签名=li]|[索引=0]|[标签名=a]|[首索引]|[内容]" },
            //                 new KeyValue() { Key = "新地址", Value = "[属性名值=class&list]|[索引=1]|[标签名=li]|[索引=0]|[标签名=a]|[首索引]|[链接]" },
            //                 new KeyValue() { Key = "采集标题", Value = "[属性名值=class&list]|[索引=10]|[标签名=li]|[索引=0]|[标签名=a]|[首索引]|[内容]" },
            //                 new KeyValue() { Key = "采集地址", Value = "[属性名值=class&list]|[索引=10]|[标签名=li]|[索引=0]|[标签名=a]|[首索引]|[链接]" },
            //            },
            //            NextPageRules = new KeyValue() {Key="", Value= "[属性名值=class&PageBar]|[首索引]|[属性名值=class&pageback]|[首索引]|[标签名=a]|[首索引]|[链接]" }
            //        }.ToJson());
            //    }
            //    var config = System.IO.File.ReadAllText(filepath).JsonTo<AcquisitionRule>();
            //    if (config == null)
            //    {
            //        Tool.Log.Error("配置文件错误");
            //        return;
            //    }
            //    Tool.Log.Debug("开始获取html");


            //    {
            //        List<List<string>> list = new List<List<string>>();

            //        var pv = RuleHelper.GetPageValue(config.Url, config, out string info);
            //        Tool.Log.Debug("GetPageValue: " + pv.ToJson());
            //        if (pv.Count == 4)
            //        {
            //            var url = pv[pv.Count - 1];
            //            var html = HttpTool.AjaxGet(url);
            //            var doc = NSoup.NSoupClient.Parse(html);
            //            var duan = doc.GetElementsByAttributeValue(config.ParagraphRules.Key, config.ParagraphRules.Value).First;
            //            var frow = RuleHelper.GetFirstRow(duan, config, out int index);
            //            Tool.Log.Debug("GetFirstRow: " + frow.ToString());
            //            //列表
            //            NSoup.Select.Elements rows = frow.Parent.Children;
            //            var length = rows.Count;

            //            Uri nextpage = null;
            //            do
            //            {
            //                nextpage = RuleHelper.GetNextPage(duan, config);
            //                Tool.Log.Debug("GetNextPage: " + nextpage.ToJson());
            //                int indextemp = index;
            //                for (int i = indextemp; i < length; i++)
            //                {
            //                    var rowstr = string.Empty;
            //                    List<string> l = RuleHelper.GetRowValue(duan, config, indextemp, out rowstr);
            //                    indextemp++;
            //                    Tool.Log.Debug("GetRowValue: " + l.ToJson());
            //                    if (!string.IsNullOrWhiteSpace(rowstr))
            //                    {
            //                        list.Add(l);
            //                    }

            //                }
            //                System.IO.File.WriteAllText(System.IO.Path.Combine
            //       (AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\1.json"),list.ToJson());
            //                break;
            //                if (nextpage != null)
            //                {
            //                    try
            //                    {
            //                        var doctemp = NSoup.NSoupClient.Parse(HttpTool.AjaxGet(nextpage.AbsoluteUri));
            //                        duan = doctemp.GetElementsByAttributeValue(config.ParagraphRules.Key, config.ParagraphRules.Value).First;
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        Tool.Log.Error(ex);
            //                    }
            //                }

            //            } while (nextpage != null);

            //            Tool.Log.Debug("采集完成");

            //        }

            //    }



            //});

            Task.Factory.StartNew(() =>
            {
                //获取配置
                var config = GetConfig();
                if (config == null)
                {
                    Tool.Log.Error("配置文件错误");
                    return;
                }
                Tool.Log.Debug("获取html");

                CollectRule collectRule = new CollectRule(config);


                //确定单页=>确定列表地址:如果有则直接(处理列表),没有则进入(确定列表)

                //确定列表

                //处理列表
                var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\has列表.txt");
                var html = System.IO.File.ReadAllText(filepath);


                Uri nextpage = null;
                do
                {
                    nextpage = collectRule.GetNextUrl();
                    dataTable = collectRule.GetNextPageList();
                    System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\data_1.json"), dataTable.ToJson());
                } while (nextpage != null) ;


                Tool.Log.Debug("采集完成");

                //{
                //    var list = new List<System.Data.DataRow>();

                //    var rule = new Rule(config);

                //    var pv = rule.GetPageValue();
                //    Tool.Log.Debug("GetPageValue: " + pv.ToJson());
                //    if (pv.Count == 4)
                //    {
                //        var url = pv[pv.Count - 1];
                //        var html = HttpTool.AjaxGet(url);
                //        var doc = NSoup.NSoupClient.Parse(html);
                //        var duan = rule.GetElementsFirstRow(doc);

                //        //列表
                //        NSoup.Select.Elements rows = rule.GetElements(doc);
                //        var length = rows.Count;
                //        int indextemp = 0;
                //        Uri nextpage = null;
                //        do
                //        {
                //            nextpage = rule.GetNextUrl(duan);
                //            Tool.Log.Debug("GetNextUrl: " + nextpage.ToJson());

                //            for (int i = indextemp; i < length; i++)
                //            {
                //                var rowstr = string.Empty;
                //                var row = rule.GetRowValue(duan, indextemp);
                //                indextemp++;
                //                Tool.Log.Debug("GetRowValue: " + row.ItemArray.ToJson());
                //                if (!string.IsNullOrWhiteSpace(rowstr))
                //                {
                //                    list.Add(row);
                //                }

                //            }
                //            System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\data_1.json"), list.ToJson());
                //            break;


                //        } while (nextpage != null);

                //        Tool.Log.Debug("采集完成");

                //    }

                //}



            });
        }


        CollectRuleConfig GetConfig()
        {
            var config = new CollectRuleConfig()
            {
                Name = "默认采集规则",
                Url = "https://qqjj18.com",
                FirstSinglePageRuleSegment = new FunctionRuleSegment("", "[Attr::id=PoShow_Box]"),
                FirstSinglePageRuleSegments = new List<FunctionRuleSegment>()
                {
                    new FunctionRuleSegment("标题","[Attr::class=list]$$[Index::1]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("新地址","[Attr::class=list]$$[Index::1]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),
                    new FunctionRuleSegment("采集标题","[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("采集地址","[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),
                },
                FirstSinglePageListRuleSegment = new FunctionRuleSegment("采集地址", "[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),

                ListRuleSegment = new FunctionRuleSegment("", "[Attr::class=Po_topic]"),
                ListPageRuleSegments = new List<FunctionRuleSegment>()
                {
                     new FunctionRuleSegment("列表标题","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Text]"),
                     new FunctionRuleSegment("列表链接","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Link]"),
                     new FunctionRuleSegment("列表编号","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Clear::href=https://ww2464.com/content_censored/]$$[Clear::href=.htm]$$[GetAttr::href]"),
                     new FunctionRuleSegment("列表小图","[Attr::class=Po_topicCG]$$[FIndex]$$[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                     new FunctionRuleSegment("列表大图","[Attr::class=Po_topicCG]$$[FIndex]$$[Tag::img]$$[FIndex]$$[Clear::onmouseover=showtrail(']$$[Clear::onmouseover=','',10,10)]$$[GetAttr::onmouseover]"),
                },
            };
            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\def_config.json");
            if (!System.IO.File.Exists(filepath))
            {
                System.IO.File.WriteAllText(filepath, config.ToJson());
            }
            return System.IO.File.ReadAllText(filepath).JsonTo<CollectRuleConfig>();
        }
    }
}
