using RunTaskForAny.Common.Domain;
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
                //var dd = reurl("01101101010000010110111000111100010000000110110001000010001110100100000001101101011011100111000000111100011011100110111001000000001111100100001001000001011100000011111001110000010000010011110001101110001110100100001100111101011011010011111101101110001111110110110000111100010000110111000001000000010000110011110000111010");
                

                //获取配置
                var config = GetConfig();
                if (config == null)
                {
                    Tool.Log.Error("配置文件错误");
                    return;
                }
                Tool.Log.Debug("获取html");

                CollectRule collectRule = new CollectRule(config);

                CollectData collectData = null;
                int i = 1;
                do
                {
                    collectData = collectRule.GetPageList();
                    if (collectData.ListData != null && collectData.ListData.Rows.Count > 0 && collectData.ContentData != null && collectData.ContentData.Rows.Count > 0)
                    {
                        var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\data_" + i + ".json");
                        if (!System.IO.File.Exists(filepath))
                        {
                            System.IO.File.WriteAllText(filepath, collectData.ToJson());
                        }

                        //var sql = collectRule.DataTableToMySql(collectData.ListData);
                        Tool.Log.Debug("采集了第" + i + "页");
                    }
                    i++;
                    break;

                } while (collectData.ListData.Rows.Count > 0 && collectData.ContentData.Rows.Count > 0);

                Tool.Log.Debug("采集完成");

            });
        }

        string[] csplit(string body)
        {
            List<string> strlist = new List<string>();
            var chunklen = 8;
            var num = body.Length / chunklen;
            var yushu = body.Length % chunklen;
            if (yushu != 0) { num = num + 1; }
            for (int i = 0; i < num; i++)
            {
                strlist.Add(body.Substring(0 + (i * chunklen), chunklen));
                if (yushu != 0)
                {
                    if ((i + 1) == num)
                    {
                        strlist.Add(body.Substring(0 + (i * chunklen), yushu));
                    }
                }
            }
            return strlist.ToArray();
        }

        public string reurl(string body)
        {
            var str = "";
            var strlist = csplit(body);
            foreach (var item in strlist)
            {
                var d = System.Convert.ToInt32(item, 2) - 10;
                var unicode = System.Convert.ToChar(d);
                str += unicode;
            }
            return str;
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
                    new FunctionRuleSegment("发行时间","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::1]$$[RemoveTag::b]$$[Text]"),
                    new FunctionRuleSegment("影片时长","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::2]$$[RemoveTag::b]$$[Text]"),
                    new FunctionRuleSegment("导演","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::3]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("製作商","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::4]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("发行商","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::5]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("系列","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::6]$$[Tag::a]$$[FIndex]$$[Text]"),
                    new FunctionRuleSegment("影片类别","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::7]$$[Tag::a]$$[List]",new List<FunctionRuleSegment>()
                    {
                        new FunctionRuleSegment("category_name","[Text]"),
                    }),//内容列表
                    new FunctionRuleSegment("女优","[Attr::id=info]$$[FIndex]$$[Attr::class=av_performer_cg_box]$$[List]",new List<FunctionRuleSegment>()
                    {
                        new FunctionRuleSegment("cg_img","[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                        new FunctionRuleSegment("cg_title","[Tag::a]$$[FIndex]$$[Text]")
                    }),//内容列表
                    new FunctionRuleSegment("图集","[Attr::class=gallery]$$[FIndex]$$[Attr::class=hvr-grow]$$[List]",new List<FunctionRuleSegment>()
                    {
                        new FunctionRuleSegment("cg_img","[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                        new FunctionRuleSegment("cg_img_big","[Tag::a]$$[FIndex]$$[Link]")
                    }),//内容列表
                    new FunctionRuleSegment("磁力集合","[Attr::class=dht_dl_area]$$[FIndex]$$[Attr::class=dht_dl_title_content]$$[List]",new List<FunctionRuleSegment>()
                    {
                        new FunctionRuleSegment("title","[Tag::a]$$[FIndex]$$[Text]"),
                        new FunctionRuleSegment("link","[Tag::a]$$[FIndex]$$[Link]"),
                        new FunctionRuleSegment("size","[Next]$$[Text]"),
                        new FunctionRuleSegment("date","[Next]$$[Next]$$[Text]")
                    }),//内容列表
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
