using System;
using System.Collections.Generic;
using System.Linq;
using RunTaskForAny.Common.Collect.FunctionRule;
using RunTaskForAny.Common.Helper;
using Stylet;

namespace CollectTool.Pages
{
    public class ShellViewModel : Screen
    {
        public List<CollectRuleConfig> CollectRuleList { get; set; }

        //public CollectConfig Config { get; set; }
        public CollectRuleConfig CollectRule { get; set; }

        private IWindowManager windowManager;

        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;

            try
            {
                var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\RuleList.json");
                if (!System.IO.File.Exists(filepath))
                {
                    CollectRuleList = new List<CollectRuleConfig>();
                    System.IO.File.WriteAllText(filepath, CollectRuleList.ToJson());
                }
                CollectRuleList = System.IO.File.ReadAllText(filepath).JsonTo<List<CollectRuleConfig>>();

            }
            catch (Exception ex)
            {
                Tool.Log.Error(ex.Message);
            }
        }

        public void NewConfig()
        {
            CollectRule = new CollectRuleConfig()
            {
                Name = "默认采集规则",
                Url = "",
                //ListRuleSegment = new FunctionRuleSegment("确定列表", "[Attr::class=Po_topic]"),
                //ListPageRuleSegments = new List<FunctionRuleSegment>()
                //{
                //     new FunctionRuleSegment("列表标题","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Text]"),
                //     new FunctionRuleSegment("列表链接","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Link]"),
                //     new FunctionRuleSegment("列表小图","[Attr::class=Po_topicCG]$$[FIndex]$$[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                //     new FunctionRuleSegment("列表大图","[Attr::class=Po_topicCG]$$[FIndex]$$[Tag::img]$$[FIndex]$$[Clear::onmouseover=showtrail(']$$[Clear::onmouseover=','',10,10)]$$[GetAttr::onmouseover]"),
                //},
                //PagingRuleSegment = new FunctionRuleSegment("下一页", "[Attr::class=pageback]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Link]"),
                //ContentPageRuleSegments = new List<FunctionRuleSegment>()
                //{
                //    new FunctionRuleSegment("标题","[Attr::id=title]$$[FIndex]$$[Text]"),
                //    new FunctionRuleSegment("图片","[Attr::id=info]$$[FIndex]$$[Attr::class=info_cg]$$[Index::0]$$[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                //    new FunctionRuleSegment("番号","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
                //    new FunctionRuleSegment("发行时间","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::1]$$[RemoveTag::b]$$[Text]"),
                //    new FunctionRuleSegment("影片时长","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::2]$$[RemoveTag::b]$$[Text]"),
                //    new FunctionRuleSegment("导演","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::3]$$[Tag::a]$$[FIndex]$$[Text]"),
                //    new FunctionRuleSegment("製作商","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::4]$$[Tag::a]$$[FIndex]$$[Text]"),
                //    new FunctionRuleSegment("发行商","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::5]$$[Tag::a]$$[FIndex]$$[Text]"),
                //    new FunctionRuleSegment("系列","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::6]$$[Tag::a]$$[FIndex]$$[Text]"),
                //    new FunctionRuleSegment("影片类别","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::7]$$[Tag::a]$$[List]",new List<FunctionRuleSegment>()
                //    {
                //        new FunctionRuleSegment("category_name","[Text]"),
                //    }),//内容列表
                //    new FunctionRuleSegment("女优","[Attr::id=info]$$[FIndex]$$[Attr::class=av_performer_cg_box]$$[List]",new List<FunctionRuleSegment>()
                //    {
                //        new FunctionRuleSegment("cg_img","[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                //        new FunctionRuleSegment("cg_title","[Tag::a]$$[FIndex]$$[Text]")
                //    }),//内容列表
                //    new FunctionRuleSegment("图集","[Attr::class=gallery]$$[FIndex]$$[Attr::class=hvr-grow]$$[List]",new List<FunctionRuleSegment>()
                //    {
                //        new FunctionRuleSegment("cg_img","[Tag::img]$$[FIndex]$$[GetAttr::src]"),
                //        new FunctionRuleSegment("cg_img_big","[Tag::a]$$[FIndex]$$[Link]")
                //    }),//内容列表
                //    new FunctionRuleSegment("磁力集合","[Attr::class=dht_dl_area]$$[FIndex]$$[Attr::class=dht_dl_title_content]$$[List]",new List<FunctionRuleSegment>()
                //    {
                //        new FunctionRuleSegment("title","[Tag::a]$$[FIndex]$$[Text]"),
                //        new FunctionRuleSegment("link","[Next]$$[Next]$$[Next]$$[Html]$$[RegexAndDecodeMagnet::\\.attr\\('href','(.+)'\\+reurl\\('(.+)'\\)\\);]"),
                //        new FunctionRuleSegment("size","[Next]$$[Text]"),
                //        new FunctionRuleSegment("date","[Next]$$[Next]$$[Text]")
                //    }),//内容列表
                //},
                //IsSaveToDataBase = 1,
                //ProviderString = "PWMIS.DataProvider.Data.MySQL,PWMIS.MySqlClient",
                //ConnectionString = "Server=localhost;Port=3306;database=Collect_v1;uid=root;password=123456;Convert Zero Datetime=True;Allow Zero Datetime=True;SslMode = none;CharSet=utf8mb4;"
            }
            ;
        }

        public void SaveConfig()
        {
            if (CollectRule == null) { return; }
            
            var f = CollectRuleList.FirstOrDefault(m => m.Name == CollectRule.Name);
            if (f != null)
            {
                this.windowManager.ShowMessageBox("该配置名称已存在!");
                return;
            }
            var list = new List<CollectRuleConfig>();
            list.Add(CollectRule);
            list.AddRange(CollectRuleList);
            CollectRuleList = null;
            CollectRuleList = list;
        }
    }
}
