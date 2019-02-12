using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public interface IFindElementByList
    {
        NSoup.Nodes.Element FindElement(NSoup.Select.Elements find_elements);
    }
}
