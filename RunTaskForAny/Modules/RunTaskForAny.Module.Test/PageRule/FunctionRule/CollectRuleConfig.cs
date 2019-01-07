using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule.FunctionRule
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
        /// 内容页功能段列表
        /// </summary>
        public List<FunctionRuleSegment> ContentPageRuleSegments { get; set; }


    }
}
