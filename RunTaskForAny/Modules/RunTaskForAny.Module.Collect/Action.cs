using RunTaskForAny.Common.Collect.FunctionRule;
using RunTaskForAny.Common.Database;
using RunTaskForAny.Common.Domain;
using RunTaskForAny.Common.Helper;
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

                switch (collect.CollectMode)
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

                                        var sql2 = collectRule.DataTableToMySql(config.Name + "_List", collectData.ListData, i);
                                        var sql3 = collectRule.DataTableToMySql(config.Name + "_Content", collectData.ContentData, i);

                                        var sql = Environment.NewLine + sql2 + Environment.NewLine + sql3 + Environment.NewLine;

                                        if (config.IsSaveToDataBase == 1)
                                        {
                                            while (true)
                                            {
                                                try
                                                {
                                                    using (var db = SQLHelper.GetDB(config.SQLType, config.ConnectionString))
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

                                        if (collectData.ListData.Rows.Count != collectData.ContentData.Rows.Count)
                                        {
                                            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\warn_data_" + i + ".sql");
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
                                    if (dataTable.Rows.Count > 0)
                                    {
                                        var sql3 = collectRule.DataTableToMySql(config.Name + "_Content", dataTable, i);

                                        var sql = Environment.NewLine + sql3 + Environment.NewLine;

                                        if (config.IsSaveToDataBase == 1)
                                        {
                                            while (true)
                                            {
                                                try
                                                {
                                                    using (var db = SQLHelper.GetDB(config.SQLType, config.ConnectionString))
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
            //    Url = "https://123.com",
            //    IsSaveToDataBase = 1,
            //    SQLType = SQLType.MYSQL,
            //    ConnectionString = "Server=localhost;Port=3306;database=Collect_v1;uid=root;password=123456;Convert Zero Datetime=True;Allow Zero Datetime=True;SslMode = none;CharSet=utf8mb4;"
            //}
            ;
            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Data\\" + filename);
            if (!System.IO.File.Exists(filepath))
            {
                System.IO.File.WriteAllText(filepath, config.ToJson());
            }
            return System.IO.File.ReadAllText(filepath).JsonTo<CollectRuleConfig>();
        }
    }
}
