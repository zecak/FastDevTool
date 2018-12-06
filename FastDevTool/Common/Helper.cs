using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.Common
{
    public class Helper
    {
        public static string GetPinyin(string cn)
        {
            return NPinyin.Pinyin.GetPinyin(cn,true);
        }
    }
}
