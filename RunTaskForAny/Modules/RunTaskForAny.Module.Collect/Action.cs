using RunTaskForAny.Common.Domain;
using RunTaskForAny.Common.Helper;
using RunTaskForAny.DataBase;
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
                var collect = GetCollect();
                
                //获取配置
                var config = GetConfig(collect.CollectFileName);
                if (config == null)
                {
                    Tool.Log.Error("配置文件错误");
                    return;
                }
                
                Tool.Log.Debug("上次采集的地址:" + collect.LastCollectListDataUrl);


                Tool.Log.Debug("开始采集..");
                CollectRule collectRule = new CollectRule(config);

                switch(collect.CollectMode)
                {
                    case 0:
                        {
                            Tool.Log.Debug("采集模式:默认方式");
                            CollectData collectData = null;
                            int i = collect.LastCollectListDataNumber;
                            do
                            {
                                try
                                {
                                    collectData = collectRule.GetPageList(collect.LastCollectListDataUrl);
                                    if (collectData.ListData != null && collectData.ListData.Rows.Count > 0 && collectData.ContentData != null && collectData.ContentData.Rows.Count > 0)
                                    {
                                        //var sql1 = collectRule.DataTableToMySql(config.Name + "_First", collectData.FirstData);
                                        var sql2 = collectRule.DataTableToMySql(config.Name + "_List", collectData.ListData, i);
                                        var sql3 = collectRule.DataTableToMySql(config.Name + "_Content", collectData.ContentData, i);

                                        var sql = Environment.NewLine + sql2 + Environment.NewLine + sql3 + Environment.NewLine;

                                        if (config.IsSaveToDataBase == 1)
                                        {
                                            while (true)
                                            {
                                                try
                                                {
                                                    using (var db = SQLHelper.GetDB(config.ProviderString, config.ConnectionString))
                                                    {

                                                        var rz = db.ExecuteNonQuery(sql);
                                                        if (rz != -1)
                                                        {
                                                            Tool.Log.Debug("已入库了:第" + i + "页 => " + collect.LastCollectListDataUrl);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            System.Threading.Thread.Sleep(3000);//3秒后重试
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Tool.Log.Error("---------------------------");
                                                    Tool.Log.Error(ex.Message);
                                                    Tool.Log.Error(Environment.NewLine);

                                                    System.Threading.Thread.Sleep(3000);//暂停3秒
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\data_" + i + ".sql");
                                            if (!System.IO.File.Exists(filepath))
                                            {
                                                System.IO.File.WriteAllText(filepath, sql);
                                            }
                                        }

                                        if (collectData.NextPageData.Rows.Count > 0)
                                        {
                                            collect.LastCollectListDataUrl = (string)collectData.NextPageData.Rows[0]["PageUrl"];
                                            collect.LastCollectListDataNumber = i;
                                        }

                                        SaveCollect(collect);

                                        Tool.Log.Debug("采集了第" + i + "页 => " + collect.LastCollectListDataUrl);
                                        collect.LastCollectListDataUrl = "";

                                    }
                                    else
                                    {
                                        Tool.Log.Debug("采集数据为空");
                                    }

                                    i++;

                                }
                                catch (Exception ex)
                                {
                                    Tool.Log.Error("===========================");
                                    Tool.Log.Error(ex.Message);
                                    Tool.Log.Error(Environment.NewLine);

                                    System.Threading.Thread.Sleep(10000);//暂停30秒
                                }

                            } while (collectData.ListData.Rows.Count > 0 && collectData.ContentData.Rows.Count > 0);
                        }
                        break;
                    case 1:
                        {
                            Tool.Log.Debug("采集模式:直接采集内容");

                            DataTable dataTable = new DataTable();

                            var i = -1;
                            foreach (var url in collect.ContentUrls)
                            {
                                try
                                {
                                    dataTable = collectRule.GetPageContent(url);
                                    if(dataTable.Rows.Count>0)
                                    {
                                        var sql3 = collectRule.DataTableToMySql(config.Name + "_Content", dataTable, i);

                                        var sql = Environment.NewLine + sql3 + Environment.NewLine;

                                        if (config.IsSaveToDataBase == 1)
                                        {
                                            while (true)
                                            {
                                                try
                                                {
                                                    using (var db = SQLHelper.GetDB(config.ProviderString, config.ConnectionString))
                                                    {

                                                        var rz = db.ExecuteNonQuery(sql);
                                                        if (rz != -1)
                                                        {
                                                            Tool.Log.Debug("已入库了: => " + collect.LastCollectListDataUrl);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            System.Threading.Thread.Sleep(3000);//3秒后重试
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Tool.Log.Error("---------------------------");
                                                    Tool.Log.Error(ex.Message);
                                                    Tool.Log.Error(Environment.NewLine);

                                                    System.Threading.Thread.Sleep(3000);//暂停3秒
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\data_" + i + ".sql");
                                            if (!System.IO.File.Exists(filepath))
                                            {
                                                System.IO.File.WriteAllText(filepath, sql);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Tool.Log.Debug("采集数据为空");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Tool.Log.Error("===========================");
                                    Tool.Log.Error(ex.Message);
                                    Tool.Log.Error(Environment.NewLine);

                                    System.Threading.Thread.Sleep(10000);//暂停10秒
                                }
                                i--;
                            }
                            
                        }
                        break;
                    default:
                        break;
                }

               

                Tool.Log.Debug("采集完成");

            });
        }

        CollectConfig GetCollect()
        {
            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\collect.json");
            var collect = System.IO.File.ReadAllText(filepath).JsonTo<CollectConfig>();
            return collect;
        }

        void SaveCollect(CollectConfig collect)
        {
            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\collect.json");
            System.IO.File.WriteAllText(filepath, collect.ToJson());
        }

        CollectRuleConfig GetConfig(string filename)
        {
            var config = new CollectRuleConfig()
            //{
            //    Name = "默认采集规则",
            //    Url = "https://qqjj18.com",
            //    FirstSinglePageRuleSegment = new FunctionRuleSegment("确定单页", "[Attr::class=categorythr]$$[FIndex]]"),
            //    FirstSinglePageRuleSegments = new List<FunctionRuleSegment>()
            //    {
            //        new FunctionRuleSegment("标题","[Attr::class=list]$$[Index::1]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
            //        new FunctionRuleSegment("新地址","[Attr::class=list]$$[Index::1]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),
            //        new FunctionRuleSegment("采集标题","[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
            //        new FunctionRuleSegment("采集地址","[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),
            //    },
            //    FirstSinglePageListRuleSegment = new FunctionRuleSegment("采集地址", "[Attr::class=list]$$[Index::10]$$[Tag::li]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Link]"),

            //    ListRuleSegment = new FunctionRuleSegment("确定列表", "[Attr::class=Po_topic]"),
            //    ListPageRuleSegments = new List<FunctionRuleSegment>()
            //    {
            //         new FunctionRuleSegment("列表标题","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Text]"),
            //         new FunctionRuleSegment("列表链接","[Attr::class=Po_topic_title]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Link]"),
            //         new FunctionRuleSegment("列表小图","[Attr::class=Po_topicCG]$$[FIndex]$$[Tag::img]$$[FIndex]$$[GetAttr::src]"),
            //         new FunctionRuleSegment("列表大图","[Attr::class=Po_topicCG]$$[FIndex]$$[Tag::img]$$[FIndex]$$[Clear::onmouseover=showtrail(']$$[Clear::onmouseover=','',10,10)]$$[GetAttr::onmouseover]"),
            //    },
            //    PagingRuleSegment = new FunctionRuleSegment("下一页", "[Attr::class=pageback]$$[FIndex]$$[Tag::a]$$[FIndex]$$[Link]"),
            //    ContentPageRuleSegments = new List<FunctionRuleSegment>()
            //    {
            //        new FunctionRuleSegment("标题","[Attr::id=title]$$[FIndex]$$[Text]"),
            //        new FunctionRuleSegment("图片","[Attr::id=info]$$[FIndex]$$[Attr::class=info_cg]$$[Index::0]$$[Tag::img]$$[FIndex]$$[GetAttr::src]"),
            //        new FunctionRuleSegment("番号","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::0]$$[Tag::a]$$[FIndex]$$[Text]"),
            //        new FunctionRuleSegment("发行时间","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::1]$$[RemoveTag::b]$$[Text]"),
            //        new FunctionRuleSegment("影片时长","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::2]$$[RemoveTag::b]$$[Text]"),
            //        new FunctionRuleSegment("导演","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::3]$$[Tag::a]$$[FIndex]$$[Text]"),
            //        new FunctionRuleSegment("製作商","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::4]$$[Tag::a]$$[FIndex]$$[Text]"),
            //        new FunctionRuleSegment("发行商","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::5]$$[Tag::a]$$[FIndex]$$[Text]"),
            //        new FunctionRuleSegment("系列","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::6]$$[Tag::a]$$[FIndex]$$[Text]"),
            //        new FunctionRuleSegment("影片类别","[Attr::id=info]$$[FIndex]$$[Attr::class=infobox]$$[Index::7]$$[Tag::a]$$[List]",new List<FunctionRuleSegment>()
            //        {
            //            new FunctionRuleSegment("category_name","[Text]"),
            //        }),//内容列表
            //        new FunctionRuleSegment("女优","[Attr::id=info]$$[FIndex]$$[Attr::class=av_performer_cg_box]$$[List]",new List<FunctionRuleSegment>()
            //        {
            //            new FunctionRuleSegment("cg_img","[Tag::img]$$[FIndex]$$[GetAttr::src]"),
            //            new FunctionRuleSegment("cg_title","[Tag::a]$$[FIndex]$$[Text]")
            //        }),//内容列表
            //        new FunctionRuleSegment("图集","[Attr::class=gallery]$$[FIndex]$$[Attr::class=hvr-grow]$$[List]",new List<FunctionRuleSegment>()
            //        {
            //            new FunctionRuleSegment("cg_img","[Tag::img]$$[FIndex]$$[GetAttr::src]"),
            //            new FunctionRuleSegment("cg_img_big","[Tag::a]$$[FIndex]$$[Link]")
            //        }),//内容列表
            //        new FunctionRuleSegment("磁力集合","[Attr::class=dht_dl_area]$$[FIndex]$$[Attr::class=dht_dl_title_content]$$[List]",new List<FunctionRuleSegment>()
            //        {
            //            new FunctionRuleSegment("title","[Tag::a]$$[FIndex]$$[Text]"),
            //            new FunctionRuleSegment("link","[Next]$$[Next]$$[Next]$$[Html]$$[RegexAndDecodeMagnet::\\.attr\\('href','(.+)'\\+reurl\\('(.+)'\\)\\);]"),
            //            new FunctionRuleSegment("size","[Next]$$[Text]"),
            //            new FunctionRuleSegment("date","[Next]$$[Next]$$[Text]")
            //        }),//内容列表
            //    },
            //    IsSaveToDataBase=1,
            //    ProviderString= "PWMIS.DataProvider.Data.MySQL,PWMIS.MySqlClient",
            //    ConnectionString= "Server=localhost;Port=3306;database=Collect_v1;uid=root;password=123456;Convert Zero Datetime=True;Allow Zero Datetime=True;SslMode = none;CharSet=utf8mb4;"
            //}
            ;
            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\"+ filename);
            if (!System.IO.File.Exists(filepath))
            {
                System.IO.File.WriteAllText(filepath, config.ToJson());
            }
            return System.IO.File.ReadAllText(filepath).JsonTo<CollectRuleConfig>();
        }
    }
}
