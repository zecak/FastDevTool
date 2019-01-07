﻿using RunTaskForAny.Common.Domain;
using RunTaskForAny.Common.Helper;
using RunTaskForAny.Module.Collect.PageRule;
using RunTaskForAny.Module.Collect.PageRule.FunctionRule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Collect
{
    public class Action : ModuleBase
    {
        public Action(InfoModel infomodel) : base(infomodel) { }

        public void Start()
        {

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

                DataTable dataTable = null;
                int i = 1;
                do
                {
                    dataTable = collectRule.GetPageList();
                    if (dataTable != null)
                    {
                        var sql = collectRule.DataTableToMySql(dataTable);
                        Tool.Log.Debug("采集了第" + i + "页");
                    }
                    i++;
                    break;

                } while (dataTable != null);

                Tool.Log.Debug("采集完成");

            });
        }


        CollectRuleConfig GetConfig()
        {
            var config = new CollectRuleConfig()
            {
                Name = "默认采集规则",
                Url = "https://qqjj18.com",
                FirstSinglePageRuleSegment = new FunctionRuleSegment("确定单页", "[Attr::class=categorythr]$$[FIndex]]"),
                FirstSinglePageRuleSegments = new List<FunctionRuleSegment>()
                {
                    new FunctionRuleSegment("标题","[Attr::class=list]$$[Index::1]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("新地址","[Attr::class=list]$$[Index::1]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),
                    new FunctionRuleSegment("采集标题","[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("采集地址","[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),
                },
                FirstSinglePageListRuleSegment = new FunctionRuleSegment("采集地址", "[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),

                ListRuleSegment = new FunctionRuleSegment("确定列表", "[Attr::class=Po_topic]"),
                ListPageRuleSegments = new List<FunctionRuleSegment>()
                {
                     new FunctionRuleSegment("列表标题","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Text]"),
                     new FunctionRuleSegment("列表链接","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Link]"),
                     new FunctionRuleSegment("列表小图","[Attr::class=Po_topicCG]$$[FIndex]$$[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                     new FunctionRuleSegment("列表大图","[Attr::class=Po_topicCG]$$[FIndex]$$[Tag::img]$$[FIndex]$$[Clear::onmouseover=showtrail(']$$[Clear::onmouseover=','',10,10)]$$[GetAttr::onmouseover]"),
                },
                PagingRuleSegment = new FunctionRuleSegment("下一页", "[Attr::class=pageback]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Link]"),
                ContentPageRuleSegments = new List<FunctionRuleSegment>()
                {
                    new FunctionRuleSegment("标题","[Attr::id=title]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("图片","[Attr::id=info]$$[FIndex]$$[Attr::class=info_cg]$$[Index::0]$$[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                    new FunctionRuleSegment("番号","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("发行时间","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::1]$$[Tag::b]$$[FIndex]$$[NextText]"),
                    new FunctionRuleSegment("影片时长","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::2]$$[Tag::b]$$[FIndex]$$[NextText]"),
                    new FunctionRuleSegment("导演","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::3]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("製作商","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::4]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("发行商","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::5]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("系列","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::6]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("影片类别","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::7]$$[RemoveTag::b]$$[Text]"),//内容列表
                    //new FunctionRuleSegment("女优","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::8]$$[RemoveTag::b]$$[Text]"),//内容列表
                    //new FunctionRuleSegment("图集","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::9]$$[RemoveTag::b]$$[Text]"),//内容列表
                    //new FunctionRuleSegment("磁力集合","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::9]$$[RemoveTag::b]$$[Text]"),//内容列表
                }
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
