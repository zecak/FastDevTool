﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Nodes;
using NSoup.Select;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public class AttrFunction : BaseFunction,IFindElements
    {
        public AttrFunction()
        {
        }
        public AttrFunction(string attrName, string attrValue)
        {
            AttrName = attrName;
            AttrValue = attrValue;
        }
        public string AttrName { get; set; }

        public string AttrValue { get; set; }

        public override string Name => "Attr";


        public override string ToSegment()
        {
            return LeftSeparator + Name + KeySeparator + AttrName + ValueSeparator + AttrValue + RightSeparator;
        }

        public override BaseFunction AnalyzeSegment(string segment)
        {
            if (!StartsWithPartSegment(segment)) { return null; }
            var temps = segment.Substring(1, segment.Length - 2).Split(new string[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (temps.Length == 2)
            {
                var vals = temps[1].Split(new string[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (vals.Length == 2)
                {
                    return new AttrFunction(vals[0], vals[1]);
                }
            }
            return null;
        }

        public Elements FindElements(Element find_element)
        {
            return find_element.GetElementsByAttributeValue(AttrName, AttrValue);
        }
    }
}
