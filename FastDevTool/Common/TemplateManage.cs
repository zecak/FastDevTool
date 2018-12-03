using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDevTool.Common
{
    public class TemplateManage
    {
        public const string DefFileName = @"ModuleTemplates\Def.txt";
        public static string GetTemplate(string filename,string tablename,string title)
        {
            if (!System.IO.File.Exists(filename)) { return ""; }
            var content = System.IO.File.ReadAllText(filename);
            return string.Format(content, ReplaceFieldValue(tablename), ReplaceFieldValue(title));
        }

        public static string GetTemplateDef(string tablename, string title)
        {
            var filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,DefFileName);
           return GetTemplate(filename, ReplaceFieldValue(tablename), ReplaceFieldValue(title));
        }

        /// <summary>
        /// 过滤sql的单引号,防注入
        /// </summary>
        /// <param name="fieldvalue"></param>
        /// <returns></returns>
        static string ReplaceFieldValue(string fieldvalue)
        {
            return fieldvalue.Replace("'", "''");
        }
    }
}
