using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.Collect.FunctionSegment
{
    public interface IFindElements
    {
        NSoup.Select.Elements FindElements(NSoup.Nodes.Element find_element);
    }
}
