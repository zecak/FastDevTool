using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{

    public class RegexAndDecodeMagnetFunction : BaseFunction, IDoValue
    {
        public string Pattern { get; set; }
        public RegexAndDecodeMagnetFunction()
        {
        }
        public RegexAndDecodeMagnetFunction(string pattern)
        {
            Pattern = pattern;
        }
        public override string Name => "RegexAndDecodeMagnet";

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                return new RegexAndDecodeMagnetFunction(temps[1]);
            }
            return null;
        }

        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + Pattern + RightSeparator;
        }

        public string DoValue(string val)
        {
            var reg = new System.Text.RegularExpressions.Regex(Pattern);
            var match = reg.Match(val);
            if (match.Success)
            {
                if (match.Groups.Count == 3)
                {
                    val = match.Groups[1].Value + reurl(match.Groups[2].Value);
                }
            }
            return val;
        }

        List<string> csplit(string body)
        {
            List<string> strlist = new List<string>();
            var chunklen = 8;
            var num = body.Length / chunklen;
            var yushu = body.Length % chunklen;
            if (yushu != 0) { num = num + 1; }
            for (int i = 0; i < num; i++)
            {
                strlist.Add(body.Substring(0 + (i * chunklen), chunklen));
                if (yushu != 0)
                {
                    if ((i + 1) == num)
                    {
                        strlist.Add(body.Substring(0 + (i * chunklen), yushu));
                    }
                }
            }
            return strlist;
        }

        /// <summary>
        /// 解密magnet:?xt=urn:btih:后的加密串(..0100101010010...)
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        string reurl(string body)
        {
            var str = "";
            var strlist = csplit(body);
            foreach (var item in strlist)
            {
                var d = System.Convert.ToInt32(item, 2) - 10;
                var unicode = System.Convert.ToChar(d);
                str += unicode;
            }
            return str;
        }
    }
}
