using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionRule
{
    public class CollectRuleConfig
    {
        public CollectRuleConfig()
        {
        }
        public CollectRuleConfig(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 采集名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 采集地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否保存到数据库:1是,0否
        /// </summary>
        public int IsSaveToDataBase { get; set; }
        /// <summary>
        /// 数据库连接:Server=localhost;Port=3306;database=Collect_v1;uid=root;password=123456;Convert Zero Datetime=True;Allow Zero Datetime=True;SslMode = none;CharSet=utf8mb4;||Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Collect_v1.mdf;Integrated Security=True;
        /// </summary>
        public string ConnectionString { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        /// <summary>
        /// 数据库类型
        /// </summary>
        public Database.SQLType SQLType { get; set; }

        /// <summary>
        /// 确定单页功能段
        /// </summary>
        public FunctionRuleSegment FirstSinglePageRuleSegment { get; set; }
        /// <summary>
        /// 单页功能段列表
        /// </summary>
        public List<FunctionRuleSegment> FirstSinglePageRuleSegments { get; set; }

        /// <summary>
        /// 单页需要继续采集列表地址
        /// </summary>
        public string FirstSinglePageListRuleSegmentUrl { get; set; }
        /// <summary>
        /// 单页需要继续采集列表地址规则
        /// </summary>
        public FunctionRuleSegment FirstSinglePageListRuleSegment { get; set; }

        /// <summary>
        /// 确定列表功能段:列表里的相同规则项
        /// </summary>
        public FunctionRuleSegment ListRuleSegment { get; set; }

        /// <summary>
        /// 列表页功能段列表
        /// </summary>
        public List<FunctionRuleSegment> ListPageRuleSegments { get; set; }

        /// <summary>
        /// 下一页功能段
        /// </summary>
        public FunctionRuleSegment PagingRuleSegment { get; set; }

        /// <summary>
        /// 下一页地址
        /// </summary>
        public string PagingRuleSegmentUrl { get; set; }

        /// <summary>
        /// 确认列表内容链接
        /// </summary>
        public FunctionRuleSegment ListContentPageRuleSegment { get; set; }
        /// <summary>
        /// 内容页功能段列表
        /// </summary>
        public List<FunctionRuleSegment> ContentPageRuleSegments { get; set; }


    }
}
