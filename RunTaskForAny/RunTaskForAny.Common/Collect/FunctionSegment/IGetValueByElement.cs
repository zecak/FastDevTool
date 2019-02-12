using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public interface IGetValueByElement
    {
       string GetValue(NSoup.Nodes.Element find_element);
    }
}
