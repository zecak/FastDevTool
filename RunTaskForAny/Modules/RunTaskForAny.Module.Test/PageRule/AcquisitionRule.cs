using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Module.Test.PageRule
{
    /// <summary>
    /// 采集规则
    /// </summary>
    [Serializable]
    public class AcquisitionRule
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string DataPath { get; set; }
        public string RulesPath { get; set; }
        public string DownPath { get; set; }

        public bool ListType { get; set; }
        /// <summary>
        /// 段(属性名值规则)
        /// </summary>
        public KeyValue ParagraphRules { get; set; }
        /// <summary>
        /// 行规则链(字段名值规则)
        /// </summary>
        public List<KeyValue> RowRules { get; set; }
        /// <summary>
        /// 下一页规则(字段名值规则)
        /// </summary>
        public KeyValue NextPageRules { get; set; }
    }

    [Serializable]
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
