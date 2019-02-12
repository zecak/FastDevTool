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
                            DataTable listData = null;
                            int i = collect.LastCollectListDataNumber;

                            var firstData = collectRule.GetFirstData();
                            if (string.IsNullOrWhiteSpace(collect.LastCollectListDataUrl))
                            {
                                if (!string.IsNullOrWhiteSpace(config.FirstSinglePageListRuleSegmentUrl))
                                {
                                    collect.LastCollectListDataUrl = config.FirstSinglePageListRuleSegmentUrl;
                                }
                                else
                                {
                                    collect.LastCollectListDataUrl = config.Url;
                                }
                            }
                            do
                            {
                                try
                                {
                                    listData = collectRule.GetPageListData(collect.LastCollectListDataUrl);
                                    var sql_list = collectRule.DataTableToMySql(config.Name + "_List", listData, i);
                                    if (config.IsSaveToDataBase == 1)
                                    {
                                        while (true)
                                        {
                                            try
                                            {
                                                using (var db = SQLHelper.GetDB(config.SQLType, config.ConnectionString))
                                                {

                                                    var rz = db.ExecuteNonQuery(sql_list);
                                                    if (rz != -1)
                                                    {
                                                        Tool.Log.Debug("列表:已入库了:第" + i + "页 => " + collect.LastCollectListDataUrl);
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
                                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\"));
                                        var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\data_list_" + i + ".sql");
                                        if (!System.IO.File.Exists(filepath))
                                        {
                                            System.IO.File.WriteAllText(filepath, sql_list);
                                        }
                                        Tool.Log.Debug("列表:已入库了:第" + i + "页 => " + collect.LastCollectListDataUrl);
                                    }

                                    DataTable contentData = null;
                                    int j = 1;
                                    foreach (DataRow row in listData.Rows)
                                    {
                                        var pageNumber = Convert.ToInt32((string)row[listData.Columns.Count - 3]);
                                        var pageIndex = Convert.ToInt32((string)row[listData.Columns.Count - 2]);
                                        var content_url = (string)row[listData.Columns.Count - 1];
                                        if (!string.IsNullOrWhiteSpace(content_url))
                                        {
                                            if(contentData==null)
                                            {
                                                contentData = collectRule.GetPageContentData(content_url, pageNumber, pageIndex);
                                            }
                                            else
                                            {
                                                var dt= collectRule.GetPageContentData(content_url, pageNumber, pageIndex);
                                                var r = contentData.NewRow();
                                                r.ItemArray = dt.Rows[0].ItemArray;
                                                contentData.Rows.Add(r);
                                            }
                                            Tool.Log.Debug("内容:已收集了:第" + j + "条 => " + content_url);
                                            j++;
                                        }
                                    }
                                    var sql_content = collectRule.DataTableToMySql(config.Name + "_Content", contentData, i);
                                    if (config.IsSaveToDataBase == 1)
                                    {
                                        while (true)
                                        {
                                            try
                                            {
                                                using (var db = SQLHelper.GetDB(config.SQLType, config.ConnectionString))
                                                {

                                                    var rz = db.ExecuteNonQuery(sql_content);
                                                    if (rz != -1)
                                                    {
                                                        Tool.Log.Debug("内容:已入库了:第" + i + "页 => " + collect.LastCollectListDataUrl);
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
                                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\"));
                                        var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\data_content_" + i + ".sql");
                                        if (!System.IO.File.Exists(filepath))
                                        {
                                            System.IO.File.WriteAllText(filepath, sql_content);
                                        }
                                        Tool.Log.Debug("内容:已入库了:第" + i + "页 => " + collect.LastCollectListDataUrl);
                                    }


                                    if (string.IsNullOrWhiteSpace(config.PagingRuleSegmentUrl))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        collect.LastCollectListDataUrl = config.PagingRuleSegmentUrl;
                                        collect.LastCollectListDataNumber = i;
                                        SaveCollect(collect);
                                    }

                                    i++;

                                }
                                catch (Exception ex)
                                {
                                    Tool.Log.Error("===========================");
                                    Tool.Log.Error(ex.Message);
                                    Tool.Log.Error(Environment.NewLine);

                                    System.Threading.Thread.Sleep(10000);//暂停10秒
                                }

                            } while (listData != null && listData.Rows.Count > 0);
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
                                            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\"));
                                            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\data_" + i + ".sql");
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
