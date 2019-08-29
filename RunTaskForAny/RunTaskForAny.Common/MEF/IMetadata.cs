using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTaskForAny.Common.MEF
{
    public interface IMetadata
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
    }
}
