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
                    if(dataTable!=null)
                    {
                        System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\data_"+ i + "_"+ dataTable.TableName + ".json"), dataTable.ToJson());
                    }
                    i++;
                    break;
                    
                } while (dataTable != null) ;
                
                Tool.Log.Debug("采集完成");
                
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
