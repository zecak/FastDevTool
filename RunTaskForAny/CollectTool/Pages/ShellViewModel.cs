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
                var filename = "RuleList.json";
                var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\");
                if (!System.IO.File.Exists(filepath+ filename))
                {
                    CollectRuleList = new List<CollectRuleConfig>();

                    System.IO.Directory.CreateDirectory(filepath);
                    System.IO.File.WriteAllText(filepath + filename, CollectRuleList.ToJson());
                }
                CollectRuleList = System.IO.File.ReadAllText(filepath + filename).JsonTo<List<CollectRuleConfig>>();

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
                ListRuleSegment = new FunctionRuleSegment(),
                ListPageRuleSegments = new List<FunctionRuleSegment>(),
                ContentPageRuleSegments = new List<FunctionRuleSegment>(),
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
