using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Collect.PageRule.FunctionRule
{
    public class CollectConfig
    {
        /// <summary>
        /// 需要采集的配置名称
        /// </summary>
        public string CollectFileName { get; set; }

        /// <summary>
        /// 采集模式:0默认方式;1直接采集内容
        /// </summary>
        public int CollectMode { get; set; }

        /// <summary>
        /// 上次采集的列表url
        /// </summary>
        public string LastCollectListDataUrl { get; set; }
        /// <summary>
        /// 上次采集的列表序号
        /// </summary>
        public int LastCollectListDataNumber { get; set; }

        /// <summary>
        /// 内容链接
        /// </summary>
        public List<string> ContentUrls { get; set; }

    }
}
